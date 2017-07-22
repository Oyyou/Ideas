using Engine;
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

namespace TopDown.Furnitures
{
  public enum PlacableObjectStates
  {
    Placing,
    Placed,
  }

  public class Furniture : Sprite, ICloneable
  {
    private GameScreen _gameState;

    private bool _updated;

    public Component Building { get; set; }

    public PlacableObjectStates State { get; set; }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);
    }

    public Furniture(Texture2D texture, GameScreen gameState) : base(texture)
    {
      _gameState = gameState;

      IsCollidable = true;
    }

    public override void Update(GameTime gameTime)
    {
      Color = Color.White;

      switch (State)
      {
        case PlacableObjectStates.Placing:

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
              State = PlacableObjectStates.Placed;

              _gameState.ItemMenu.CurrentButton.Amount--;

              if (_gameState.ItemMenu.CurrentButton.Amount > 0)
              {
                _gameState.ItemMenu.CurrentButton.CurrentState = Controls.ItemMenu.ItemMenuButtonStates.Clicked;

                var newFurniture = (Furniture)this.Clone();
                newFurniture.State = PlacableObjectStates.Placing;

                _gameState.SelectedBuilding.Components.Add(newFurniture);
                _gameState.State = States.GameStates.PlacingItems;
              }
              else
              {
                _gameState.ItemMenu.CurrentButton.CurrentState = Controls.ItemMenu.ItemMenuButtonStates.Placed;
                _gameState.State = States.GameStates.ItemMenu;
              }
            }
          }
          else
          {
            Color = Color.Red;
          }

          break;
        case PlacableObjectStates.Placed:

          break;
        default:
          break;
      }
    }

    private bool IsInParent()
    {
      return this.Rectangle.Left >= Building.Rectangle.Left &&
        this.Rectangle.Top >= Building.Rectangle.Top &&
        this.Rectangle.Right <= Building.Rectangle.Right &&
        this.Rectangle.Bottom <= Building.Rectangle.Bottom;
    }
  }
}
