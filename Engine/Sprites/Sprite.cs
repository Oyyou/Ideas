using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Engine.Models;
using Engine.Managers;
using Engine.States;
using Newtonsoft.Json;

namespace Engine.Sprites
{
  public class SerializableObject
  {
    public string Type { get; set; }

    public string TexturePath { get; set; }

    public Vector2 Position { get; set; }

    public Color Colour { get; set; }

    public float Rotation { get; set; }

    public Vector2 Origin { get; set; }

    public float Layer { get; set; }
  }

  public class Sprite : Component, ICloneable
  {
    protected AnimationManager _animationManager;

    protected Dictionary<string, Animation> _animations;

    private Color _color;

    private float _layer;

    private float _rotation;

    private Vector2 _origin;

    private float _scale;

    private Rectangle _sourceRectangle;

    private SpriteEffects _spriteEffect;

    protected Texture2D _texture;

    private float _timer;

    public override List<Rectangle> CollisionRectangles
    {
      get
      {
        if (!IsCollidable)
          return new List<Rectangle>();

        return new List<Rectangle>()
        {
          Rectangle,
        };
      }
    }

    public Color Color
    {
      get { return _color; }
      set
      {
        _color = value;

        if (_animationManager != null)
          _animationManager.Color = _color;

        SO.Colour = _color;
      }
    }

    public override float Layer
    {
      get { return _layer; }
      set
      {
        _layer = value;

        if (_animationManager != null)
          _animationManager.Layer = _layer;

        SO.Layer = _layer;
      }
    }

    public float? LifeTimer;

    public Vector2 Origin
    {
      get { return _origin; }
      set
      {
        _origin = value;
        SO.Origin = _origin;
      }
    }

    public override Vector2 Position
    {
      get { return new Vector2(Rectangle.X, Rectangle.Y); }
      set
      {
        Rectangle = new Rectangle((int)value.X, (int)value.Y, Rectangle.Width, Rectangle.Height);

        if (_animationManager != null)
          _animationManager.Position = value;

        SO.Position = new Vector2(Rectangle.X, Rectangle.Y);
      }
    }

    public float Rotation
    {
      get { return _rotation; }
      set
      {
        _rotation = value;

        SO.Rotation = value;
      }
    }

    public float Scale
    {
      get { return _scale; }
      set
      {
        _scale = value;

        if (_animationManager != null)
          _animationManager.Scale = _scale;
      }
    }

    public Rectangle SourceRectangle
    {
      get
      {
        return _sourceRectangle;
      }
      set
      {
        _sourceRectangle = value;
        Rectangle = new Rectangle(Rectangle.X, Rectangle.Y, value.Width, value.Height);
      }
    }

    public SpriteEffects SpriteEffect
    {
      get { return _spriteEffect; }
      set
      {
        _spriteEffect = value;

        if (_animationManager != null)
          _animationManager.SpriteEffect = _spriteEffect;
      }
    }

    public Vector2 Velocity;

    public SerializableObject SO;

    public Sprite(Dictionary<string, Animation> animations)
    {
      _animations = animations;

      _animationManager = new AnimationManager(_animations.FirstOrDefault().Value);

      Rectangle = new Rectangle(0, 0, _animationManager.FrameWidth, _animationManager.FrameHeight);

      Initialise();
    }

    [JsonConstructor]
    public Sprite(Texture2D texture)
    {
      _texture = texture;

      Rectangle = new Rectangle(0, 0, _texture.Width, _texture.Height);

      SourceRectangle = new Rectangle(0, 0, _texture.Width, _texture.Height);

      Initialise();
    }

    private void Initialise()
    {
      SO = new SerializableObject()
      {
        Type = this.GetType().ToString(),
        TexturePath = _texture != null ? _texture.Name : "",
      };

      Components = new List<Component>();

      Color = Color.White;

      Origin = new Vector2(0, 0);

      IsVisible = true;

      IsCollidable = true;

      Scale = 1f;
    }

    public override void LoadContent(ContentManager content)
    {

    }

    protected virtual void SetAnimation()
    {

    }

    public override void UnloadContent()
    {
      _texture?.Dispose();

      _animationManager?.UnloadContent();

      foreach (var sprite in Components)
        sprite.UnloadContent();

      Components.Clear();
    }

    public override void Update(GameTime gameTime)
    {
      _timer += (float)gameTime.ElapsedGameTime.TotalSeconds * State.GameSpeed;

      if (LifeTimer != null && _timer >= LifeTimer)
      {
        IsRemoved = true;
      }

      if (_animationManager != null)
        _animationManager.Update(gameTime);

      foreach (var sprite in Components)
        sprite.Update(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (!IsVisible)
        return;

      Position += Velocity;

      if (_texture != null)
      {
        var layer = Layer;// + Position.Y / 100000;

        spriteBatch.Draw(
          _texture,
          Position,
          SourceRectangle,
          Color,
          Rotation,
          Origin,
          Scale,
          SpriteEffect,
          MathHelper.Clamp(layer, 0f, 1f));
      }

      if (_animationManager != null)
      {
        SetAnimation();
        _animationManager.Draw(gameTime, spriteBatch);
      }

      foreach (var sprite in Components)
        sprite.Draw(gameTime, spriteBatch);

      //Velocity = Vector2.Zero;
    }

    public override object Clone()
    {
      var sprite = base.Clone() as Sprite;

      if (_animations != null)
      {
        sprite._animations = this._animations.ToDictionary(c => c.Key, v => v.Value.Clone() as Animation);
        sprite._animationManager = sprite._animationManager.Clone() as AnimationManager;
      }

      return sprite;
    }
  }
}
