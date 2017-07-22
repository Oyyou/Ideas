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

            if (IsHovering)
              return Color.Orange;

            return Color.Yellow;

          case ItemMenuButtonStates.Placed:
            return Color.Gray;
        }

        return Color.White;
      }
    }

    public int Amount { get; set; }

    public bool CanClick { get; set; }

    protected override void DrawText(SpriteBatch spriteBatch)
    {
      if (string.IsNullOrEmpty(Text) || _font == null)
        return;

      var newText = Text + " x " + Amount;

      if (Amount == -1)
        newText = Text;

      float x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(newText).X / 2);
      float y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(newText).Y / 2);

      spriteBatch.DrawString(_font, newText, new Vector2(x, y), PenColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
    }

    public BuildMenuSubButton Parent { get; set; }

    public Component PlacingObject { get; set; }

    public int? StartAmount { get; private set; }

    public ItemMenuButtonStates CurrentState { get; set; }

    public ItemMenuButtonStates PreviousState { get; set; }

    public ItemMenuButton(Texture2D texture, SpriteFont font) : base(texture, font)
    {
      Amount = -1;

      CurrentState = ItemMenuButtonStates.Clickable;

      CanClick = true;
    }

    public override void Update(GameTime gameTime)
    {
      if (StartAmount == null)
      {
        StartAmount = Amount;
      }

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

          // This is retarded. Fix
          //if (Amount > 0)
          //{
          //  Amount--;

          //  if (Amount > 0)
          //    CurrentState = ItemMenuButtonStates.Clickable;
          //}

          break;
        default:
          break;
      }
    }
  }
}
