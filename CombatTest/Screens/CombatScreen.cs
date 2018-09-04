using Engine.Cameras;
using Engine.Input;
using Engine.Logic;
using Engine.Models;
using Engine.States;
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
        Width = 20,
        Height = 20,
        TileWidth = 32,
        TileHeight = 32,
      };

      _grid = new Grid(_graphicsDevice, _map);

      _pathViewer = new PathViewer(_graphicsDevice, _map);

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

      _gui = new CombatGUI(_squad)
      {
        EndTurnClick = EndTurnClick,
      };
      _gui.LoadContent(_content);

      LoadTiledMap();

      foreach (var sprite in _sprites)
        _map.AddObject(new Rectangle(sprite.GridRectangle.X / 32, sprite.GridRectangle.Y / 32, sprite.GridRectangle.Width / 32, sprite.GridRectangle.Height / 32));

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
      /// reasons to update the pathViewer
      ///  Clicked a different hero
      ///  The hero has moved from A to B, and still has a turn

      _previousHero = _currentHero;
      _currentHero = _gui.SelectedHeroIndex > -1 ? _heroes[_gui.SelectedHeroIndex] : null;

      if (_previousHero != _currentHero)
      {
        _pathViewer.SetTarget(_currentHero);
      }

      if (_currentHero != null && _currentHero.HasFinishedWalking)
        _pathViewer.SetTarget(_currentHero);

      if (GameMouse.ClickableObjects.Count == 0 && GameMouse.Clicked && _currentHero != null && _heroes.All(c => c.WalkingPath.Count == 0))
      {
        var targetPoint = new Point(GameMouse.RectangleWithCamera.X / 32, GameMouse.RectangleWithCamera.Y / 32);

        if (_currentHero.Villager.Turns > 0 && _currentHero.WalkingPath.Count == 0)
        {
          var point = new Point((int)_currentHero.Position.X / 32, (int)_currentHero.Position.Y / 32);

          var status = Pathfinder.Find(_map.GetMap(), point, targetPoint);

          if (status == PathStatus.Valid)
          {
            _currentHero.SetPath(Pathfinder.Path);
            _pathViewer.Clear();
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

      _pathViewer.Update(gameTime);

      // Only update the "_heroPanel" when nobody is moving
      //if (_heroes.All(c => c.WalkingPath.Count == 0))
        _gui.Update(gameTime);

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
      _graphicsDevice.Clear(Color.CornflowerBlue);

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
