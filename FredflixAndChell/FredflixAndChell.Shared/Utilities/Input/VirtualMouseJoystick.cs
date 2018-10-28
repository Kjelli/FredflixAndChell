using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FredflixAndChell.Shared.Utilities.Input
{
    class VirtualMouseJoystick : VirtualJoystick.Node
    {
        public Vector2 ReferencePoint { get; set; }

        public VirtualMouseJoystick(Vector2 referencePoint)
        {
            ReferencePoint = referencePoint;
        }
        public override Vector2 value => new Vector2(
            (float)Math.Cos(Mathf.angleBetweenVectors(ReferencePoint, Nez.Input.mousePosition)),
            -(float)Math.Sin(Mathf.angleBetweenVectors(ReferencePoint, Nez.Input.mousePosition)));
    }
}
