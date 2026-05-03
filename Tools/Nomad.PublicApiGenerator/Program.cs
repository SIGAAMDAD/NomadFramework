using System.Text.RegularExpressions;

static bool Skip(string path) {
	string normalized = path.Replace('\\', '/');
	return normalized.Contains("/bin/", StringComparison.Ordinal)
		|| normalized.Contains("/obj/", StringComparison.Ordinal)
		|| path.EndsWith(".g.cs", StringComparison.Ordinal);
}

static string NormalizeWhitespace(string text) => Regex.Replace(text.Trim(), @"\s+", " ");

static bool IsPublicDeclaration(string text) =>
	Regex.IsMatch(text, @"^(public|protected public)\b")
	&& !text.StartsWith("public const", StringComparison.Ordinal);

static string StripBody(string text) {
	int braceIndex = text.IndexOf('{', StringComparison.Ordinal);
	if (braceIndex >= 0) {
		text = text[..braceIndex];
	}

	return text.Trim().TrimEnd(';');
}

static string Color(string code, string text) => Console.IsOutputRedirected ? text : $"\u001b[{code}m{text}\u001b[0m";
static string Green(string text) => Color("32", text);
static string Yellow(string text) => Color("33", text);

string repo = Directory.GetCurrentDirectory();
string outDir = "";
bool shipped = false;
bool fail = false;

for (int i = 0; i < args.Length; i++) {
	if (args[i] == "--repo-root" && i + 1 < args.Length) {
		repo = Path.GetFullPath(args[++i]);
	} else if (args[i] == "--output" && i + 1 < args.Length) {
		outDir = Path.GetFullPath(args[++i]);
	} else if (args[i] == "--shipped") {
		shipped = true;
	} else if (args[i] == "--fail-on-diff") {
		fail = true;
	}
}

outDir = outDir == "" ? Path.Combine(repo, "ApiSurface") : outDir;
Directory.CreateDirectory(outDir);

int changed = 0;
foreach (string moduleDir in Directory.GetDirectories(Path.Combine(repo, "Source"), "Nomad.*").Order()) {
	string publicDir = Path.Combine(moduleDir, "Public");
	if (!Directory.Exists(publicDir)) {
		continue;
	}

	string module = Path.GetFileName(moduleDir);
	var lines = new SortedSet<string>(StringComparer.Ordinal);

	foreach (string file in Directory.GetFiles(publicDir, "*.cs", SearchOption.AllDirectories).Where(path => !Skip(path)).Order()) {
		foreach (string rawLine in File.ReadLines(file)) {
			string line = NormalizeWhitespace(rawLine);
			if (IsPublicDeclaration(line)) {
				lines.Add($"{Path.GetRelativePath(publicDir, file).Replace('\\', '/')}: {StripBody(line)}");
			}
		}
	}

	string path = Path.Combine(outDir, $"{module}.PublicAPI.{(shipped ? "Shipped" : "Unshipped")}.txt");
	string text = string.Join(Environment.NewLine, lines) + Environment.NewLine;

	if (!File.Exists(path) || File.ReadAllText(path) != text) {
		changed++;
		string label = File.Exists(path) ? Yellow("CHANGED") : Green("NEW");
		Console.WriteLine($"{label}: {Path.GetRelativePath(repo, path).Replace('\\', '/')}");
	}

	File.WriteAllText(path, text);
}

string summaryLabel = changed == 0 ? Green("SUCCESS") : Yellow("CHANGED");
Console.WriteLine($"{summaryLabel}: Public API files changed or created: {changed}");
Environment.Exit(fail && changed > 0 ? 1 : 0);
