using FredflixAndChell.Shared.GameObjects.Players;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Components.Interactables
{
    public class InteractableComponent : Component
    {
        public Action<Player> OnInteract { get; set; }
    }
}
