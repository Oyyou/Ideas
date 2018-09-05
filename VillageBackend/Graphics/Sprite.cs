using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageBackend.Graphics
{
  public class Sprite : ICloneable
  {
    private float _opacity = 1f;

    protected Texture2D _texture;

    public bool IsFixedLayer = false;

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

    public Rectangle GridRectangle1x1
    {
      get
      {
        return new Rectangle(GridRectangle.X / 32, GridRectangle.Y / 32, GridRectangle.Width / 32, GridRectangle.Height / 32);
      }
    }

    public Rectangle? SourceRectangle { get; set; } = null;

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

    public Sprite(Texture2D texture, int frameIndex, int frameWidth, int frameHeight)
      :this(texture)
    {
      SourceRectangle = GetSourceRectangle(texture, frameIndex, frameWidth, frameHeight);
    }

    private Rectangle? GetSourceRectangle(Texture2D texture, int frameIndex, int frameWidth, int frameHeight)
    {
      int index = 0;
      for (int y = 0; y < texture.Height / frameHeight; y++)
      {
        for (int x = 0; x < texture.Width / frameWidth; x++)
        {
          if (index == frameIndex)
          {
            return new Rectangle(x * frameWidth, y * frameHeight, frameWidth, frameHeight);
          }

          index++;
        }
      }

      return null;
    }

    public virtual void Update(GameTime gameTime)
    {

    }

    public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Position, SourceRectangle, Colour * Opacity, 0f, Origin, 1f, SpriteEffects.None, IsFixedLayer ? Layer : 0.3f + ((float)GridRectangle.Y / 1000f));
    }

    public object Clone()
    {
      return this.MemberwiseClone();
    }
  }
}
