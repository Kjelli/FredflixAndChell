
using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Utilities.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FredflixAndChell.Shared.GameObjects.Players.Characters
{
    public static class Characters
    {
        private static bool _isInitialized = false;
        private static Dictionary<string, CharacterParameters> _characters = new Dictionary<string, CharacterParameters>();

        public static List<CharacterParameters> All()
        {
            if (!_isInitialized)
            {
                LoadFromData();
            }
            return _characters.Values.ToList();
        }

        public static CharacterParameters Get(string name)
        {
            if (!_isInitialized)
            {
                LoadFromData();
            }
            return _characters[name];
        }

        public static void LoadFromData()
        {
            Console.WriteLine("Loading .fml files for characters...");
            var characterFilenames = Directory.EnumerateFiles($"{Constants.Assets.DataDirectory}/characters", "*.fml");
            foreach (var characterFilename in characterFilenames)
            {
                var character = YamlSerializer.DeserializeCharacterParameters(characterFilename);
                _characters[character.CharacterName] = character;
            }
            _isInitialized = true;
        }

        public static CharacterParameters GetNextAfter(string name)
        {
            var list = _characters.Values.ToList();
            var next = list[(list.IndexOf(_characters[name]) + 1) % _characters.Count];
            return next;
        }
    }
}
