using System.Net.Http.Headers;
using System.Net.Http.Json;
using Nomad.Tools;

namespace Nomad.PublishUnityPackage
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

        private static string Green(string text)
        {
            return Color("32", text);
        }

        private static string Yellow(string text)
        {
            return Color("33", text);
        }

        public static async Task<int> Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: publish <GitHubToken>");
                return 1;
            }

            string token = args[0];
            string root = ManifestLoader.FindRepoRoot();

            string packageRoot = Path.Combine(
                root,
                "UnityIntegration",
                "Packages",
                "com.nomad.subsystems"
            );

            string packageJson = Path.Combine(packageRoot, "package.json");
            string version = ExtractVersion(packageJson);

            string tgzPath = Path.Combine(
                root,
                "Build",
                $"com.nomad.subsystems-{version}.tgz"
            );

            if (!File.Exists(tgzPath))
            {
                Console.WriteLine($"{Yellow("MISSING")}: Package file not found:");
                Console.WriteLine(tgzPath);
                return 1;
            }

            string repo = "NomadEngine/Nomad"; // customize this

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("NomadPublisher", "1.0"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);

            // Create GitHub release
            var release = new
            {
                tag_name = $"v{version}",
                name = $"Nomad Unity Package {version}",
                body = "Automated release of Nomad Unity package.",
                draft = false,
                prerelease = false
            };

            var releaseResponse = await client.PostAsJsonAsync(
                $"https://api.github.com/repos/{repo}/releases",
                release
            );

            if (!releaseResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"{Red("ERROR")}: Failed to create release:");
                Console.WriteLine(await releaseResponse.Content.ReadAsStringAsync());
                return 1;
            }

            var releaseData = await releaseResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            string uploadUrl = releaseData!["upload_url"].ToString()!.Split('{')[0];

            // Upload asset
            using FileStream fs = File.OpenRead(tgzPath);
            using StreamContent content = new StreamContent(fs);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/gzip");

            var uploadResponse = await client.PostAsync(
                $"{uploadUrl}?name=com.nomad.subsystems-{version}.tgz",
                content
            );

            if (!uploadResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"{Red("ERROR")}: Failed to upload asset:");
                Console.WriteLine(await uploadResponse.Content.ReadAsStringAsync());
                return 1;
            }

            Console.WriteLine($"{Green("SUCCESS")}: Unity package published successfully.");
            return 0;
        }

        private static string ExtractVersion(string packageJson)
        {
            string json = File.ReadAllText(packageJson);

            const string key = "\"version\":";
            int idx = json.IndexOf(key, StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
                return "0.0.0";

            int start = json.IndexOf('"', idx + key.Length) + 1;
            int end = json.IndexOf('"', start);

            return json.Substring(start, end - start);
        }
    }
}
