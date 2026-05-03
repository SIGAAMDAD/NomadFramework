using System.Xml.Linq;

static string? PropertyValue(XDocument document, string name) =>
	document.Descendants().FirstOrDefault(element => element.Name.LocalName == name)?.Value.Trim();

static bool IsTrue(string? value) => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
static bool IsFalse(string? value) => string.Equals(value, "false", StringComparison.OrdinalIgnoreCase);

static bool Ignored(string path) {
	string normalized = path.Replace('\\', '/');
	return normalized.Contains("/bin/", StringComparison.Ordinal)
		|| normalized.Contains("/obj/", StringComparison.Ordinal);
}

static void AddFinding(List<Finding> findings, string level, string path, string message) =>
	findings.Add(new Finding(level, path, message));

static string Color(string code, string text) => Console.IsOutputRedirected ? text : $"\u001b[{code}m{text}\u001b[0m";
static string Red(string text) => Color("31", text);
static string Green(string text) => Color("32", text);
static string Yellow(string text) => Color("33", text);

static string ColorLevel(string level) =>
	level switch {
		"ERROR" => Red(level),
		"WARN" => Yellow(level),
		_ => level
	};

string repo = Directory.GetCurrentDirectory();
bool strict = false;
bool warnAsError = false;

for (int i = 0; i < args.Length; i++) {
	if (args[i] == "--repo-root" && i + 1 < args.Length) {
		repo = Path.GetFullPath(args[++i]);
	} else if (args[i] == "--strict") {
		strict = true;
	} else if (args[i] == "--warnings-as-errors") {
		warnAsError = true;
	} else if (args[i] is "--help" or "-h") {
		Console.WriteLine("Usage: Nomad.PackageValidator [--repo-root <path>] [--strict] [--warnings-as-errors]");
		return;
	}
}

string source = Path.Combine(repo, "Source");
if (!Directory.Exists(source)) {
	Console.Error.WriteLine($"{Red("ERROR")}: Source not found: {source}");
	Environment.Exit(2);
}

var findings = new List<Finding>();

foreach (string project in Directory.GetFiles(source, "*.csproj", SearchOption.AllDirectories).Where(path => !Ignored(path)).Order()) {
	XDocument document;
	try {
		document = XDocument.Load(project);
	} catch (Exception exception) {
		AddFinding(findings, "ERROR", project, $"XML parse failed: {exception.Message}");
		continue;
	}

	if (IsTrue(PropertyValue(document, "IsTool"))
		|| IsTrue(PropertyValue(document, "IsSampleProject"))
		|| IsFalse(PropertyValue(document, "IsPackable"))) {
		continue;
	}

	string module = Path.GetFileNameWithoutExtension(project);
	string? packageId = PropertyValue(document, "PackageId");
	string? version = PropertyValue(document, "Version");
	string? description = PropertyValue(document, "Description");
	string directory = Path.GetDirectoryName(project)!;

	if (string.IsNullOrWhiteSpace(packageId)) {
		AddFinding(findings, "ERROR", project, "PackageId is required.");
	} else {
		if (!packageId.StartsWith("NomadFramework.", StringComparison.Ordinal)) {
			AddFinding(findings, "ERROR", project, $"PackageId '{packageId}' must start with NomadFramework.");
		}

		if (packageId.Contains(".Nomad.", StringComparison.Ordinal)) {
			AddFinding(findings, "ERROR", project, $"PackageId '{packageId}' contains redundant .Nomad. segment.");
		}

		string suffix = module.StartsWith("Nomad.", StringComparison.Ordinal) ? module["Nomad.".Length..] : module;
		string expected = $"NomadFramework.{suffix}";
		if (!string.Equals(packageId, expected, StringComparison.Ordinal)) {
			AddFinding(findings, "WARN", project, $"PackageId is '{packageId}', expected '{expected}'.");
		}
	}

	if (string.IsNullOrWhiteSpace(description)) {
		AddFinding(findings, "ERROR", project, "Description is required for packable modules.");
	}

	if (strict && string.IsNullOrWhiteSpace(version)) {
		AddFinding(findings, "WARN", project, "Version is not declared in this project; this is okay only if centralized.");
	}

	if (!File.Exists(Path.Combine(directory, "README.md"))) {
		AddFinding(findings, "WARN", project, "Module README.md is missing.");
	}

	if (!File.Exists(Path.Combine(directory, "ROADMAP.md"))) {
		AddFinding(findings, "WARN", project, "Module ROADMAP.md is missing.");
	}
}

foreach (Finding finding in findings) {
	Console.WriteLine($"{ColorLevel(finding.Level)}: {Path.GetRelativePath(repo, finding.Path).Replace('\\', '/')}: {finding.Message}");
}

int errors = findings.Count(finding => finding.Level == "ERROR");
int warnings = findings.Count(finding => finding.Level == "WARN");
string summaryLabel = errors == 0 && (!warnAsError || warnings == 0) ? Green("SUCCESS") : Red("ERROR");
Console.WriteLine($"{summaryLabel}: Validated packages. Errors: {Red(errors.ToString())}, Warnings: {Yellow(warnings.ToString())}");
Environment.Exit(errors > 0 || (warnAsError && warnings > 0) ? 1 : 0);

internal sealed record Finding(string Level, string Path, string Message);
