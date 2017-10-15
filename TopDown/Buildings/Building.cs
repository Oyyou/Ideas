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
using Engine.Controls;

namespace TopDown.Buildings
{
  public enum BuildingStates
  {
    Placing,
    Placed,
    Constructing,
    Built,
    Demolishing,
  }

  public class Building : Component
  {
    public class DoorLocation
    {
      public bool IsValid;

      public Vector2 Position { get; set; }
    }

    protected class Wall
    {
      [Flags]
      public enum Directions
      {
        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
      }

      public Directions Direction { get; set; }

      public Vector2 Position { get; set; }
    }

    protected float _buttonTimer;

    protected float _constructTimer;

    protected OptionsButton _demolishButton;

    protected float _demolishTimer;

    protected OptionsButton _fireButton;

    protected GameScreen _gameScreen;

    protected OptionsButton _hireButton;

    protected float _hitTimer;

    protected OptionsButton _inspectButton;

    protected const float _maxConstructTimer = 2.5f;

    protected const float _maxDemolishTimer = 2.5f;

    protected const float _maxHitTimer = 0.3f;

    protected int _outsideExtraHeight { get { return _spriteOutsideTop.Rectangle.Height - _spriteInside.Rectangle.Height; } }

    protected List<Sprite> _particles;

    protected Texture2D _pixel;

    protected bool _showButtons;

    protected Sprite _spriteInside;

    protected Sprite _spriteOutsideTop;

    protected Sprite _spriteOutsideBottom;

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

    public List<OptionsButton> _buttons;

    public Color Color { get; set; }

    public const float DefaultLayer = 0.8f;

    /// <summary>
    /// Locations in which there has to be at least one path so we can place the building.
    /// </summary>
    public List<DoorLocation> DoorLocations;

    public bool HasStaff
    {
      get
      {
        return _gameScreen.NPCComponents.Any(c => c.Workplace == this);
      }
    }

    public string Name { get; set; }

    /// <summary>
    /// This is the fella in charge of construction
    /// </summary>
    public NPC NPCBuilder { get; private set; }

    /// <summary>
    /// NPC go boom-boom
    /// </summary>
    public NPC NPCDemolisher { get; private set; }

    public override Vector2 Position
    {
      get { return _spriteOutsideTop.Position; }
      set
      {
        _spriteInside.Position = value;
        _spriteOutsideTop.Position = new Vector2(_spriteInside.Position.X, _spriteInside.Position.Y - _outsideExtraHeight);
        _spriteOutsideBottom.Position = new Vector2(_spriteInside.Position.X, _spriteInside.Position.Y - _outsideExtraHeight);

        SetDoorLocations();
      }
    }

