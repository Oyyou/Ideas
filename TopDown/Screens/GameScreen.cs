using Engine;
using Engine.Controls;
using Engine.Models;
using Engine.Sprites;
using Engine.States;
using Engine.TmxSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.Buildings;
using TopDown.Controls;
using TopDown.Controls.BuildMenu;
using TopDown.Core;
using TopDown.Resources;
using TopDown.Sprites;
using TopDown.Sprites.Fences;

namespace TopDown.States
{
  public enum States
  {
    Playing,
    BuildMenu,
    PlacingBuilding,
    ItemMenu,
    PlacingItems,
  }

  public class GameScreen : State
  {
    private BuildMenuWindow _buildMenu;

    private Camera _camera;

    private List<Component> _gameComponents;

    private List<Component> _guiComponents;

    private ItemMenu _itemMenu;

    public static Controls.Keyboard Keyboard;

    public static Controls.Mouse Mouse;

    public TopDown.Sprites.Player Player { get; private set; }

    public Models.Resources Resources { get; set; }

    public Building SelectedBuilding { get; set; }

    public States State { get; set; }

    public void AddComponent(Building building)
    {
      SelectedBuilding = building;
      building.LoadContent(_content);

      _gameComponents.Add(building);
    }

    private void BuildMenuUpdate(GameTime gameTime)
    {
      foreach (var component in _guiComponents)
        component.Update(gameTime);

      if (Keyboard.IsKeyPressed(Keys.B) ||
          Keyboard.IsKeyPressed(Keys.Escape))
      {
        State = States.Playing;
      }
    }

    public override void Draw(GameTime gameTime)
    {
      _spriteBatch.Begin(
        SpriteSortMode.FrontToBack,
        BlendState.AlphaBlend,
        null, null, null, null,
        _camera.Transform);

      foreach (var component in _gameComponents)
        component.Draw(gameTime, _spriteBatch);

      _spriteBatch.End();

      _spriteBatch.Begin(SpriteSortMode.FrontToBack);

      foreach (var component in _guiComponents)
        component.Draw(gameTime, _spriteBatch);

      _spriteBatch.End();
    }

