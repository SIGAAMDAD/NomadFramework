using System.Diagnostics;
using System.Linq;

namespace Nomad.Tool
{
    public static class Program
    {
        private static string Color(string code, string text)
        {
            return Console.IsOutputRedirected ? text : $"\u001b[{code}m{text}\u001b[0m";
        }

        private static string Red(string text)
        {
            return Color("31", text);
        }

        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return 0;
            }

            string command = args[0];
            string[] rest = args.Skip(1).ToArray();

            if (command == "subsystems")
                return DispatchSubsystems(rest);

            Console.WriteLine($"{Red("ERROR")}: Unknown command: {command}");
            PrintHelp();
            return 1;
        }

        private static int DispatchSubsystems(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("subsystems commands:");
                Console.WriteLine("  create <Name>");
                Console.WriteLine("  delete <Name>");
                Console.WriteLine("  rename <Old> <New>");
                Console.WriteLine("  deps <Name> add|remove <Dep>");
                Console.WriteLine("  graph");
                Console.WriteLine("  docs");
                Console.WriteLine("  validate");
                return 0;
            }

            string cmd = args[0];
            string[] rest = args.Skip(1).ToArray();

            return cmd switch
            {
                "create" => Run("Nomad.CreateSubsystem", rest),
                "delete" => Run("Nomad.DeleteSubsystem", rest),
                "rename" => Run("Nomad.RenameSubsystem", rest),
                "deps" => Run("Nomad.EditDependencies", rest),
                "graph" => Run("Nomad.GenerateGraphViz", rest),
                "docs" => Run("Nomad.GenerateSubsystemDocs", rest),
                "validate" => Run("Nomad.ValidateManifest", rest),
                _ => Unknown(cmd)
            };
        }

        private static int Run(string toolName, string[] args)
        {
            string exe = Path.Combine("Tools", toolName, $"{toolName}.exe");

            if (!File.Exists(exe))
            {
                Console.WriteLine($"{Red("ERROR")}: Tool not found: {exe}");
                return 1;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                UseShellExecute = false
            };
            startInfo.ArgumentList.Add(exe);
            startInfo.ArgumentList.Concat(args);

            Process p = Process.Start(startInfo)!;

            p.WaitForExit();
            return p.ExitCode;
        }

        private static int Unknown(string cmd)
        {
            Console.WriteLine($"{Red("ERROR")}: Unknown subsystems command: {cmd}");
            return 1;
        }

        private static void PrintHelp()
        {
            Console.WriteLine("nomad commands:");
            Console.WriteLine("  subsystems <command>");
        }
    }
}
