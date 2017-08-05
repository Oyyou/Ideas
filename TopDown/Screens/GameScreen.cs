using Engine;
using Engine.Controls;
using Engine.Models;
using Engine.Sprites;
using Engine.States;
using Engine.TmxSharp;
using Engine.Utilities;
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
using TopDown.Buildings.Labour;
using TopDown.Controls;
using TopDown.Controls.BuildMenu;
using TopDown.Controls.CraftingMenu;
using TopDown.Controls.InventoryMenu;
using TopDown.Controls.ItemMenu;
using TopDown.Controls.JobMenu;
using TopDown.Controls.Toolbars;
using TopDown.Core;
using TopDown.Furnitures;
using TopDown.Items;
using TopDown.Logic;
using TopDown.Resources;
using TopDown.Sprites;
using static TopDown.Logic.Pathfinder;

namespace TopDown.States
{
  public enum GameStates
  {
    Playing,
    Paused,
    BuildMenu,
    PlacingBuilding,
    ItemMenu,
    PlacingItems,
    JobMenu,
    CraftingMenu,
    InventoryMenu,
  }

  public class GameScreen : State
  {
    private BuildMenuWindow _buildMenu;

    private Camera _camera;

    public CraftingMenuWindow CraftingMenu;

    private SpriteFont _font;

    private List<Component> _gameComponents;

    private List<Component> _guiComponents;

    public IEnumerable<Building> BuildingComponents
    {
      get
      {
        return _gameComponents.Where(c => c is Building).Cast<Building>();
      }
    }

    public List<Component> CollidableComponents
    {
      get
      {
        return _gameComponents.Where(c => c.CollisionRectangles != null && c.CollisionRectangles.Count > 0).ToList();
      }
    }

    public IEnumerable<SmallHouse> HouseComponents
    {
      get
      {
        return _gameComponents.Where(c => c is SmallHouse).Cast<SmallHouse>();
      }
    }

    /// <summary>
    /// Items in the inventory
    /// </summary>
    public List<Item> InventoryItems { get; set; }

    public InventoryMenuWindow InventoryMenu { get; set; }

    public ItemMenu ItemMenu { get; set; }

    public JobMenuWindow JobMenu;

    public static Controls.Keyboard Keyboard;

    public static MessageBox MessageBox;

    public static Controls.Mouse Mouse;

    public int MaxNPCCount
    {
      get
      {
        return _gameComponents.Where(c => c is SmallHouse).Count() * SmallHouse.MaxResidents;
      }
    }

