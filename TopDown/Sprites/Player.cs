using Engine.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TopDown.Sprites
{
  public class Player : Sprite
  {
    public override List<Rectangle> CollisionRectangles
    {
      get
      {
        return new List<Rectangle>()
        {
          new Rectangle(Rectangle.X, Rectangle.Y + (Rectangle.Height - 32), 32, 32),
        };
      }
    }

    public bool IsIn(Rectangle rectangle)
    {
      var playerRectangle = CollisionRectangles.FirstOrDefault();

      return playerRectangle.Left >= rectangle.Left &&
        playerRectangle.Top >= rectangle.Top &&
        playerRectangle.Right <= rectangle.Right &&
        playerRectangle.Bottom <= rectangle.Bottom;
    }

    public Player(Dictionary<string, Animation> animations)
     : base(animations)
    {
    }

    protected override void SetAnimation()
    {
      if (Velocity.X > 0)
      {
        _animationManager.Play(_animations["WalkRight"]);
      }
      else if (Velocity.X < 0)
      {
        _animationManager.Play(_animations["WalkLeft"]);
      }
      else if (Velocity.Y > 0)
      {
        _animationManager.Play(_animations["WalkDown"]);
      }
      else if (Velocity.Y < 0)
      {
        _animationManager.Play(_animations["WalkUp"]);
      }
      else
      {
        _animationManager.Stop();
      }
    }

    public override void Update(GameTime gameTime)
    {
      var speed = 1f;

      Velocity = Vector2.Zero;

      if (Keyboard.GetState().IsKeyDown(Keys.A))
        Velocity.X = -speed;
      else if (Keyboard.GetState().IsKeyDown(Keys.D))
        Velocity.X = speed;
      else if (Keyboard.GetState().IsKeyDown(Keys.W))
        Velocity.Y = -speed;
      else if (Keyboard.GetState().IsKeyDown(Keys.S))
        Velocity.Y = speed;

      if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
        Velocity *= 2;

      _animationManager.Update(gameTime);
    }
  }
}