    public override void LoadContent(GameModel gameModel)
    {
      base.LoadContent(gameModel);

      State = States.Playing;

      _buildMenu = new BuildMenuWindow(this);
      _itemMenu = new ItemMenu(this);

      _camera = new Camera();

      Keyboard = new Controls.Keyboard();

      Mouse = new TopDown.Controls.Mouse(_camera);

      Resources = new Models.Resources();

      Player = new TopDown.Sprites.Player(
          new Dictionary<string, Animation>()
          {
            { "WalkLeft", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkLeft"), 3) },
            { "WalkRight", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkRight"), 3) },
            { "WalkUp", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkUp"), 3) },
            { "WalkDown", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkDown"), 3) },
          }
        )
      {
        Layer = 0.9f,
        Position = new Vector2(32, 32),
      };

      _gameComponents = new List<Component>()
      {
        Player,
      };

      var map = TmxMap.Load("Content/Maps/Level01.tmx");

      var textures = map.Tileset.Select(c => _content.Load<Texture2D>("Tilemaps/" + c.Name)).ToList();

      var x = 0;
      var y = 0;

      foreach (var layer in map.Layer)
      {
        foreach (var data in layer.Data)
        {
          Texture2D texture = null;

          int count = 0;
          var i = 0;
          for (; i < map.Tileset.Length; i++)
          {
            var tileset = map.Tileset[i];

            count += tileset.TileCount;

            if (data.GID > count)
            {
              continue;
            }

            texture = textures[i];
            break;
          }

          var id = map.Tileset.ToList().GetRange(0, i).Sum(c => c.TileCount);

          var position = new Vector2(x * map.TileWidth, y * map.TileHeight);
          var sourceRectangle = new Rectangle(((data.GID - 1) - id) * map.TileWidth, 0, map.TileWidth, map.TileHeight);

          // TODO: Set Y for sourceRectangle;

          if (data.GID != 0)
          {
            switch (data.GID)
            {
              case 8:
                _gameComponents.Add(
                  new Rock(texture, this)
                  {
                    Layer = 0.1f,
                    Position = position,
                    SourceRectangle = sourceRectangle,
                  }
                );
                break;

              default:
                _gameComponents.Add(
                  new Sprite(texture)
                  {
                    Layer = 0.1f,
                    Position = position,
                    SourceRectangle = sourceRectangle,
                  }
                );
                break;
            }
          }

          x++;
          if (x >= layer.Width)
          {
            x = 0;
            y++;
          }
        }
      }

      var buttonTexture = gameModel.ContentManger.Load<Texture2D>("Controls/Button");
      var buttonFont = gameModel.ContentManger.Load<SpriteFont>("Fonts/Font");

      _guiComponents = new List<Component>()
      {
        Keyboard,
        Mouse,
        new Toolbar(this),
        new ResourceView(Resources),
        _buildMenu,
        _itemMenu,
      };

      foreach (var component in _gameComponents)
        component.LoadContent(_content);

      foreach (var component in _guiComponents)
        component.LoadContent(_content);
    }

    public GameScreen()
    {

    }

    private void PlayingUpdate(GameTime gameTime)
    {
      foreach (var component in _guiComponents)
        component.Update(gameTime);

      foreach (var component in _gameComponents)
        component.Update(gameTime);

      for (int i = 0; i < _gameComponents.Count; i++)
      {
        for (int j = i + 1; j < _gameComponents.Count; j++)
        {
          _gameComponents[i].CheckCollision(_gameComponents[j]);
        }
      }

      _camera.Follow(((Sprite)_gameComponents[0]).Position);

      if (GameScreen.Keyboard.IsKeyPressed(Keys.B))
        State = States.BuildMenu;
    }

    public override void PostUpdate(GameTime gameTime)
    {
      for (int i = 0; i < _gameComponents.Count; i++)
      {
        _gameComponents[i].PostUpdate(gameTime);

        if (_gameComponents[i].IsRemoved)
        {
          _gameComponents.RemoveAt(i);
          i--;
        }
      }
    }

    public override void UnloadContent()
    {
      foreach (var component in _gameComponents)
        component.UnloadContent();

      foreach (var component in _guiComponents)
        component.UnloadContent();

      _guiComponents.Clear();
    }

    public override void Update(GameTime gameTime)
    {
      switch (State)
      {
        case States.Playing:

          PlayingUpdate(gameTime);

          break;
        case States.BuildMenu:

          BuildMenuUpdate(gameTime);

          break;
        case States.PlacingBuilding:

          foreach (var component in _guiComponents)
            component.Update(gameTime);

          foreach (var component in _gameComponents)
            component.Update(gameTime);

          for (int i = 0; i < _gameComponents.Count; i++)
          {
            for (int j = i + 1; j < _gameComponents.Count; j++)
            {
              _gameComponents[i].CheckCollision(_gameComponents[j]);
            }
          }

          _camera.Follow(((Sprite)_gameComponents[0]).Position);

          break;
        case States.ItemMenu:
          
          foreach (var component in _guiComponents)
            component.Update(gameTime);

          foreach (var component in _gameComponents)
            component.Update(gameTime);

          for (int i = 0; i < _gameComponents.Count; i++)
          {
            for (int j = i + 1; j < _gameComponents.Count; j++)
            {
              _gameComponents[i].CheckCollision(_gameComponents[j]);
            }
          }

          _camera.Follow(((Sprite)_gameComponents[0]).Position);

          if (GameScreen.Keyboard.IsKeyPressed(Keys.B))
          {
            State = States.BuildMenu;
            SelectedBuilding.IsRemoved = true;
          }

          if (GameScreen.Keyboard.IsKeyPressed(Keys.Escape))
          {
            State = States.Playing;
            SelectedBuilding.IsRemoved = true;
          }

          break;
        case States.PlacingItems:

          foreach (var component in _guiComponents)
            component.Update(gameTime);

          foreach (var component in _gameComponents)
            component.Update(gameTime);

          for (int i = 0; i < _gameComponents.Count; i++)
          {
            for (int j = i + 1; j < _gameComponents.Count; j++)
            {
              _gameComponents[i].CheckCollision(_gameComponents[j]);
            }
          }

          _camera.Follow(((Sprite)_gameComponents[0]).Position);

          if (GameScreen.Keyboard.IsKeyPressed(Keys.Escape))
          {
            State = States.ItemMenu;
            SelectedBuilding.Components.Last().IsRemoved = true;
          }

          break;
        default:
          throw new Exception("Unknown state: " + State.ToString());
      }
    }
  }
}
