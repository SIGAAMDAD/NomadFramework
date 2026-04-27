using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nomad.FileSystem.Private.Services;
using Nomad.Core.Logger;
using NUnit.Framework;

namespace Nomad.Tests.FileSystem.Private.Services
{
	[TestFixture]
	[Category("Nomad.FileSystem")]
	[Category("Services")]
	[Category("Unit")]
	public sealed class RecursiveFileSearcherTests
	{
		private sealed class TempDirectory : IDisposable
		{
			public string Path { get; }

			public TempDirectory()
			{
				Path = System.IO.Path.Combine(
					TestContext.CurrentContext.WorkDirectory,
					$"RecursiveFileSearcherTests_{Guid.NewGuid():N}"
				);

				Directory.CreateDirectory(Path);
			}

			public string CreateDirectory(string relativePath)
			{
				string fullPath = System.IO.Path.Combine(
					Path,
					NormalizeInputPath(relativePath)
				);

				Directory.CreateDirectory(fullPath);
				return fullPath;
			}

			public string CreateFile(string relativePath, string contents = "")
			{
				string fullPath = System.IO.Path.Combine(
					Path,
					NormalizeInputPath(relativePath)
				);

				string? directory = System.IO.Path.GetDirectoryName(fullPath);
				if (!string.IsNullOrEmpty(directory))
				{
					Directory.CreateDirectory(directory);
				}

				File.WriteAllText(fullPath, contents);
				return fullPath;
			}

			public void Dispose()
			{
				try
				{
					if (Directory.Exists(Path))
					{
						Directory.Delete(Path, recursive: true);
					}
				}
				catch
				{
					// Cleanup should not hide the original test failure.
				}
			}
		}

		private static RecursiveFileSearcher CreateSearcher(
			bool? ignoreCase = false,
			bool useIndex = false)
		{
			/*
				RecursiveFileSearcher only uses ILoggerCategory in warning/error
				paths for FindFiles/GetFilesAsList. These tests avoid those
				environment-dependent warning branches, so null! is sufficient.

				If you want to test warning logging branches too, replace null!
				with your project's concrete test logger category.
			*/
			ILoggerCategory category = null!;

			return new RecursiveFileSearcher(
				category,
				ignoreCase,
				useIndex
			);
		}

		private static string NormalizeExpectedPath(string path)
		{
			return path.Trim()
				.Replace('/', System.IO.Path.DirectorySeparatorChar)
				.Replace('\\', System.IO.Path.DirectorySeparatorChar);
		}

		private static string NormalizeDirectoryExpectedPath(string path)
		{
			string fullPath = System.IO.Path.GetFullPath(path);

			if (!fullPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString(), StringComparison.CurrentCultureIgnoreCase))
			{
				fullPath += System.IO.Path.DirectorySeparatorChar;
			}

			return fullPath;
		}

		private static string NormalizeInputPath(string path)
		{
			return path.Replace('/', System.IO.Path.DirectorySeparatorChar)
				.Replace('\\', System.IO.Path.DirectorySeparatorChar);
		}

		private static void AssertPathEqual(string? actual, string expected)
		{
			Assert.That(actual, Is.EqualTo(NormalizeExpectedPath(expected)));
		}

		[Test]
		public void Constructor_WithDefaults_CreatesEmptySearchDirectoryList()
		{
			using var searcher = CreateSearcher();

			Assert.That(searcher.SearchDirectories, Is.Empty);
		}

		[Test]
		public void Constructor_WithUseIndexTrue_CreatesSearchDirectoryList()
		{
			using var searcher = CreateSearcher(useIndex: true);

			Assert.That(searcher.SearchDirectories, Is.Empty);
		}

		[Test]
		public void AddSearchDirectory_WithNullDirectory_ThrowsArgumentException()
		{
			using var searcher = CreateSearcher();

			var ex = Assert.Throws<ArgumentException>(() =>
			{
				searcher.AddSearchDirectory(null!);
			});

			Assert.That(ex!.ParamName, Is.EqualTo("directory"));
		}

