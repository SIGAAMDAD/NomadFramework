using Nomad.ToggleSubsystem.Core;
using Nomad.Tools;

static string Color(string code, string text) => Console.IsOutputRedirected ? text : $"\u001b[{code}m{text}\u001b[0m";
static string Yellow(string text) => Color("33", text);

string root = ManifestLoader.FindRepoRoot();
var manifest = ManifestLoader.Load(root);

if (args.Length < 2)
{
    Console.WriteLine("Usage: toggle <SubsystemName> <enable|disable>");
    return;
}

string name = args[0];
string action = args[1];

var subsystem = manifest.Subsystems
    .FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

if (subsystem == null)
{
    Console.WriteLine($"{Yellow("MISSING")}: Unknown subsystem: {name}");
    return;
}

ToggleLogic.Toggle(subsystem.Define, action == "enable");
