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

    private GameScreen _gameScreen;

    private List<Vector2> _walkingPath;

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

    public Sprite DisplaySprite { get; private set; }

    public string Job { get; set; }

    public Building JobBuilding { get; private set; }

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

    public event EventHandler Work;

    public void AssignJob()
    {
      var jb = _gameScreen.JobMenu.JobButton.JobBuilding;

      // Remove the same job from any NPC that is currently working said job
      foreach (var npc in _gameScreen.NPCComponents.Where(c => c.JobBuilding == jb))
      {
        npc.Unemploy();
      }

      JobBuilding = jb;

      Job = JobBuilding.Name;

      Work += JobBuilding.Work;
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
      JobBuilding = null;
      Job = "Unemployed";
    }

    public override void Update(GameTime gameTime)
    {
      if (_walkingPath.Count > 0)
      {
        if (_walkingPath[0] == Position)
          _walkingPath.RemoveAt(0);
      }

      Work?.Invoke(this, new EventArgs());

      //if (_walkingPath.Count == 0)
      //{
      //  Random random = new Random();

      //  var value = random.Next(0, _gameScreen.PathComponents.Count);

      //  _walkingPath = _gameScreen.PathFinder.FindPath(Position, _gameScreen.PathComponents[value].Position);
      //}

      //var targetPosition = _walkingPath.Count > 0 ? _walkingPath.FirstOrDefault() : Position;

      //if (Position.X < targetPosition.X)
      //  Velocity.X = 1;
      //else if (Position.X > targetPosition.X)
      //  Velocity.X = -1;
      //else if (Position.Y < targetPosition.Y)
      //  Velocity.Y = 1;
      //else if (Position.Y > targetPosition.Y)
      //  Velocity.Y = -1;

      //_animationManager.Update(gameTime);
    }
  }
}
