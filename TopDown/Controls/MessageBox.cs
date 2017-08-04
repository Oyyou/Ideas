using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TopDown.Controls
{
  public class MessageBox : Component
  {
    private SpriteFont _font;

    private Texture2D _texture;

    private float _timer;

    private const float _timerMax = 3.0f;

    private string _text;

    public override Vector2 Position
    {
      get
      {
        return new Vector2((Game1.ScreenWidth / 2) - (_texture.Width / 2), 48);
      }

      set
      {
        base.Position = value;
      }
    }

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (!IsVisible)
        return;

      spriteBatch.Draw(_texture, Position, null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, Layer);

      if (!string.IsNullOrEmpty(_text))
      {
        float x = (Position.X + (_texture.Width / 2)) - (_font.MeasureString(_text).X / 2);
        float y = (Position.Y + (_texture.Height / 2)) - (_font.MeasureString(_text).Y / 2);

        spriteBatch.DrawString(_font, _text, new Vector2(x, y), Color.Black, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer + 0.0001f);
      }
    }

    public override void LoadContent(ContentManager content)
    {

    }

    public MessageBox(Texture2D texture, SpriteFont font)
    {
      _texture = texture;

      _font = font;
    }

    public void Show(string text, bool fadeOut = true)
    {
      _text = text;

      _timer = _timerMax;

      if (fadeOut)
        _timer = 0;

      IsVisible = true;
    }

    public override void UnloadContent()
    {
      _texture.Dispose();
    }

    public override void Update(GameTime gameTime)
    {
      if (!IsVisible)
        return;

      _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

      if (_timer > _timerMax)
      {
        _timer = 0.0f;
        IsVisible = false;
      }
    }
  }
}
