﻿using Microsoft.Xna.Framework;
using Nez;
using System;

namespace FredflixAndChell.Shared.Utilities.Input
{
    public class VirtualMouseXAxis : VirtualAxis.Node
    {
        public Vector2 ReferencePoint { get; set; }

        public VirtualMouseXAxis(Vector2 referencePoint)
        {
            ReferencePoint = referencePoint;
        }

        public override float value => (float) Math.Cos(Mathf.angleBetweenVectors(ReferencePoint, Nez.Input.mousePosition));
    }
}