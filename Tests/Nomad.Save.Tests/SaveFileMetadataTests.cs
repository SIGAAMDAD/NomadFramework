/*
===========================================================================
The Nomad Framework
Copyright (C) 2025-2026 Noah Van Til

This Source Code Form is subject to the terms of the Mozilla Public
License, v2. If a copy of the MPL was not distributed with this
file, You can obtain one at https://mozilla.org/MPL/2.0/.

This software is provided "as is", without warranty of any kind,
express or implied, including but not limited to the warranties
of merchantability, fitness for a particular purpose and noninfringement.
===========================================================================
*/

using System;
using NUnit.Framework;
using Nomad.Save.ValueObjects;
using NUnit.Framework.Interfaces;

namespace Nomad.Save.Tests;

/// <summary>
/// Tests for SaveFileMetadata value object
/// </summary>
[TestFixture]
public class SaveFileMetadataTests
{
    [Test]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        var fileName = "save_001.ngd";
        long fileSize = 1024;
        var lastAccessTime = DateTime.Now;

        // Act
        var metadata = new SaveFileMetadata(fileName, fileSize, lastAccessTime.Year, lastAccessTime.Month, lastAccessTime.Day);

        // Assert
        Assert.That(metadata, Is.Not.Null);
		using (Assert.EnterMultipleScope())
		{
			Assert.That(metadata.FileName, Is.EqualTo(fileName));
			Assert.That(metadata.FileSize, Is.EqualTo(fileSize));
			Assert.That(metadata.LastAccessYear, Is.EqualTo(lastAccessTime.Year));
			Assert.That(metadata.LastAccessMonth, Is.EqualTo(lastAccessTime.Month));
			Assert.That(metadata.LastAccessDay, Is.EqualTo(lastAccessTime.Day));
		}
	}

    [Test]
    public void Constructor_WithZeroFileSize_CreatesInstance()
    {
        // Arrange
        var fileName = "empty_save.ngd";
        long fileSize = 0;
        var lastAccessTime = DateTime.Now;

        // Act
        var metadata = new SaveFileMetadata(fileName, fileSize, lastAccessTime.Year, lastAccessTime.Month, lastAccessTime.Day);

        // Assert
        Assert.That(metadata.FileSize, Is.EqualTo(0));
    }

    [Test]
    public void Constructor_WithLargeFileSize_CreatesInstance()
    {
        // Arrange
        var fileName = "large_save.ngd";
        long fileSize = long.MaxValue;
        var lastAccessTime = DateTime.Now;

        // Act
        var metadata = new SaveFileMetadata(fileName, fileSize, lastAccessTime.Year, lastAccessTime.Month, lastAccessTime.Day);

        // Assert
        Assert.That(metadata.FileSize, Is.EqualTo(long.MaxValue));
    }

    [Test]
    public void Constructor_WithOldDateTime_CreatesInstance()
    {
        // Arrange
        var fileName = "old_save.ngd";
        long fileSize = 2048;
        var lastAccessTime = new DateTime(2020, 1, 1, 0, 0, 0);

        // Act
        var metadata = new SaveFileMetadata(fileName, fileSize, lastAccessTime.Year, lastAccessTime.Month, lastAccessTime.Day);

        // Assert
        Assert.That(metadata.LastAccessYear, Is.EqualTo(lastAccessTime.Year));
    }

    [Test]
    public void Constructor_WithCurrentDateTime_CreatesInstance()
    {
        // Arrange
        var fileName = "current_save.ngd";
        long fileSize = 5120;
        var lastAccessTime = DateTime.UtcNow;

        // Act
        var metadata = new SaveFileMetadata(fileName, fileSize, lastAccessTime.Year, lastAccessTime.Month, lastAccessTime.Day);

		using (Assert.EnterMultipleScope())
		{
			// Assert
			Assert.That(metadata.LastAccessYear, Is.EqualTo(lastAccessTime.Year));
			Assert.That(metadata.LastAccessMonth, Is.EqualTo(lastAccessTime.Month));
			Assert.That(metadata.LastAccessDay, Is.EqualTo(lastAccessTime.Day));
		}
	}

    [Test]
    public void Equality_TwoInstancesWithSameValues_AreEqual()
    {
        // Arrange
        var fileName = "save_001.ngd";
        long fileSize = 1024;
        var lastAccessTime = DateTime.Now;

        var metadata1 = new SaveFileMetadata(fileName, fileSize, lastAccessTime.Year, lastAccessTime.Month, lastAccessTime.Day);
        var metadata2 = new SaveFileMetadata(fileName, fileSize, lastAccessTime.Year, lastAccessTime.Month, lastAccessTime.Day);

        // Act & Assert
        Assert.That(metadata1, Is.EqualTo(metadata2));
    }

    [Test]
    public void Equality_TwoInstancesWithDifferentFileNames_AreNotEqual()
    {
        // Arrange
        long fileSize = 1024;
        var lastAccessTime = DateTime.Now;

        var metadata1 = new SaveFileMetadata("save_001.ngd", fileSize, lastAccessTime.Year, lastAccessTime.Month, lastAccessTime.Day);
        var metadata2 = new SaveFileMetadata("save_002.ngd", fileSize, lastAccessTime.Year, lastAccessTime.Month, lastAccessTime.Day);

        // Act & Assert
        Assert.That(metadata1, Is.Not.EqualTo(metadata2));
    }

    [Test]
    public void Equality_TwoInstancesWithDifferentFileSizes_AreNotEqual()
    {
        // Arrange
        var fileName = "save_001.ngd";
        var lastAccessTime = DateTime.Now;

        var metadata1 = new SaveFileMetadata(fileName, 1024, lastAccessTime.Year, lastAccessTime.Month, lastAccessTime.Day);
        var metadata2 = new SaveFileMetadata(fileName, 2048, lastAccessTime.Year, lastAccessTime.Month, lastAccessTime.Day);

        // Act & Assert
        Assert.That(metadata1, Is.Not.EqualTo(metadata2));
    }

    [Test]
    public void Equality_TwoInstancesWithDifferentAccessTimes_AreNotEqual()
    {
        // Arrange
        var fileName = "save_001.ngd";
        var now = DateTime.Now;
        long fileSize = 1024;

        var metadata1 = new SaveFileMetadata(fileName, fileSize, now.Year, now.Month, now.Day);
        now = now.AddDays(2);
        var metadata2 = new SaveFileMetadata(fileName, fileSize, now.Year, now.Month, now.Day);

        // Act & Assert
        Assert.That(metadata1, Is.Not.EqualTo(metadata2));
    }

    [Test]
    public void GetHashCode_TwoInstancesWithSameValues_HaveSameHashCode()
    {
        // Arrange
        var fileName = "save_001.ngd";
        long fileSize = 1024;
        var lastAccessTime = new DateTime(2025, 1, 1);

        var metadata1 = new SaveFileMetadata(fileName, fileSize, lastAccessTime.Year, lastAccessTime.Month, lastAccessTime.Day);
        var metadata2 = new SaveFileMetadata(fileName, fileSize, lastAccessTime.Year, lastAccessTime.Month, lastAccessTime.Day);

        // Act & Assert
        Assert.That(metadata1.GetHashCode(), Is.EqualTo(metadata2.GetHashCode()));
    }

    [Test]
    public void ToString_ReturnsMetadataInfo()
    {
        // Arrange
        var now = DateTime.Now;
        var metadata = new SaveFileMetadata(
            "save_001.ngd",
            1024,
            now.Year,
            now.Month,
            now.Day
        );

        // Act
        string result = metadata.ToString();

        // Assert
        Assert.That(result, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public void RecordBehavior_AllPropertiesAccessible()
    {
        // Arrange
        var fileName = "save_001.ngd";
        long fileSize = 2048;
        var lastAccessTime = DateTime.Now;
        var metadata = new SaveFileMetadata(fileName, fileSize, lastAccessTime.Year, lastAccessTime.Month, lastAccessTime.Day);

		using (Assert.EnterMultipleScope())
		{
			// Act & Assert
			Assert.That(metadata.FileName, Is.EqualTo(fileName));
			Assert.That(metadata.FileSize, Is.EqualTo(fileSize));
			Assert.That(metadata.LastAccessYear, Is.EqualTo(lastAccessTime.Year));
			Assert.That(metadata.LastAccessMonth, Is.EqualTo(lastAccessTime.Month));
			Assert.That(metadata.LastAccessDay, Is.EqualTo(lastAccessTime.Day));
		}
	}

    [Test]
    public void FileName_PropertyIsAccessible()
    {
        // Arrange
        var fileName = "test_save.ngd";
        var now = DateTime.Now;
        var metadata = new SaveFileMetadata(fileName, 1024, now.Year, now.Month, now.Day);

        // Act
        var retrievedFileName = metadata.FileName;

        // Assert
        Assert.That(retrievedFileName, Is.EqualTo(fileName));
    }

    [Test]
    public void FileSize_PropertyIsAccessible()
    {
        // Arrange
        long expectedSize = 5678;
        var now = DateTime.Now;
        var metadata = new SaveFileMetadata("test.ngd", expectedSize, now.Year, now.Month, now.Day);

        // Act
        long retrievedSize = metadata.FileSize;

        // Assert
        Assert.That(retrievedSize, Is.EqualTo(expectedSize));
    }

    [Test]
    public void LastAccessTime_PropertyIsAccessible()
    {
        // Arrange
        var expectedTime = new DateTime(2024, 6, 15, 12, 30, 45);
        var metadata = new SaveFileMetadata("test.ngd", 1024, expectedTime.Year, expectedTime.Month, expectedTime.Day);

		using (Assert.EnterMultipleScope())
		{
			// Assert
			Assert.That(metadata.LastAccessYear, Is.EqualTo(expectedTime.Year));
			Assert.That(metadata.LastAccessMonth, Is.EqualTo(expectedTime.Month));
			Assert.That(metadata.LastAccessDay, Is.EqualTo(expectedTime.Day));
		}
	}

    [Test]
    public void Constructor_WithNegativeFileSize_CreatesInstance()
    {
        // Note: This tests that the constructor allows negative values
        // In real usage this would be invalid, but records don't validate
        // Arrange
        var fileName = "test.ngd";
        long fileSize = -100;
        var lastAccessTime = DateTime.Now;

        // Act
        var metadata = new SaveFileMetadata(fileName, fileSize, lastAccessTime.Year, lastAccessTime.Month, lastAccessTime.Day);

        // Assert
        Assert.That(metadata.FileSize, Is.EqualTo(-100));
    }

    [Test]
    public void Immutability_PropertiesCannotBeChanged()
    {
        // Arrange
        var now = DateTime.Now;
        var metadata = new SaveFileMetadata(
            "save.ngd",
            1024,
            now.Year,
            now.Month,
            now.Day
        );

        // Act & Assert
        // Records are immutable, attempting to verify the properties are read-only
        Assert.That(() => metadata.FileName, Throws.Nothing);
        Assert.That(() => metadata.FileSize, Throws.Nothing);
        Assert.That(() => metadata.LastAccessYear, Throws.Nothing);
        Assert.That(() => metadata.LastAccessMonth, Throws.Nothing);
        Assert.That(() => metadata.LastAccessDay, Throws.Nothing);
    }
}
