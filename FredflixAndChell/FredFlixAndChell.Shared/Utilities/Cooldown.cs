using Microsoft.Xna.Framework;
using Nez;
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

        public void Update()
        {
            if(_cooldownTimer > 0)
            {
                _cooldownTimer -= Time.deltaTime;
                
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
