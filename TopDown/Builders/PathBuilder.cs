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
    Finished,
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
      foreach (var path in _pathPositions)
      {
        spriteBatch.Draw(_texture, path, Color.White);
      }
    }

    public override void LoadContent(ContentManager content)
    {
      Components = new List<Component>();

      _newComponents = new List<Sprite>();

      _texture = content.Load<Texture2D>("Sprites/Paths/StonePath");
    }

    public PathBuilder(GameScreen gameScreen)
    {
      _gameScreen = gameScreen;
    }

    public override void UnloadContent()
    {

    }
    
    private Vector2 _currentMousePosition;
    private Vector2 _previousMousePosition;

    private Texture2D _texture;

    private List<Vector2> _pathPositions = new List<Vector2>();
    private GameScreen _gameScreen;

    public override void Update(GameTime gameTime)
    {
      switch (State)
      {
        case PathBuilderStates.Selecting:
          break;
        case PathBuilderStates.Placing:

          PlacingPath();

          break;
        default:
          break;
      }
    }

    private void PlacingPath()
    {
      _previousMousePosition = _currentMousePosition;

      _currentMousePosition = new Vector2(
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.X / 32) * 32,
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.Y / 32) * 32);

      if (GameScreen.Mouse.LeftDown)
      {
        if (!_pathPositions.Contains(_currentMousePosition))
          _pathPositions.Add(_currentMousePosition);
      }
      else if (GameScreen.Mouse.RightDown)
      {
        _pathPositions.Remove(_currentMousePosition);
      }
    }
  }
}
