
using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.GameObjects.Weapons.Parameters;
using FredflixAndChell.Shared.Utilities.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FredflixAndChell.Shared.GameObjects.Weapons
{
    public static class Guns
    {
        private static bool _isInitialized = false;
        private static Dictionary<string, GunParameters> _guns = new Dictionary<string, GunParameters>();

        public static List<GunParameters> All()
        {
            if (!_isInitialized)
            {
                LoadFromData();
            }
            return _guns.Values.ToList();
        }

        public static GunParameters Get(string name)
        {
            if (!_isInitialized)
            {
                LoadFromData();
            }

            if (_guns.ContainsKey(name))
                return _guns[name];

            return null;
        }

        public static GunParameters GetNextAfter(string name)
        {
            var list = _guns.Values.ToList();
            var next = list[(list.IndexOf(_guns[name]) + 1) % _guns.Count];
            return next;
        }

        private static void LoadFromData()
        {
            Console.WriteLine("Loading .fml files for guns...");
            var gunFilenames = Directory.EnumerateFiles($"{Constants.Assets.DataDirectory}/weapons/guns", "*.fml");
            foreach (var gunFilename in gunFilenames)
            {
                var gun = YamlSerializer.DeserializeGunParameters(gunFilename);
                _guns[gun.Name] = gun;
            }
            _isInitialized = true;
        }
    }

    public static class Melees
    {
        private static bool _isInitialized = false;
        private static Dictionary<string, MeleeParameters> _melees = new Dictionary<string, MeleeParameters>();

        public static List<MeleeParameters> All()
        {
            if (!_isInitialized)
            {
                LoadFromData();
            }
            return _melees.Values.ToList();
        }

        public static MeleeParameters Get(string name)
        {
            if (!_isInitialized)
            {
                LoadFromData();
            }

            if (_melees.ContainsKey(name))
                return _melees[name];

            return null;
        }

        public static MeleeParameters GetNextAfter(string name)
        {
            var list = _melees.Values.ToList();
            var next = list[(list.IndexOf(_melees[name]) + 1) % _melees.Count];
            return next;
        }

        private static void LoadFromData()
        {
            Console.WriteLine("Loading .fml files for guns...");
            var meleeFileNames = Directory.EnumerateFiles($"{Constants.Assets.DataDirectory}/weapons/melee", "*.fml");
            foreach (var meleeFilename in meleeFileNames)
            {
                var melee = YamlSerializer.DeserializeMeleeParameters(meleeFilename);
                _melees[melee.Name] = melee;
            }
            _isInitialized = true;
        }
    }
}
