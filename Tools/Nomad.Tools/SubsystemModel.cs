#pragma warning disable CA2227
namespace Nomad.Tools
{
    public class NomadManifest
    {
        public List<NomadSubsystem> Subsystems { get; set; } = new();
    }

    public class NomadSubsystem
    {
        public string Name { get; set; } = "";
        public string Define { get; set; } = "";
        public string Description { get; set; } = "";
        public List<string> DependsOn { get; set; } = new();
        public bool UnityCompatible { get; set; }
        public bool DefaultEnabled { get; set; }
    }
}
#pragma warning restore CA2227