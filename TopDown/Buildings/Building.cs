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
    Built,
  }

  public class Building : Component
  {
    private Sprite _buildingSprite;

    private float _buildTimer;

    private Sprite _builtSprite;

    private GameScreen _gameState;

    private float _hitTimer;

    private const float _maxBuildTimer = 2.5f;

    private const float _maxHitTimer = 0.3f;

    private Sprite _placedSprite;

    private SoundEffect _soundEffect;

    /// <summary>
    /// An instance of the _soundEffect so that when we call 'Play', it will only play if finished.
    /// </summary>
    private SoundEffectInstance _soundEffectInstance;

    private Texture2D _woodChipTexture;

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
          case BuildingStates.Built:
            CollisionRectangles = new List<Rectangle>()
            {
              new Rectangle((int)Position.X, (int)Position.Y, width, 1),
              new Rectangle((int)Position.X, (int)Position.Y, 1, height),
              new Rectangle((int)Position.X + (width - 1), (int)Position.Y, 1, height),
              new Rectangle((int)Position.X, (int)Position.Y + height, 21, 1),
              new Rectangle((int)Position.X + 61, (int)Position.Y + height, 131, 1),
            };
            break;
        }
      }
    }

    public Color Color { get; set; }

    public Sprite CurrentSprite
    {
      get
      {
        switch (BuildingState)
        {
          case BuildingStates.Placing:
            return _builtSprite;

          case BuildingStates.Placed:
            return _placedSprite;

          case BuildingStates.Building:
            return _buildingSprite;

          case BuildingStates.Built:
            return _builtSprite;

          default:
            throw new Exception("Unknown state: " + BuildingState);
        }
      }
    }

    public float Layer
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
        //_buildingSprite.Position = value;
        _builtSprite.Position = value;
        _placedSprite.Position = value;
      }
    }

    public Rectangle Rectangle
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
                BuildingState = BuildingStates.Built;
                _gameState.State = States.States.Playing;
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
    }

    public Building(GameScreen gameState)
    {
      _gameState = gameState;
    }

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      switch (BuildingState)
      {
        case BuildingStates.Placing:
          _builtSprite.Draw(gameTime, spriteBatch);
          break;

        case BuildingStates.Placed:
          _placedSprite.Draw(gameTime, spriteBatch);
          break;

        case BuildingStates.Building:
          _buildingSprite.Draw(gameTime, spriteBatch);
          break;

        case BuildingStates.Built:
          _builtSprite.Draw(gameTime, spriteBatch);
          break;

        default:
          break;
      }

      foreach (var component in Components)
        component.Draw(gameTime, spriteBatch);
    }

    private void GenerateParticle(float lifeTimer)
    {
      var position = new Vector2(
        GameEngine.Random.Next((int)Position.X, (int)Position.X + Rectangle.Width),
        GameEngine.Random.Next((int)Position.Y, (int)Position.Y + Rectangle.Height)
      );

      Components.Add(
        new Particle(_woodChipTexture)
        {
          Position = position,
          Layer = 0.8f + 0.01f,
          LifeTimer = lifeTimer,
          Rotation = MathHelper.ToRadians(GameEngine.Random.Next(0, 360)),
        }
      );
    }

    public override void LoadContent(ContentManager content)
    {
      _builtSprite = new Sprite(content.Load<Texture2D>("Buildings/SmallHouse"));

      _placedSprite = new Sprite(content.Load<Texture2D>("Buildings/SmallHouse_Placed"));

      _soundEffect = content.Load<SoundEffect>("Sounds/Sawing");
      _soundEffectInstance = _soundEffect.CreateInstance();

      _woodChipTexture = content.Load<Texture2D>("FX/WoodChip");

      Components = new List<Component>();
    }

    public override void UnloadContent()
    {
      _builtSprite?.UnloadContent();
      _placedSprite?.UnloadContent();
      _buildingSprite?.UnloadContent();
      _soundEffect?.Dispose();

      foreach (var component in Components)
        component.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      switch (BuildingState)
      {
        case BuildingStates.Placing:

          if (Keyboard.GetState().IsKeyDown(Keys.B))
          {
            _gameState.State = States.States.BuildMenu;
            IsRemoved = true;
          }

          if (Keyboard.GetState().IsKeyDown(Keys.Escape))
          {
            _gameState.State = States.States.Playing;
            IsRemoved = true;
          }

          Position = new Vector2(
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.X / 32) * 32,
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.Y / 32) * 32);

          if (GameScreen.Mouse.LeftClicked)
          {
            BuildingState = BuildingStates.Placed;
            _gameState.State = States.States.ItemMenu;
          }

          break;
        case BuildingStates.Placed:

          break;
        case BuildingStates.Building:

          Build(gameTime);
          break;
        case BuildingStates.Built:
          break;
        default:
          break;
      }

      foreach (var component in Components)
        component.Update(gameTime);
    }
  }
}
