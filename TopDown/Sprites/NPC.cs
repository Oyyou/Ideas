using Engine.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Microsoft.Xna.Framework;
using TopDown.States;
using Engine;
using TopDown.Buildings;
using TopDown.Items;
using Microsoft.Xna.Framework.Content;
using TopDown.Skills;
using TopDown.Furnitures;
using VillageBackend.Models;

namespace TopDown.Sprites
{
  public class NPC : Sprite
  {
    #region Fields

    private float? _defaultLayer;

    private GameScreen _gameScreen;

    /// <summary>
    /// The path the NPC is currently walking on
    /// </summary>
    private List<Vector2> _walkingPath;

    #endregion

    #region Properties

    /// <summary>
    /// Construct a building
    /// </summary>
    public event Action<NPC, GameTime> Construct;

    /// <summary>
    /// We don't need the NPCs to collide with anything
    /// </summary>
    public override List<Rectangle> CollisionRectangles
    {
      get
      {
        return new List<Rectangle>();
      }
    }

    /// <summary>
    /// The item that the NPC is currently crafting
    /// </summary>
    public Item CraftingItem { get; set; }

    public List<Item> CraftingItems { get; set; }

    /// <summary>
    /// Demolish a building
    /// </summary>
    public event Action<NPC, GameTime> Demolish;

    /// <summary>
    /// The image shown on the "Job Menu"
    /// </summary>
    public Sprite DisplaySprite { get; private set; }

    /// <summary>
    /// Where the NPC lives
    /// </summary>
    public Building Home { get; set; }

    public bool IsBuilding { get { return Construct != null; } }

    public bool IsWorking { get { return Work != null; } }

    /// <summary>
    /// The job title (TODO: get this from the 'Workplace'
    /// </summary>
    public string Job { get; set; }

    public string Name
    {
      get
      {
        return Villager.Name;
      }
    }

    public override Vector2 Position
    {
      get
      {
        return base.Position + new Vector2(0, _animationManager.FrameHeight - 32);
      }
      set
      {
        base.Position = value - new Vector2(0, _animationManager.FrameHeight - 32);
      }
    }

    public Skills.Skills Skills { get; set; }

    public Villager Villager { get; set; }

    /// <summary>
    /// The event gained from the workplace
    /// </summary>
    public Action<NPC, GameTime> Work;

    /// <summary>
    /// Where the NPC works (TODO: add a schedule per workplace, or NPC)
    /// </summary>
    public Building Workplace { get; private set; }

    #endregion

    #region Methods

    public NPC(Dictionary<string, Animation> animations, GameScreen gameScreen) : base(animations)
    {
      _gameScreen = gameScreen;

      _walkingPath = new List<Vector2>();

      var animation = _animations.First().Value;

      DisplaySprite = new Sprite(animation.Texture)
      {
        SourceRectangle = new Rectangle(0, 0, animation.FrameWidth, animation.FrameHeight),
        Scale = 0.5f,
      };

      Villager = new Villager();

      Job = "Unemployed";
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      CraftingItems = new List<Item>();

      Skills = new Skills.Skills()
      {
        Blacksmith = new Skill()
        {
          Name = "Blacksmith",
          Level = 0,
          Experience = 0,
        }
      };
    }

    public void AssignJob()
    {
      if (this.Villager.JobId == null)
        return;      

      var jb = _gameScreen.BuildingComponents.Where(c => c.Job != null && c.Job.Id == this.Villager.JobId.Value).FirstOrDefault();

      Workplace = jb;

      if (Work == Workplace.Work)
        return;

      Job = Workplace.Name;

      Work = Workplace.Work;
      Construct = null;
      Demolish = null;

      _walkingPath.Clear();
    }

    protected override void SetAnimation()
    {
      if (Velocity.X > 0)
      {
        _animationManager.Play(_animations["WalkRight"]);
      }
      else if (Velocity.X < 0)
      {
        _animationManager.Play(_animations["WalkLeft"]);
      }
      else if (Velocity.Y > 0)
      {
        _animationManager.Play(_animations["WalkDown"]);
      }
      else if (Velocity.Y < 0)
      {
        _animationManager.Play(_animations["WalkUp"]);
      }
      else
      {
        _animationManager.Stop();
      }
    }

    public void Unemploy()
    {
      // if they're already unemployed, no point in doing the rest
      if (Work == null)
        return;

      _walkingPath.Clear();
      Villager.JobId = null;
      Work = null;
      Workplace = null;
      Job = "Unemployed";
    }

    public override void UnloadContent()
    {
      base.UnloadContent();

      _walkingPath.Clear();

      DisplaySprite.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      if (_defaultLayer == null)
        _defaultLayer = Layer;

      Velocity = Vector2.Zero;

      Layer = _defaultLayer.Value;

      if (Villager.JobId != null)
      {
        AssignJob();
      }
      else
      {
        Unemploy();
      }

      foreach (var building in _gameScreen.BuildingComponents)
      {
        if (new Rectangle((int)Position.X, (int)Position.Y, 32, 32).Intersects(building.Rectangle)) // Need to add 'IsIn'
        {
          Layer = Building.DefaultLayer + 0.0025f;
        }
      }

      if (_walkingPath.Count > 0)
      {
        if (_walkingPath[0] == Position)
          _walkingPath.RemoveAt(0);
      }

      //Villager.IsAtWork = false;

      var isWorkHours = _gameScreen.Time.Hour >= 8 && _gameScreen.Time.Hour < 17;
      var goWork = Work != null && isWorkHours;
      var build = Construct != null && isWorkHours;
      var demolish = Demolish != null && isWorkHours;

      if (goWork)
      {
        Work(this, gameTime);
      }
      else if (build)
      {
        Construct(this, gameTime);
      }
      else if (demolish)
      {
        Demolish(this, gameTime);
      }
      else // goHome
      {
        IsVisible = true;

        var bedPosition = Home.Components.Where(c => c is Bed).FirstOrDefault().Position + new Vector2(0, 32);

        WalkTo(bedPosition);

        if (this.Position == bedPosition)
        {
          _animationManager.Play(_animations["WalkDown"]);
        }
      }

      _animationManager.Update(gameTime);
    }

    public void WalkTo(Vector2 target)
    {
      if (Position == target)
        return;      

      if (_walkingPath.Count == 0)
        _walkingPath = _gameScreen.PathFinder.FindPath(Position, target);

      var targetPosition = _walkingPath.Count > 0 ? _walkingPath.FirstOrDefault() : Position;

      var speed = 1f;// * Engine.States.State.GameSpeed;

      //while ((Position.X + speed) % speed != 0)
      //  speed--;
      //while ((Position.Y + speed) % speed != 0)
      //  speed--;

      if (Position.X < targetPosition.X)
        Velocity.X = speed;
      else if (Position.X > targetPosition.X)
        Velocity.X = -speed;
      else if (Position.Y < targetPosition.Y)
        Velocity.Y = speed;
      else if (Position.Y > targetPosition.Y)
        Velocity.Y = -speed;
    }

    #endregion
  }
}
