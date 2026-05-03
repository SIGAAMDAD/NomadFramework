using System.Text.Json;

namespace Nomad.Tools
{
    public static class ManifestLoader
    {
        public static string FindRepoRoot()
        {
            var dir = AppContext.BaseDirectory;

            while (dir != null)
            {
                if (File.Exists(Path.Combine(dir, "NomadSubsystems.json")))
                    return dir;

                dir = Directory.GetParent(dir)?.FullName;
            }

            throw new Exception("Repo root not found.");
        }

        public static NomadManifest Load(string root)
        {
            string path = Path.Combine(root, "NomadSubsystems.json");

            return JsonSerializer.Deserialize<NomadManifest>(
                File.ReadAllText(path),
                new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                }
            )!;
        }

        public static void Save(string root, NomadManifest manifest)
        {
            string path = Path.Combine(root, "NomadSubsystems.json");

            File.WriteAllText(
                path,
                JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true })
            );
        }
    }
}