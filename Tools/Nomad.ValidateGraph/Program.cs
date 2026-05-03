using Nomad.ValidateGraph.Core;
using Nomad.Tools;

string root = ManifestLoader.FindRepoRoot();
var manifest = ManifestLoader.Load(root);
GraphValidator.Validate(manifest);