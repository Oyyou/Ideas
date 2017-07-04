using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ideas.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Ideas.FX;
using Microsoft.Xna.Framework.Content;

namespace Ideas.Sprites
{
  public class Player : Sprite
  {
    private KeyboardState _currentKey;

    private List<Block> _blocks;

    private GrassParticle _grassParticle;

    private bool _jumping;

    private KeyboardState _previousKey;

    public Rectangle BottomRectangle
    {
      get
      {
        return new Rectangle(
          Rectangle.X + 2,
          Rectangle.Bottom - 7,
          Rectangle.Width - 4,
          7);
      }
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      _grassParticle = new GrassParticle(content.Load<Texture2D>("Sprites/FX/GrassParticle"));
    }

    public Player(Dictionary<string, Animation> animations, List<Block> blocks) : base(animations)
    {
      _blocks = blocks;
    }

    public override void Update(GameTime gameTime)
    {
      _previousKey = _currentKey;
      _currentKey = Keyboard.GetState();

      if (_currentKey.IsKeyDown(Keys.D))
      {
        Velocity.X = 3f;
        SpriteEffect = SpriteEffects.None;
      }
      else if (_currentKey.IsKeyDown(Keys.A))
      {
        Velocity.X = -3f;
        SpriteEffect = SpriteEffects.FlipHorizontally;
      }
      else Velocity.X = 0f;

      if ((_currentKey.IsKeyDown(Keys.Space) && _previousKey.IsKeyUp(Keys.Space)) && !_jumping)
      {
        Velocity.Y = -7f;
        _jumping = true;
      }

      if (Velocity.Y != 0)
      {
        if (Velocity.Y < 0)
          _animationManager.Play(_animations["Jumping"]);
        else if (Velocity.Y > 0)
          _animationManager.Play(_animations["Falling"]);
      }
      else
      {
        if (Velocity.X > 0)
        {
          _animationManager.Play(_animations["Running"]);
        }
        else if (Velocity.X < 0)
        {
          _animationManager.Play(_animations["Running"]);
        }
        else
        {
          _animationManager.Play(_animations["Idle"]);
        }
      }

      Velocity.Y += 0.3f;

      Velocity.Y = MathHelper.Clamp(Velocity.Y, -7, 7);

      foreach (var block in _blocks)
      {
        var rectangle = block.Rectangle;

        //foreach (var rectangle in block.Rectangles)
        {
          if (!rectangle.Intersects(this.Rectangle))
            continue;

          if ((this.Velocity.X > 0 && this.IsTouchingLeft(rectangle)) ||
              (this.Velocity.X < 0 && this.IsTouchingRight(rectangle)))
            this.Velocity.X = 0;

          if (this.Velocity.Y > 0 && this.IsTouchingTop(rectangle))

          {
            this.Velocity.Y = 0;
            _jumping = false;
          }

          if (this.Velocity.Y < 0 && this.IsTouchingBottom(rectangle))
          {
            this.Velocity.Y = 0;
          }
        }
      }

      Position += Velocity;

      _animationManager.Update(gameTime);
    }
  }
}
