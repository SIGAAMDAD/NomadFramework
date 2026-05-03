using Nomad.UpgradeManifest.Core;
using Nomad.Tools;

string root = ManifestLoader.FindRepoRoot();
ManifestUpgrader.Upgrade(root);