    public IEnumerable<NPC> NPCComponents
    {
      get
      {
        return _gameComponents.Where(c => c is NPC).Cast<NPC>();
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

    public List<Building> Workplaces
    {
      get
      {
        var components = _gameComponents.Where(c => c is Blacksmith || c is Tavern || c is Mine).Cast<Building>().ToList();

        return components ?? new List<Building>();
      }
    }

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

    public void AddNPC()
    {
      if (NPCComponents.Count() >= MaxNPCCount)
        return;

      var playerAnimations = new Dictionary<string, Animation>()
      {
        { "WalkDown", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkDown"), 4) },
        { "WalkLeft", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkLeft"), 4) },
        { "WalkRight", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkRight"), 4) },
        { "WalkUp", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkUp"), 4) },
      };

      var house = HouseComponents.Where(c => c.ResidentCount < SmallHouse.MaxResidents && (c.State == BuildingStates.Built_In || c.State == BuildingStates.Built_Out)).FirstOrDefault();

      if (house == null)
        return;

      var npc = new NPC(playerAnimations, this)
      {
        Position = new Vector2(736, 288),
        IsCollidable = false,
        Layer = Building.DefaultLayer + 0.001f,
        Home = house,
      };

      house.ResidentCount++;

      npc.LoadContent(_content);

      _gameComponents.Add(npc);
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
      _spriteBatch.DrawString(_font, $"{NPCComponents.Count()}/{MaxNPCCount}", new Vector2(x, 25), Color.Red);

      foreach (var component in _guiComponents)
        component.Draw(gameTime, _spriteBatch);

      _spriteBatch.End();
    }

    public GameScreen()
    {

    }

    private void ItemMenuUpdate(GameTime gameTime)
    {
      Time = Time.AddSeconds(30);

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

    public override void LoadContent(GameModel gameModel)
    {
      base.LoadContent(gameModel);

      _font = _content.Load<SpriteFont>("Fonts/Font");

      State = GameStates.Playing;

      PathFinder = new Pathfinder();

      _buildMenu = new BuildMenuWindow(this);

      CraftingMenu = new CraftingMenuWindow(this);

      JobMenu = new JobMenuWindow(this);

      InventoryMenu = new InventoryMenuWindow(this);

      ItemMenu = new ItemMenu(this);

      InventoryItems = new List<Item>();

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

      var buttonTexture = gameModel.ContentManger.Load<Texture2D>("Controls/Button");
      var buttonFont = gameModel.ContentManger.Load<SpriteFont>("Fonts/Font");

      _guiComponents = new List<Component>()
      {
        new FrameCounter(),
        Keyboard,
        Mouse,
        MessageBox,
        new TopToolbar(this),
        new BottomToolbar(this),
        new ResourceView(Resources),
        _buildMenu,
        CraftingMenu,
        InventoryMenu,
        JobMenu,
        ItemMenu,
      };

      foreach (var component in _gameComponents)
        component.LoadContent(_content);

      foreach (var component in _guiComponents)
        component.LoadContent(_content);

      foreach (var objectGroup in map.ObjectGroups)
      {
        switch (objectGroup.Name)
        {
          case "Blacksmith":

            var blacksmith = new Blacksmith(this, _content.Load<Texture2D>("Buildings/Blacksmith/In"), _content.Load<Texture2D>("Buildings/Blacksmith/Out"))
            {
            };

            blacksmith.LoadContent(_content);


            foreach (var collisionObject in objectGroup.CollisionObjects)
            {
              var position = new Vector2(collisionObject.X, collisionObject.Y);

              switch (collisionObject.Name)
              {
                case "Building":

                  blacksmith.Position = position;

                  break;

                case "Anvil":
                  var anvil = new Furniture(_content.Load<Texture2D>("Furniture/Anvil"), this)
                  {
                    Position = position,
                    State = PlacableObjectStates.Placed,
                  };

                  anvil.LoadContent(_content);

                  blacksmith.Components.Add(anvil);

                  break;

                default:
                  throw new Exception("Unknown object: " + collisionObject.Name);
              }
            }

            blacksmith.State = BuildingStates.Built_Out;
            _gameComponents.Add(blacksmith);

            break;

          case "NPCs":

            foreach (var collisionObject in objectGroup.CollisionObjects)
            {
              var position = new Vector2(collisionObject.X, collisionObject.Y);

              var npc = new NPC(playerAnimations, this)
              {
                Position = position,
                IsCollidable = false,
                Layer = Building.DefaultLayer + 0.001f,
              };

              npc.LoadContent(_content);

              _gameComponents.Add(npc);
            }

            break;

          default:
            //throw new Exception("Unknown group: " + objectGroup.Name);
            break;
        }
      }

      UpdateMap();
    }

    private void MenuUpdate(GameTime gameTime, Keys menuKey)
    {
      foreach (var component in _guiComponents)
        component.Update(gameTime);

      if (Keyboard.IsKeyPressed(menuKey) ||
        Keyboard.IsKeyPressed(Keys.Escape))
      {
        State = GameStates.Playing;
      }
    }

    public void UpdateMap()
    {
      var pathPositions = BuildingComponents.SelectMany(c => c.PathPositions).ToList();

      var pathSearchNodes = new List<SearchNode>();

      foreach (var path in PathComponents)
      {
        var neighbors = new List<Rectangle>()
        {
          new Rectangle((int)path.Position.X, (int)path.Position.Y - 32, 32, 32),
          new Rectangle((int)path.Position.X, (int)path.Position.Y + 32, 32, 32),
          new Rectangle((int)path.Position.X - 32, (int)path.Position.Y, 32, 32),
          new Rectangle((int)path.Position.X + 32, (int)path.Position.Y, 32, 32),
        };

        var searchNode = new SearchNode()
        {
          Position = path.Position / 32,
          Walkable = true,
          Neighbors = new SearchNode[4],
        };

        foreach (var workplace in BuildingComponents)
        {
          for (int i = 0; i < neighbors.Count; i++)
          {
            var neigbor = neighbors[i];

            if (workplace.Rectangle.Intersects(neigbor))
            {
              searchNode.Neighbors[i] = new SearchNode();

              if (workplace.DoorLocations != null)
              {
                foreach (var doorLocation in workplace.DoorLocations)
                {
                  if (path.Position == new Vector2(doorLocation.Position.X, doorLocation.Position.Y))
                    searchNode.Neighbors[i] = null;
                }
              }
            }
          }
        }

        pathSearchNodes.Add(searchNode);
      }

      pathPositions.AddRange(pathSearchNodes);

      PathFinder.UpdateMap(pathPositions);

      PathFinder.WriteMap();
    }

    private void PlacingBuildingUpdate(GameTime gameTime)
    {
      Time = Time.AddSeconds(30);

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
      Time = Time.AddSeconds(30);

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
      Time = Time.AddSeconds(30);

      AddNPC();

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

      if (Keyboard.IsKeyPressed(Keys.P))
        State = GameStates.Paused;

      if (Keyboard.IsKeyPressed(Keys.B))
        State = GameStates.BuildMenu;

      if (Keyboard.IsKeyPressed(Keys.J))
        State = GameStates.JobMenu;

      if (Keyboard.IsKeyPressed(Keys.C))
        State = GameStates.CraftingMenu;

      if (Keyboard.IsKeyPressed(Keys.I))
        State = GameStates.InventoryMenu;

      if (Keyboard.IsKeyPressed(Keys.Enter))
        MessageBox.Show("You just pressed Enter. Well done :)");

      if (Keyboard.IsKeyPressed(Keys.P))
        State = GameStates.Paused;
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
      switch (State)
      {
        case GameStates.Playing:

          PlayingUpdate(gameTime);

          break;

        case GameStates.Paused:

          MenuUpdate(gameTime, Keys.P);

          break;

        case GameStates.BuildMenu:

          MenuUpdate(gameTime, Keys.B);

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

          MenuUpdate(gameTime, Keys.J);

          break;

        case GameStates.CraftingMenu:

          MenuUpdate(gameTime, Keys.C);

          break;

        case GameStates.InventoryMenu:

          MenuUpdate(gameTime, Keys.I);

          break;
        default:
          throw new Exception("Unknown state: " + State.ToString());
      }
    }
  }
}
