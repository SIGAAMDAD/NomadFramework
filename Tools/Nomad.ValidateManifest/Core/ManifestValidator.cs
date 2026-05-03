using Nomad.Tools;

namespace Nomad.ValidateManifest.Core
{
    public static class ManifestValidator
    {
        private static string Color(string code, string text)
        {
            return Console.IsOutputRedirected ? text : $"\u001b[{code}m{text}\u001b[0m";
        }

        private static string Red(string text)
        {
            return Color("31", text);
        }

        public static bool Validate(NomadManifest manifest)
        {
            bool ok = true;
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var s in manifest.Subsystems)
            {
                if (!names.Add(s.Name))
                {
                    Console.WriteLine($"{Red("ERROR")}: Duplicate subsystem name: {s.Name}");
                    ok = false;
                }

                if (string.IsNullOrWhiteSpace(s.Name))
                {
                    Console.WriteLine($"{Red("ERROR")}: Subsystem with empty name.");
                    ok = false;
                }

                if (s.DependsOn.Contains(s.Name))
                {
                    Console.WriteLine($"{Red("ERROR")}: {s.Name} depends on itself.");
                    ok = false;
                }
            }

            foreach (var s in manifest.Subsystems)
            {
                foreach (var dep in s.DependsOn)
                {
                    if (!names.Contains(dep))
                    {
                        Console.WriteLine($"{Red("ERROR")}: {s.Name} depends on missing subsystem {dep}");
                        ok = false;
                    }
                }
            }

            return ok;
        }
    }
}
