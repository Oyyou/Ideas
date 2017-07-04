using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Ideas.Models;
using Ideas.Managers;

namespace Ideas.Sprites
{
  public class Sprite : Component
  {
    protected AnimationManager _animationManager;

    protected Dictionary<string, Animation> _animations;

    private float _layer;

    private float _rotation;

    private SpriteEffects _spriteEffect;

    private Texture2D _texture;

    public List<Component> Components;

    public float Layer
    {
      get { return _layer; }
      set
      {
        _layer = value;

        if (_animationManager != null)
          _animationManager.Layer = Layer;
      }
    }

    public Vector2 Position
    {
      get { return new Vector2(Rectangle.X, Rectangle.Y); }
      set
      {
        Rectangle = new Rectangle((int)value.X, (int)value.Y, Rectangle.Width, Rectangle.Height);

        if (_animationManager != null)
          _animationManager.Position = value;
      }
    }

    public Rectangle Rectangle;
    
    public float Rotation;

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
      if (_texture != null)
        spriteBatch.Draw(_texture,
          Rectangle,
          null,
          Color.White,
          Rotation,
          new Vector2(0, 0),
          SpriteEffect,
          Layer);

      if (_animationManager != null)
      {
        _animationManager.Draw(gameTime, spriteBatch);
      }

      foreach (var sprite in Components)
        sprite.Draw(gameTime, spriteBatch);
    }

    private void Initialise()
    {
      Components = new List<Component>();
    }

    protected bool IsTouchingLeft(Sprite sprite)
    {
      return IsTouchingLeft(sprite.Rectangle);
    }

    protected bool IsTouchingLeft(Rectangle rectangle)
    {
      return this.Rectangle.Right + this.Velocity.X > rectangle.Left &&
             this.Rectangle.Left < rectangle.Left &&
             this.Rectangle.Bottom > rectangle.Top + 8 &&
             this.Rectangle.Top < rectangle.Bottom - 8;
    }

    protected bool IsTouchingRight(Sprite sprite)
    {
      return IsTouchingRight(sprite.Rectangle);
    }

    protected bool IsTouchingRight(Rectangle rectangle)
    {
      return this.Rectangle.Left + this.Velocity.X < rectangle.Right &&
             this.Rectangle.Right > rectangle.Right &&
             this.Rectangle.Bottom > rectangle.Top + 8 &&
             this.Rectangle.Top < rectangle.Bottom - 8;
    }

    protected bool IsTouchingTop(Sprite sprite)
    {
      return IsTouchingTop(sprite.Rectangle);
    }

    protected bool IsTouchingTop(Rectangle rectangle)
    {
      return this.Rectangle.Bottom + this.Velocity.Y > rectangle.Top &&
             this.Rectangle.Top < rectangle.Top &&
             this.Rectangle.Right > rectangle.Left &&
             this.Rectangle.Left < rectangle.Right;
    }
    
    protected bool IsTouchingBottom(Sprite sprite)
    {
      return IsTouchingBottom(sprite.Rectangle);
    }

    protected bool IsTouchingBottom(Rectangle rectangle)
    {
      return this.Rectangle.Top + this.Velocity.Y < rectangle.Bottom &&
             this.Rectangle.Bottom > rectangle.Bottom &&
             this.Rectangle.Right > rectangle.Left &&
             this.Rectangle.Left < rectangle.Right;
    }

    public override void LoadContent(ContentManager content)
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

      Initialise();
    }

    public override void UnloadContent()
    {
      _texture.Dispose();

      foreach (var sprite in Components)
        sprite.UnloadContent();

      Components.Clear();
    }

    public override void Update(GameTime gameTime)
    {
      if (_animationManager != null)
        _animationManager.Update(gameTime);

      foreach (var sprite in Components)
        sprite.Update(gameTime);

    }
  }
}
