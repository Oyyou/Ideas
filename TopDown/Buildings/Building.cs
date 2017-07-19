using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TopDown.States;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using TopDown.FX;
using Engine.Sprites;

namespace TopDown.Buildings
{
  public enum BuildingStates
  {
    Placing,
    Placed,
    Building,
    Built_Out,
    Built_In,
  }

  public class Building : Component
  {
    protected float _buildTimer;

    protected Sprite _spriteOutside;

    protected GameScreen _gameState;

    protected float _hitTimer;

    protected const float _maxBuildTimer = 2.5f;

    protected const float _maxHitTimer = 0.3f;

    protected virtual int _outsideExtraHeight { get; }

    protected virtual int _outsideExtraWidth { get; }

    protected List<Sprite> _particles;

    protected Sprite _spriteInside;

    protected SoundEffect _soundEffect;

    /// <summary>
    /// An instance of the _soundEffect so that when we call 'Play', it will only play if finished.
    /// </summary>
    protected SoundEffectInstance _soundEffectInstance;

    protected Texture2D _textureInside;

    protected Texture2D _textureOutside;

    protected Texture2D _woodChipTexture;

    public virtual BuildingStates BuildingState
    {
      get { return State; }
      set
      {
        if (State == value)
          return;

        State = value;

        switch (State)
        {
          case BuildingStates.Placing:
          case BuildingStates.Placed:
          case BuildingStates.Building:
            CollisionRectangles = new List<Rectangle>();
            break;
        }
      }
    }

    public Color Color { get; set; }

    public const float DefaultLayer = 0.8f;

    /// <summary>
    /// Locations in which there has to be at least one path so we can place the building.
    /// </summary>
    public virtual List<Vector2> DoorLocations
    {
      get
      {
        return new List<Vector2>()
        {
          new Vector2(_spriteInside.Rectangle.X + 32, _spriteInside.Rectangle.Bottom),
        };
      }
    }

    public override Rectangle Rectangle
    {
      get
      {
        return _spriteInside.Rectangle;
      }

      set
      {
        base.Rectangle = value;
      }
    }

    public BuildingStates State;

    public void Build(GameTime gameTime)
    {
      if (GameScreen.Mouse.MouseState != Controls.MouseStates.Building)
        return;

      if (GameScreen.Mouse.RectangleWithCamera.Intersects(_spriteInside.Rectangle))
      {
        if (Vector2.Distance(_spriteInside.Position + new Vector2(_spriteInside.Rectangle.Width / 2, _spriteInside.Rectangle.Height / 2), _gameState.Player.Position) < 150)
        {
          Color = Color.Yellow;

          if (GameScreen.Mouse.LeftDown)
          {
            _hitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_hitTimer > _maxHitTimer)
            {
              var positions = new List<Vector2>();

              for (int i = 0; i < 10; i++)
              {
                GenerateParticle(_maxHitTimer);
              }

              _buildTimer += _hitTimer;

              if (_buildTimer > _maxBuildTimer)
              {
                BuildingState = BuildingStates.Built_Out;
                _gameState.State = States.GameStates.Playing;
              }

              _hitTimer = 0f;

              _soundEffectInstance.Play();
            }
          }
        }
        else
        {
          Color = Color.Red;
          _hitTimer = 0;
          _buildTimer = 0;
        }
      }
      else
      {
        _hitTimer = 0;
        _buildTimer = 0;
      }

      foreach (var component in Components)
      {
        component.Layer = _spriteInside.Layer + 0.001f;
        component.Update(gameTime);
      }

      foreach (var component in _particles)
        component.Update(gameTime);
    }

