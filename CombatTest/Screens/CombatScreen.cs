using Engine;
using Engine.Cameras;
using Engine.Input;
using Engine.Logic;
using Engine.Models;
using Engine.States;
using Engine.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using TiledReader;
using VillageBackend.Graphics;
using VillageBackend.Models;
using VillageBackend.World;
using VillageGUI.Interface.Buttons;
using VillageGUI.Interface.GUIs;
using static Engine.Logic.Pathfinder;

namespace CombatTest.Screens
{
  public enum CombatStates
  {
    PlayerTurn,
    EnemyTurn,
  }

  public class CombatScreen : State
  {
    private Squad _squad;

    private CombatGUI _gui;

    private Camera_2D _camera;

    private Map _map;

    private Grid _grid;

    private PathViewer _pathViewer;

    private Hero _currentHero;
    private Hero _previousHero;

    private List<Hero> _heroes = new List<Hero>();

    private List<Sprite> _tiles = new List<Sprite>();

    private List<Sprite> _sprites;

    private Dictionary<int, Sprite> _spriteFactory = new Dictionary<int, Sprite>();

    private KeyboardState _currentKey;
    private KeyboardState _previousKey;

    public CombatStates State { get; private set; }

    public CombatScreen(Squad squad)
    {
      _squad = squad;
    }

    public override void OnScreenResize()
    {
      _gui.OnScreenResize();
    }

    public override void LoadContent(GameModel gameModel)
    {
      base.LoadContent(gameModel);

      _camera = new Camera_2D();

      _map = new Map()
      {
        Width = 30,
        Height = 30,
        TileWidth = 32,
        TileHeight = 32,
      };

      _grid = new Grid(_graphicsDevice, _map);

      _pathViewer = new PathViewer(_graphicsDevice, _map);

      _sprites = new List<Sprite>();

      for (int i = 0; i < _squad.Villagers.Count; i++)
      {
        _heroes.Add(new Hero(_content.Load<Texture2D>("Villagers/Pig"))
        {
          Position = new Vector2(32 * i, 64),
          Layer = 0.4f,
          Origin = new Vector2(0, 15),
          Villager = _squad.Villagers[i],
        });
      }

      _heroes.Last().Position = new Vector2(8 * 32, 18 * 32);

      _gui = new CombatGUI(_squad)
      {
        EndTurnClick = EndTurnClick,
      };
      _gui.LoadContent(_content);

      LoadTiledMap();

      foreach (var sprite in _sprites)
        _map.AddObject(sprite.GridRectangle1x1);

      foreach (var sprite in _heroes)
        _map.AddObject(sprite.GridRectangle1x1);

      OnScreenResize();
    }

    private void EndTurnClick(Button button)
    {
      State = CombatStates.EnemyTurn;
      button.CurrentState = ButtonStates.Hovering;
    }