    /// <summary>
    /// The points inside a building used for path finding
    /// </summary>
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
            {
              foreach (var wallTest in Walls)
              {
                if (wallTest.Position == new Vector2(x / 32, y / 32))
                {
                  var directions = wallTest.Direction.ToString().Split(',').Select(c => (Wall.Directions)Enum.Parse(typeof(Wall.Directions), c.Trim()));

                  foreach (var direction in directions)
                  {
                    switch (direction)
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
              }
            }

            if (DoorLocations != null)
            {
              // The position inside the building
              var actualPosition = position * 32;

              // Top
              if (y == 0)
              {
                foreach (var doorLocation in DoorLocations)
                {
                  if (new Vector2(doorLocation.Position.X, doorLocation.Position.Y + 32) == actualPosition)
                  {
                    searchNode.Neighbors[0] = null;
                  }
                }
              }

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
              if (x == 0)
              {
                foreach (var doorLocation in DoorLocations)
                {
                  if (new Vector2(doorLocation.Position.X + 32, doorLocation.Position.Y) == actualPosition)
                  {
                    searchNode.Neighbors[2] = null;
                  }
                }
              }

              // Bottom
              if (x == Rectangle.Width - 32)
              {
                foreach (var doorLocation in DoorLocations)
                {
                  if (new Vector2(doorLocation.Position.X - 32, doorLocation.Position.Y) == actualPosition)
                  {
                    searchNode.Neighbors[3] = null;
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
          case BuildingStates.Constructing:
            CollisionRectangles = new List<Rectangle>();
            break;
        }
      }
    }

    /// <summary>
    /// The walls inside the building that we can't go through
    /// </summary>
    protected virtual List<Wall> Walls
    {
      get
      {
        return new List<Wall>();
      }
    }

    public void Build(GameTime gameTime)
    {
      foreach (var component in Components)
      {
        component.Layer = _spriteInside.Layer + 0.001f;
        component.Update(gameTime);
      }

      foreach (var component in _particles)
        component.Update(gameTime);
    }

    public Building(GameScreen gameState, Texture2D textureInside, Texture2D textureOutsideTop, Texture2D textureOutsideBottom)
    {
      _gameScreen = gameState;

      _textureInside = textureInside;

      _particles = new List<Sprite>();

      IsCollidable = true;

      _spriteOutsideTop = new Sprite(textureOutsideTop)
      {
        Layer = DefaultLayer + 0.01f,
      };

      _spriteOutsideBottom = new Sprite(textureOutsideBottom)
      {
        Layer = DefaultLayer + 0.005f,
      };

      _spriteInside = new Sprite(_textureInside)
      {
        Layer = DefaultLayer,
      };
    }

    protected virtual bool CanPlace()
    {
      bool canPlace = false;

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

      return canPlace;
    }

    public override void CheckCollision(Component component)
    {

    }

    /// <summary>
    /// Construct the building
    /// </summary>
    /// <param name="npc">Who will be constructing</param>
    /// <param name="gameTime"></param>
    public void Construct(NPC npc, GameTime gameTime)
    {
      var doorLocation = DoorLocations.Where(c => c.IsValid).FirstOrDefault().Position;

      // Walk to the door if we're not there
      if (npc.Position != doorLocation)
      {
        npc.WalkTo(doorLocation);
        return;
      }

      // Otherwise we wanna build it!
      _hitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds * Engine.States.State.GameSpeed;

      if (_hitTimer > _maxHitTimer)
      {
        var positions = new List<Vector2>();

        for (int i = 0; i < 10; i++)
        {
          GenerateParticle(_maxHitTimer);
        }

        _constructTimer += _hitTimer;

        if (_constructTimer > _maxConstructTimer)
        {
          _gameScreen.Notifications.Add(_gameScreen.Time, $"{npc.Name} finished building {this.Name}");

          State = BuildingStates.Built;
          //_gameScreen.State = States.GameStates.Playing;
          _gameScreen.UpdateMap();
          npc.Construct -= this.Construct;
        }

        _hitTimer = 0f;

        _soundEffectInstance.Play();
      }
    }

    /// <summary>
    /// Demolish the building
    /// </summary>
    /// <param name="npc">Who will be demolishing</param>
    /// <param name="gameTime"></param>
    public void Demolish(NPC npc, GameTime gameTime)
    {
      var doorLocation = DoorLocations.Where(c => c.IsValid).FirstOrDefault().Position;

      // Walk to the door if we're not there
      if (npc.Position != doorLocation)
      {
        npc.WalkTo(doorLocation);
        return;
      }

      // Otherwise we wanna demolish it!
      _hitTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

      if (_hitTimer > _maxHitTimer)
      {
        var positions = new List<Vector2>();

        for (int i = 0; i < 10; i++)
        {
          GenerateParticle(_maxHitTimer);
        }

        _demolishTimer += _hitTimer;

        if (_demolishTimer > _maxDemolishTimer)
        {
          _gameScreen.Notifications.Add(_gameScreen.Time, $"{npc.Name} finished demolishing {this.Name}");

          IsRemoved = true;

          IsRemoved = true;
          //_gameScreen.State = States.GameStates.Playing;
          _gameScreen.UpdateMap();
          npc.Demolish -= this.Demolish;
        }

        _hitTimer = 0f;

        _soundEffectInstance.Play();
      }
    }

    private void DemloshButton_Click(object sender, EventArgs e)
    {
      if (HasStaff)
      {
        GameScreen.MessageBox.Show("Can't demolish a staffed building.");
      }
      else
      {
        State = BuildingStates.Demolishing;
      }
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

        case BuildingStates.Constructing:
          _spriteInside.Draw(gameTime, spriteBatch);

          foreach (var particle in _particles)
            particle.Draw(gameTime, spriteBatch);

          //foreach (var rec in CollisionRectangles)
          //  spriteBatch.Draw(texture: _t, destinationRectangle: rec, color: Color.Red, layerDepth: 1f);

          break;

        case BuildingStates.Built:

          _spriteInside.Draw(gameTime, spriteBatch);

          _spriteOutsideTop.Draw(gameTime, spriteBatch);

          _spriteOutsideBottom.Draw(gameTime, spriteBatch);

          foreach (var component in Components)
            component.Draw(gameTime, spriteBatch);

          DrawButtons(gameTime, spriteBatch);

          //foreach (var rec in CollisionRectangles)
          //  spriteBatch.Draw(texture: _t, destinationRectangle: rec, color: Color.Red, layerDepth: 1f);
          break;

        case BuildingStates.Demolishing:

          _spriteInside.Draw(gameTime, spriteBatch);

          _spriteOutsideTop.Draw(gameTime, spriteBatch);

          _spriteOutsideBottom.Draw(gameTime, spriteBatch);


          foreach (var component in Components)
            component.Draw(gameTime, spriteBatch);

          DrawButtons(gameTime, spriteBatch);

          //foreach (var rec in CollisionRectangles)
          //  spriteBatch.Draw(texture: _t, destinationRectangle: rec, color: Color.Red, layerDepth: 1f);
          break;

        default:
          break;
      }
    }

    private void DrawButtons(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (!_showButtons)
        return;

      var buttonHeight = _buttons.Sum(c => c.Rectangle.Height + 5);

      var y = _spriteInside.Rectangle.Y;

      foreach (var button in _buttons)
      {
        var x = _spriteInside.Rectangle.X + (_spriteInside.Rectangle.Width / 2) - (button.Rectangle.Width / 2);

        y -= button.Rectangle.Height + 5;

        button.Position = new Vector2(x, y);
        button.Layer = 0.83f;


        button.Draw(gameTime, spriteBatch);
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

      LoadButtons(content);

      _buttons = new List<OptionsButton>();

      foreach (var button in _buttons)
        button.LoadContent(content);
    }

    private void LoadButtons(ContentManager content)
    {
      var buttonTexture = content.Load<Texture2D>("Controls/Button");
      var buttonFont = content.Load<SpriteFont>("Fonts/Font");

      _demolishButton = new OptionsButton(buttonTexture, buttonFont);
      _demolishButton.Text = "Demolish";
      _demolishButton.Click += DemloshButton_Click;

      _inspectButton = new OptionsButton(content.Load<Texture2D>("Controls/Button"), content.Load<SpriteFont>("Fonts/Font"));
      _inspectButton.Text = "Inspect";
      _inspectButton.Click += InspectButton_Click;

      _fireButton = new OptionsButton(buttonTexture, buttonFont);
      _fireButton.Text = "Fire";
      _fireButton.Click += FireButton_Click;

      _hireButton = new OptionsButton(buttonTexture, buttonFont);
      _hireButton.Text = "Hire";
      _hireButton.Click += HireButton_Click;
    }

    private void InspectButton_Click(object sender, EventArgs e)
    {
      _gameScreen.Inspect(this);
    }

    private void HireButton_Click(object sender, EventArgs e)
    {
      _gameScreen.State = GameStates.JobMenu;

      _gameScreen.JobMenu.SetButtons();
      _gameScreen.JobMenu.JobButton = _gameScreen.JobMenu.Buttons.Where(c => c.JobBuilding == this).FirstOrDefault();
      _gameScreen.JobMenu.JobButton.IsSelected = true;
    }

    private void FireButton_Click(object sender, EventArgs e)
    {
      var npc = _gameScreen.NPCComponents.Where(c => c.Workplace == this).FirstOrDefault();

      if (npc == null)
        return;

      npc.Unemploy();
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

    public override void UnloadContent()
    {
      _spriteInside?.UnloadContent();
      _spriteOutsideTop.UnloadContent();
      _spriteOutsideBottom.UnloadContent();

      _soundEffect?.Dispose();

      foreach (var component in Components)
        component.UnloadContent();

      Walls?.Clear();
    }

    public override void Update(GameTime gameTime)
    {
      _updated = true;

      switch (State)
      {
        case BuildingStates.Placing:

          PlacingUpdate();

          break;
        case BuildingStates.Placed:

          foreach (var furniture in Components.ToArray())
          {
            furniture.Layer = _spriteInside.Layer + 0.001f;
            furniture.Update(gameTime);
          }


          break;
        case BuildingStates.Constructing:

          if (NPCBuilder != null)
          {
            if (NPCBuilder.IsWorking)
              NPCBuilder = null;
          }

          if (NPCBuilder == null)
          {
            var npc = _gameScreen.NPCComponents
              .Where(c => !c.IsBuilding && !c.IsWorking)
              .OrderBy(c =>
                Vector2.Distance(
                  c.Home.DoorLocations.FirstOrDefault().Position,
                  this.DoorLocations.Where(v => v.IsValid).FirstOrDefault().Position))
              .FirstOrDefault();

            if (npc == null)
              return;

            NPCBuilder = npc;

            NPCBuilder.Construct += Construct;
          }

          Build(gameTime);
          break;

        case BuildingStates.Built:

          _particles.Clear();

          foreach (var component in Components)
          {
            component.Layer = DefaultLayer + 0.0020f;
            component.Update(gameTime);
          }

          if (GameScreen.Mouse.RectangleWithCamera.Intersects(_spriteInside.Rectangle) && GameScreen.Mouse.LeftClicked)
          {
            _showButtons = !_showButtons;
          }

          if (_showButtons)
          {
            foreach (var button in _buttons)
              button.Update(gameTime);
          }

          break;
        case BuildingStates.Demolishing:

          if (NPCDemolisher != null)
          {
            if (NPCDemolisher.IsWorking)
              NPCDemolisher = null;
          }

          if (NPCDemolisher == null)
          {
            var npc = _gameScreen.NPCComponents
              .Where(c => !c.IsBuilding && !c.IsWorking)
              .OrderBy(c =>
                Vector2.Distance(
                  c.Home.DoorLocations.FirstOrDefault().Position,
                  this.DoorLocations.Where(v => v.IsValid).FirstOrDefault().Position))
              .FirstOrDefault();

            NPCDemolisher = npc;

            NPCDemolisher.Demolish += Demolish;
          }

          Build(gameTime);
          break;

        default:
          break;
      }

      if (GameScreen.Mouse.LeftClicked)
      {
        if (!GameScreen.Mouse.RectangleWithCamera.Intersects(_spriteInside.Rectangle))
          _showButtons = false;
      }
    }

    private void PlacingUpdate()
    {
      _spriteInside.Position = new Vector2(
        (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.X / 32) * 32,
        (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.Y / 32) * 32);

      SetDoorLocations();

      bool canPlace = CanPlace();

      _spriteInside.Color = Color.White;

      if (!canPlace)
        _spriteInside.Color = Color.Red;

      if (GameScreen.Mouse.LeftClicked && canPlace)
      {
        _spriteOutsideTop.Position = new Vector2(_spriteInside.Position.X, _spriteInside.Position.Y - _outsideExtraHeight);
        _spriteOutsideBottom.Position = new Vector2(_spriteInside.Position.X, _spriteInside.Position.Y - _outsideExtraHeight);
        State = BuildingStates.Placed;
        _gameScreen.State = States.GameStates.ItemMenu;
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
