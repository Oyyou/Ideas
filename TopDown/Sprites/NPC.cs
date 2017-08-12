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

namespace TopDown.Sprites
{
  public class NPC : Sprite
  {
    public static string[] Names = new string[]
    {
      "Bob",
      "Jimmy",
      "Fred",
      "Tim",
      "John",
    };

    #region Fields

    private GameScreen _gameScreen;

    /// <summary>
    /// The path the NPC is currently walking on
    /// </summary>
    private List<Vector2> _walkingPath;

    #endregion

    #region Properties

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
    /// The image shown on the "Job Menu"
    /// </summary>
    public Sprite DisplaySprite { get; private set; }

    /// <summary>
    /// Where the NPC lives
    /// </summary>
    public Building Home { get; set; }

    /// <summary>
    /// The job title (TODO: get this from the 'Workplace'
    /// </summary>
    public string Job { get; set; }

    public string Name { get; set; }

    public override Vector2 Position
    {
      get
      {
        return base.Position + new Vector2(0, 24);
      }
      set
      {
        base.Position = value - new Vector2(0, 24);
      }
    }

    public delegate void WorkEvent(NPC npc, GameTime gameTime);

    /// <summary>
    /// The even gained from the workplace
    /// </summary>
    public event WorkEvent Work;

    /// <summary>
    /// Where the NPC works (TODO: add a schedule per workplace, or NPC)
    /// </summary>
    public Building Workplace { get; private set; }

    #endregion

    #region Methods

    public void AssignJob()
    {
      var jb = _gameScreen.JobMenu.JobButton.JobBuilding;

      // Remove the same job from any NPC that is currently working said job
      foreach (var npc in _gameScreen.NPCComponents.Where(c => c.Workplace == jb))
      {
        npc.Unemploy();
      }

      _walkingPath.Clear();

      Workplace = jb;

      Job = Workplace.Name;

      Work += Workplace.Work;
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      CraftingItems = new List<Item>();
    }

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

      Name = Names[GameEngine.Random.Next(0, Names.Length)];

      Job = "Unemployed";
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
      _walkingPath.Clear();
      Work -= Workplace.Work;
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
      if (_walkingPath.Count > 0)
      {
        if (_walkingPath[0] == Position)
          _walkingPath.RemoveAt(0);
      }

      var goWork = Work != null && _gameScreen.Time.Hour >= 8 && _gameScreen.Time.Hour < 17;

      if (goWork)
        Work?.Invoke(this, gameTime);
      else
        WalkTo(new Vector2(Home.Rectangle.X + 32, Home.Rectangle.Y + 32));

      _animationManager.Update(gameTime);
    }

    public void WalkTo(Vector2 target)
    {
      if (Position == target)
      {
        return;
      }

      if (_walkingPath.Count == 0)
        _walkingPath = _gameScreen.PathFinder.FindPath(Position, target);

      var targetPosition = _walkingPath.Count > 0 ? _walkingPath.FirstOrDefault() : Position;

      if (Position.X < targetPosition.X)
        Velocity.X = 1;
      else if (Position.X > targetPosition.X)
        Velocity.X = -1;
      else if (Position.Y < targetPosition.Y)
        Velocity.Y = 1;
      else if (Position.Y > targetPosition.Y)
        Velocity.Y = -1;
    }

    #endregion
  }
}
