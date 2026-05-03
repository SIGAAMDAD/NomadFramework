using System.Diagnostics;
using System.Text;

static (int Code, string Output, string Error) Git(string repo, string args) {
	using var process = Process.Start(new ProcessStartInfo("git", args) {
		WorkingDirectory = repo,
		RedirectStandardOutput = true,
		RedirectStandardError = true,
		UseShellExecute = false
	})!;

	string output = process.StandardOutput.ReadToEnd();
	string error = process.StandardError.ReadToEnd();
	process.WaitForExit();
	return (process.ExitCode, output, error);
}

static string? Module(string path) {
	string[] parts = path.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);

	if (parts.Length >= 2 && parts[0] == "Source" && parts[1].StartsWith("Nomad.", StringComparison.Ordinal)) {
		return parts[1];
	}

	if (parts.Length >= 2 && parts[0] == "Tests" && parts[1].StartsWith("Nomad.", StringComparison.Ordinal)) {
		return parts[1].Replace(".Tests", "", StringComparison.Ordinal);
	}

	return null;
}

static string Kind(string subject) {
	subject = subject.ToLowerInvariant();
	if (subject.StartsWith("feat", StringComparison.Ordinal)) {
		return "Added";
	}

	if (subject.StartsWith("fix", StringComparison.Ordinal)) {
		return "Fixed";
	}

	if (subject.StartsWith("docs", StringComparison.Ordinal)) {
		return "Docs";
	}

	if (subject.StartsWith("test", StringComparison.Ordinal)) {
		return "Tests";
	}

	if (subject.StartsWith("perf", StringComparison.Ordinal)) {
		return "Performance";
	}

	return "Changed";
}

static string Color(string code, string text) => Console.IsOutputRedirected ? text : $"\u001b[{code}m{text}\u001b[0m";
static string Red(string text) => Color("31", text);
static string Green(string text) => Color("32", text);

string repo = Directory.GetCurrentDirectory();
string from = "";
string to = "HEAD";
string outputPath = "";

for (int i = 0; i < args.Length; i++) {
	if (args[i] == "--repo-root" && i + 1 < args.Length) {
		repo = Path.GetFullPath(args[++i]);
	} else if (args[i] == "--from" && i + 1 < args.Length) {
		from = args[++i];
	} else if (args[i] == "--to" && i + 1 < args.Length) {
		to = args[++i];
	} else if (args[i] == "--output" && i + 1 < args.Length) {
		outputPath = Path.GetFullPath(args[++i]);
	}
}

if (from == "") {
	var tag = Git(repo, "describe --tags --abbrev=0");
	from = tag.Code == 0 ? tag.Output.Trim() : "HEAD~50";
}

if (outputPath == "") {
	outputPath = Path.Combine(repo, "Artifacts", "ReleaseNotes", "RELEASE_NOTES.md");
}

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

var log = Git(repo, $"log --name-only --pretty=format:%H%x1f%s%x1e {from}..{to}");
if (log.Code != 0) {
	Console.Error.WriteLine($"{Red("ERROR")}: {log.Error}");
	Environment.Exit(1);
}

var groups = new SortedDictionary<string, List<Commit>>(StringComparer.Ordinal);

foreach (string record in log.Output.Split('\x1e', StringSplitOptions.RemoveEmptyEntries)) {
	string[] lines = record.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
	if (lines.Length == 0) {
		continue;
	}

	string[] head = lines[0].Split('\x1f');
	if (head.Length < 2) {
		continue;
	}

	string sha = head[0];
	string subject = head[1];
	IEnumerable<string?> modules = lines.Skip(1).Select(Module).Where(module => module != null).Distinct().DefaultIfEmpty("Repository");

	foreach (string? module in modules) {
		groups.TryAdd(module!, []);
		groups[module!].Add(new Commit(sha, subject));
	}
}

var builder = new StringBuilder($"# Release Notes\n\nRange: `{from}..{to}`\n\n");
foreach (var group in groups) {
	builder.AppendLine($"## {group.Key}\n");
	foreach (Commit commit in group.Value.Distinct()) {
		builder.AppendLine($"- **{Kind(commit.Subject)}:** {commit.Subject} (`{commit.Sha[..Math.Min(7, commit.Sha.Length)]}`)");
	}

	builder.AppendLine();
}

File.WriteAllText(outputPath, builder.ToString());
Console.WriteLine($"{Green("SUCCESS")}: Generated {outputPath}");

internal sealed record Commit(string Sha, string Subject);
