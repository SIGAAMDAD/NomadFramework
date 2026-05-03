using Nomad.Tools;

namespace Nomad.SyncUnity
{
    public static class Program
    {
        private static string Color(string code, string text)
        {
            return Console.IsOutputRedirected ? text : $"\u001b[{code}m{text}\u001b[0m";
        }

        private static string Green(string text)
        {
            return Color("32", text);
        }

        private static string Yellow(string text)
        {
            return Color("33", text);
        }

        public static int Main(string[] args)
        {
            string root = ManifestLoader.FindRepoRoot();
            string sourceRoot = Path.Combine(root, "Source");
            string unityRoot = Path.Combine(root, "UnityIntegration", "Runtime");

            Directory.CreateDirectory(unityRoot);

            NomadManifest manifest = ManifestLoader.Load(root);

            // Build the list of subsystem folder names (e.g., "Nomad.Core")
            HashSet<string> allowed = manifest.Subsystems
                .Where(s => s.UnityCompatible)
                .Select(s => s.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            Console.WriteLine("=== Nomad.SyncUnity ===");
            Console.WriteLine($"Repo root: {root}");
            Console.WriteLine($"Source root: {sourceRoot}");
            Console.WriteLine($"Unity root: {unityRoot}");
            Console.WriteLine();

            // Clean Unity folder before syncing
            /*
            foreach (string existing in Directory.GetDirectories(unityRoot))
            {
                string name = Path.GetFileName(existing);

                if (!allowed.Contains(name))
                {
                    Console.WriteLine($"Removing stale folder: {name}");
                    Directory.Delete(existing, true);
                }
            }
            */

            // Copy only subsystem folders
            foreach (string dir in Directory.GetDirectories(sourceRoot))
            {
                string name = Path.GetFileName(dir);

                if (!allowed.Contains(name))
                {
                    Console.WriteLine($"{Yellow("SKIP")}: non-unity-compatible folder: {name}");
                    continue;
                }

                string target = Path.Combine(unityRoot, name);

                if (Directory.Exists(target))
                {
                    Console.WriteLine($"Refreshing subsystem: {name}");
                    Directory.Delete(target, true);
                }
                else
                {
                    Console.WriteLine($"Syncing subsystem: {name}");
                }

                CopyRecursive(dir, target);
            }

            Console.WriteLine();
            Console.WriteLine($"{Green("SUCCESS")}: Sync complete.");
            return 0;
        }

        private static void CopyRecursive(string src, string dst)
        {
            Directory.CreateDirectory(dst);

            foreach (string file in Directory.GetFiles(src))
            {
                string name = Path.GetFileName(file);
                string extension = name.Substring(name.LastIndexOf('.') + 1);
                if (extension == "csproj" || extension == "slnx")
                {
                    continue;
                }
                File.Copy(file, Path.Combine(dst, name));
            }

            foreach (string dir in Directory.GetDirectories(src))
            {
                string name = Path.GetFileName(dir);
                if (name == "bin" || name == "obj")
                {
                    continue;
                }
                CopyRecursive(dir, Path.Combine(dst, name));
            }
        }
    }
}
