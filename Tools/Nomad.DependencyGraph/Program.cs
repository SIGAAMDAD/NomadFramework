using System.Text;
using System.Xml.Linq;

static bool Skip(string path) {
	string normalized = path.Replace('\\', '/');
	return normalized.Contains("/bin/", StringComparison.Ordinal)
		|| normalized.Contains("/obj/", StringComparison.Ordinal);
}

static string Color(string code, string text) => Console.IsOutputRedirected ? text : $"\u001b[{code}m{text}\u001b[0m";
static string Green(string text) => Color("32", text);
static string Yellow(string text) => Color("33", text);

string repo = Directory.GetCurrentDirectory();
string outDir = "";
bool fail = true;

for (int i = 0; i < args.Length; i++) {
	if (args[i] == "--repo-root" && i + 1 < args.Length) {
		repo = Path.GetFullPath(args[++i]);
	} else if (args[i] == "--output" && i + 1 < args.Length) {
		outDir = Path.GetFullPath(args[++i]);
	} else if (args[i] == "--no-fail-on-cycle") {
		fail = false;
	}
}

outDir = outDir == "" ? Path.Combine(repo, "Artifacts", "DependencyGraph") : outDir;
Directory.CreateDirectory(outDir);

var graph = new Dictionary<string, List<string>>(StringComparer.Ordinal);

foreach (string project in Directory.GetFiles(Path.Combine(repo, "Source"), "*.csproj", SearchOption.AllDirectories).Where(path => !Skip(path)).Order()) {
	string module = Path.GetFileNameWithoutExtension(project);
	XDocument document = XDocument.Load(project);

	graph[module] = document
		.Descendants()
		.Where(element => element.Name.LocalName == "ProjectReference")
		.Select(element => element.Attribute("Include")?.Value)
		.Where(value => !string.IsNullOrWhiteSpace(value))
		.Select(value => Path.GetFileNameWithoutExtension(value!))
		.Where(value => value.StartsWith("Nomad.", StringComparison.Ordinal))
		.Distinct()
		.Order()
		.ToList();
}

foreach (string dependency in graph.Values.SelectMany(value => value).Distinct()) {
	graph.TryAdd(dependency, []);
}

var markdown = new StringBuilder("# NomadFramework Dependency Graph\n\n| Module | Depends On |\n|---|---|\n");
foreach (var item in graph.OrderBy(item => item.Key)) {
	markdown.AppendLine($"| `{item.Key}` | {string.Join(", ", item.Value.Select(value => $"`{value}`"))} |");
}

var dot = new StringBuilder("digraph NomadFramework {\n  rankdir=LR;\n  node [shape=box];\n");
foreach (var item in graph.OrderBy(item => item.Key)) {
	dot.AppendLine($"  \"{item.Key}\";");
	foreach (string dependency in item.Value) {
		dot.AppendLine($"  \"{item.Key}\" -> \"{dependency}\";");
	}
}

dot.AppendLine("}");

File.WriteAllText(Path.Combine(outDir, "dependency-graph.md"), markdown.ToString());
File.WriteAllText(Path.Combine(outDir, "dependency-graph.dot"), dot.ToString());

var cycles = new List<List<string>>();
var visiting = new HashSet<string>();
var visited = new HashSet<string>();
var path = new List<string>();

void Dfs(string node) {
	if (visiting.Contains(node)) {
		int index = path.IndexOf(node);
		if (index >= 0) {
			cycles.Add(path.Skip(index).Append(node).ToList());
		}

		return;
	}

	if (!visited.Add(node)) {
		return;
	}

	visiting.Add(node);
	path.Add(node);

	if (graph.TryGetValue(node, out List<string>? dependencies)) {
		foreach (string dependency in dependencies) {
			Dfs(dependency);
		}
	}

	path.RemoveAt(path.Count - 1);
	visiting.Remove(node);
}

foreach (string node in graph.Keys) {
	Dfs(node);
}

foreach (var cycle in cycles) {
	Console.WriteLine($"{Yellow("CYCLE")}: {string.Join(" -> ", cycle)}");
}

string summaryLabel = cycles.Count == 0 ? Green("SUCCESS") : Yellow("WARNING");
Console.WriteLine($"{summaryLabel}: Modules: {graph.Count}, Cycles: {Yellow(cycles.Count.ToString())}. Output: {outDir}");
Environment.Exit(fail && cycles.Count > 0 ? 1 : 0);