		[Test]
		public void AddSearchDirectory_WithEmptyDirectory_ThrowsArgumentException()
		{
			using var searcher = CreateSearcher();

			var ex = Assert.Throws<ArgumentException>(() =>
			{
				searcher.AddSearchDirectory(string.Empty);
			});

			Assert.That(ex!.ParamName, Is.EqualTo("directory"));
		}

		[Test]
		public void AddSearchDirectory_WithWhitespaceDirectory_ThrowsArgumentException()
		{
			using var searcher = CreateSearcher();

			var ex = Assert.Throws<ArgumentException>(() =>
			{
				searcher.AddSearchDirectory("   ");
			});

			Assert.That(ex!.ParamName, Is.EqualTo("directory"));
		}

		[Test]
		public void AddSearchDirectory_WithMissingDirectory_ThrowsDirectoryNotFoundException()
		{
			using var searcher = CreateSearcher();

			string missingDirectory = System.IO.Path.Combine(
				TestContext.CurrentContext.WorkDirectory,
				$"missing_{Guid.NewGuid():N}"
			);

			var ex = Assert.Throws<DirectoryNotFoundException>(() =>
			{
				searcher.AddSearchDirectory(missingDirectory);
			});

			Assert.That(ex!.Message, Does.Contain(missingDirectory));
		}

		[Test]
		public void AddSearchDirectory_AppendsNormalPriorityDirectory()
		{
			using var tempA = new TempDirectory();
			using var tempB = new TempDirectory();
			using var searcher = CreateSearcher();

			searcher.AddSearchDirectory(tempA.Path);
			searcher.AddSearchDirectory(tempB.Path);

			Assert.Multiple(() =>
			{
				Assert.That(searcher.SearchDirectories, Has.Count.EqualTo(2));
				Assert.That(searcher.SearchDirectories[0], Is.EqualTo(NormalizeDirectoryExpectedPath(tempA.Path)));
				Assert.That(searcher.SearchDirectories[1], Is.EqualTo(NormalizeDirectoryExpectedPath(tempB.Path)));
			});
		}

		[Test]
		public void AddSearchDirectory_WithHighPriority_InsertsAtBeginning()
		{
			using var tempA = new TempDirectory();
			using var tempB = new TempDirectory();
			using var searcher = CreateSearcher();

			searcher.AddSearchDirectory(tempA.Path);
			searcher.AddSearchDirectory(tempB.Path, highPriority: true);

			Assert.Multiple(() =>
			{
				Assert.That(searcher.SearchDirectories, Has.Count.EqualTo(2));
				Assert.That(searcher.SearchDirectories[0], Is.EqualTo(NormalizeDirectoryExpectedPath(tempB.Path)));
				Assert.That(searcher.SearchDirectories[1], Is.EqualTo(NormalizeDirectoryExpectedPath(tempA.Path)));
			});
		}

		[Test]
		public void AddSearchDirectory_AllowsDuplicateDirectories()
		{
			using var temp = new TempDirectory();
			using var searcher = CreateSearcher();

			searcher.AddSearchDirectory(temp.Path);
			searcher.AddSearchDirectory(temp.Path);

			Assert.That(searcher.SearchDirectories, Has.Count.EqualTo(2));
		}

		[Test]
		public void SearchDirectories_ReturnsReadOnlyView()
		{
			using var temp = new TempDirectory();
			using var searcher = CreateSearcher();

			searcher.AddSearchDirectory(temp.Path);

			Assert.That(searcher.SearchDirectories, Is.AssignableTo<IReadOnlyList<string>>());
			Assert.That(searcher.SearchDirectories[0], Is.EqualTo(NormalizeDirectoryExpectedPath(temp.Path)));
		}

		[Test]
		public void FindFile_WithNullRelativePath_ThrowsArgumentException()
		{
			using var searcher = CreateSearcher();

			var ex = Assert.Throws<ArgumentException>(() =>
			{
				searcher.FindFile(null!);
			});

			Assert.That(ex!.ParamName, Is.EqualTo("relativePath"));
		}