    public Building(GameScreen gameState, Texture2D textureInside, Texture2D textureOutside)
    {
      _gameState = gameState;

      _textureInside = textureInside;

      _textureOutside = textureOutside;

      _particles = new List<Sprite>();

      IsCollidable = true;
    }

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      switch (BuildingState)
      {
        case BuildingStates.Placing:
          _spriteInside.Draw(gameTime, spriteBatch);

          foreach (var doorLocation in DoorLocations)
          {
            spriteBatch.Draw(_t, new Rectangle(doorLocation.ToPoint(), new Point(32, 32)), Color.Red);
          }

          break;

        case BuildingStates.Placed:
          _spriteInside.Draw(gameTime, spriteBatch);

          foreach (var component in Components)
            component.Draw(gameTime, spriteBatch);

          break;

        case BuildingStates.Building:
          _spriteInside.Draw(gameTime, spriteBatch);

          foreach (var particle in _particles)
            particle.Draw(gameTime, spriteBatch);

          //foreach (var rec in CollisionRectangles)
          //  spriteBatch.Draw(texture: _t, destinationRectangle: rec, color: Color.Red, layerDepth: 1f);

          break;

        case BuildingStates.Built_In:
          _spriteInside.Draw(gameTime, spriteBatch);

          foreach (var component in Components)
            component.Draw(gameTime, spriteBatch);

          //foreach (var rec in CollisionRectangles)
          //  spriteBatch.Draw(_t, rec, Color.Red);
          break;

        case BuildingStates.Built_Out:

          _spriteInside.Layer = _spriteOutside.Layer - 0.001f;
          _spriteInside.Draw(gameTime, spriteBatch);

          _spriteOutside.Draw(gameTime, spriteBatch);


          foreach (var component in Components)
            component.Draw(gameTime, spriteBatch);

          //foreach (var rec in CollisionRectangles)
          //  spriteBatch.Draw(texture: _t, destinationRectangle: rec, color: Color.Red, layerDepth: 1f);
          break;

        default:
          break;
      }
    }

    protected Texture2D _t;

    protected void GenerateParticle(float lifeTimer)
    {
      var position = new Vector2(
        GameEngine.Random.Next(_spriteInside.Rectangle.Left, _spriteInside.Rectangle.Right),
        GameEngine.Random.Next(_spriteInside.Rectangle.Top, _spriteInside.Rectangle.Bottom)
      );

      _particles.Add(
        new Particle(_woodChipTexture)
        {
          Position = position,
          Layer = _spriteInside.Layer + 0.01f,
          LifeTimer = lifeTimer,
          Rotation = MathHelper.ToRadians(GameEngine.Random.Next(0, 360)),
        }
      );
    }

    public override void LoadContent(ContentManager content)
    {
      _t = content.Load<Texture2D>("Pixel");

      _spriteOutside = new Sprite(_textureOutside)
      {
        Layer = DefaultLayer,
      };

      _spriteInside = new Sprite(_textureInside)
      {
        Layer = DefaultLayer,
      };

      _soundEffect = content.Load<SoundEffect>("Sounds/Sawing");
      _soundEffectInstance = _soundEffect.CreateInstance();

      _woodChipTexture = content.Load<Texture2D>("FX/WoodChip");

      Components = new List<Component>();
    }

    public override void UnloadContent()
    {
      _spriteOutside?.UnloadContent();
      _spriteInside?.UnloadContent();
      _soundEffect?.Dispose();

      foreach (var component in Components)
        component.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      switch (BuildingState)
      {
        case BuildingStates.Placing:

          _spriteInside.Position = new Vector2(
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.X / 32) * 32,
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.Y / 32) * 32);

          bool canPlace = false;

          _spriteInside.Color = Color.White;

          foreach (var component in _gameState.PathComponents)
          {
            if (component.Rectangle.Intersects(_spriteInside.Rectangle))
            {
              canPlace = false;
              GameScreen.MessageBox.Show("Trying to build over path", false);
              break;
            }

            if (DoorLocations.Any(c => c == component.Position))
            {
              canPlace = true;
              continue;
            }
          }

          if (!canPlace && !GameScreen.MessageBox.IsVisible)
            GameScreen.MessageBox.Show("Door needs to connect to path", false);

          if (canPlace)
          {
            foreach (var component in _gameState.CollidableComponents)
            {
              if (component.CollisionRectangles.Any(c => c.Intersects(this.Rectangle)))
              {
                canPlace = false;
                GameScreen.MessageBox.Show("Trying to build over object", false);
                break;
              }
            }
          }

          if (!canPlace)
            _spriteInside.Color = Color.Red;

          if (GameScreen.Mouse.LeftClicked && canPlace)
          {
            _spriteOutside.Position = new Vector2(_spriteInside.Position.X - (_outsideExtraWidth / 2), _spriteInside.Position.Y - _outsideExtraHeight);
            BuildingState = BuildingStates.Placed;
            _gameState.State = States.GameStates.ItemMenu;
          }

          break;
        case BuildingStates.Placed:

          foreach (var component in Components)
          {
            component.Layer = _spriteInside.Layer + 0.001f;
            component.Update(gameTime);
          }


          break;
        case BuildingStates.Building:
          Build(gameTime);
          break;

        case BuildingStates.Built_Out:

          _particles.Clear();

          foreach (var component in Components)
          {
            component.Layer = _spriteOutside.Layer - 0.001f;
            component.Update(gameTime);
          }

          if (_gameState.Player.IsIn(_spriteInside.Rectangle) ||
            (_gameState.SelectedPathBuilder != null && _gameState.SelectedPathBuilder.State == Builders.PathBuilderStates.Placing))
          {
            State = BuildingStates.Built_In;
          }

          //if (_gameState.Player.Rectangle.Y <= InsideRectangle.Y + 60)
          //  _spriteOutside.Layer = _gameState.Player.Layer + 0.001f;
          //else
          //  _spriteOutside.Layer = Building.DefaultLayer;

          break;

        case BuildingStates.Built_In:

          _particles.Clear();

          _spriteInside.Layer = Building.DefaultLayer;

          foreach (var component in Components)
          {
            component.Layer = _spriteInside.Layer + 0.001f;
            component.Update(gameTime);
          }

          if (!_gameState.Player.IsIn(_spriteInside.Rectangle) &&
            !(_gameState.SelectedPathBuilder != null && _gameState.SelectedPathBuilder.State == Builders.PathBuilderStates.Placing))
          {
            // Set Position
            //Position = new Vector2(Position.X - (_template.OutExtraWidth / 2), Position.Y - _template.OutExtraHeight);
            State = BuildingStates.Built_Out;
          }

          break;

        default:
          break;
      }
    }
  }
}
