using Engine;
using Engine.Interface.Windows;
using Engine.Models;
using Engine.Sprites;
using Engine.States;
using Engine.TmxSharp;
using Engine.Utilities;
using GUITest.Interface.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using TopDown.Builders;
using TopDown.Buildings;
using TopDown.Buildings.Housing;
using TopDown.Buildings.Labour;
using TopDown.Controls;
using TopDown.Controls.BuildMenu;
using TopDown.Controls.InspectMenu;
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
using VillageBackend.Managers;
using VillageBackend.Models;
using static TopDown.Logic.Pathfinder;

namespace TopDown.States
{
  public enum Layers
  {
    Floor,          // 0.1
    Path,           // 0.2
    BuildingFloor,  // 0.3
    BuildingBottom, // 0.4
    Flora,          // 0.5
    NPC,            // 0.6
    BuildingTop,    // 0.7
  }

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
    InspectMenu,
  }

  public class GameScreen : State
  {
    private BuildMenuWindow _buildMenu;

    private Camera _camera;

    private SpriteFont _font;

    private List<Component> _guiComponents;

    #region Managers

    private ItemManager _itemManager;

    private JobManager _jobManager;

    private VillagerManager _villagerManager;

    #endregion

    private RenderTarget2D _renderTarget;

    private GUITest.Interface.Toolbar _toolbar;

    public IEnumerable<Building> BuildingComponents
    {
      get
      {
        return GameComponents.Where(c => c is Building && !c.IsRemoved).Cast<Building>()
          .Where(c => c.State == BuildingStates.Built);
      }
    }

    public List<Component> CollidableComponents
    {
      get
      {
        return GameComponents.Where(c => c.CollisionRectangles != null && c.CollisionRectangles.Count > 0).ToList();
      }
    }

    public List<Component> GameComponents;

    public IEnumerable<SmallHouse> HouseComponents
    {
      get
      {
        return GameComponents.Where(c => c is SmallHouse).Cast<SmallHouse>();
      }
    }

    /// <summary>
    /// Items in the inventory
    /// </summary>
    public List<Item> InventoryItems { get; set; }

    public InspectMenuWindow InspectMenu { get; set; }

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
        return GameComponents.Where(c => c is SmallHouse && ((SmallHouse)c).State == BuildingStates.Built).Count() * SmallHouse.MaxResidents;
      }
    }

    public Notifications Notifications { get; private set; }

    public IEnumerable<NPC> NPCComponents
    {
      get
      {
        return GameComponents.Where(c => c is NPC).Cast<NPC>();
      }
    }

    public PathBuilder SelectedPathBuilder { get; set; }

    public List<Component> PathComponents
    {
      get
      {
        return GameComponents.Where(c => c is Path).ToList();
      }
    }

    public Pathfinder PathFinder;

    //public TopDown.Sprites.Player Player { get; private set; }

    public VillageBackend.Models.Resources Resources { get; set; }

    public Building SelectedBuilding { get; set; }

    public GameStates State { get; set; }

    public DateTime Time { get; set; }

    public List<Building> Workplaces
    {
      get
      {
        var components = BuildingComponents.Where(c => c is Blacksmith || c is Tavern || c is Mine || c is Farm).Cast<Building>().ToList();

        return components ?? new List<Building>();
      }
    }

    public void AddComponent(Building building)
    {
      building.IsRemoved = false;

      SelectedBuilding = building;

      AddComponent(component: building);
      _jobManager.Add(building.Job);
    }

    public void AddComponent(Component component)
    {
      component.LoadContent(_content);

      GameComponents.Add(component);
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

      var house = HouseComponents.Where(c => c.ResidentCount < SmallHouse.MaxResidents && (c.State == BuildingStates.Built)).FirstOrDefault();

      if (house == null)
        return;

      var doorPosition = house.DoorLocations.FirstOrDefault().Position;

      var pigAnimations = new Dictionary<string, Animation>()
      {
        { "WalkDown", new Animation(_content.Load<Texture2D>("Sprites/NPCs/PigWalkingDown"), 4) },
        { "WalkLeft", new Animation(_content.Load<Texture2D>("Sprites/NPCs/PigWalkingLeft"), 4) },
        { "WalkRight", new Animation(_content.Load<Texture2D>("Sprites/NPCs/PigWalkingRight"), 4) },
        { "WalkUp", new Animation(_content.Load<Texture2D>("Sprites/NPCs/PigWalkingUp"), 4) },
      };

      var butterflyAnimations = new Dictionary<string, Animation>()
      {
        { "WalkDown", new Animation(_content.Load<Texture2D>("Sprites/NPCs/ButterflyWalkingDown"), 4) },
        { "WalkLeft", new Animation(_content.Load<Texture2D>("Sprites/NPCs/ButterflyWalkingLeft"), 4) },
        { "WalkRight", new Animation(_content.Load<Texture2D>("Sprites/NPCs/ButterflyWalkingRight"), 4) },
        { "WalkUp", new Animation(_content.Load<Texture2D>("Sprites/NPCs/ButterflyWalkingUp"), 4) },
      };

      var chickenAnimations = new Dictionary<string, Animation>()
      {
        { "WalkDown", new Animation(_content.Load<Texture2D>("Sprites/NPCs/ChickenWalkingDown"), 4) },
        { "WalkLeft", new Animation(_content.Load<Texture2D>("Sprites/NPCs/ChickenWalkingLeft"), 4) },
        { "WalkRight", new Animation(_content.Load<Texture2D>("Sprites/NPCs/ChickenWalkingRight"), 4) },
        { "WalkUp", new Animation(_content.Load<Texture2D>("Sprites/NPCs/ChickenWalkingUp"), 4) },
      };

      var animations = new List<Dictionary<string, Animation>>()
      {
        //playerAnimations,
        pigAnimations,
        butterflyAnimations,
        chickenAnimations,
      };


      var npc = new NPC(animations[GameEngine.Random.Next(0, animations.Count)], this)
      {
        Position = doorPosition,
        IsCollidable = false,
        Layer = Building.DefaultLayer + 0.0075f,
        Home = house,
      };

      house.ResidentCount++;

      npc.LoadContent(_content);

      GameComponents.Add(npc);
      _villagerManager.Add(npc.Villager);
    }

    public override void Draw(GameTime gameTime)
    {
      //if (baseScreenSize == Vector2.Zero)
      //  SetGlobalTransform();

      //Console.Clear();
      //Console.WriteLine(Mouse.PositionWithCamera);

      // The "_renderTarget" is essentially a shot of the game. We draw to it, and then it-itself.
      //_graphicsDevice.SetRenderTarget(_renderTarget);

      _graphicsDevice.Clear(Color.Black);
      //penumbra.BeginDraw();

      _spriteBatch.Begin(
        SpriteSortMode.FrontToBack,
        BlendState.AlphaBlend,
        SamplerState.PointWrap, null, null, null,
       _camera.Transform);// * scale);

      foreach (var component in GameComponents)
        component.Draw(gameTime, _spriteBatch);


      _spriteBatch.End();

      //penumbra.Draw(gameTime);

      //_graphicsDevice.SetRenderTarget(null);

      //_graphicsDevice.Clear(Color.Black);

      //_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
      //            SamplerState.LinearClamp, DepthStencilState.None,
      //            RasterizerState.CullNone, null, _camera.Transform);

      //_spriteBatch.Draw(_renderTarget, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.White);

      //_spriteBatch.End();


      DrawGui(gameTime);
    }

    public void DrawGui(GameTime gameTime)
    {
      Window?.Draw(gameTime, _spriteBatch, _graphicsDeviceManager);

      _spriteBatch.Begin(SpriteSortMode.FrontToBack);

      var time = Time.ToString("hh:mm") + (Time.Hour >= 12 ? " pm" : " am");

      var x = GameEngine.ScreenWidth - _font.MeasureString(time).X - 10;

      _spriteBatch.DrawString(_font, time, new Vector2(x, 5), Color.Red);
      _spriteBatch.DrawString(_font, $"{NPCComponents.Count()}/{MaxNPCCount}", new Vector2(x, 25), Color.Red);

      foreach (var component in _guiComponents)
        component.Draw(gameTime, _spriteBatch);

      _toolbar.Draw(gameTime, _spriteBatch);

      _spriteBatch.End();
    }

    public GameScreen()
    {
    }

    public void Inspect(Building building)
    {
      InspectMenu.Building = building;
      State = GameStates.InspectMenu;
    }

    private void ItemMenuUpdate(GameTime gameTime)
    {
      Time = new DateTime(2018, 08, 10, 12, 0, 0, 0);

      foreach (var component in _guiComponents)
        component.Update(gameTime);

      foreach (var component in GameComponents)
        component.Update(gameTime);

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

      var timeStarted = DateTime.Now.TimeOfDay;

      Console.WriteLine("Loading");

      GameSpeed = 1f;

      _renderTarget = new RenderTarget2D(
        _graphicsDevice,
        _graphicsDevice.PresentationParameters.BackBufferWidth,
        _graphicsDevice.PresentationParameters.BackBufferHeight,
        false,
        _graphicsDevice.PresentationParameters.BackBufferFormat,
        DepthFormat.Depth24);

      _font = _content.Load<SpriteFont>("Fonts/Font");

      State = GameStates.Playing;

      PathFinder = new Pathfinder();

      _buildMenu = new BuildMenuWindow(this);

      JobMenu = new JobMenuWindow(this);

      InspectMenu = new InspectMenuWindow(this);

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

      Notifications = new Notifications();

      Resources = new VillageBackend.Models.Resources()
      {
        Food = 100,
        Gold = 100,
        Stone = 100,
        Wood = 100,
      };

      _itemManager = new ItemManager(Resources);

      _jobManager = new JobManager();

      _villagerManager = new VillagerManager();

      _toolbar = new GUITest.Interface.Toolbar(this, gameModel.ContentManger);

      GameComponents = new List<Component>()
      {
        //Player,
      };

      // The background
      var dot = new Texture2D(_graphicsDevice, 1, 1);
      dot.SetData(new Color[] { new Color(0, 200, 0), });

      GameComponents.Add(new Sprite(dot)
      {
        IsCollidable = false,
        IsEnabled = false, // So we don't update
        Position = new Vector2(0, 0),
        Scale = 10000,
      });

      // Doing this loads the window into memory
      Console.WriteLine("-->Windows");
      _windows = new List<Window>()
      {
        new CraftingWindow(_content, _itemManager),
        new JobsWindow(_content, _jobManager, _villagerManager),
      };

      Console.WriteLine("-->Map");

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
              case 2:
                GameComponents.Add(
                  new Sprite(texture)
                  {
                    IsCollidable = false,
                    Layer = 0.1f,
                    Position = position,
                    SourceRectangle = sourceRectangle,
                  }
                );
                break;

              case 8:
                GameComponents.Add(
                  new Rock(texture, this)
                  {
                    Layer = 0.1f,
                    Position = position,
                    SourceRectangle = sourceRectangle,
                  }
                );
                break;

              case 9:
                GameComponents.Add(
                  new Path(texture)
                  {
                    Layer = 0.1f,
                    Position = position,
                    SourceRectangle = sourceRectangle,
                  }
                );
                break;

              default:
                GameComponents.Add(
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
        new ResourceView(Resources),
        _buildMenu,
        InspectMenu,
        InventoryMenu,
        JobMenu,
        ItemMenu,
        Notifications,
        new GameSpeedController(),
      };

      foreach (var component in GameComponents)
        component.LoadContent(_content);

      foreach (var component in _guiComponents)
        component.LoadContent(_content);

      Console.WriteLine("-->Buildings");

      foreach (var objectGroup in map.ObjectGroups)
      {
        switch (objectGroup.Name)
        {
          case "Blacksmith":

            var blacksmith = new Blacksmith(this,
              _content.Load<Texture2D>("Buildings/Blacksmith/In"),
              _content.Load<Texture2D>("Buildings/Blacksmith/Out_Top"),
              _content.Load<Texture2D>("Buildings/Blacksmith/Out_Bottom"));

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

            blacksmith.State = BuildingStates.Built;
            GameComponents.Add(blacksmith);
            _jobManager.Add(blacksmith.Job);

            break;

          case "SmallHouse":

            var smallHouse = new SmallHouse(this,
              _content.Load<Texture2D>("Buildings/SmallHouse/In"),
              _content.Load<Texture2D>("Buildings/SmallHouse/Out_Top"),
              _content.Load<Texture2D>("Buildings/SmallHouse/Out_Bottom"));

            smallHouse.LoadContent(_content);

            foreach (var collisionObject in objectGroup.CollisionObjects)
            {
              var position = new Vector2(collisionObject.X, collisionObject.Y);

              switch (collisionObject.Name)
              {
                case "Building":

                  smallHouse.Position = position;

                  break;

                case "Bed":
                  var bed = new Bed(_content.Load<Texture2D>("Furniture/Bed"), this)
                  {
                    Position = position,
                    State = PlacableObjectStates.Placed,
                  };

                  bed.LoadContent(_content);

                  smallHouse.Components.Add(bed);

                  break;

                default:
                  throw new Exception("Unknown object: " + collisionObject.Name);
              }
            }

            smallHouse.State = BuildingStates.Built;
            GameComponents.Add(smallHouse);

            break;

          case "NPCs":

            foreach (var collisionObject in objectGroup.CollisionObjects)
            {
              var position = new Vector2(collisionObject.X, collisionObject.Y);

              var pigAnimations = new Dictionary<string, Animation>()
              {
                { "WalkDown", new Animation(_content.Load<Texture2D>("Sprites/NPCs/PigWalkingDown"), 4) },
                { "WalkLeft", new Animation(_content.Load<Texture2D>("Sprites/NPCs/PigWalkingLeft"), 4) },
                { "WalkRight", new Animation(_content.Load<Texture2D>("Sprites/NPCs/PigWalkingRight"), 4) },
                { "WalkUp", new Animation(_content.Load<Texture2D>("Sprites/NPCs/PigWalkingUp"), 4) },
              };

              var npc = new NPC(pigAnimations, this)
              {
                Position = position,
                IsCollidable = false,
                Layer = Building.DefaultLayer + 0.001f,
              };

              npc.LoadContent(_content);
              _villagerManager.Add(npc.Villager);

              GameComponents.Add(npc);
            }

            break;

          default:
            //throw new Exception("Unknown group: " + objectGroup.Name);
            break;
        }
      }

      Console.WriteLine("-->Updating Path");

      UpdateMap();

      var timeEnded = DateTime.Now.TimeOfDay;

      Console.WriteLine("Total load time: " + (timeEnded - timeStarted));
    }

    private void MenuUpdate(GameTime gameTime, Keys? menuKey)
    {
      foreach (var component in _guiComponents)
        component.Update(gameTime);

      _toolbar.Update(gameTime);
      Window?.Update(gameTime);

      if ((menuKey != null && Keyboard.IsKeyPressed(menuKey.Value)) ||
        Keyboard.IsKeyPressed(Keys.Escape))
      {
        State = GameStates.Playing;
        CloseWindow();
      }
    }

    private void PlacingBuildingUpdate(GameTime gameTime)
    {
      Time = new DateTime(2018, 08, 10, 12, 0, 0, 0);

      foreach (var component in _guiComponents)
        component.Update(gameTime);

      foreach (var component in GameComponents)
        component.Update(gameTime);

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
      Time = new DateTime(2018, 08, 10, 12, 0, 0, 0);

      foreach (var component in _guiComponents)
        component.Update(gameTime);

      foreach (var component in GameComponents)
        component.Update(gameTime);

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
      // Time = Time.AddSeconds(10 * GameSpeed);
      Time = new DateTime(2018, 08, 10, 12, 0, 0, 0);

      AddNPC();

      foreach (var component in _guiComponents)
        component.Update(gameTime);

      _toolbar.Update(gameTime);
      Window?.Update(gameTime);

      foreach (var component in GameComponents)
        component.Update(gameTime);

      if (Keyboard.IsKeyPressed(Keys.P))
        State = GameStates.Paused;

      if (Keyboard.IsKeyPressed(Keys.B))
        State = GameStates.BuildMenu;

      if (Keyboard.IsKeyPressed(Keys.J))
        State = GameStates.JobMenu;

      if (Keyboard.IsKeyPressed(Keys.C))
      {
        //var selectedItem = CraftingMenu.ComboBox.SelectedItem;
        //CraftingMenu.ComboBox.Reset();

        //var npcs = NPCComponents.Where(c => c.Workplace != null && c.Workplace.Name == "Blacksmith");

        //var items = new List<ComboBoxItem>();
        //foreach (var npc in npcs)
        //{
        //  var item = new ComboBoxItem(CraftingMenu.ComboBox);

        //  item.Content = npc;

        //  item.LoadContent(_content);

        //  item.Text = npc.Name;

        //  items.Add(item);
        //}

        //CraftingMenu.ComboBox.Items = items;

        //if (selectedItem != null)
        //{
        //  if (CraftingMenu.ComboBox.Items.Any(c => c.Text == selectedItem.Text))
        //  {
        //    CraftingMenu.ComboBox.SelectedItem = CraftingMenu.ComboBox.Items.Where(c => c.Text == selectedItem.Text).FirstOrDefault();
        //  }
        //}

        State = GameStates.CraftingMenu;

      }

      if (Keyboard.IsKeyPressed(Keys.I))
        State = GameStates.InventoryMenu;

      if (Keyboard.IsKeyPressed(Keys.Enter))
        MessageBox.Show("You just pressed Enter. Well done :)");

      if (Keyboard.IsKeyPressed(Keys.P))
        State = GameStates.Paused;
    }

    public override void PostUpdate(GameTime gameTime)
    {
      for (int i = 0; i < GameComponents.Count; i++)
      {
        GameComponents[i].PostUpdate(gameTime);

        if (GameComponents[i].IsRemoved)
        {
          GameComponents.RemoveAt(i);
          i--;
        }
      }
    }

    public override void OnScreenResize()
    {
      _toolbar.OnScreenResize();
      Window?.OnScreenResize();
    }

    public void SetGameSpeed()
    {
      // For the time being - the GameSpeed needs to be a multiple of 32

      if (Keyboard.IsKeyDown(Keys.D1))
      {
        GameSpeed = 1f;
        State = GameStates.Playing;
      }
      else if (Keyboard.IsKeyDown(Keys.D2))
      {
        GameSpeed = 2f;
        State = GameStates.Playing;
      }
      else if (Keyboard.IsKeyDown(Keys.D3))
      {
        GameSpeed = 4f;
        State = GameStates.Playing;
      }
    }

    public override void UnloadContent()
    {
      foreach (var component in GameComponents)
        component.UnloadContent();

      foreach (var component in _guiComponents)
        component.UnloadContent();

      GameComponents.Clear();

      _guiComponents.Clear();

      _toolbar.UnloadContent();
      Window?.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      if (Keyboard.IsKeyPressed(Keys.T))
        Notifications.Add(Time, "This is a test notification");

      _camera.Update(gameTime);

      SetGameSpeed();

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

        case GameStates.InspectMenu:

          MenuUpdate(gameTime, null);

          break;

        default:
          throw new Exception("Unknown state: " + State.ToString());
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
  }
}
