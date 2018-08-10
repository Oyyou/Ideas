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

namespace Engine.Sprites
{
  public class Sprite : Component, ICloneable
  {
    protected AnimationManager _animationManager;

    protected Dictionary<string, Animation> _animations;

    private Color _color;

    private float _layer;

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
      }
    }

    public float? LifeTimer;

    public Vector2 Origin { get; set; }

    public override Vector2 Position
    {
      get { return new Vector2(Rectangle.X, Rectangle.Y); }
      set
      {
        Rectangle = new Rectangle((int)value.X, (int)value.Y, Rectangle.Width, Rectangle.Height);

        if (_animationManager != null)
          _animationManager.Position = value;
      }
    }

    public float Rotation;

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

      Velocity = Vector2.Zero;
    }

    private void Initialise()
    {
      Components = new List<Component>();

      Color = Color.White;

      Origin = new Vector2(0, 0);

      IsVisible = true;

      IsCollidable = true;

      Scale = 1f;
    }

    protected bool IsTouchingLeft(Sprite sprite)
    {
      foreach (var rectangle in sprite.CollisionRectangles)
      {
        if (IsTouchingLeft(rectangle))
          return true;
      }

      return false;
    }

    protected bool IsTouchingLeft(Rectangle rectangle)
    {
      foreach (var rect in this.CollisionRectangles)
      {
        if (rect.Right + this.Velocity.X > rectangle.Left &&
            rect.Left < rectangle.Left &&
            rect.Bottom > rectangle.Top &&
            rect.Top < rectangle.Bottom)
        {
          return true;
        }
      }

      return false;
    }

    protected bool IsTouchingRight(Sprite sprite)
    {
      foreach (var rectangle in sprite.CollisionRectangles)
      {
        if (IsTouchingRight(rectangle))
          return true;
      }

      return false;
    }

    protected bool IsTouchingRight(Rectangle rectangle)
    {
      foreach (var rect in this.CollisionRectangles)
      {
        if (rect.Left + this.Velocity.X < rectangle.Right &&
            rect.Right > rectangle.Right &&
            rect.Bottom > rectangle.Top &&
            rect.Top < rectangle.Bottom)
        {
          return true;
        }
      }

      return false;
    }

    protected bool IsTouchingTop(Sprite sprite)
    {
      foreach (var rectangle in sprite.CollisionRectangles)
      {
        if (IsTouchingTop(rectangle))
          return true;
      }

      return false;
    }

    protected bool IsTouchingTop(Rectangle rectangle)
    {
      foreach (var rect in this.CollisionRectangles)
      {
        if (rect.Bottom + this.Velocity.Y > rectangle.Top &&
            rect.Top < rectangle.Top &&
            rect.Right > rectangle.Left &&
            rect.Left < rectangle.Right)
        {
          return true;
        }
      }

      return false;
    }

    protected bool IsTouchingBottom(Sprite sprite)
    {
      foreach (var rectangle in sprite.CollisionRectangles)
      {
        if (IsTouchingBottom(rectangle))
          return true;
      }

      return false;
    }

    protected bool IsTouchingBottom(Rectangle rectangle)
    {
      foreach (var rect in this.CollisionRectangles)
      {
        if (rect.Top + this.Velocity.Y < rectangle.Bottom &&
            rect.Bottom > rectangle.Bottom &&
            rect.Right > rectangle.Left &&
            rect.Left < rectangle.Right)
        {
          return true;
        }
      }

      return false;
    }

    public override void LoadContent(ContentManager content)
    {

    }

    protected virtual void SetAnimation()
    {

    }

    public Sprite(Dictionary<string, Animation> animations)
    {
      _animations = animations;

      _animationManager = new AnimationManager(_animations.FirstOrDefault().Value);

      Rectangle = new Rectangle(0, 0, _animationManager.FrameWidth, _animationManager.FrameHeight);

      Initialise();
    }

    public Sprite(Texture2D texture)
    {
      _texture = texture;

      Rectangle = new Rectangle(0, 0, _texture.Width, _texture.Height);

      SourceRectangle = new Rectangle(0, 0, _texture.Width, _texture.Height);

      Initialise();
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
  }
}
