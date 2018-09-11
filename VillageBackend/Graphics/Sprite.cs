using Engine.Managers;
using Engine.Models;
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
    private Vector2 _position;

    private float _layer;

    private float _opacity = 1f;

    protected Dictionary<string, Animation> _animations;

    protected AnimationManager _animationManager;

    protected Texture2D _texture;

    public bool IsFixedLayer = false;

    public Vector2 Position
    {
      get { return _position; }
      set
      {
        _position = value;

        if (_animationManager != null)
          _animationManager.Position = _position;
      }
    }

    public Rectangle Rectangle
    {
      get
      {
        int width = 0;
        int height = 0;

        if (_animationManager != null)
        {
          width = _animationManager.FrameWidth;
          height = _animationManager.FrameHeight;
        }

        if (_texture != null)
        {
          width = _texture.Width;
          height = _texture.Height;
        }

        return new Rectangle((int)Position.X, (int)Position.Y, width, height);
      }
    }

    public virtual Rectangle GridRectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y + 64, Rectangle.Width, Rectangle.Height - 64);
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

    public float Layer
    {
      get { return IsFixedLayer ? _layer : 0.3f + ((float)GridRectangle.Y / 1000f); }
      set
      {
        _layer = value;

        if (_animationManager != null)
          _animationManager.Layer = _layer;
      }
    }

    public Sprite(Texture2D texture)
    {
      _texture = texture;

      _animations = null;

      _animationManager = null;
    }

    public Sprite(Dictionary<string, Animation> animations)
    {
      _animations = animations;

      _animationManager = new AnimationManager(_animations.FirstOrDefault().Value);

      _texture = null;
    }

    public Sprite(Texture2D texture, int frameIndex, int frameWidth, int frameHeight)
      : this(texture)
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
      if (_texture != null)
        spriteBatch.Draw(_texture, Position, SourceRectangle, Colour * Opacity, 0f, Origin, 1f, SpriteEffects.None, Layer);

      if (_animationManager != null)
      {
        _animationManager.Layer = this.Layer;
        _animationManager.Draw(gameTime, spriteBatch);
      }
    }

    public object Clone()
    {
      return this.MemberwiseClone();
    }
  }
}
