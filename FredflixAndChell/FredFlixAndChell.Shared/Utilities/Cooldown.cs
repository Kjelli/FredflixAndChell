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
        private bool _unscaled;
        private float _duration;
        private float _cooldownTimer;

        public Cooldown(float duration, bool unscaled = false)
        {
            _unscaled = unscaled;
            _duration = duration;
        }

        public void Update()
        {
            if(_cooldownTimer > 0)
            {
                _cooldownTimer -= _unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            }
            else
            {
                _cooldownTimer = 0;
            }
        }

        public void Start()
        {
            _cooldownTimer = _duration;
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
