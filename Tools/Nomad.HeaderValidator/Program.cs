const string Header = """
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

""";

static bool Skip(string path) {
	string normalized = path.Replace('\\', '/');
	return normalized.Contains("/bin/")
		|| normalized.Contains("/obj/")
		|| normalized.Contains("/Generated/")
		|| normalized.EndsWith(".g.cs", StringComparison.Ordinal);
}

static bool HasHeader(string text) {
	text = text.TrimStart('\uFEFF').TrimStart();
	return text.StartsWith("/*", StringComparison.Ordinal)
		&& text.Contains("The Nomad Framework", StringComparison.Ordinal)
		&& text.Contains("Mozilla Public", StringComparison.Ordinal)
		&& text.Contains("MPL", StringComparison.Ordinal);
}

static string Color(string code, string text) => Console.IsOutputRedirected ? text : $"\u001b[{code}m{text}\u001b[0m";
static string Green(string text) => Color("32", text);
static string Yellow(string text) => Color("33", text);

string repo = Directory.GetCurrentDirectory();
bool fix = false;
bool tests = false;
bool tools = false;

for (int i = 0; i < args.Length; i++) {
	if (args[i] == "--repo-root" && i + 1 < args.Length) {
		repo = Path.GetFullPath(args[++i]);
	} else if (args[i] == "--fix") {
		fix = true;
	} else if (args[i] == "--include-tests") {
		tests = true;
	} else if (args[i] == "--include-tools") {
		tools = true;
	}
}

var roots = new List<string> {
	Path.Combine(repo, "Source")
};

if (tests) {
	roots.Add(Path.Combine(repo, "Tests"));
}

if (tools) {
	roots.Add(Path.Combine(repo, "Tools"));
}

var missing = new List<string>();

foreach (string root in roots.Where(Directory.Exists)) {
	foreach (string file in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories).Where(path => !Skip(path))) {
		string text = File.ReadAllText(file);
		if (HasHeader(text)) {
			continue;
		}

		missing.Add(file);
		if (fix) {
			File.WriteAllText(file, Header + text.TrimStart('\uFEFF').TrimStart());
		}
	}
}

foreach (string file in missing) {
	string label = fix ? Green("FIXED") : Yellow("MISSING");
	Console.WriteLine($"{label}: {Path.GetRelativePath(repo, file).Replace('\\', '/')}");
}

string summaryLabel = missing.Count == 0 || fix ? Green("SUCCESS") : Yellow("MISSING");
Console.WriteLine($"{summaryLabel}: {missing.Count} file(s) missing Nomad MPL header.");
Environment.Exit(missing.Count == 0 || fix ? 0 : 1);
