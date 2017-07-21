using Engine.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Engine.Sprites;
using Microsoft.Xna.Framework;
using TopDown.States;
using TopDown.Buildings;
using TopDown.Controls.BuildMenu;
using Engine;

namespace TopDown.Controls.ItemMenu
{
  public enum ItemMenuButtonStates
  {
    Clickable,
    Clicked,
    Placed,
  }

  public class ItemMenuButton : Button
  {
    protected override Color _colour
    {
      get
      {
        switch (CurrentState)
        {
          case ItemMenuButtonStates.Clickable:

            if (IsHovering)
              return Color.DarkGreen;

            return Color.Green;

          case ItemMenuButtonStates.Clicked:

            if(IsHovering)
              return Color.Orange;

            return Color.Yellow;

          case ItemMenuButtonStates.Placed:
            return Color.Gray;
        }

        return Color.White;
      }
    }

    public bool CanClick { get; set; }

    public BuildMenuSubButton Parent { get; set; }

    public Component PlacingObject { get; set; }

    public ItemMenuButtonStates CurrentState { get; set; }

    public ItemMenuButtonStates PreviousState { get; set; }
    
    public ItemMenuButton(Texture2D texture, SpriteFont font) : base(texture, font)
    {
      CurrentState = ItemMenuButtonStates.Clickable;

      CanClick = true;
    }

    public override void Update(GameTime gameTime)
    {
      PreviousState = CurrentState;

      switch (CurrentState)
      {
        case ItemMenuButtonStates.Clickable:

          IsClicked = false;
          IsHovering = false;

          if (GameScreen.Mouse.Rectangle.Intersects(Rectangle))
          {
            IsHovering = true;

            if (GameScreen.Mouse.LeftClicked && CanClick)
            {
              CurrentState = ItemMenuButtonStates.Clicked;
              IsClicked = true;
              OnClick();
            }
          }

          break;
        case ItemMenuButtonStates.Clicked:

          if (GameScreen.Mouse.Rectangle.Intersects(Rectangle))
          {
            IsHovering = true;

            if (GameScreen.Mouse.LeftClicked)
            {
              OnClick();
            }
          }

          break;
        case ItemMenuButtonStates.Placed:

          break;
        default:
          break;
      }
    }
  }
}
