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
    private BuildingStates _buildingState;

    private Texture2D _buildingTexture;

    private float _buildTimer;

    private Texture2D _builtTexture;

    private GameState _gameState;

    private float _hitTimer;

    private const float _maxBuildTimer = 2.5f;

    private const float _maxHitTimer = 0.3f;

    private Texture2D _placedTexture;

    private SoundEffect _soundEffect;

    /// <summary>
    /// An instance of the _soundEffect so that when we call 'Play', it will only play if finished.
    /// </summary>
    private SoundEffectInstance _soundEffectInstance;

    private Texture2D _woodChipTexture;

    public BuildingStates BuildingState
    {
      get { return _buildingState; }
      set
      {
        if (_buildingState == value)
          return;

        _buildingState = value;

        switch (_buildingState)
        {
          case BuildingStates.Placing:
            CollisionRectangles = new List<Rectangle>();
            break;
          case BuildingStates.Placed:
          case BuildingStates.Building:
            CollisionRectangles = new List<Rectangle>()
            {
              new Rectangle((int)Position.X, (int)Position.Y, _placedTexture.Width, _placedTexture.Height),
            };
            break;
          case BuildingStates.Built:
            CollisionRectangles = new List<Rectangle>()
            {
              new Rectangle((int)Position.X, (int)Position.Y, _placedTexture.Width, 1),
              new Rectangle((int)Position.X, (int)Position.Y, 1, _placedTexture.Height),
              new Rectangle((int)Position.X + (_placedTexture.Width - 1), (int)Position.Y, 1, _placedTexture.Height),
              new Rectangle((int)Position.X, (int)Position.Y + _placedTexture.Height, 21, 1),
              new Rectangle((int)Position.X + 61, (int)Position.Y + _placedTexture.Height, 131, 1),
            };
            break;
        }
      }
    }

    public Color Color { get; set; }

    public Vector2 Position;

    public Rectangle Rectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, _placedTexture.Width, _placedTexture.Height);
      }
    }

    public void Build(GameTime gameTime)
    {
      if (GameState.Mouse.MouseState != Controls.MouseStates.Building)
        return;

      if (GameState.Mouse.RectangleWithCamera.Intersects(this.Rectangle))
      {
        if (Vector2.Distance(this.Position + new Vector2(Rectangle.Width / 2, Rectangle.Height / 2), _gameState.Player.Position) < 150)
        {
          Color = Color.Yellow;

          if (GameState.Mouse.LeftDown)
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
                BuildingState = BuildingStates.Built;

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

    public Building(GameState gameState)
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
          spriteBatch.Draw(_builtTexture, Position, Color.White);
          break;
        case BuildingStates.Placed:
          spriteBatch.Draw(_placedTexture, Position, Color.White);
          break;
        case BuildingStates.Building:
          spriteBatch.Draw(_buildingTexture, Position, Color.White);
          break;
        case BuildingStates.Built:
          spriteBatch.Draw(_builtTexture, Position, Color.White);
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
      _builtTexture = content.Load<Texture2D>("Buildings/SmallHouse");

      _placedTexture = content.Load<Texture2D>("Buildings/SmallHouse_Placed");

      _soundEffect = content.Load<SoundEffect>("Sounds/Sawing");
      _soundEffectInstance = _soundEffect.CreateInstance();

      _woodChipTexture = content.Load<Texture2D>("FX/WoodChip");

      Components = new List<Component>();
    }

    public override void UnloadContent()
    {
      _builtTexture?.Dispose();
      _buildingTexture?.Dispose();
      _placedTexture?.Dispose();
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
            IsRemoved = true;

          Position = new Vector2(
            (float)Math.Floor((decimal)GameState.Mouse.PositionWithCamera.X / 32) * 32,
            (float)Math.Floor((decimal)GameState.Mouse.PositionWithCamera.Y / 32) * 32);

          if (GameState.Mouse.LeftClicked)
            BuildingState = BuildingStates.Placed;

          break;
        case BuildingStates.Placed:
          Build(gameTime);
          break;
        case BuildingStates.Building:
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
