using Engine;
using Engine.Cameras;
using Engine.Input;
using Engine.Logic;
using Engine.Models;
using Engine.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledReader;
using VillageBackend.Graphics;
using VillageBackend.Managers;
using VillageBackend.Models;
using VillageGUI.Interface.Combat;
using static Engine.Logic.Pathfinder;

namespace CombatTest.Screens
{
  // TODO: Move to somewhere more appropriate
  public class Map
  {
    public int Width { get; set; }

    public int Height { get; set; }

    /// <summary>
    /// These are non-collidable objects
    /// </summary>
    public List<Rectangle> MapObjects { get; private set; } = new List<Rectangle>();

    public void AddObject(Rectangle rectangle)
    {
      if (MapObjects.Any(c => c.Intersects(rectangle)))
        throw new Exception("Object already exists in position: " + rectangle);

      MapObjects.Add(rectangle);
    }

    public char[,] GetMap()
    {
      var map = new char[Height, Width];

      for (int y = 0; y < Height; y++)
      {
        for (int x = 0; x < Width; x++)
        {
          map[y, x] = '0';

          var rectangle = new Rectangle(x, y, 1, 1);

          if (MapObjects.Any(c => c.Intersects(rectangle)))
          {
            map[y, x] = '1';
          }
        }
      }

      return map;
    }

    public void Write(List<Point> path = null)
    {
      if (path == null)
        path = new List<Point>();

      for (int y = 0; y <= Height; y++)
      {
        for (int x = 0; x <= Width; x++)
        {
          if (path.Any(c => c == new Point(x, y)))
            Console.Write("P");
          else if (MapObjects.Any(c => c.Intersects(new Rectangle(x, y, 1, 1))))
            Console.Write("#");
          else
            Console.Write("0");
        }

        Console.WriteLine();
      }

      Console.WriteLine();
    }
  }

  public enum CombatStates
  {
    PlayerTurn,
    EnemyTurn,
  }

  public class CombatScreen : State
  {
    private Squad _squad;

    private HeroPanel _heroPanel;

    private Camera_2D _camera;

    private Map _map;

    private List<Hero> _heroes = new List<Hero>();

    private List<Sprite> _tiles = new List<Sprite>();

    private List<Sprite> _sprites;

    private Dictionary<int, Sprite> _spriteFactory = new Dictionary<int, Sprite>();

    public CombatStates State { get; private set; }

    public CombatScreen(Squad squad)
    {
      _squad = squad;
    }

    public override void OnScreenResize()
    {
      _heroPanel.SetPositions();
    }

    public override void LoadContent(GameModel gameModel)
    {
      base.LoadContent(gameModel);

      _camera = new Camera_2D();

      _map = new Map()
      {
        Width = 20,
        Height = 20,
      };

      _sprites = new List<Sprite>();

      foreach (var villager in _squad.Villagers)
      {
        _heroes.Add(new Hero(_content.Load<Texture2D>("Villagers/Pig"))
        {
          Position = new Vector2(32, 64),
          Layer = 0.4f,
          Origin = new Vector2(0, 15),
          Villager = villager,
        });
      }

      _heroPanel = new HeroPanel(_squad)
      {
        Layer = 0.9f,
      };

      LoadTiledMap();

      foreach (var sprite in _sprites)
        _map.AddObject(new Rectangle(sprite.GridRectangle.X / 32, sprite.GridRectangle.Y / 32, sprite.GridRectangle.Width / 32, sprite.GridRectangle.Height / 32));

      _heroPanel.LoadContent(_content);

      OnScreenResize();
    }

