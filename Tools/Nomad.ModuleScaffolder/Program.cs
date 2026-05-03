using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;

static string Need(string[] a, ref int i, string opt) { if (i + 1 >= a.Length) throw new ArgumentException($"{opt} requires a value."); return a[++i]; }
static string Norm(string name) => name.StartsWith("Nomad.", StringComparison.Ordinal) ? name : $"Nomad.{name}";
static string[] Split(string s) => s.Split([',',';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(Norm).Distinct().ToArray();
static void WriteNew(string path, string text, bool force) { if (File.Exists(path) && !force) return; Directory.CreateDirectory(Path.GetDirectoryName(path)!); File.WriteAllText(path, text.Replace("\r\n", "\n", StringComparison.Ordinal)); }
static string Esc(string s) => System.Security.SecurityElement.Escape(s) ?? s;
static string Color(string code, string text) => Console.IsOutputRedirected ? text : $"\u001b[{code}m{text}\u001b[0m";
static string Green(string text) => Color("32", text);

string repo = Directory.GetCurrentDirectory(), category = "Uncategorized", desc = "", module = "";
string[] deps = [];
bool unity = true, godot = true, headless = true, tests = true, manifest = true, solution = true, force = false, defaultEnabled = false;
for (int i = 0; i < args.Length; i++) {
    string x = args[i];
    if (x == "--repo-root") repo = Path.GetFullPath(Need(args, ref i, x));
    else if (x is "--module" or "-m") module = Norm(Need(args, ref i, x));
    else if (x == "--category") category = Need(args, ref i, x);
    else if (x == "--description") desc = Need(args, ref i, x);
    else if (x == "--depends-on") deps = Split(Need(args, ref i, x));
    else if (x == "--unity") unity = Boolean.Parse(Need(args, ref i, x));
    else if (x == "--godot") godot = Boolean.Parse(Need(args, ref i, x));
    else if (x == "--headless") headless = Boolean.Parse(Need(args, ref i, x));
    else if (x == "--default-enabled") defaultEnabled = Boolean.Parse(Need(args, ref i, x));
    else if (x == "--no-tests") tests = false;
    else if (x == "--no-manifest") manifest = false;
    else if (x == "--no-solution") solution = false;
    else if (x == "--force") force = true;
    else if (x is "--help" or "-h") { Console.WriteLine("Usage: Nomad.ModuleScaffolder Nomad.Localization --depends-on Nomad.Core,Nomad.Events [--category Foundation]"); return; }
    else if (!x.StartsWith('-') && module == "") module = Norm(x);
}
if (module == "") throw new ArgumentException("Module name is required.");
if (desc == "") desc = $"{module} module for NomadFramework.";

string sourceDir = Path.Combine(repo,"Source",module);
if (Directory.Exists(sourceDir) && !force) throw new IOException($"Module already exists: {sourceDir}. Use --force to add missing scaffold files.");
Directory.CreateDirectory(Path.Combine(sourceDir,"Public"));
Directory.CreateDirectory(Path.Combine(sourceDir,"Private"));
Directory.CreateDirectory(Path.Combine(sourceDir,"Properties"));
string refs = String.Join(Environment.NewLine, deps.Select(d => $"    <ProjectReference Include=\"..\\{d}\\{d}.csproj\" />"));
string suffix = module["Nomad.".Length..];

WriteNew(Path.Combine(sourceDir,$"{module}.csproj"), $"""
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <PackageId>NomadFramework.{suffix}</PackageId>
    <Version>0.1.0-alpha1</Version>
    <Description>{Esc(desc)}</Description>
    <NomadValidateAsModule>true</NomadValidateAsModule>
  </PropertyGroup>
  <ItemGroup>
{refs}
  </ItemGroup>
</Project>
""", force);
WriteNew(Path.Combine(sourceDir,"Properties","AssemblyInfo.cs"), $"""
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo( "{module}.Tests" )]
""", force);
WriteNew(Path.Combine(sourceDir,"README.md"), $"# {module}\n\n{desc}\n\n## Dependencies\n\n{String.Join("\n", deps.Select(d => $"- `{d}`"))}\n\n## Bootstrap\n\nTODO\n", force);
WriteNew(Path.Combine(sourceDir,"ROADMAP.md"), $"# ROADMAP\n\n## {module}\n\n- Define bootstrap lifecycle.\n- Add public/private tests.\n- Document examples.\n", force);
WriteNew(Path.Combine(sourceDir,"CHANGELOG.md"), "# CHANGELOG\n\n## 0.1.0-alpha1\n\n- Initial scaffold.\n", force);

if (tests) {
    string testDir = Path.Combine(repo,"Tests",$"{module}.Tests");
    foreach (string d in new[]{"Public","Private","Integration","Fixtures","TestData"}) Directory.CreateDirectory(Path.Combine(testDir,d));
    WriteNew(Path.Combine(testDir,$"{module}.Tests.csproj"), $"""
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net10.0</TargetFrameworks>
    <IsTestProject>true</IsTestProject>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\Source\{module}\{module}.csproj" />
  </ItemGroup>
</Project>
""", force);
}

if (manifest) {
    string mp = Path.Combine(repo,"NomadSubsystems.json");
    JsonObject root = File.Exists(mp) ? JsonNode.Parse(File.ReadAllText(mp))!.AsObject() : new JsonObject();
    JsonArray arr = root["Subsystems"] as JsonArray ?? [];
    root["Subsystems"] = arr;
    bool exists = arr.OfType<JsonObject>().Any(o => o["Name"]?.GetValue<string>() == module);
    if (!exists) arr.Add(new JsonObject{
        ["Name"] = module,
        ["Define"] = "NOMAD_ENABLE_" + suffix.Replace('.','_').ToUpperInvariant(),
        ["Category"] = category,
        ["Description"] = desc,
        ["DependsOn"] = new JsonArray(deps.Select(d => JsonValue.Create(d)).ToArray<JsonNode?>()),
        ["UnityCompatible"] = unity,
        ["GodotCompatible"] = godot,
        ["HeadlessCompatible"] = headless,
        ["DefaultEnabled"] = defaultEnabled
    });
    File.WriteAllText(mp, root.ToJsonString(new JsonSerializerOptions{WriteIndented=true}) + Environment.NewLine);
}

if (solution) {
    string sp = File.Exists(Path.Combine(repo,"NomadFramework.All.slnx")) ? Path.Combine(repo,"NomadFramework.All.slnx") : Path.Combine(repo,"NomadFramework.slnx");
    if (File.Exists(sp)) {
        XDocument doc = XDocument.Load(sp); string pp = $"Source/{module}/{module}.csproj";
        if (!doc.Root!.Elements().Any(e => e.Attribute("Path")?.Value.Replace('\\','/') == pp)) { doc.Root.Add(new XElement("Project", new XAttribute("Path", pp))); doc.Save(sp); }
    }
}
Console.WriteLine($"{Green("SUCCESS")}: Scaffolded {module}.");
