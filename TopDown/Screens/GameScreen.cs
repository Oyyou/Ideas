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
using TopDown.Builders;
using TopDown.Buildings;
using TopDown.Buildings.Housing;
using TopDown.Controls;
using TopDown.Controls.BuildMenu;
using TopDown.Controls.ItemMenu;
using TopDown.Controls.JobMenu;
using TopDown.Core;
using TopDown.Logic;
using TopDown.Resources;
using TopDown.Sprites;

namespace TopDown.States
{
  public enum GameStates
  {
    Playing,
    BuildMenu,
    PlacingBuilding,
    ItemMenu,
    PlacingItems,
    JobMenu,
  }

  public class GameScreen : State
  {
    private BuildMenuWindow _buildMenu;

    private Camera _camera;

    private SpriteFont _font;

    private List<Component> _gameComponents;

    private List<Component> _guiComponents;

    private JobMenuWindow _jobMenu;

    public List<Component> CollidableComponents
    {
      get
      {
        return _gameComponents.Where(c => c.CollisionRectangles != null && c.CollisionRectangles.Count > 0).ToList();
      }
    }

    public ItemMenu ItemMenu { get; set; }

    public static Controls.Keyboard Keyboard;

    public static MessageBox MessageBox;

    public static Controls.Mouse Mouse;

    public IEnumerable<Component> NPCComponents
    {
      get
      {
        return _gameComponents.Where(c => c is NPC);
      }
    }

    public PathBuilder SelectedPathBuilder { get; set; }

    public List<Component> PathComponents
    {
      get
      {
        return _gameComponents.Where(c => c is Path).ToList();
      }
    }

    public Pathfinder PathFinder;

    public TopDown.Sprites.Player Player { get; private set; }

    public Models.Resources Resources { get; set; }

    public Building SelectedBuilding { get; set; }

    public GameStates State { get; set; }

    public DateTime Time { get; set; }

    public void AddComponent(Building building)
    {
      SelectedBuilding = building;

      AddComponent(component: building);
    }

    public void AddComponent(Component component)
    {
      component.LoadContent(_content);

      _gameComponents.Add(component);
    }

    public void AddComponent(PathBuilder pathBuilder)
    {
      SelectedPathBuilder = pathBuilder;

      AddComponent(component: pathBuilder);
    }

    private void BuildMenuUpdate(GameTime gameTime)
    {
      foreach (var component in _guiComponents)
        component.Update(gameTime);

      if (Keyboard.IsKeyPressed(Keys.B) ||
          Keyboard.IsKeyPressed(Keys.Escape))
      {
        State = GameStates.Playing;
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

      var time = Time.ToString("hh:mm") + (Time.Hour >= 12 ? " pm" : " am");

      var x = GameEngine.ScreenWidth - _font.MeasureString(time).X - 10;

      _spriteBatch.DrawString(_font, time, new Vector2(x, 5), Color.Red);

      foreach (var component in _guiComponents)
        component.Draw(gameTime, _spriteBatch);

      _spriteBatch.End();
    }

    public GameScreen()
    {

    }
        
    private void ItemMenuUpdate(GameTime gameTime)
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
      {
        State = GameStates.BuildMenu;

        if (SelectedBuilding != null)
        {
          SelectedBuilding.IsRemoved = true;
          SelectedBuilding = null;
        }

        if (SelectedPathBuilder != null)
        {
          SelectedPathBuilder.IsRemoved = true;
          SelectedPathBuilder = null;
        }

        ItemMenu.FullReset();
      }

      if (GameScreen.Keyboard.IsKeyPressed(Keys.Escape))
      {
        State = GameStates.Playing;

        if (SelectedBuilding != null)
        {
          SelectedBuilding.IsRemoved = true;
          SelectedBuilding = null;
        }

        if (SelectedPathBuilder != null)
        {
          SelectedPathBuilder.IsRemoved = true;
          SelectedPathBuilder = null;
        }

        ItemMenu.FullReset();
      }
    }

    private void JobMenuUpdate(GameTime gameTime)
    {
      foreach (var component in _guiComponents)
        component.Update(gameTime);

      if (Keyboard.IsKeyPressed(Keys.J) ||
        Keyboard.IsKeyPressed(Keys.Escape))
      {
        State = GameStates.Playing;
      }
    }

