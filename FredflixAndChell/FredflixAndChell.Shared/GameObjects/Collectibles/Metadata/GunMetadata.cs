namespace FredflixAndChell.Shared.GameObjects.Collectibles.Metadata
{
    public class GunMetadata : CollectibleMetadata
    {
        public const string AmmoKey = "ammo";
        public const string MagazineAmmoKey = "magazineammo";

        public GunMetadata(int ammo, int magazineAmmo)
        {
            SetAmmo(ammo);
            SetMagazineAmmo(magazineAmmo);
        }

        public void SetAmmo(int ammo)
        {
            Data[AmmoKey] = ammo;
        }

        public void SetMagazineAmmo(int magazineAmmo)
        {
            Data[MagazineAmmoKey] = magazineAmmo;
        }

        public int? GetAmmo()
        {
            return Data.ContainsKey(AmmoKey) ? (int?)Data[AmmoKey] : null;
        }

        public int? GetMagazineAmmo()
        {
            return Data.ContainsKey(MagazineAmmoKey) ? (int?)Data[MagazineAmmoKey] : null;
        }
    }
}
