using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nez.Particles;
using Microsoft.Xna.Framework.Input;
using Nez.UI;
using Nez;
using Microsoft.Xna.Framework;
using FredflixAndChell.Shared.Assets;
using FredflixAndChell.Shared.Utilities;

namespace FredflixAndChell.Shared.Particles
{
    public class ParticleEngine : Component, IUpdatable
    {

        string[] _particleConfigs = new string[1];

        ParticleEmitter _particleEmitter;

        int _currentParticleSystem = 0;

        bool _isCollisionEnabled;
        bool _simulateInWorldSpace = true;

        private float _duration;

        public bool Enabled => throw new NotImplementedException();

        public int UpdateOrder => throw new NotImplementedException();


        public ParticleEngine(string config)
        {
            _particleConfigs[0] = config;
        }

        public ParticleEngine(string[] config)
        {
            _particleConfigs = config;
        }

        public override void onAddedToEntity()
        {
            loadParticleSystem();
            if (_particleEmitter.isPlaying)
            {
                _particleEmitter.pause();
            }
        }



        void loadParticleSystem()
        {
            // kill the ParticleEmitter if we already have one
            if (_particleEmitter != null)
                entity.removeComponent(_particleEmitter);

            // load up the config then add a ParticleEmitter
            var particleSystemConfig = entity.scene.content.Load<ParticleEmitterConfig>(_particleConfigs[_currentParticleSystem]);
            _particleEmitter = entity.addComponent(new ParticleEmitter(particleSystemConfig));
            _particleEmitter.pause();

            // set state based on the values of our CheckBoxes
            _particleEmitter.collisionConfig.enabled = _isCollisionEnabled;
            _particleEmitter.simulateInWorldSpace = _simulateInWorldSpace;

            
        }

        public void Play()
        {
            //0 = plays foreveah
            Play(0);
        }

        public void Play(float duration)
        {
            _particleEmitter.play();
            _duration = duration;
        }

        public void Stop()
        {
            _particleEmitter.pauseEmission();
        }

        public void update()
        {
            
            if (_duration != 0 && _particleEmitter.elapsedTime > _duration)
            {
                Stop();
            }
            
        }
    }
}
