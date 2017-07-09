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

    public event EventHandler Click;
    
    public bool IsClicked { get; set; }

    public bool IsHovering { get; set; }

    public Color PenColor { get; set; }

    public ItemMenuOptionStates State { get; set; }

    public string Text { get; set; }

    public ItemMenuOption(Texture2D texture, SpriteFont font) : base(texture)
    {
      _font = font;

      State = ItemMenuOptionStates.Clickable;

      PenColor = Color.Black;
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
      switch (State)
      {
        case ItemMenuOptionStates.Clickable:
          Color = Color.Green;

          IsClicked = false;
          IsHovering = false;

          if (GameScreen.Mouse.Rectangle.Intersects(Rectangle))
          {
            IsHovering = true;
            Color = Color.DarkGreen;

            if (GameScreen.Mouse.LeftClicked)
            {
              State = ItemMenuOptionStates.Clicked;
              IsClicked = true;
              Click?.Invoke(this, new EventArgs());
            }
          }

          break;
        case ItemMenuOptionStates.Clicked:
          Color = Color.Yellow;
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
