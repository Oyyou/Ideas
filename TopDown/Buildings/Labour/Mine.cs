﻿using Engine.Sprites;
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
using Microsoft.Xna.Framework.Graphics;
using TopDown.Sprites;

namespace TopDown.Buildings.Labour
{
  public class Mine : Building
  {
    protected override int _outsideExtraHeight => 32;

    protected override int _outsideExtraWidth => 0;

    public override BuildingStates State
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
            CollisionRectangles = new List<Rectangle>();
            break;
          case BuildingStates.Placed:
          case BuildingStates.Building:
            CollisionRectangles = new List<Rectangle>()
            {
              _spriteInside.Rectangle,
            };
            break;
          case BuildingStates.Built_Out:

            var height = _spriteInside.Rectangle.Height;
            var width = _spriteInside.Rectangle.Width;

            var x = (int)_spriteInside.Position.X;
            var y = (int)_spriteInside.Position.Y;

            var collisionRectangles = new List<Rectangle>()
            {
              new Rectangle(x + 32, y + 32, width - 64, 1), // Top
              new Rectangle(x + 32, y + 32, 1, height - 64), // Left
              new Rectangle(x + (width - 1) - 32, y + 32, 1, height - 64), // Right
              new Rectangle(x + 32, y + (height - 1) - 32, 32, 1), // bottom left
              new Rectangle(x + 96, y + (height - 1) - 32, 32, 1), // bottom right
            };

            if (Components != null)
              collisionRectangles.AddRange(Components.SelectMany(c => c.CollisionRectangles).ToList());

            CollisionRectangles = collisionRectangles;

            break;
        }
      }
    }

    public Mine(GameScreen gameState, Texture2D textureInside, Texture2D textureOutside) : base(gameState, textureInside, textureOutside)
    {
      Name = "Mine";
    }

    protected override void SetDoorLocations()
    {
      DoorLocations = new List<DoorLocation>();

      for (int y = -32; y <= Rectangle.Height; y += 32)
      {
        for (int x = -32; x <= Rectangle.Width; x += 32)
        {
          var position = new Vector2((Rectangle.X + x), (Rectangle.Y + y));

          if (x == -32 || y == -32 ||
            x == Rectangle.Width || y == Rectangle.Height)
          {
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
      var position = new Vector2(Rectangle.X + 64, Rectangle.Y + 64);

      npc.WalkTo(position);
    }
  }
}