    private void LoadTiledMap()
    {
      Console.WriteLine("-->Map");
      var tiledMap = TiledMap.Load("Content/TileMaps", "Level_001.tmx");

      foreach (var tileset in tiledMap.Tileset)
      {
        var texture = _content.Load<Texture2D>("TileMaps/" + System.IO.Path.GetFileNameWithoutExtension(tileset.Image.Source));

        int index = tileset.FirstGId;

        for (int y = 0; y < texture.Height / tileset.TileHeight; y++)
        {
          for (int x = 0; x < texture.Width / tileset.TileWidth; x++)
          {
            Sprite sprite = null;

            switch (tileset.Name)
            {
              case "Roads":

                sprite = new Sprite(texture, index - 1, tileset.TileWidth, tileset.TileHeight)
                {
                  Layer = 0.1f,
                  IsFixedLayer = true,
                };

                break;

              case "Buildings":

                sprite = new Sprite(texture, index - 1, tileset.TileWidth, tileset.TileHeight)
                {
                  Layer = 0.5f,
                };

                break;

              default:
                throw new Exception("Unknown layer: " + tileset.Name);
            }

            _spriteFactory.Add(index, sprite);

            index++;
          }
        }
      }

      var textures = tiledMap.Tileset.Select(c => _content.Load<Texture2D>("TileMaps/" + System.IO.Path.GetFileNameWithoutExtension(c.Image.Source)));

      foreach (var layer in tiledMap.Layer)
      {
        var lines = layer.Data.Split('\n').Where(c => !string.IsNullOrEmpty(c)).ToList();

        var texture = textures.Where(c => System.IO.Path.GetFileNameWithoutExtension(c.Name) == layer.Name).FirstOrDefault();

        int x = 0;
        int y = 0;

        foreach (var line in lines)
        {
          var values = line.Split(',').Where(c => !string.IsNullOrEmpty(c)).ToList();

          foreach (var value in values)
          {
            var number = int.Parse(value);

            if (number > 0)
            {
              var sprite = _spriteFactory[number].Clone() as Sprite;
              sprite.Position = new Vector2(x * 32, y * 32);

              switch (layer.Name)
              {
                case "Roads":

                  _tiles.Add(sprite);

                  break;

                case "Buildings":

                  _sprites.Add(sprite);

                  break;

                default:
                  break;
              }
            }

            x++;
          }
          y++;
          x = 0;
        }
      }
    }

    public override void UnloadContent()
    {
      _heroPanel.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      GameMouse.Update(_camera.Transform);

      _camera.Update(gameTime);

      switch (State)
      {
        case CombatStates.PlayerTurn:
          PlayerUpdate(gameTime);
          break;
        case CombatStates.EnemyTurn:
          EnemyUpdate(gameTime);
          break;
        default:
          break;
      }
    }

    private void PlayerUpdate(GameTime gameTime)
    {
      if (GameMouse.ClickableObjects.Count == 0 && GameMouse.Clicked)
      {
        var targetPoint = new Point(GameMouse.RectangleWithCamera.X / 32, GameMouse.RectangleWithCamera.Y / 32);

        var hero = _heroes[_heroPanel.SelectedHeroIndex];

        if (hero.Villager.Turns > 0 && hero.WalkingPath.Count == 0)
        {
          var point = new Point((int)hero.Position.X / 32, (int)hero.Position.Y / 32);

          var status = Pathfinder.Find(_map.GetMap(), point, targetPoint);

          if (status == PathStatus.Valid)
          {
            hero.SetPath(Pathfinder.Path);
          }
          else if (status == PathStatus.Invalid)
          {
            foreach (var error in Pathfinder.Errors)
              Console.WriteLine(error);
          }
        }
      }

      foreach (var hero in _heroes)
        hero.Update(gameTime);

      _heroPanel.Update(gameTime);

      if (_heroes.Sum(c => c.Villager.Turns) == 0) // or if we click "end turn"
        State = CombatStates.EnemyTurn;
    }

    private void EnemyUpdate(GameTime gameTime)
    {
      // TODO: Implement

      State = CombatStates.PlayerTurn;
      ResetHeroes();
    }

    private void ResetHeroes()
    {
      foreach(var hero in _heroes)
      {
        hero.Villager.Turns = hero.Villager.MaxTurns;
      }
    }

    public override void PostUpdate(GameTime gameTime)
    {

    }

    public override void Draw(GameTime gameTime)
    {
      _graphicsDevice.Clear(Color.CornflowerBlue);

      _spriteBatch.Begin(
        SpriteSortMode.FrontToBack,
        BlendState.AlphaBlend,
        SamplerState.PointWrap, null, null, null,
       _camera.Transform);

      foreach (var tile in _tiles)
        tile.Draw(gameTime, _spriteBatch);

      foreach (var villager in _heroes)
        villager.Draw(gameTime, _spriteBatch);

      foreach (var sprite in _sprites)
      {
        if (_camera.Position.Y < sprite.Rectangle.Y)
          sprite.Opacity = 0.5f;
        else sprite.Opacity = 1f;

        sprite.Draw(gameTime, _spriteBatch);
      }

      _spriteBatch.End();

      _spriteBatch.Begin(SpriteSortMode.FrontToBack);

      _heroPanel.Draw(gameTime, _spriteBatch);

      _spriteBatch.End();
    }
  }
}
