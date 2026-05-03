using Nomad.Tools;

namespace Nomad.ValidateGraph.Core
{
    public static class GraphValidator
    {
        private static string Color(string code, string text)
        {
            return Console.IsOutputRedirected ? text : $"\u001b[{code}m{text}\u001b[0m";
        }

        private static string Red(string text)
        {
            return Color("31", text);
        }

        private static string Green(string text)
        {
            return Color("32", text);
        }

        public static void Validate(NomadManifest manifest)
        {
            var map = manifest.Subsystems.ToDictionary(s => s.Name);

            foreach (var s in manifest.Subsystems)
            {
                foreach (var dep in s.DependsOn)
                {
                    if (!map.ContainsKey(dep))
                        Console.WriteLine($"{Red("ERROR")}: {s.Name} depends on missing subsystem {dep}");
                }
            }

            foreach (var s in manifest.Subsystems)
            {
                if (HasCycle(s.Name, map, new(), new()))
                    Console.WriteLine($"{Red("ERROR")}: Circular dependency involving {s.Name}");
            }

            Console.WriteLine($"{Green("SUCCESS")}: Subsystem graph validation complete.");
        }

        private static bool HasCycle(
            string node,
            Dictionary<string, NomadSubsystem> map,
            HashSet<string> visited,
            HashSet<string> stack)
        {
            if (!visited.Add(node))
                return false;

            stack.Add(node);

            foreach (var dep in map[node].DependsOn)
            {
                if (!visited.Contains(dep) && HasCycle(dep, map, visited, stack))
                    return true;

                if (stack.Contains(dep))
                    return true;
            }

            stack.Remove(node);
            return false;
        }
    }
}
