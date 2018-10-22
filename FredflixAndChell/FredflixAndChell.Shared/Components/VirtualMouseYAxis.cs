using Microsoft.Xna.Framework;
using Nez;
using System;

namespace FredflixAndChell.Shared.Components
{
    public class VirtualMouseYAxis : VirtualAxis.Node
    {
        public Vector2 ReferencePoint { get; set; }

        public VirtualMouseYAxis(Vector2 referencePoint)
        {
            ReferencePoint = referencePoint;
        }

        public override float value => (float) Math.Sin(Mathf.angleBetweenVectors(ReferencePoint,Input.mousePosition));
    }
}