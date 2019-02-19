using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace FredflixAndChell.Shared.Utilities.Graphics
{
    public static class Materials
    {
        private static Material _laser;
        public static ReflectionMaterial ReflectionMaterial { get; set; }
        public static Material Laser => _laser ?? (_laser = new Material(BlendState.AlphaBlend)); 
    }
}
