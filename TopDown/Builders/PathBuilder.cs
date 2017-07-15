using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Sprites;
using TopDown.States;
using Microsoft.Xna.Framework.Input;
using TopDown.Logic;

namespace TopDown.Builders
{
  public enum PathBuilderStates
  {
    Selecting,
    Placing,
  }

  public class PathBuilder : Component
  {
    /// <summary>
    /// The components we're placing
    /// </summary>
    private List<Sprite> _newComponents;

    public PathBuilderStates State { get; set; }

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      foreach (var component in _newComponents)
      {
        component.Draw(gameTime, spriteBatch);
      }
    }

    public override void LoadContent(ContentManager content)
    {
      Components = new List<Component>();

      _newComponents = new List<Sprite>();

      _texture = content.Load<Texture2D>("Sprites/Paths/StonePath");
    }

    public override void UnloadContent()
    {

    }

    private MouseState _currentMouse;
    private MouseState _previousMouse;

    private Vector2? _startPathPosition;

    private Texture2D _texture;

    public override void Update(GameTime gameTime)
    {
      _previousMouse = _currentMouse;
      _currentMouse = Mouse.GetState();

      var mousePosition = new Vector2(
            (float)Math.Floor((decimal)GameScreen.Mouse.Position.X / 32) * 32,
            (float)Math.Floor((decimal)GameScreen.Mouse.Position.Y / 32) * 32);

      if (_currentMouse.LeftButton == ButtonState.Released)
        _startPathPosition = null;

      if (_previousMouse.LeftButton == ButtonState.Released && _currentMouse.LeftButton == ButtonState.Pressed)
      {
        _startPathPosition = mousePosition;
      }

      if (_startPathPosition == null)
        return;

      if (Vector2.Distance(mousePosition, _startPathPosition.Value) > 100)
      {
        var x1 = mousePosition.X / 32;
        var y1 = mousePosition.Y / 32;
        var x2 = _startPathPosition.Value.X / 32;
        var y2 = _startPathPosition.Value.Y / 32;

        var minX = (int)Math.Min(x1, x2);
        var minY = (int)Math.Min(y1, y2);

        Pathfinder pf = new Pathfinder(minX, minY, 100, 100);
        var t = pf.FindPath(_startPathPosition.Value, mousePosition);

        _newComponents = new List<Sprite>();
        foreach (var sprite in t)
        {
          _newComponents.Add(new Sprite(_texture)
          {
            Position = (sprite * 32),  
          });
        }

      }
    }
  }
}
