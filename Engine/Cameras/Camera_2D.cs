﻿using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Cameras
{
  public class Camera_2D
  {
    private int _currentScrollValue;

    private int _previousScrollValue;

    private float _scale = 1f;

    public Vector2 Position = new Vector2(160, 160);

    public float Scale
    {
      get { return _scale; }
      set
      {
        _scale = MathHelper.Clamp(value, 0.8f, 1.4f);
      }
    }

    public Matrix Transform { get; private set; }

    public void Follow(Vector2 target)
    {
      Position = target;

      Transform = Matrix.CreateTranslation(-Position.X + (GameEngine.ScreenWidth / 2), -Position.Y + (GameEngine.ScreenHeight / 2), 0);
    }

    public void Update(GameTime gameTime)
    {
      _previousScrollValue = _currentScrollValue;
      _currentScrollValue = Mouse.GetState().ScrollWheelValue;

      var speed = 250f * (float)gameTime.ElapsedGameTime.TotalSeconds;

      if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
        speed *= 2;

      speed = (int)Math.Ceiling(speed);

      if (Keyboard.GetState().IsKeyDown(Keys.A))
        Position.X -= speed;
      else if (Keyboard.GetState().IsKeyDown(Keys.D))
        Position.X += speed;

      if (Keyboard.GetState().IsKeyDown(Keys.W))
        Position.Y -= speed;
      else if (Keyboard.GetState().IsKeyDown(Keys.S))
        Position.Y += speed;

      //if (_previousScrollValue < _currentScrollValue)
      //  Scale += 0.05f;
      //else if (_previousScrollValue > _currentScrollValue)
      //  Scale -= 0.05f;

      Transform = Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
        Matrix.CreateScale(Scale) *
         Matrix.CreateTranslation((GameEngine.ScreenWidth / 2), (GameEngine.ScreenHeight / 2), 0);
    }
  }
}