		[Test]
		public void FindFile_WithEmptyRelativePath_ThrowsArgumentException()
		{
			using var searcher = CreateSearcher();

			var ex = Assert.Throws<ArgumentException>(() =>
			{
				searcher.FindFile(string.Empty);
			});

			Assert.That(ex!.ParamName, Is.EqualTo("relativePath"));
		}

		[Test]
		public void FindFile_WithWhitespaceRelativePath_ThrowsArgumentException()
		{
			using var searcher = CreateSearcher();

			var ex = Assert.Throws<ArgumentException>(() =>
			{
				searcher.FindFile("   ");
			});

			Assert.That(ex!.ParamName, Is.EqualTo("relativePath"));
		}

		[Test]
		public void FindFile_WithNoSearchDirectories_ReturnsNull()
		{
			using var searcher = CreateSearcher();

			string? result = searcher.FindFile("Config/settings.json");

			Assert.That(result, Is.Null);
		}

		[Test]
		public void FindFile_WithoutIndex_FindsExactRelativeFile()
		{
			using var temp = new TempDirectory();
			string filePath = temp.CreateFile("Config/settings.json", "{}");

			using var searcher = CreateSearcher(useIndex: false);
			searcher.AddSearchDirectory(temp.Path);

			string? result = searcher.FindFile("Config/settings.json");

			AssertPathEqual(result, filePath);
		}

		[Test]
		public void FindFile_WithoutIndex_NormalizesForwardAndBackSlashes()
		{
			using var temp = new TempDirectory();
			string filePath = temp.CreateFile("Config/Bindings/input.json", "{}");

			using var searcher = CreateSearcher(useIndex: false);
			searcher.AddSearchDirectory(temp.Path);

			string? result = searcher.FindFile("Config\\Bindings/input.json");

			AssertPathEqual(result, filePath);
		}

		[Test]
		public void FindFile_WithoutIndex_TrimsRelativePath()
		{
			using var temp = new TempDirectory();
			string filePath = temp.CreateFile("Config/settings.json", "{}");

			using var searcher = CreateSearcher(useIndex: false);
			searcher.AddSearchDirectory(temp.Path);

			string? result = searcher.FindFile("  Config/settings.json  ");

			AssertPathEqual(result, filePath);
		}

		[Test]
		public void FindFile_WithoutIndex_ReturnsFirstMatchBySearchDirectoryPriority()
		{
			using var lowPriority = new TempDirectory();
			using var highPriority = new TempDirectory();

			lowPriority.CreateFile("Config/settings.json", "low");
			string expected = highPriority.CreateFile("Config/settings.json", "high");

			using var searcher = CreateSearcher(useIndex: false);
			searcher.AddSearchDirectory(lowPriority.Path);
			searcher.AddSearchDirectory(highPriority.Path, highPriority: true);

			string? result = searcher.FindFile("Config/settings.json");

			AssertPathEqual(result, expected);
		}

		[Test]
		public void FindFile_WithoutIndex_SupportsProjectStylePathRepeatingSearchRootFolderName()
		{
			using var assets = new TempDirectory();
			string filePath = assets.CreateFile("Config/settings.json", "{}");

			using var searcher = CreateSearcher(useIndex: false);
			searcher.AddSearchDirectory(assets.Path);

			string searchRootName = System.IO.Path.GetFileName(assets.Path);
			string? result = searcher.FindFile($"{searchRootName}/Config/settings.json");

			AssertPathEqual(result, filePath);
		}

		[Test]
		public void FindFile_WithoutIndex_DoesNotAllowPathTraversalOutsideSearchRoot()
		{
			using var root = new TempDirectory();
			using var outside = new TempDirectory();

			outside.CreateFile("secret.txt", "outside");

			using var searcher = CreateSearcher(useIndex: false);
			searcher.AddSearchDirectory(root.Path);

			string? result = searcher.FindFile("../secret.txt");

			Assert.That(result, Is.Null);
		}

		[Test]
		public void FindFile_WithoutIndex_ReturnsNullForMissingFile()
		{
			using var temp = new TempDirectory();
			using var searcher = CreateSearcher(useIndex: false);

			searcher.AddSearchDirectory(temp.Path);

			string? result = searcher.FindFile("missing.txt");

			Assert.That(result, Is.Null);
		}

