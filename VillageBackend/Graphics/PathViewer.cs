using Engine.Input;
using Engine.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBackend.World;

namespace VillageBackend.Graphics
{
  public class PathViewer
  {
    private List<Sprite> _possibleTiles;

    private List<Sprite> _currentPath;

    private Hero _selectedHero;

    private Sprite _selectedTile;

    private Map _map;

    private readonly Texture2D _texture;

    private Vector2 _currentMousePosition;
    private Vector2 _previousMousePosition;

    private Hero _previouslyUpdatedHero;
    private Vector2 _previouslyUpdatedPoint;

    public PathViewer(GraphicsDevice graphicsDevice, Map map)
    {
      _map = map;

      _texture = new Texture2D(graphicsDevice, 1, 1);
      _texture.SetData(new Color[] { new Color(255, 255, 255), });

      _possibleTiles = new List<Sprite>();
      _currentPath = new List<Sprite>();

      _selectedTile = new Sprite(_texture)
      {
        Layer = 0.21f,
        IsFixedLayer = true,
        Colour = Color.Blue,
        SourceRectangle = new Rectangle(0, 0, 32, 32),
        Opacity = 0.6f,
      };
    }

    public void SetTarget(Hero hero)
    {
      if (hero == null)
      {
        _previouslyUpdatedPoint = new Vector2();
        Clear();
        _selectedHero = null;
        return;
      }

      if (hero.WalkingPath.Count > 0)
      {
        Clear();
        return;
      }

      _selectedHero = hero;

      var centrePosition = hero.Position;
      var centrePoint = centrePosition / 32;

      if (_previouslyUpdatedPoint == centrePoint)
        return;

      //if (_previouslyUpdatedHero == hero)
      //  return;

      _previouslyUpdatedPoint = centrePoint;
      _previouslyUpdatedHero = hero;

      var distance = hero.Villager.Distance;

      Clear();

      // Don't draw the "_possibleTiles" if we're moving
      if (hero.WalkingPath.Count > 0)
        return;

      Console.WriteLine("Start: " + DateTime.Now.ToString("hh:mm:ss:ff"));

      for (int y = 0; y < _map.Height; y++)
      {
        for (int x = 0; x < _map.Width; x++)
        {
          var point = new Vector2(x, y);

          if (centrePoint == point)
            continue;

          var pointRectangle = new Rectangle(x, y, 1, 1);

          var actualDistance = Vector2.Distance(centrePoint, point);

          if (actualDistance <= distance)
          {
            if (_map.MapObjects.Any(c => c.Intersects(pointRectangle)))
              continue;

            //if (actualDistance > distance - hero.Villager.Speed)
            //{
              var result = Pathfinder.Find(_map.GetMap(), centrePoint.ToPoint(), point.ToPoint());

              if (result.Status == PathStatus.Invalid)
                continue;

              if (result.Path.Count > distance)
                continue;
            //}

            var sprite = new Sprite(_texture)
            {
              Layer = _selectedTile.Layer - 0.02f,
              IsFixedLayer = true,
              Colour = result.Path.Count <= (hero.Villager.Speed) ? Color.Green : Color.Orange,
              Position = new Vector2(x * 32, y * 32),
              SourceRectangle = new Rectangle(0, 0, 32, 32),
              Opacity = 0.6f,
            };

            _possibleTiles.Add(sprite);
          }
        }
      }

      Console.WriteLine("End: " + DateTime.Now.ToString("hh:mm:ss:ff"));
    }

    public void Clear()
    {
      _possibleTiles.Clear();
      _currentPath.Clear();
    }

    public void Update(GameTime gameTime)
    {
      _selectedTile.Opacity = 0f;
      if (_selectedHero == null)
        return;

      if (_selectedHero.WalkingPath.Count > 0)
        return;

      _selectedTile.Opacity = 1f;

      var distance = _selectedHero.Villager.Distance;

      var centrePosition = _selectedHero.Position;
      var centrePoint = centrePosition / 32;

      _previousMousePosition = _currentMousePosition;
      _currentMousePosition = new Vector2((int)Math.Floor(GameMouse.RectangleWithCamera.X / 32d) * 32, (int)Math.Floor(GameMouse.RectangleWithCamera.Y / 32d) * 32);

      if (_previousMousePosition != _currentMousePosition) // Figure out why this is incorrect
      {
        _selectedTile.Position = _currentMousePosition;

        var result = Pathfinder.Find(_map.GetMap(), centrePoint.ToPoint(), (_currentMousePosition / 32).ToPoint());

        if (result.Status == PathStatus.Valid)
        {
          _currentPath = result.Path.Select(c => new Sprite(_texture)
          {
            Position = c.ToVector2() * 32,
            Layer = _selectedTile.Layer - 0.01f,
            IsFixedLayer = true,
            SourceRectangle = new Rectangle(0, 0, 32, 32),
            Colour = Color.Blue,
            Opacity = 0.8f,
          }).Take(distance).ToList();
        }
      }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      foreach (var tile in _possibleTiles)
        tile.Draw(gameTime, spriteBatch);

      foreach (var path in _currentPath)
        path.Draw(gameTime, spriteBatch);

      _selectedTile.Draw(gameTime, spriteBatch);
    }
  }
}