    public override void LoadContent(GameModel gameModel)
    {
      base.LoadContent(gameModel);

      _font = _content.Load<SpriteFont>("Fonts/Font");

      State = GameStates.Playing;

      PathFinder = new Pathfinder();

      _buildMenu = new BuildMenuWindow(this);

      _jobMenu = new JobMenuWindow(this);

      ItemMenu = new ItemMenu(this);
      //ItemMenu.LoadContent(_content);

      _camera = new Camera();

      Keyboard = new Controls.Keyboard();

      MessageBox = new MessageBox(_content.Load<Texture2D>("Controls/MessageBox"), _content.Load<SpriteFont>("Fonts/Font"))
      {
        Layer = 0.999f,
      };

      Mouse = new TopDown.Controls.Mouse(_camera);

      Resources = new Models.Resources();

      var playerAnimations = new Dictionary<string, Animation>()
      {
        { "WalkDown", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkDown"), 4) },
        { "WalkLeft", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkLeft"), 4) },
        { "WalkRight", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkRight"), 4) },
        { "WalkUp", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkUp"), 4) },
      };

      Player = new TopDown.Sprites.Player(playerAnimations)
      {
        Layer = 0.9f,
        Position = new Vector2(512, 320),
      };

      _gameComponents = new List<Component>()
      {
        Player,
        //new PathBuilder(),
      };

      var map = TmxMap.Load("Content/Maps/Level01.tmx");

      var textures = map.Tileset.Select(c => _content.Load<Texture2D>("Tilemaps/" + c.Name)).ToList();

      foreach (var layer in map.Layer)
      {
        var x = 0;
        var y = 0;

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

              case 9:
                _gameComponents.Add(
                  new Path(texture)
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

      foreach (var objectGroup in map.ObjectGroups)
      {
        switch (objectGroup.Name)
        {
          case "Buildings":

            foreach (var collisionObject in objectGroup.CollisionObjects)
            {
              switch (collisionObject.Name)
              {
                case "SmallHouse":

                  var smallHouse = new SmallHouse(this, _content.Load<Texture2D>("Buildings/SmallHouse/In"), _content.Load<Texture2D>("Buildings/SmallHouse/Out"))
                  {
                    Position = new Vector2(collisionObject.X, collisionObject.Y),
                    State = BuildingStates.Built_Out,
                  };

                  smallHouse.LoadContent(_content);

                  _gameComponents.Add(smallHouse);

                  break;

                default:
                  throw new Exception("Unknown building: " + collisionObject.Name);

              }
            }

            break;

          case "NPCs":

            foreach (var collisionObject in objectGroup.CollisionObjects)
            {
              var position = new Vector2(collisionObject.X, collisionObject.Y);

              _gameComponents.Add(new NPC(playerAnimations, this)
              {
                Position = position,
                IsCollidable = false,
                Layer = 0.9f,
              });
            }

            break;

          default:
            throw new Exception("Unknown group: " + objectGroup.Name);
        }
      }

      PathFinder.UpdateMap(PathComponents.Select(c => c.Position).ToList());

      var buttonTexture = gameModel.ContentManger.Load<Texture2D>("Controls/Button");
      var buttonFont = gameModel.ContentManger.Load<SpriteFont>("Fonts/Font");

      _guiComponents = new List<Component>()
      {
        Keyboard,
        Mouse,
        MessageBox,
        new Toolbar_Top(this),
        new Toolbar_Bottom(this),
        new ResourceView(Resources),
        _buildMenu,
        _jobMenu,
        ItemMenu,
      };

      foreach (var component in _gameComponents)
        component.LoadContent(_content);

      foreach (var component in _guiComponents)
        component.LoadContent(_content);
    }

    private void PlacingBuildingUpdate(GameTime gameTime)
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

      if (Keyboard.IsKeyPressed(Keys.B))
      {
        State = GameStates.BuildMenu;

        if (SelectedBuilding != null)
        {
          SelectedBuilding.IsRemoved = true;
          SelectedBuilding = null;
        }

        if (SelectedPathBuilder != null)
        {
          SelectedPathBuilder.IsRemoved = true;
          SelectedPathBuilder = null;
        }
      }

      if (Keyboard.IsKeyPressed(Keys.Escape))
      {
        State = GameStates.Playing;

        if (SelectedBuilding != null)
        {
          SelectedBuilding.IsRemoved = true;
          SelectedBuilding = null;
        }

        if (SelectedPathBuilder != null)
        {
          SelectedPathBuilder.IsRemoved = true;
          SelectedPathBuilder = null;
        }
      }
    }

    private void PlacingItemsUpdate(GameTime gameTime)
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

      if (GameScreen.Keyboard.IsKeyPressed(Keys.Escape))
      {
        State = GameStates.ItemMenu;

        if (SelectedBuilding != null)
          SelectedBuilding.Components.Last().IsRemoved = true;

        if (SelectedPathBuilder != null)
        {
          SelectedPathBuilder.State = PathBuilderStates.Selecting;
          SelectedPathBuilder.Components.ForEach(c => c.IsRemoved = true);
          SelectedPathBuilder.Path = null;
        }

        ItemMenu.CurrentButton.CurrentState = ItemMenuButtonStates.Clickable;
      }
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
        State = GameStates.BuildMenu;

      if (GameScreen.Keyboard.IsKeyPressed(Keys.J))
        State = GameStates.JobMenu;

      if (Keyboard.IsKeyPressed(Keys.Enter))
        MessageBox.Show("You just pressed Enter. Well done :)");
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

      _gameComponents.Clear();

      _guiComponents.Clear();
    }

    public override void Update(GameTime gameTime)
    {
      Time = Time.AddSeconds(1);

      switch (State)
      {
        case GameStates.Playing:

          PlayingUpdate(gameTime);

          break;
        case GameStates.BuildMenu:

          BuildMenuUpdate(gameTime);

          break;
        case GameStates.PlacingBuilding:

          PlacingBuildingUpdate(gameTime);

          break;
        case GameStates.ItemMenu:

          ItemMenuUpdate(gameTime);

          break;
        case GameStates.PlacingItems:

          PlacingItemsUpdate(gameTime);

          break;

        case GameStates.JobMenu:

          JobMenuUpdate(gameTime);

          break;
        default:
          throw new Exception("Unknown state: " + State.ToString());
      }
    }
  }
}
