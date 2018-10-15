using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FredflixAndChell.Shared.Utilities
{
    public class Cooldown
    {
        private float _cooldown { get; set; }
        private float _cooldownTimer { get; set; }

        public Cooldown(float duration)
        {
            _cooldown = duration;
        }


        public void Update(GameTime gameTime)
        {
            if(_cooldownTimer > 0)
            {
                _cooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                
            }
            else
            {
                _cooldownTimer = 0;
            }
        }

        public void Start()
        {
            _cooldownTimer = _cooldown;
        }

        public bool IsReady()
        {
            return !IsOnCooldown();
        }

        public bool IsOnCooldown()
        {
            return _cooldownTimer > 0;
        }

        public void Reset()
        {
            _cooldownTimer = 0;
        }


    }
}
