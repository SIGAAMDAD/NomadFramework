using System.Text.Json;
using Nomad.Tools;

namespace Nomad.UpgradeManifest.Core
{
    public static class ManifestUpgrader
    {
        private static string Color(string code, string text)
        {
            return Console.IsOutputRedirected ? text : $"\u001b[{code}m{text}\u001b[0m";
        }

        private static string Green(string text)
        {
            return Color("32", text);
        }

        public static void Upgrade(string path)
        {
            // simple example: ensure description exists
            var manifest = ManifestLoader.Load(path);

            foreach (var s in manifest.Subsystems)
            {
                if (string.IsNullOrWhiteSpace(s.Description))
                    s.Description = $"Subsystem {s.Name}";
            }

            var upgraded = JsonSerializer.Serialize(
                manifest,
                new JsonSerializerOptions { WriteIndented = true }
            );

            File.WriteAllText(path, upgraded);
            Console.WriteLine($"{Green("SUCCESS")}: Manifest upgraded.");
        }
    }
}
