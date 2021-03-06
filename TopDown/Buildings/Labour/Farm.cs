﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using TopDown.States;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using TopDown.Sprites;
using VillageBackend.Models;
using Engine.Models;

namespace TopDown.Buildings.Labour
{
  public class Farm : Building
  {
    private Chicken _chickenPrefab;

    protected override List<Wall> Walls
    {
      get
      {
        return new List<Wall>()
        {
          new Wall()
          {
            Direction = Wall.Directions.Left | Wall.Directions.Up,
            Position = new Vector2(0, 0),
          },
          new Wall()
          {
            Direction = Wall.Directions.Up | Wall.Directions.Right,
            Position = new Vector2(1, 0),
          },
          new Wall()
          {
            Direction = Wall.Directions.Left,
            Position = new Vector2(0, 1),
          },
          new Wall()
          {
            Direction = Wall.Directions.Right | Wall.Directions.Down,
            Position = new Vector2(1, 1),
          },
        };
      }
    }

    public Farm(GameScreen gameState, Texture2D textureInside, Texture2D textureOutsideTop, Texture2D textureOutsideBottom) : base(gameState, textureInside, textureOutsideTop, textureOutsideBottom)
    {
      Name = "Farm";

      Job = new Job()
      {
        Name = Name,
      };
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      _buttons = new List<OptionsButton>()
      {
        _demolishButton,
        _fireButton,
        _hireButton,
      };

      foreach (var button in _buttons)
        button.LoadContent(content);

      var chickenAnimations = new Dictionary<string, Animation>()
      {
        { "Peck", new Animation(content.Load<Texture2D>("Sprites/Animals/Chicken_Peck"), 3) },
        { "Walk", new Animation(content.Load<Texture2D>("Sprites/Animals/Chicken_Walk"), 4) },
      };

      _chickenPrefab = new Chicken(chickenAnimations)
      {
        Layer = this.Layer + 0.01f,
      };
    }

    protected override void SetDoorLocations()
    {
      DoorLocations = new List<DoorLocation>();

      // These are the points next to the walls
      var invalidPositions = new List<Vector2>()
      {
        new Vector2(0, -32),
        new Vector2(32, -32),
        new Vector2(-32, 0),
        new Vector2(-32, 32),
      };

      for (int y = -32; y <= Rectangle.Height; y += 32)
      {
        for (int x = -32; x <= Rectangle.Width; x += 32)
        {
          if (invalidPositions.Any(c => c == new Vector2(x, y)))
            continue;

          var position = new Vector2((Rectangle.X + x), (Rectangle.Y + y));

          if (x == -32 || y == -32 ||
            x == Rectangle.Width || y == Rectangle.Height)
          {
            if ((x == -32 && y == -32) ||
              (x == -32 && y == Rectangle.Height) ||
              (x == Rectangle.Width && y == -32) ||
              (x == Rectangle.Width && y == Rectangle.Height))
            {
              continue;
            }

            DoorLocations.Add(new DoorLocation()
            {
              Position = position,
              IsValid = false,
            });
          }
        }
      }
    }

    public override void Work(NPC npc, GameTime gameTime)
    {
      var workPosition = new Vector2(Rectangle.X + 32, Rectangle.Y + 32);

      if (npc.Position != workPosition)
        npc.WalkTo(workPosition);
    }

    public override void OnBuilt()
    {
      var chicken1 = _chickenPrefab.Clone() as Chicken;
      var chicken2 = _chickenPrefab.Clone() as Chicken;
      var chicken3 = _chickenPrefab.Clone() as Chicken;

      chicken1.Position = new Vector2(Position.X + 32, Position.Y + 128);
      chicken2.Position = new Vector2(Position.X + 96, Position.Y + 36);
      chicken3.Position = new Vector2(Position.X + 224, Position.Y + 160);

      Components.Add(chicken1);
      Components.Add(chicken2);
      Components.Add(chicken3);
    }
  }
}
