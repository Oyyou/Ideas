﻿using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Sprites;
using TopDown.States;
using Microsoft.Xna.Framework.Input;

namespace TopDown.Buildings
{
  public enum FurnatureStates
  {
    Placing,
    Placed,
  }

  public class Furniture : Sprite
  {
    private GameScreen _gameState;

    private Building _parent;

    private bool _updated;

    public FurnatureStates State { get; set; }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);
    }

    public Furniture(Texture2D texture, GameScreen gameState, Building parent) : base(texture)
    {
      _gameState = gameState;

      _parent = parent;
    }

    public override void Update(GameTime gameTime)
    {
      Color = Color.White;

      switch (State)
      {
        case FurnatureStates.Placing:

          if (!_updated)
          {
            _updated = true;
            break;
          }

          Position = new Vector2(
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.X / 32) * 32,
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.Y / 32) * 32);

          if (IsInParent())
          {
            if (GameScreen.Mouse.LeftClicked)
            {
              State = FurnatureStates.Placed;
              _gameState.State = States.States.ItemMenu;
            }
          }
          else
          {
            Color = Color.Red;
          }

          break;
        case FurnatureStates.Placed:

          break;
        default:
          break;
      }
    }

    private bool IsInParent()
    {
      return this.Rectangle.Left >= _parent.Rectangle.Left &&
        this.Rectangle.Top >= _parent.Rectangle.Top &&
        this.Rectangle.Right <= _parent.Rectangle.Right &&
        this.Rectangle.Bottom <= _parent.Rectangle.Bottom;
    }
  }
}
