using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Managers
{
  public class AnimationManager : Component
  {
    private float _timer;

    private bool _updated;

    public Animation Animation { get; private set; }

    public int CurrentFrame { get; private set; }

    public int FrameHeight
    {
      get { return Animation.FrameHeight; }
    }

    public int FrameWidth
    {
      get { return Animation.FrameWidth; }
    }

    public float Layer { get; set; }

    public Vector2 Position { get; set; }

    public int PreviousFrame { get; private set; }

    public SpriteEffects SpriteEffect { get; set; }

    public AnimationManager(Animation animation)
    {
      Animation = animation;

      _updated = false;

      SpriteEffect = SpriteEffects.None;
    }

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(
        Animation.Texture,
        Position,
        new Rectangle(
          CurrentFrame * Animation.FrameWidth,
          0,
          Animation.FrameWidth,
          Animation.FrameHeight),
        Color.White,
        0f,
        new Vector2(0, 0),
        1f,
        SpriteEffect,
        Layer);

      _updated = false;
    }

    public override void LoadContent(ContentManager content)
    {

    }

    public void Play(Animation animation)
    {
      if (Animation == animation)
        return;

      Animation = animation;

      Stop();
    }

    public void Stop()
    {

      PreviousFrame = 0;
      CurrentFrame = 0;
      _timer = 0;
    }

    public override void UnloadContent()
    {

    }

    public override void Update(GameTime gameTime)
    {
      _updated = true;

      _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

      PreviousFrame = CurrentFrame;

      if (_timer > Animation.Speed)
      {
        _timer = 0;

        CurrentFrame++;

        if (CurrentFrame >= Animation.FrameCount)
          CurrentFrame = 0;
      }
    }
  }
}
