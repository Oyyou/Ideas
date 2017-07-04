using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ideas.Controls
{
  public class Button : Component
  {
    private MouseState _currentMouse;

    private SpriteFont _font;

    private MouseState _previousMouse;

    private Texture2D _texture;

    public event EventHandler Click;

    public Color PenColor { get; set; }

    public bool IsHovering { get; private set; }

    public Vector2 Position;

    public Rectangle Rectangle { get; private set; }

    public bool Selected { get; set; }

    public string Text { get; set; }

    public Button(Texture2D texture, SpriteFont font)
    {
      _texture = texture;

      _font = font;

      Rectangle = new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);

      PenColor = Color.Black;

      Selected = false;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (!Selected)
      {
        if (IsHovering)
        {
          spriteBatch.Draw(_texture, Rectangle, Color.Gray);
        }
        else
        {
          spriteBatch.Draw(_texture, Rectangle, Color.White);
        }
      }
      else
      {
        spriteBatch.Draw(_texture, Rectangle, Color.Yellow);
      }

      if (!string.IsNullOrEmpty(Text))
      {
        float x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
        float y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

        spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColor);
      }
    }

    public override void LoadContent(ContentManager content)
    {

    }

    public override void UnloadContent()
    {

    }

    public override void Update(GameTime gameTime)
    {
      Rectangle = new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);

      _previousMouse = _currentMouse;
      _currentMouse = Mouse.GetState();

      var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

      IsHovering = false;

      if (mouseRectangle.Intersects(Rectangle))
      {
        IsHovering = true;

        if (_currentMouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
        {                    
          Click?.Invoke(this, new EventArgs());
        }
      }
    }
  }
}
