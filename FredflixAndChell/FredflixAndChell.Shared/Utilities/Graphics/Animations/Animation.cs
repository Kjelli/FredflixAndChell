using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FredflixAndChell.Shared.Utilities.Graphics.Animations
{
    public class Animation
    {
        public Texture2D[] Textures { get; set; }
        public AnimationSettings Settings { get; set; }
        public int ElapsedMillis { get; set; }
        public int CurrentFrameIndex { get; set; }
        public bool IsPlaying { get; set; }
        public Texture2D CurrentFrame => Textures[CurrentFrameIndex % Textures.Length];

        public Animation(Texture2D[] textures, AnimationSettings animationSettings, int startFrame = 0)
        {
            Textures = textures;
            Settings = animationSettings;
            CurrentFrameIndex = startFrame;

            IsPlaying = Settings.Autoplay;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsPlaying) return;

            ElapsedMillis += gameTime.ElapsedGameTime.Milliseconds;
            if(ElapsedMillis > Settings.FrameDurationMillis)
            {
                ElapsedMillis = 0;
                CurrentFrameIndex = (CurrentFrameIndex + 1) % Textures.Length;
                if (!Settings.Loop) IsPlaying = false;
            }
        }

        public void Start()
        {
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
        }
    }

    public class AnimationSettings
    {
        public float FrameDurationMillis { get; set; }
        public bool Loop { get; set; }
        public bool Autoplay { get; set; }
    }
}
