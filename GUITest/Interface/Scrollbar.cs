using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUITest.Interface
{
  public class Scrollbar
  {
    private int _currentScrollValue;

    private bool _isSelected = false;

    private Texture2D _inner;

    private int _innerHeight;

    private Rectangle _innerRectangle
    {
      get
      {
        return new Rectangle((int)Position.X + 3, _innerY, _inner.Width, _innerHeight);
      }
    }

    public int _innerY;

    private Texture2D _outer;

    private int _previousScrollValue;

    public int MinContentHeight { get; set; }

    /// <summary>
    /// The position of the scrollbar
    /// </summary>
    public Vector2 Position;

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_outer, Position, Color.White);

      spriteBatch.Draw(_inner, _innerRectangle, Color.White);
    }

    public Scrollbar(ContentManager content)
    {
      _inner = content.Load<Texture2D>("Interface/Scrollbar_Inner");

      _outer = content.Load<Texture2D>("Interface/Scrollbar_Outer");
    }

    public void Update(GameTime gameTime, int contentHeight)
    {
      var scrollTrackSpace = contentHeight - (_outer.Height - 6);
      var scrollThumbSpace = _outer.Height;

      _innerHeight = scrollThumbSpace + scrollTrackSpace;

      _previousScrollValue = _currentScrollValue;
      _currentScrollValue = Mouse.GetState().ScrollWheelValue;

      var mouseRectangle = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);

      if (mouseRectangle.Intersects(_innerRectangle) && Mouse.GetState().LeftButton == ButtonState.Pressed)
      {
        _isSelected = true;
      }

      if (Mouse.GetState().LeftButton == ButtonState.Released)
      {
        _isSelected = false;
      }

      if (_isSelected)
      {
        _innerY = mouseRectangle.Y - (_innerRectangle.Height / 2);
      }

      if (_previousScrollValue != _currentScrollValue)
      {
        _innerY += (_previousScrollValue - _currentScrollValue) / 10;
      }

      _innerY = MathHelper.Clamp(_innerY, (int)Position.Y + 3, ((int)Position.Y + _outer.Height - 3) - _innerRectangle.Height);
    }
  }
}
