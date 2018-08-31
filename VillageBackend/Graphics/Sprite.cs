using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageBackend.Graphics
{
  public class Sprite
  {
    private float _opacity = 1f;

    protected Texture2D _texture;

    public Vector2 Position { get; set; }

    public Rectangle Rectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
      }
    }

    public virtual Rectangle GridRectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y + 64, _texture.Width, _texture.Height - 64);
      }
    }

    public Color Colour { get; set; } = Color.White;

    public float Opacity
    {
      get { return MathHelper.Clamp(_opacity, 0f, 1f); }
      set
      {
        _opacity = MathHelper.Clamp(value, 0f, 1f);
      }
    }

    public Vector2 Origin { get; set; } = new Vector2(0, 0);

    public float Layer { get; set; }

    public Sprite(Texture2D texture)
    {
      _texture = texture;
    }

    public virtual void Update(GameTime gameTime)
    {

    }

    public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Position, null, Colour * Opacity, 0f, Origin, 1f, SpriteEffects.None, GridRectangle.Y / 1000f);
    }
  }
}
