using System.Text.Json.Nodes;
using System.Xml.Linq;

static string? PropertyValue(XDocument document, string name) =>
	document.Descendants().FirstOrDefault(element => element.Name.LocalName == name)?.Value.Trim();

static string[] TargetFrameworks(XDocument document) {
	string? value = PropertyValue(document, "TargetFrameworks") ?? PropertyValue(document, "TargetFramework");
	return string.IsNullOrWhiteSpace(value)
		? ["inherited"]
		: value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

static bool Skip(string path) {
	string normalized = path.Replace('\\', '/');
	return normalized.Contains("/bin/", StringComparison.Ordinal)
		|| normalized.Contains("/obj/", StringComparison.Ordinal);
}

static string Mark(bool value) => value ? "yes" : "no";
static string Color(string code, string text) => Console.IsOutputRedirected ? text : $"\u001b[{code}m{text}\u001b[0m";
static string Green(string text) => Color("32", text);

string repo = Directory.GetCurrentDirectory();
string output = "";

for (int i = 0; i < args.Length; i++) {
	if (args[i] == "--repo-root" && i + 1 < args.Length) {
		repo = Path.GetFullPath(args[++i]);
	} else if (args[i] == "--output" && i + 1 < args.Length) {
		output = Path.GetFullPath(args[++i]);
	}
}

output = output == "" ? Path.Combine(repo, "Documentation", "CompatibilityMatrix.md") : output;
Directory.CreateDirectory(Path.GetDirectoryName(output)!);

var manifest = new Dictionary<string, JsonObject>();
string manifestPath = Path.Combine(repo, "NomadSubsystems.json");

if (File.Exists(manifestPath)) {
	var root = JsonNode.Parse(File.ReadAllText(manifestPath))!.AsObject();
	foreach (JsonObject subsystem in root["Subsystems"]?.AsArray()?.OfType<JsonObject>() ?? []) {
		string? name = subsystem["Name"]?.GetValue<string>();
		if (!string.IsNullOrWhiteSpace(name)) {
			manifest[name] = subsystem;
		}
	}
}

var rows = new List<string>();

foreach (string project in Directory.GetFiles(Path.Combine(repo, "Source"), "Nomad.*.csproj", SearchOption.AllDirectories).Where(path => !Skip(path)).Order()) {
	string name = Path.GetFileNameWithoutExtension(project);
	XDocument document = XDocument.Load(project);
	manifest.TryGetValue(name, out JsonObject? subsystem);

	string category = subsystem?["Category"]?.GetValue<string>() ?? "Uncategorized";
	string description = (subsystem?["Description"]?.GetValue<string>() ?? PropertyValue(document, "Description") ?? "").Replace("|", "\\|", StringComparison.Ordinal);
	string status = subsystem?["Status"]?.GetValue<string>() ?? (PropertyValue(document, "Version")?.Contains("alpha", StringComparison.OrdinalIgnoreCase) == true ? "alpha" : "unknown");
	string[] targetFrameworks = TargetFrameworks(document);
	bool unity = subsystem?["UnityCompatible"]?.GetValue<bool>() ?? targetFrameworks.Contains("netstandard2.1");
	bool godot = subsystem?["GodotCompatible"]?.GetValue<bool>() ?? name.Contains("Godot", StringComparison.Ordinal);
	bool headless = subsystem?["HeadlessCompatible"]?.GetValue<bool>() ?? !name.Contains("EngineUtils", StringComparison.Ordinal);

	rows.Add($"| `{name}` | {category} | {status} | {string.Join(", ", targetFrameworks.Select(tfm => $"`{tfm}`"))} | {Mark(unity)} | {Mark(godot)} | {Mark(headless)} | {description} |");
}

File.WriteAllText(
	output,
	"# NomadFramework Compatibility Matrix\n\n| Module | Category | Status | TFMs | Unity | Godot | Headless | Description |\n|---|---|---:|---|---:|---:|---:|---|\n"
		+ string.Join(Environment.NewLine, rows)
		+ Environment.NewLine);

Console.WriteLine($"{Green("SUCCESS")}: Wrote {output}");