		[Test]
		public void FindFile_WithIndex_FindsExactRelativeFile()
		{
			using var temp = new TempDirectory();
			string filePath = temp.CreateFile("Data/map.bin", "map");

			using var searcher = CreateSearcher(useIndex: true);
			searcher.AddSearchDirectory(temp.Path);

			string? result = searcher.FindFile("Data/map.bin");

			AssertPathEqual(result, filePath);
		}

		[Test]
		public void FindFile_WithIndex_ReturnsFirstMatchBySearchDirectoryPriority()
		{
			using var lowPriority = new TempDirectory();
			using var highPriority = new TempDirectory();

			lowPriority.CreateFile("Data/map.bin", "low");
			string expected = highPriority.CreateFile("Data/map.bin", "high");

			using var searcher = CreateSearcher(useIndex: true);
			searcher.AddSearchDirectory(lowPriority.Path);
			searcher.AddSearchDirectory(highPriority.Path, highPriority: true);

			string? result = searcher.FindFile("Data/map.bin");

			AssertPathEqual(result, expected);
		}

		[Test]
		public void FindFile_WithIndex_SupportsProjectStylePathRepeatingSearchRootFolderName()
		{
			using var root = new TempDirectory();
			string filePath = root.CreateFile("Data/map.bin", "map");

			using var searcher = CreateSearcher(useIndex: true);
			searcher.AddSearchDirectory(root.Path);

			string searchRootName = System.IO.Path.GetFileName(root.Path);
			string? result = searcher.FindFile($"{searchRootName}/Data/map.bin");

			AssertPathEqual(result, filePath);
		}

		[Test]
		public void FindFile_WithIndex_ReturnsNullForMissingFile()
		{
			using var temp = new TempDirectory();
			using var searcher = CreateSearcher(useIndex: true);

			searcher.AddSearchDirectory(temp.Path);

			string? result = searcher.FindFile("missing.txt");

			Assert.That(result, Is.Null);
		}

		[Test]
		public void FindFile_WithIndexAndIgnoreCaseTrue_FindsDifferentCaseRelativePath()
		{
			using var temp = new TempDirectory();
			string filePath = temp.CreateFile("Data/MixedCase.Asset", "asset");

			using var searcher = CreateSearcher(ignoreCase: true, useIndex: true);
			searcher.AddSearchDirectory(temp.Path);

			string? result = searcher.FindFile("data/mixedcase.asset");

			AssertPathEqual(result, filePath);
		}

		[Test]
		public void FindFile_WithIndexAndIgnoreCaseFalse_DoesNotFindDifferentCaseRelativePath()
		{
			using var temp = new TempDirectory();
			temp.CreateFile("Data/MixedCase.Asset", "asset");

			using var searcher = CreateSearcher(ignoreCase: false, useIndex: true);
			searcher.AddSearchDirectory(temp.Path);

			string? result = searcher.FindFile("data/mixedcase.asset");

			Assert.That(result, Is.Null);
		}

		[Test]
		public void FindFile_WithIndex_DoesNotSeeFilesCreatedAfterDirectoryWasIndexed()
		{
			using var temp = new TempDirectory();

			using var searcher = CreateSearcher(useIndex: true);
			searcher.AddSearchDirectory(temp.Path);

			temp.CreateFile("late-created.txt", "late");

			string? result = searcher.FindFile("late-created.txt");

			Assert.That(result, Is.Null);
		}

