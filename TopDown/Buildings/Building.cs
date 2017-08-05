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
using static TopDown.Logic.Pathfinder;
using TopDown.Sprites;

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
    public class DoorLocation
    {
      public bool IsValid;

      public Vector2 Position { get; set; }
    }

    protected float _buildTimer;

    protected Sprite _spriteOutside;

    protected GameScreen _gameScreen;

    protected float _hitTimer;

    protected const float _maxBuildTimer = 2.5f;

    protected const float _maxHitTimer = 0.3f;

    protected virtual int _outsideExtraHeight { get; }

    protected virtual int _outsideExtraWidth { get; }

    protected List<Sprite> _particles;

    protected Texture2D _pixel;

    protected Sprite _spriteInside;

    protected SoundEffect _soundEffect;

    /// <summary>
    /// An instance of the _soundEffect so that when we call 'Play', it will only play if finished.
    /// </summary>
    protected SoundEffectInstance _soundEffectInstance;

    protected BuildingStates _state;

    protected Texture2D _textureInside;

    protected Texture2D _textureOutside;

    private bool _updated = false;

    protected Texture2D _woodChipTexture;

    public Color Color { get; set; }

    public const float DefaultLayer = 0.8f;

    /// <summary>
    /// Locations in which there has to be at least one path so we can place the building.
    /// </summary>
    public List<DoorLocation> DoorLocations;

    public override Vector2 Position
    {
      get { return _spriteOutside.Position; }
      set
      {
        _spriteInside.Position = value;
        _spriteOutside.Position = new Vector2(_spriteInside.Position.X - (_outsideExtraWidth / 2), _spriteInside.Position.Y - _outsideExtraHeight);

        SetDoorLocations();
        SetWalls();
      }
    }

    public string Name { get; set; }

    protected class Wall
    {
      public enum Directions
      {
        Up,
        Down,
        Left,
        Right,
      }

      public Directions Direction { get; set; }

      public Vector2 Position { get; set; }
    }

    public virtual List<SearchNode> PathPositions
    {
      get
      {
        var searchNodes = new List<SearchNode>();

        var index = 0;

        for (int y = 0; y < Rectangle.Height; y += 32)
        {
          for (int x = 0; x < Rectangle.Width; x += 32)
          {
            index++;

            var position = new Vector2((Rectangle.X + x) / 32, (Rectangle.Y + y) / 32);

            var searchNode = new SearchNode()
            {
              Position = position,
              Walkable = true,
              Neighbors = new SearchNode[]
              {
                y == 0 ? new SearchNode() : null, // Top
                y == Rectangle.Height - 32 ? new SearchNode() : null, // Down
                x == 0 ? new SearchNode() : null, // Left
                x == Rectangle.Width - 32 ? new SearchNode() : null, // Right
              },
            };

            if (Walls != null)
              foreach (var wallTest in Walls)
              {
                if (wallTest.Position == new Vector2(x / 32, y / 32))
                {
                  switch (wallTest.Direction)
                  {
                    case Wall.Directions.Up:
                      searchNode.Neighbors[0] = new SearchNode();
                      break;
                    case Wall.Directions.Down:
                      searchNode.Neighbors[1] = new SearchNode();
                      break;
                    case Wall.Directions.Left:
                      searchNode.Neighbors[2] = new SearchNode();
                      break;
                    case Wall.Directions.Right:
                      searchNode.Neighbors[3] = new SearchNode();
                      break;
                  }
                }
              }

            if (DoorLocations != null)
            {
              var actualPosition = position * 32;

              // Bottom
              if (y == Rectangle.Height - 32)
              {
                foreach (var doorLocation in DoorLocations)
                {
                  if (new Vector2(doorLocation.Position.X, doorLocation.Position.Y - 32) == actualPosition)
                  {
                    searchNode.Neighbors[1] = null;
                  }
                }
              }

              // Left
              if (x == Rectangle.X - 32)
              {
                foreach (var doorLocation in DoorLocations)
                {
                  if (new Vector2(doorLocation.Position.X - 32, doorLocation.Position.Y) == actualPosition)
                  {
                    searchNode.Neighbors[2] = null;
                  }
                }
              }
            }

            searchNodes.Add(searchNode);
          }
        }

        return searchNodes;
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

    protected List<Wall> Walls;

    public virtual BuildingStates State
    {
      get { return _state; }
      set
      {
        if (_state == value)
          return;

        _state = value;

        switch (_state)
        {
          case BuildingStates.Placing:
          case BuildingStates.Placed:
          case BuildingStates.Building:
            CollisionRectangles = new List<Rectangle>();
            break;
        }
      }
    }

    public void Build(GameTime gameTime)
    {
      if (GameScreen.Mouse.MouseState != Controls.MouseStates.Building)
        return;

      if (GameScreen.Mouse.RectangleWithCamera.Intersects(_spriteInside.Rectangle))
      {
        if (Vector2.Distance(_spriteInside.Position + new Vector2(_spriteInside.Rectangle.Width / 2, _spriteInside.Rectangle.Height / 2), _gameScreen.Player.Position) < 150)
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
                State = BuildingStates.Built_Out;
                _gameScreen.State = States.GameStates.Playing;
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
      _gameScreen = gameState;

      _textureInside = textureInside;

      _textureOutside = textureOutside;

      _particles = new List<Sprite>();

      IsCollidable = true;

      _spriteOutside = new Sprite(_textureOutside)
      {
        Layer = DefaultLayer,
      };

      _spriteInside = new Sprite(_textureInside)
      {
        Layer = DefaultLayer,
      };
    }

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (!_updated)
        return;

      switch (State)
      {
        case BuildingStates.Placing:

          _spriteInside.Draw(gameTime, spriteBatch);

          foreach (var doorLocation in DoorLocations)
          {
            var color = Color.LightGreen;
            if (!doorLocation.IsValid)
              color = Color.Red;

            spriteBatch.Draw(texture: _pixel,
              destinationRectangle: new Rectangle(doorLocation.Position.ToPoint(), new Point(32, 32)),
              color: color,
              layerDepth: _spriteInside.Layer + 0.01f);
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
      _pixel = content.Load<Texture2D>("Pixel");

      _soundEffect = content.Load<SoundEffect>("Sounds/Sawing");
      _soundEffectInstance = _soundEffect.CreateInstance();

      _woodChipTexture = content.Load<Texture2D>("FX/WoodChip");

      Components = new List<Component>();
    }

    protected virtual void SetDoorLocations()
    {
      DoorLocations = new List<DoorLocation>()
      {
        new DoorLocation()
        {
          Position = new Vector2(_spriteInside.Rectangle.X + 32, _spriteInside.Rectangle.Bottom),
          IsValid = false,
        },
      };
    }

    protected virtual void SetWalls()
    {
      Walls = new List<Wall>();
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
      _updated = true;

      switch (State)
      {
        case BuildingStates.Placing:

          _spriteInside.Position = new Vector2(
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.X / 32) * 32,
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.Y / 32) * 32);

          SetDoorLocations();

          bool canPlace = false;

          _spriteInside.Color = Color.White;

          foreach (var component in _gameScreen.PathComponents)
          {
            if (component.Rectangle.Intersects(_spriteInside.Rectangle))
            {
              canPlace = false;
              GameScreen.MessageBox.Show("Trying to build over path", false);
              break;
            }

            DoorLocations.ForEach(c =>
            {
              if (c.Position == component.Position)
              {
                canPlace = true;
                c.IsValid = true;
              }
            });
          }

          if (!canPlace && !GameScreen.MessageBox.IsVisible)
            GameScreen.MessageBox.Show("Door needs to connect to path", false);

          if (canPlace)
          {
            foreach (var component in _gameScreen.CollidableComponents)
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
            State = BuildingStates.Placed;
            _gameScreen.State = States.GameStates.ItemMenu;
          }

          break;
        case BuildingStates.Placed:

          foreach (var furniture in Components.ToArray())
          {
            furniture.Layer = _spriteInside.Layer + 0.001f;
            furniture.Update(gameTime);
          }


          break;
        case BuildingStates.Building:
          Build(gameTime);
          break;

        case BuildingStates.Built_Out:

          _particles.Clear();

          _spriteInside.Layer = DefaultLayer;
          _spriteOutside.Layer = DefaultLayer + 0.002f;

          foreach (var component in Components)
          {
            component.Layer = _spriteOutside.Layer - 0.001f;
            component.Update(gameTime);
          }

          if (_gameScreen.Player.IsIn(_spriteInside.Rectangle) ||
            (_gameScreen.SelectedPathBuilder != null && _gameScreen.SelectedPathBuilder.State == Builders.PathBuilderStates.Placing))
          {
            _state = BuildingStates.Built_In;
          }

          break;

        case BuildingStates.Built_In:

          _particles.Clear();

          _spriteInside.Layer = Building.DefaultLayer;

          foreach (var furniture in Components)
          {
            furniture.Layer = _spriteInside.Layer + 0.001f;
            furniture.Update(gameTime);
          }

          if (!_gameScreen.Player.IsIn(_spriteInside.Rectangle) &&
            !(_gameScreen.SelectedPathBuilder != null && _gameScreen.SelectedPathBuilder.State == Builders.PathBuilderStates.Placing))
          {
            _state = BuildingStates.Built_Out;
          }

          break;

        default:
          break;
      }
    }

    /// <summary>
    /// The action the NPC will perform when hired
    /// </summary>
    /// <param name="npc">The NPC assigned to the building</param>
    public virtual void Work(NPC npc, GameTime gameTime)
    {

    }
  }
}