    private void LoadTiledMap()
    {
      Console.WriteLine("-->Map");
      var tiledMap = TiledMap.Load("Content/TileMaps", "Level_001.tmx");

      var grassSprites = GetGrassSprites();

      foreach (var tileset in tiledMap.Tileset)
      {
        var texture = _content.Load<Texture2D>("TileMaps/" + System.IO.Path.GetFileNameWithoutExtension(tileset.Image.Source));

        int index = tileset.FirstGId;

        for (int y = 0; y < texture.Height / tileset.TileHeight; y++)
        {
          for (int x = 0; x < texture.Width / tileset.TileWidth; x++)
          {
            Sprite sprite = null;

            int frameIndex = index - tileset.FirstGId;

            switch (tileset.Name)
            {
              case "Roads":

                sprite = new Sprite(texture, frameIndex, tileset.TileWidth, tileset.TileHeight)
                {
                  Layer = 0.1f,
                  IsFixedLayer = true,
                };

                break;

              case "Buildings":

                sprite = new Sprite(texture, frameIndex, tileset.TileWidth, tileset.TileHeight)
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
            else
            {
              if(layer.Name == "Roads")
              {
                var grassSprite = grassSprites[GameEngine.Random.Next(0, grassSprites.Count)].Clone() as Sprite;
                grassSprite.Position = new Vector2(x * 32, y * 32);

                _tiles.Add(grassSprite);
              }
            }

            x++;
          }
          y++;
          x = 0;
        }
      }
    }

    private List<Sprite> GetGrassSprites()
    {
      var grassSprites = new List<Sprite>();

      var grassTexture = _content.Load<Texture2D>("TileMaps/Grass");

      int grassIndex = 0;
      for (int y = 0; y < grassTexture.Height / 32; y++)
      {
        for (int x = 0; x < grassTexture.Width / 32; x++)
        {
          grassSprites.Add(new Sprite(grassTexture, grassIndex, 32, 32)
          {
            Layer = 0.1f,
            IsFixedLayer = true,
          });

          grassIndex++;
        }
      }

      return grassSprites;
    }

    public override void UnloadContent()
    {
      _gui.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      GameMouse.Update(_camera.Transform);

      _previousKey = _currentKey;
      _currentKey = Keyboard.GetState();

      if (_previousKey.IsKeyDown(Keys.F1) && _currentKey.IsKeyUp(Keys.F1))
        _grid.IsVisible = !_grid.IsVisible;

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
      /// Pathfinding
      ///  At the start of each round - find the paths for all NPCs
      ///  When a hero moves - find all new paths (async?)

      /// reasons to update the pathViewer
      ///  Clicked a different hero
      ///  The hero has moved from A to B, and still has a turn

      _previousHero = _currentHero;
      _currentHero = _gui.SelectedHeroIndex > -1 ? _heroes[_gui.SelectedHeroIndex] : null;

      //if (_previousHero != _currentHero)
      //{
      _pathViewer.SetTarget(_currentHero);
      //}

      //if (_currentHero != null && _currentHero.HasFinishedWalking)
      //{
      //  // This method keeps getting called. Try and f
      //  _pathViewer.SetTarget(_currentHero);
      //}

      if (_currentHero != null && _currentHero.Villager.Turns == 0)
        _gui.Clear();

      if (GameMouse.ClickableObjects.Count == 0 && GameMouse.Clicked && _currentHero != null && _heroes.All(c => c.WalkingPath.Count == 0))
      {
        var targetPoint = new Point(GameMouse.RectangleWithCamera.X / 32, GameMouse.RectangleWithCamera.Y / 32);

        if (_currentHero.Villager.Turns > 0 && _currentHero.WalkingPath.Count == 0)
        {
          var heroPoint = new Point((int)_currentHero.Position.X / 32, (int)_currentHero.Position.Y / 32);

          var distance = _currentHero.Villager.Distance + 1;

          var actualDistance = Vector2.Distance(heroPoint.ToVector2(), targetPoint.ToVector2());

          if (actualDistance <= distance)
          {
            var result = Pathfinder.Find(_map.GetMap(), heroPoint, targetPoint);

            if (result.Status == PathStatus.Valid)
            {
              var path = result.Path.Take(_currentHero.Villager.Distance).ToList();

              _currentHero.SetPath(path);
              _map.RemoveObject(_currentHero.GridRectangle1x1);
              _map.AddObject(new Rectangle(path.Last().X, path.Last().Y, 1, 1));
              //_gui.Clear();
            }
            else if (result.Status == PathStatus.Invalid)
            {
              foreach (var error in result.Errors)
                Console.WriteLine(error);
            }
          }
        }
      }

      foreach (var hero in _heroes)
        hero.Update(gameTime);

      _pathViewer.Update(gameTime);

      // Only update the "_heroPanel" when nobody is moving
      //if (_heroes.All(c => c.WalkingPath.Count == 0))
      _gui.Update(gameTime, _heroes.All(c => c.WalkingPath.Count == 0));

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
      foreach (var hero in _heroes)
      {
        hero.Villager.Turns = hero.Villager.MaxTurns;
      }
    }

    public override void PostUpdate(GameTime gameTime)
    {

    }

    public override void Draw(GameTime gameTime)
    {
      _graphicsDevice.Clear(new Color(40, 40, 40));

      _spriteBatch.Begin(
        SpriteSortMode.FrontToBack,
        BlendState.AlphaBlend,
        SamplerState.PointWrap, null, null, null,
       _camera.Transform);

      _grid.Draw(gameTime, _spriteBatch);

      foreach (var tile in _tiles)
        tile.Draw(gameTime, _spriteBatch);

      _pathViewer.Draw(gameTime, _spriteBatch);

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

      _gui.Draw(gameTime, _spriteBatch);
    }
  }
}
