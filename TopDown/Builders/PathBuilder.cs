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
using TopDown.Buildings;
using TopDown.Sprites;

namespace TopDown.Builders
{
  public enum PathBuilderStates
  {
    Selecting,
    Placing,
  }

  public class PathBuilder : Component
  {
    public List<Path> Paths { get; set; }

    public Path Path { get; set; }

    public PathBuilderStates State { get; set; }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      switch (State)
      {
        case PathBuilderStates.Selecting:
        case PathBuilderStates.Placing:

          Path?.Draw(gameTime, spriteBatch);

          foreach (var component in Paths)
            component.Draw(gameTime, spriteBatch);

          break;
      }
    }

    public override void LoadContent(ContentManager content)
    {
      Components = new List<Component>();

      Paths = new List<Path>();

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
        case PathBuilderStates.Placing:

          PlacingPath();

          break;
      }
    }

    private void PlacingPath()
    {
      _previousMousePosition = _currentMousePosition;

      _currentMousePosition = new Vector2(
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.X / 32) * 32,
            (float)Math.Floor((decimal)GameScreen.Mouse.PositionWithCamera.Y / 32) * 32);

      Path.Position = _currentMousePosition;

      if (GameScreen.Mouse.LeftDown)
      {
        if (!_pathPositions.Contains(_currentMousePosition))
        {
          _pathPositions.Add(_currentMousePosition);

          var sprite = Path.Clone() as Path;
          
          Paths.Add(sprite);
        }
      }
      else if (GameScreen.Mouse.RightDown)
      {
        _pathPositions.Remove(_currentMousePosition);

        Paths.Remove(Paths.Where(c => c.Position == _currentMousePosition).FirstOrDefault());
      }
    }
  }
}
