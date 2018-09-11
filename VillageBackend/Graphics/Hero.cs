using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VillageBackend.Models;

namespace VillageBackend.Graphics
{
  public class Hero : Person
  {
    public Villager Villager { get; set; }

    public Hero(Dictionary<string, Animation> animations) : base(animations)
    {

    }

    protected override void Walk()
    {
      if (WalkingPath == null)
        return;

      if (WalkingPath.Count == 0)
        return;

      if (_originalWalkingPath == null)
        _originalWalkingPath = new List<Vector2>(WalkingPath);

      if (WalkingPath.FirstOrDefault() == Position)
        WalkingPath.RemoveAt(0);

      if (WalkingPath.Count == 0)
      {
        var distance = _originalWalkingPath.Count;
        var usedTurns = (int)Math.Ceiling((float)distance / (float)Villager.Speed);

        Console.WriteLine(usedTurns);

        Villager.Turns -= usedTurns;
        HasFinishedWalking = true;
        _originalWalkingPath = null;
      }

      var targetPosition = WalkingPath.Count > 0 ? WalkingPath.FirstOrDefault() : Position;

      var speed = 1f;

      _velocity = new Vector2();

      if (Position.X < targetPosition.X)
      {
        _velocity.X = speed;
      }
      else if (Position.X > targetPosition.X)
      {
        _velocity.X = -speed;
      }
      else if (Position.Y < targetPosition.Y)
      {
        _velocity.Y = speed;
      }
      else if (Position.Y > targetPosition.Y)
      {
        _velocity.Y = -speed;
      }
    }
  }
}
