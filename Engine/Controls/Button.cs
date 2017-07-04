using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Controls
{
  public class Button : Component
  {
    protected Color _colour
    {
      get
      {
        if (IsSelected)
          return Color.Yellow;

        if (IsHovering)
          return Color.Gray;

        return Color.White;
      }
    }

    protected MouseState _currentMouse;

    protected SpriteFont _font;

    protected MouseState _previousMouse;

    protected Texture2D _texture;

    public event EventHandler Click;

    public Color PenColor { get; set; }

    public bool IsHovering { get; private set; }

    public bool IsSelected { get; set; }

    public float Layer { get; set; }

    public Vector2 Position
    {
      get { return new Vector2(Rectangle.X, Rectangle.Y); }
      set
      {
        Rectangle = new Rectangle((int)value.X, (int)value.Y, Rectangle.Width, Rectangle.Height);
      }
    }

    public Rectangle Rectangle { get; private set; }

    public string Text { get; set; }

    public Button(Texture2D texture)
    {
      _texture = texture;

      _font = null;

      Rectangle = new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);

      PenColor = Color.Black;

      IsSelected = false;
    }

    public Button(Texture2D texture, SpriteFont font)
    {
      _texture = texture;

      _font = font;

      Rectangle = new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);

      PenColor = Color.Black;

      IsSelected = false;
    }

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Rectangle, null, _colour, 0, new Vector2(0, 0), SpriteEffects.None, Layer);

      if (!string.IsNullOrEmpty(Text) && _font != null)
      {
        float x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
        float y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

        spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColor, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
      }
    }

    public override void LoadContent(ContentManager content)
    {

    }

    public override void UnloadContent()
    {
      _texture.Dispose();
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

        if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
        {
          //IsSelected = true;
          Click?.Invoke(this, new EventArgs());
        }
      }
    }
  }
}
