using Engine;
using Engine.Models;
using Engine.Sprites;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDown.Sprites
{
  public class Chicken : Sprite
  {
    private float _timer;

    private Vector2 _position;

    private Vector2? _startPosition;

    public override Vector2 Position
    {
      get
      {
        return _position;
      }
      set
      {
        _position = value;
        _animationManager.Position = _position;
      }
    }

    public Chicken(Dictionary<string, Animation> animations) : base(animations)
    {
      DoSomething();
    }

    public override void Update(GameTime gameTime)
    {
      _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

      if (_startPosition == null)
        _startPosition = Position;

      if (_timer > 5f)
        DoSomething();

      if (_animationManager != null)
        _animationManager.Update(gameTime);

      ClampPosition();
    }

    private void DoSomething()
    {
      var value = GameEngine.Random.Next(0, 3);

      switch (value)
      {
        case 0:
          Velocity = Vector2.Zero;
          _animationManager.Play(_animations["Peck"]);
          _timer = 3f;
          break;

        case 1:
          Walk();
          _timer = 0f;

          break;

        case 2:
          Velocity = Vector2.Zero;
          _animationManager.Stop();
          _timer = 0f;
          break;
      }
    }

    private void Walk()
    {
      _animationManager.Play(_animations["Walk"]);

      var speedX = (float)GameEngine.Random.Next(-4, 4) / 10;
      var speedY = (float)GameEngine.Random.Next(-4, 4) / 10;

      Velocity = new Vector2(speedX, speedY);

      if (Velocity.X < 0)
        _animationManager.SpriteEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
      else if (Velocity.X > 0)
        _animationManager.SpriteEffect = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
    }

    private void ClampPosition()
    {
      var newPos = Position + Velocity;

      var area = 32;

      Position = Vector2.Clamp(newPos, _startPosition.Value - new Vector2(area, area), _startPosition.Value + new Vector2(area, area));
    }
  }
}
