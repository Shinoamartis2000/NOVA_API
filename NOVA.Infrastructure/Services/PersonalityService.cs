using System.Text.Json;
using NOVA.Core.Models;

namespace NOVA.Infrastructure.Services
{
    public class PersonalityService
    {
        private readonly string _configPath;
        public PersonalityConfig Personality { get; private set; }

        public PersonalityService(string configPath = "Config/Personality.json")
        {
            var baseDir = AppContext.BaseDirectory;
            var possiblePaths = new[]
            {
                Path.Combine(baseDir, configPath),
                Path.Combine(Directory.GetParent(baseDir)?.Parent?.Parent?.FullName ?? "", "NOVA.Infrastructure", configPath),
                Path.Combine(Directory.GetCurrentDirectory(), configPath)
            };

            _configPath = possiblePaths.FirstOrDefault(File.Exists) ?? possiblePaths.First();
            LoadPersonality();
        }

        private void LoadPersonality()
        {
            if (!File.Exists(_configPath))
                throw new FileNotFoundException($"Personality configuration file not found at {_configPath}");

            var json = File.ReadAllText(_configPath);

            Personality = JsonSerializer.Deserialize<PersonalityConfig>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new PersonalityConfig
            {
                Name = "N.O.V.A",
                Tone = "neutral",
                Description = "Default configuration"
            };
        }

        public string GetSystemPrompt()
        {
            return $"You are {Personality.Name}, a {Personality.Tone} AI assistant. " +
                   $"Your core traits: {string.Join(", ", Personality.Traits ?? new List<string>())}. " +
                   $"Description: {Personality.Description}";
        }
    }
}
