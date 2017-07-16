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

namespace TopDown.Controls.BuildMenu
{
  public enum ItemMenuOptionStates
  {
    Clickable,
    Clicked,
    Placed,
  }

  public class ItemMenuOption : Sprite
  {
    private SpriteFont _font;

    public bool CanClick { get; set; }

    public event EventHandler Click;

    public Furniture Furniture { get; set; }

    public bool IsClicked { get; set; }

    public bool IsHovering { get; set; }

    public BuildMenuSubItem Parent { get; set; }

    public Color PenColor { get; set; }

    public ItemMenuOptionStates CurrentState { get; set; }

    public ItemMenuOptionStates PreviousState { get; set; }

    public string Text { get; set; }

    public ItemMenuOption(Texture2D texture, SpriteFont font) : base(texture)
    {
      _font = font;

      CurrentState = ItemMenuOptionStates.Clickable;

      PenColor = Color.Black;

      CanClick = true;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      base.Draw(gameTime, spriteBatch);

      if (!string.IsNullOrEmpty(Text))
      {
        float x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
        float y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

        spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
      }
    }

    public override void Update(GameTime gameTime)
    {
      PreviousState = CurrentState;

      switch (CurrentState)
      {
        case ItemMenuOptionStates.Clickable:
          Color = Color.Green;

          IsClicked = false;
          IsHovering = false;

          if (GameScreen.Mouse.Rectangle.Intersects(Rectangle))
          {
            IsHovering = true;
            Color = Color.DarkGreen;

            if (GameScreen.Mouse.LeftClicked && CanClick)
            {
              CurrentState = ItemMenuOptionStates.Clicked;
              IsClicked = true;
              Click?.Invoke(this, new EventArgs());
            }
          }

          break;
        case ItemMenuOptionStates.Clicked:
          Color = Color.Yellow;

          if (GameScreen.Mouse.Rectangle.Intersects(Rectangle))
          {
            IsHovering = true;
            Color = Color.Orange;

            if (GameScreen.Mouse.LeftClicked)
            {
              Click?.Invoke(this, new EventArgs());
            }
          }

          break;
        case ItemMenuOptionStates.Placed:
          Color = Color.Gray;
          break;
        default:
          break;
      }
    }
  }
}
