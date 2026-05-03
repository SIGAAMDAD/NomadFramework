using System.Xml.Linq;

namespace Nomad.ToggleSubsystem.Core;

public static class ToggleLogic
{
    private static string Color(string code, string text)
    {
        return Console.IsOutputRedirected ? text : $"\u001b[{code}m{text}\u001b[0m";
    }

    private static string Green(string text)
    {
        return Color("32", text);
    }

    public static void Toggle(string define, bool enable)
    {
        string propsPath = "Directory.Build.props";

        if (!File.Exists(propsPath))
        {
            File.WriteAllText(propsPath,
@"<Project>
  <PropertyGroup>
    <DefineConstants></DefineConstants>
  </PropertyGroup>
</Project>");
        }

        var xml = XDocument.Load(propsPath);
        var node = xml.Root!.Element("PropertyGroup")!.Element("DefineConstants")!;

        var defines = node.Value.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();

        if (enable && !defines.Contains(define))
            defines.Add(define);

        if (!enable)
            defines.Remove(define);

        node.Value = string.Join(";", defines);
        xml.Save(propsPath);

        Console.WriteLine($"{Green("SUCCESS")}: Updated DefineConstants: {node.Value}");
    }
}
