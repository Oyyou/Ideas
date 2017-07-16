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
using TopDown.Buildings.Templates;

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

    protected Sprite _builtSprite;

    protected GameScreen _gameState;

    protected float _hitTimer;

    protected const float _maxBuildTimer = 2.5f;

    protected const float _maxHitTimer = 0.3f;

    protected List<Sprite> _particles;

    protected Sprite _placedSprite;

    protected SoundEffect _soundEffect;

    /// <summary>
    /// An instance of the _soundEffect so that when we call 'Play', it will only play if finished.
    /// </summary>
    protected SoundEffectInstance _soundEffectInstance;

    protected BuildingTemplate _template;

    protected Texture2D _woodChipTexture;

    public BuildingStates BuildingState
    {
      get { return State; }
      set
      {
        if (State == value)
          return;

        State = value;

        var height = CurrentSprite.Rectangle.Height;
        var width = CurrentSprite.Rectangle.Width;

        switch (State)
        {
          case BuildingStates.Placing:
            CollisionRectangles = new List<Rectangle>();
            break;
          case BuildingStates.Placed:
          case BuildingStates.Building:
            CollisionRectangles = new List<Rectangle>()
            {
              new Rectangle((int)Position.X, (int)Position.Y, width, height),
            };
            break;
          case BuildingStates.Built_Out:

            var yDiff = _template.OutExtraHeight;
            var xDiff = _template.OutExtraWidth;

            var collisionRectangles = new List<Rectangle>()
            {
              new Rectangle((int)Position.X, (int)Position.Y, width - xDiff, 1), // Top
              new Rectangle((int)Position.X, (int)Position.Y, 1, height - yDiff), // Left
              new Rectangle((int)Position.X + (width - 1) - xDiff, (int)Position.Y, 1, height - yDiff), // Right
              new Rectangle((int)Position.X, (int)Position.Y + height - yDiff - 1, 21, 1), // bottom left
              new Rectangle((int)Position.X + 61, (int)Position.Y + height - yDiff - 1, 131, 1), // bottom right
            };

            collisionRectangles.AddRange(Components.SelectMany(c => c.CollisionRectangles).ToList());

            CollisionRectangles = collisionRectangles;

            break;
        }
      }
    }

    public Color Color { get; set; }

    public virtual Sprite CurrentSprite
    {
      get
      {
        switch (BuildingState)
        {
          case BuildingStates.Placing:
          case BuildingStates.Building:
          case BuildingStates.Placed:
          case BuildingStates.Built_In:
            return _placedSprite;

          case BuildingStates.Built_Out:
            return _builtSprite;

          default:
            throw new Exception("Unknown state: " + BuildingState);
        }
      }
    }

    public const float DefaultLayer = 0.8f;

    public Rectangle InsideRectangle
    {
      get
      {
        switch (State)
        {
          case BuildingStates.Placing:
          case BuildingStates.Built_In:
          case BuildingStates.Placed:
          case BuildingStates.Building:
            return Rectangle;

          case BuildingStates.Built_Out:
            return new Rectangle(Rectangle.X + (_template.OutExtraWidth / 2), Rectangle.Y + _template.OutExtraHeight, Rectangle.Width - _template.OutExtraWidth, Rectangle.Height - _template.OutExtraHeight);

        }

        return Rectangle;
      }
    }

    public override float Layer
    {
      get
      {
        return CurrentSprite.Layer;
      }
    }

    public Vector2 Position
    {
      get { return CurrentSprite.Position; }
      set
      {
        if (_builtSprite != null)
          _builtSprite.Position = value;

        if (_placedSprite != null)
          _placedSprite.Position = value;
      }
    }

    public override Rectangle Rectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, CurrentSprite.Rectangle.Width, CurrentSprite.Rectangle.Height);
      }
    }

    public BuildingStates State;

    public void Build(GameTime gameTime)
    {
      if (GameScreen.Mouse.MouseState != Controls.MouseStates.Building)
        return;

      if (GameScreen.Mouse.RectangleWithCamera.Intersects(this.Rectangle))
      {
        if (Vector2.Distance(this.Position + new Vector2(Rectangle.Width / 2, Rectangle.Height / 2), _gameState.Player.Position) < 150)
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
                _gameState.State = States.States.Playing;

                Position = new Vector2(Position.X - (_template.OutExtraWidth / 2), Position.Y - _template.OutExtraHeight);
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
        component.Update(gameTime);

      foreach (var component in _particles)
        component.Update(gameTime);
    }

    public Building(GameScreen gameState, BuildingTemplate template)
    {
      _gameState = gameState;

      _template = template;

      _particles = new List<Sprite>();
    }

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      CurrentSprite.Draw(gameTime, spriteBatch);

      switch (BuildingState)
      {
        case BuildingStates.Placing:

          break;

        case BuildingStates.Placed:

          foreach (var component in Components)
            component.Draw(gameTime, spriteBatch);

          break;

        case BuildingStates.Building:

          foreach (var particle in _particles)
            particle.Draw(gameTime, spriteBatch);

          break;

        case BuildingStates.Built_In:

          foreach (var component in Components)
          {
            component.Layer = CurrentSprite.Layer + 0.001f;
            component.Draw(gameTime, spriteBatch);
          }

          //foreach (var rec in CollisionRectangles)
          //  spriteBatch.Draw(_t, rec, Color.Red);
          break;

        case BuildingStates.Built_Out:

          foreach (var component in Components)
          {
            component.Layer = CurrentSprite.Layer - 0.001f;
            component.Draw(gameTime, spriteBatch);
          }

          foreach (var rec in CollisionRectangles)
            spriteBatch.Draw(_t, rec, Color.Red);
          break;

        default:
          break;
      }
    }

    protected Texture2D _t;

    protected void GenerateParticle(float lifeTimer)
    {
      var position = new Vector2(
        GameEngine.Random.Next((int)Position.X, (int)Position.X + Rectangle.Width),
        GameEngine.Random.Next((int)Position.Y, (int)Position.Y + Rectangle.Height)
      );

      _particles.Add(
        new Particle(_woodChipTexture)
        {
          Position = position,
          Layer = DefaultLayer + 0.01f,
          LifeTimer = lifeTimer,
          Rotation = MathHelper.ToRadians(GameEngine.Random.Next(0, 360)),
        }
      );
    }

    public override void LoadContent(ContentManager content)
    {
      _t = content.Load<Texture2D>("Buildings/SmallHouse/Out");

      _builtSprite = new Sprite(content.Load<Texture2D>("Buildings/SmallHouse/Out"))
      {
        Layer = DefaultLayer,
      };

      _placedSprite = new Sprite(content.Load<Texture2D>("Buildings/SmallHouse/In"))
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
      _builtSprite?.UnloadContent();
      _placedSprite?.UnloadContent();
      _soundEffect?.Dispose();

      foreach (var component in Components)
        component.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      switch (BuildingState)
      {
        case BuildingStates.Placing:

          Position = new Vector2(
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.X / 32) * 32,
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.Y / 32) * 32);

          bool canPlace = true; 

          CurrentSprite.Color = Color.White;

          foreach (var component in _gameState.CollidableComponents)
          {
            if (component.CollisionRectangles.Any(c => c.Intersects(this.Rectangle)))
            {
              canPlace = false;
              CurrentSprite.Color = Color.Red;
              break;
            }
          }

          if (GameScreen.Mouse.LeftClicked && canPlace)
          {
            BuildingState = BuildingStates.Placed;
            _gameState.State = States.States.ItemMenu;
          }

          break;
        case BuildingStates.Placed:

          foreach (var component in Components)
            component.Update(gameTime);


          break;
        case BuildingStates.Building:
          Build(gameTime);
          break;

        case BuildingStates.Built_Out:

          _particles.Clear();

          foreach (var component in Components)
            component.Update(gameTime);

          if (_gameState.Player.IsIn(this.InsideRectangle))
          {
            Position = new Vector2(Position.X + (_template.OutExtraWidth / 2), Position.Y + _template.OutExtraHeight);
            State = BuildingStates.Built_In;
          }

          if (_gameState.Player.Rectangle.Y <= InsideRectangle.Y + 60)
            CurrentSprite.Layer = _gameState.Player.Layer + 0.001f;
          else
            CurrentSprite.Layer = Building.DefaultLayer;

          break;

        case BuildingStates.Built_In:

          _particles.Clear();

          foreach (var component in Components)
            component.Update(gameTime);

          if (!_gameState.Player.IsIn(this.InsideRectangle))
          {
            // Set Position
            Position = new Vector2(Position.X - (_template.OutExtraWidth / 2), Position.Y - _template.OutExtraHeight);
            State = BuildingStates.Built_Out;
          }

          CurrentSprite.Layer = Building.DefaultLayer;

          break;

        default:
          break;
      }
    }
  }
}
