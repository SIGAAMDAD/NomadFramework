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
bool warnAsError = false;

for (int i = 0; i < args.Length; i++) {
	if (args[i] == "--repo-root" && i + 1 < args.Length) {
		repo = Path.GetFullPath(args[++i]);
	} else if (args[i] == "--warnings-as-errors") {
		warnAsError = true;
	}
}

var findings = new List<Finding>();
string testsRoot = Path.Combine(repo, "Tests");
string sourceRoot = Path.Combine(repo, "Source");

if (!Directory.Exists(testsRoot)) {
	Console.Error.WriteLine($"{Yellow("MISSING")}: Tests directory missing.");
	Environment.Exit(2);
}

foreach (string project in Directory.GetFiles(testsRoot, "*.csproj", SearchOption.AllDirectories).Where(path => !Ignored(path)).Order()) {
	XDocument document = XDocument.Load(project);
	string name = Path.GetFileNameWithoutExtension(project);

	if (!name.EndsWith(".Tests", StringComparison.Ordinal)) {
		findings.Add(new Finding("WARN", project, "Test project name should end with .Tests."));
	}

	if (!IsTrue(PropertyValue(document, "IsTestProject"))) {
		findings.Add(new Finding("ERROR", project, "IsTestProject must be true."));
	}

	if (PropertyValue(document, "IsPackable") is string packable && !IsFalse(packable)) {
		findings.Add(new Finding("ERROR", project, "IsPackable must be false."));
	}

	string module = name.EndsWith(".Tests", StringComparison.Ordinal) ? name[..^".Tests".Length] : name;
	bool refsModule = document
		.Descendants()
		.Where(element => element.Name.LocalName == "ProjectReference")
		.Select(element => element.Attribute("Include")?.Value.Replace('\\', '/') ?? "")
		.Any(include => include.Contains($"Source/{module}/{module}.csproj", StringComparison.Ordinal)
			|| include.Contains($"{module}/{module}.csproj", StringComparison.Ordinal));

	if (!refsModule) {
		findings.Add(new Finding("WARN", project, $"Does not directly reference expected module {module}."));
	}

	foreach (string directory in new[] { "Public", "Private", "Integration", "Fixtures" }) {
		if (!Directory.Exists(Path.Combine(Path.GetDirectoryName(project)!, directory))) {
			findings.Add(new Finding("WARN", project, $"Recommended directory missing: {directory}/"));
		}
	}
}

if (Directory.Exists(sourceRoot)) {
	foreach (string project in Directory.GetFiles(sourceRoot, "Nomad.*.csproj", SearchOption.AllDirectories).Where(path => !Ignored(path)).Order()) {
		XDocument document = XDocument.Load(project);
		if (IsTrue(PropertyValue(document, "IsTool"))) {
			continue;
		}

		string module = Path.GetFileNameWithoutExtension(project);
		string expected = Path.Combine(testsRoot, $"{module}.Tests", $"{module}.Tests.csproj");
		if (!File.Exists(expected)) {
			findings.Add(new Finding("WARN", project, $"Missing matching test project: Tests/{module}.Tests/{module}.Tests.csproj"));
		}
	}
}

foreach (Finding finding in findings) {
	Console.WriteLine($"{ColorLevel(finding.Level)}: {Path.GetRelativePath(repo, finding.Path).Replace('\\', '/')}: {finding.Message}");
}

int errors = findings.Count(finding => finding.Level == "ERROR");
int warnings = findings.Count(finding => finding.Level == "WARN");
string summaryLabel = errors == 0 && (!warnAsError || warnings == 0) ? Green("SUCCESS") : Red("ERROR");
Console.WriteLine($"{summaryLabel}: Test validation complete. Errors: {Red(errors.ToString())}, Warnings: {Yellow(warnings.ToString())}");
Environment.Exit(errors > 0 || (warnAsError && warnings > 0) ? 1 : 0);

internal sealed record Finding(string Level, string Path, string Message);
