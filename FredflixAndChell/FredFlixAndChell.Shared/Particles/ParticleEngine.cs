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

namespace FredflixAndChell.Shared.Particles
{
    public class ParticleEngine : Component, IUpdateable
    {

        string[] _particleConfigs = new string[] {
                ParticleDesigner.fire,
        };

        ParticleEmitter _particleEmitter;

        int _currentParticleSystem = 0;

        bool _isCollisionEnabled;
        bool _simulateInWorldSpace = true;

        public bool Enabled => throw new NotImplementedException();

        public int UpdateOrder => throw new NotImplementedException();

        bool IUpdateable.Enabled => throw new NotImplementedException();

        int IUpdateable.UpdateOrder => throw new NotImplementedException();

       

        public override void onAddedToEntity()
        {
            loadParticleSystem();
            _particleEmitter.play();
        }

        void loadParticleSystem()
        {
            // kill the ParticleEmitter if we already have one
            if (_particleEmitter != null)
                entity.removeComponent(_particleEmitter);

            // load up the config then add a ParticleEmitter
            var particleSystemConfig = entity.scene.content.Load<ParticleEmitterConfig>(_particleConfigs[_currentParticleSystem]);
            _particleEmitter = entity.addComponent(new ParticleEmitter(particleSystemConfig));

            // set state based on the values of our CheckBoxes
            _particleEmitter.collisionConfig.enabled = _isCollisionEnabled;
            _particleEmitter.simulateInWorldSpace = _simulateInWorldSpace;
        }


        void IUpdateable.Update(GameTime gameTime)
        {
            loadParticleSystem();
        }

        event EventHandler<EventArgs> IUpdateable.EnabledChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        event EventHandler<EventArgs> IUpdateable.UpdateOrderChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }
    }
}