		[Test]
		public void FindAllFiles_WithNullRelativePath_ReturnsEmptyList()
		{
			using var searcher = CreateSearcher();

			List<string> result = searcher.FindAllFiles(null!);

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void FindAllFiles_WithEmptyRelativePath_ReturnsEmptyList()
		{
			using var searcher = CreateSearcher();

			List<string> result = searcher.FindAllFiles(string.Empty);

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void FindAllFiles_WithWhitespaceRelativePath_ReturnsEmptyList()
		{
			using var searcher = CreateSearcher();

			List<string> result = searcher.FindAllFiles("   ");

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void FindAllFiles_WithoutIndex_ReturnsAllMatchesInPriorityOrder()
		{
			using var first = new TempDirectory();
			using var second = new TempDirectory();

			string firstPath = first.CreateFile("Config/settings.json", "first");
			string secondPath = second.CreateFile("Config/settings.json", "second");

			using var searcher = CreateSearcher(useIndex: false);
			searcher.AddSearchDirectory(first.Path);
			searcher.AddSearchDirectory(second.Path);

			List<string> result = searcher.FindAllFiles("Config/settings.json");

			Assert.That(
				result.Select(NormalizeExpectedPath),
				Is.EqualTo(new[]
				{
					NormalizeExpectedPath(firstPath),
					NormalizeExpectedPath(secondPath)
				})
			);
		}

		[Test]
		public void FindAllFiles_WithoutIndex_ReturnsEmptyListWhenNoMatchesExist()
		{
			using var temp = new TempDirectory();
			using var searcher = CreateSearcher(useIndex: false);

			searcher.AddSearchDirectory(temp.Path);

			List<string> result = searcher.FindAllFiles("missing.txt");

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void FindAllFiles_WithoutIndex_SupportsProjectStylePathRepeatingSearchRootFolderName()
		{
			using var root = new TempDirectory();
			string filePath = root.CreateFile("Config/settings.json", "{}");

			using var searcher = CreateSearcher(useIndex: false);
			searcher.AddSearchDirectory(root.Path);

			string searchRootName = System.IO.Path.GetFileName(root.Path);
			List<string> result = searcher.FindAllFiles($"{searchRootName}/Config/settings.json");

			Assert.That(
				result.Select(NormalizeExpectedPath),
				Is.EqualTo(new[] { NormalizeExpectedPath(filePath) })
			);
		}

		[Test]
		public void FindAllFiles_WithIndex_ReturnsAllMatchesInPriorityOrder()
		{
			using var first = new TempDirectory();
			using var second = new TempDirectory();

			string firstPath = first.CreateFile("Data/map.bin", "first");
			string secondPath = second.CreateFile("Data/map.bin", "second");

			using var searcher = CreateSearcher(useIndex: true);
			searcher.AddSearchDirectory(first.Path);
			searcher.AddSearchDirectory(second.Path);

			List<string> result = searcher.FindAllFiles("Data/map.bin");

			Assert.That(
				result.Select(NormalizeExpectedPath),
				Is.EqualTo(new[]
				{
					NormalizeExpectedPath(firstPath),
					NormalizeExpectedPath(secondPath)
				})
			);
		}

		[Test]
		public void FindAllFiles_WithIndex_ReturnsEmptyListWhenNoMatchesExist()
		{
			using var temp = new TempDirectory();
			using var searcher = CreateSearcher(useIndex: true);

			searcher.AddSearchDirectory(temp.Path);

			List<string> result = searcher.FindAllFiles("missing.txt");

			Assert.That(result, Is.Empty);
		}

		[Test]
		public void FindAllFiles_WithIndexAndIgnoreCaseTrue_FindsDifferentCaseRelativePath()
		{
			using var temp = new TempDirectory();
			string filePath = temp.CreateFile("Data/MixedCase.Asset", "asset");

			using var searcher = CreateSearcher(ignoreCase: true, useIndex: true);
			searcher.AddSearchDirectory(temp.Path);

			List<string> result = searcher.FindAllFiles("data/mixedcase.asset");

			Assert.That(
				result.Select(NormalizeExpectedPath),
				Is.EqualTo(new[] { NormalizeExpectedPath(filePath) })
			);
		}

		[Test]
		public void FindDirectory_WithNullPath_ThrowsArgumentException()
		{
			using var searcher = CreateSearcher();

			var ex = Assert.Throws<ArgumentException>(() =>
			{
				searcher.FindDirectory(null!);
			});

			Assert.That(ex!.ParamName, Is.EqualTo("path"));
		}

		[Test]
		public void FindDirectory_WithEmptyPath_ThrowsArgumentException()
		{
			using var searcher = CreateSearcher();

			var ex = Assert.Throws<ArgumentException>(() =>
			{
				searcher.FindDirectory(string.Empty);
			});

			Assert.That(ex!.ParamName, Is.EqualTo("path"));
		}

		[Test]
		public void FindDirectory_WithWhitespacePath_ThrowsArgumentException()
		{
			using var searcher = CreateSearcher();

			var ex = Assert.Throws<ArgumentException>(() =>
			{
				searcher.FindDirectory("   ");
			});

			Assert.That(ex!.ParamName, Is.EqualTo("path"));
		}

		[Test]
		public void FindDirectory_WithRootedExistingPath_ReturnsNormalizedFullPath()
		{
			using var temp = new TempDirectory();
			string directory = temp.CreateDirectory("Config");

			using var searcher = CreateSearcher();

			string? result = searcher.FindDirectory(directory);

			AssertPathEqual(result, System.IO.Path.GetFullPath(directory));
		}

		[Test]
		public void FindDirectory_WithRelativePath_FindsDirectoryInSearchRoot()
		{
			using var temp = new TempDirectory();
			string directory = temp.CreateDirectory("Config/Bindings");

			using var searcher = CreateSearcher();
			searcher.AddSearchDirectory(temp.Path);

			string? result = searcher.FindDirectory("Config/Bindings");

			AssertPathEqual(result, directory);
		}

		[Test]
		public void FindDirectory_WithProjectStylePathRepeatingSearchRootFolderName_FindsDirectory()
		{
			using var root = new TempDirectory();
			string directory = root.CreateDirectory("Config/Bindings");

			using var searcher = CreateSearcher();
			searcher.AddSearchDirectory(root.Path);

			string searchRootName = System.IO.Path.GetFileName(root.Path);
			string? result = searcher.FindDirectory($"{searchRootName}/Config/Bindings");

			AssertPathEqual(result, directory);
		}

		[Test]
		public void FindDirectory_WithPathEqualToSearchRootName_ReturnsSearchRoot()
		{
			using var root = new TempDirectory();

			using var searcher = CreateSearcher();
			searcher.AddSearchDirectory(root.Path);

			string searchRootName = System.IO.Path.GetFileName(root.Path);
			string? result = searcher.FindDirectory(searchRootName);

			AssertPathEqual(result, NormalizeDirectoryExpectedPath(root.Path).TrimEnd(System.IO.Path.DirectorySeparatorChar));
		}

		[Test]
		public void FindDirectory_WithPathTraversalOutsideSearchRoot_ReturnsNullWhenOutsideDirectoryDoesNotMatchFallback()
		{
			using var root = new TempDirectory();
			using var searcher = CreateSearcher();

			searcher.AddSearchDirectory(root.Path);

			string? result = searcher.FindDirectory("../definitely_missing_directory");

			Assert.That(result, Is.Null);
		}

		[Test]
		public void FindDirectory_WithExistingDirectoryButNoSearchRoot_ReturnsNormalizedFullPathViaFallback()
		{
			using var temp = new TempDirectory();

			using var searcher = CreateSearcher();

			string? result = searcher.FindDirectory(temp.Path);

			AssertPathEqual(result, System.IO.Path.GetFullPath(temp.Path));
		}

		[Test]
		public void FindDirectory_ReturnsNullForMissingDirectory()
		{
			using var temp = new TempDirectory();
			using var searcher = CreateSearcher();

			searcher.AddSearchDirectory(temp.Path);

			string? result = searcher.FindDirectory("Missing");

			Assert.That(result, Is.Null);
		}

		[Test]
		public void FindFiles_WithMissingSearchDirectory_ReturnsNull()
		{
			using var searcher = CreateSearcher();

			string missingDirectory = System.IO.Path.Combine(
				TestContext.CurrentContext.WorkDirectory,
				$"missing_{Guid.NewGuid():N}"
			);

			List<string>? result = searcher.FindFiles(missingDirectory, "*.txt");

			Assert.That(result, Is.Null);
		}

		[Test]
		public void FindFiles_WithExistingDirectory_ReturnsRecursiveMatches()
		{
			using var temp = new TempDirectory();

			string rootFile = temp.CreateFile("root.txt", "root");
			string nestedFile = temp.CreateFile("Nested/nested.txt", "nested");
			temp.CreateFile("Nested/ignored.bin", "ignored");

			using var searcher = CreateSearcher();

			List<string>? result = searcher.FindFiles(temp.Path, "*.txt");

			Assert.That(result, Is.Not.Null);

			Assert.That(
				result!.Select(NormalizeExpectedPath).OrderBy(static path => path),
				Is.EqualTo(new[]
				{
					NormalizeExpectedPath(rootFile),
					NormalizeExpectedPath(nestedFile)
				}.OrderBy(static path => path))
			);
		}

		[Test]
		public void FindFiles_WithExistingDirectoryAndNoMatches_ReturnsEmptyList()
		{
			using var temp = new TempDirectory();

			temp.CreateFile("file.bin", "bin");

			using var searcher = CreateSearcher();

			List<string>? result = searcher.FindFiles(temp.Path, "*.txt");

			Assert.That(result, Is.Not.Null);
			Assert.That(result, Is.Empty);
		}

		[Test]
		public void FindFileWithExtensions_ReturnsFirstMatchingExtensionInOrder()
		{
			using var temp = new TempDirectory();

			temp.CreateFile("Config/settings.yaml", "yaml");
			string expected = temp.CreateFile("Config/settings.json", "json");

			using var searcher = CreateSearcher();
			searcher.AddSearchDirectory(temp.Path);

			string? result = searcher.FindFileWithExtensions(
				"Config/settings",
				".json",
				".yaml"
			);

			AssertPathEqual(result, expected);
		}

		[Test]
		public void FindFileWithExtensions_TriesLaterExtensionsWhenEarlierOnesAreMissing()
		{
			using var temp = new TempDirectory();

			string expected = temp.CreateFile("Config/settings.yaml", "yaml");

			using var searcher = CreateSearcher();
			searcher.AddSearchDirectory(temp.Path);

			string? result = searcher.FindFileWithExtensions(
				"Config/settings",
				".json",
				".yaml"
			);

			AssertPathEqual(result, expected);
		}

		[Test]
		public void FindFileWithExtensions_ReturnsNullWhenNoExtensionMatches()
		{
			using var temp = new TempDirectory();

			using var searcher = CreateSearcher();
			searcher.AddSearchDirectory(temp.Path);

			string? result = searcher.FindFileWithExtensions(
				"Config/settings",
				".json",
				".yaml"
			);

			Assert.That(result, Is.Null);
		}

		[Test]
		public void FindFileWithExtensions_WithNoExtensions_ReturnsNull()
		{
			using var temp = new TempDirectory();

			temp.CreateFile("Config/settings.json", "json");

			using var searcher = CreateSearcher();
			searcher.AddSearchDirectory(temp.Path);

			string? result = searcher.FindFileWithExtensions("Config/settings");

			Assert.That(result, Is.Null);
		}

		[Test]
		public void Dispose_CanBeCalledMultipleTimes()
		{
			using var searcher = CreateSearcher(useIndex: true);

			Assert.DoesNotThrow(() =>
			{
				searcher.Dispose();
				searcher.Dispose();
				searcher.Dispose();
			});
		}

		[Test]
		public void Dispose_WithoutIndex_CanBeCalledMultipleTimes()
		{
			using var searcher = CreateSearcher(useIndex: false);

			Assert.DoesNotThrow(() =>
			{
				searcher.Dispose();
				searcher.Dispose();
				searcher.Dispose();
			});
		}

		[Test]
		public void PublicMethods_StillUseExistingStateAfterDisposeBecauseDisposedStateIsNotGuarded()
		{
			using var temp = new TempDirectory();
			string filePath = temp.CreateFile("Config/settings.json", "{}");

			var searcher = CreateSearcher(useIndex: false);
			searcher.AddSearchDirectory(temp.Path);

			searcher.Dispose();

			string? result = searcher.FindFile("Config/settings.json");

			AssertPathEqual(result, filePath);
		}
	}
}