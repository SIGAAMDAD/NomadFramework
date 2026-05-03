using Nomad.ValidateManifest.Core;
using Nomad.Tools;

string root = ManifestLoader.FindRepoRoot();
var manifest = ManifestLoader.Load(root);

bool ok = ManifestValidator.Validate(manifest);
Console.WriteLine(ok ? "Manifest valid." : "Manifest has errors.");
Environment.Exit(ok ? 0 : 1);