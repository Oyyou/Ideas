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
    protected virtual Color _colour
    {
      get
      {
        if (!IsEnabled)
          return Color.Black;

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

    public bool IsClicked { get; set; }

    public bool IsHovering { get; set; }

    public bool IsSelected { get; set; }

    public override Vector2 Position
    {
      get { return new Vector2(Rectangle.X, Rectangle.Y); }
      set
      {
        Rectangle = new Rectangle((int)value.X, (int)value.Y, Rectangle.Width, Rectangle.Height);
      }
    }

    public override Rectangle Rectangle { get; set; }

    public float Scale { get; set; }

    public string Text { get; set; }

    public Button(Texture2D texture)
    {
      _texture = texture;

      _font = null;

      Initialise();
    }

    public Button(Texture2D texture, SpriteFont font)
    {
      _texture = texture;

      _font = font;

      Initialise();
    }

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (!IsVisible)
        return;

      spriteBatch.Draw(_texture, Position, null, _colour, 0, new Vector2(0, 0), Scale, SpriteEffects.None, Layer);

      foreach (var component in Components)
        component.Draw(gameTime, spriteBatch);

      DrawText(spriteBatch);
    }

    protected virtual void DrawText(SpriteBatch spriteBatch)
    {
      if (string.IsNullOrEmpty(Text) || _font == null)
        return;

      float x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
      float y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

      spriteBatch.DrawString(_font, Text, new Vector2(x, y), PenColor, 0, new Vector2(0, 0), Scale, SpriteEffects.None, Layer + 0.001f);
    }

    private void Initialise()
    {
      Rectangle = new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);

      PenColor = Color.Black;

      IsSelected = false;

      IsEnabled = true;

      IsVisible = true;

      Scale = 1f;
    }

    protected virtual void OffHover()
    {

    }

    protected virtual void OnHover()
    {

    }

    public override void LoadContent(ContentManager content)
    {
      Components = new List<Component>();
    }

    public virtual void OnClick()
    {
      Click?.Invoke(this, new EventArgs());
    }

    public override void UnloadContent()
    {
      _texture.Dispose();

      foreach (var component in Components)
        component.UnloadContent();

      Components.Clear();
    }

    public override void Update(GameTime gameTime)
    {
      if (!IsEnabled)
        return;

      Rectangle = new Rectangle((int)Position.X, (int)Position.Y, (int)(_texture.Width * Scale), (int)(_texture.Height * Scale));

      _previousMouse = _currentMouse;
      _currentMouse = Mouse.GetState();

      var mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

      IsHovering = false;
      IsClicked = false;

      if (mouseRectangle.Intersects(Rectangle))
      {
        IsHovering = true;

        if (_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
        {
          IsClicked = true;
          OnClick();
        }
      }

      foreach (var component in Components)
        component.Update(gameTime);

      if (IsHovering)
        OnHover();
      else OffHover();
    }
  }
}
