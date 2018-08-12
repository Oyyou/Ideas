using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageGUI.Interface.Buttons
{
  public class Button
  {
    public Action<object> Click;

    public Color Color;

    public ButtonStates CurrentState;

    public readonly SpriteFont Font;

    public readonly Vector2 Origin;

    public Color PenColor;

    public Vector2 Position;

    public ButtonStates PreviousState;

    public Rectangle Rectangle
    {
      get
      {
        var width = Texture.Width * Scale;
        var height = Texture.Height * Scale;

        return new Rectangle((int)(Position.X - (width / 2)), (int)(Position.Y - (height / 2)), (int)width, (int)height);
      }
    }

    public float Scale;

    public string Text { get; set; }

    public readonly Texture2D Texture;

    public Button(Texture2D texture)
    {
      Texture = texture;

      Color = Color.White;

      Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

      PenColor = Color.Black;

      Scale = 1f;

      CurrentState = ButtonStates.Nothing;
    }

    public Button(Texture2D texture, SpriteFont font) : this(texture)
    {
      Font = font;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(Texture, Position, null, Color, 0f, Origin, Scale, SpriteEffects.None, 0.1f);

      DrawText(spriteBatch);
    }

    protected virtual void DrawText(SpriteBatch spriteBatch)
    {
      if (string.IsNullOrEmpty(Text) || Font == null)
        return;

      float x = (Rectangle.X + (Rectangle.Width / 2)) - (Font.MeasureString(Text).X / 2);
      float y = (Rectangle.Y + (Rectangle.Height / 2)) - (Font.MeasureString(Text).Y / 2);

      spriteBatch.DrawString(Font, Text, new Vector2(x, y), PenColor, 0, new Vector2(0, 0), Scale, SpriteEffects.None, 0.11f);
    }

    public void OnClick()
    {
      Click?.Invoke(this);
    }

    public void UnloadContent()
    {
      Texture.Dispose();
    }
  }
}
