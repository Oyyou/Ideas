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
using Penumbra;

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
  }

  public class GameScreen : State
  {
    private BuildMenuWindow _buildMenu;

    private Camera _camera;

    private Effect _effect;

    private SpriteFont _font;

    private List<Component> _guiComponents;

    private RenderTarget2D _renderTarget;

    private PenumbraComponent penumbra;
    Vector2 baseScreenSize = Vector2.Zero;
    private Matrix globalTransformation;

    List<Hull> HullList = new List<Hull>();
    Hull hull;

    public IEnumerable<Building> BuildingComponents
    {
      get
      {
        return GameComponents.Where(c => c is Building).Cast<Building>()
          .Where(c => c.State == BuildingStates.Built_In || c.State == BuildingStates.Built_Out);
      }
    }

    public CraftingMenuWindow CraftingMenu;

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
        return GameComponents.Where(c => c is SmallHouse && (((SmallHouse)c).State == BuildingStates.Built_In || ((SmallHouse)c).State == BuildingStates.Built_Out)).Count() * SmallHouse.MaxResidents;
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

    public Models.Resources Resources { get; set; }

    public Building SelectedBuilding { get; set; }

    public GameStates State { get; set; }

    public DateTime Time { get; set; }

    public List<Building> Workplaces
    {
      get
      {
        var components = BuildingComponents.Where(c => c is Blacksmith || c is Tavern || c is Mine).Cast<Building>().ToList();

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

      var house = HouseComponents.Where(c => c.ResidentCount < SmallHouse.MaxResidents && (c.State == BuildingStates.Built_In || c.State == BuildingStates.Built_Out)).FirstOrDefault();

      if (house == null)
        return;

      var doorPosition = house.DoorLocations.FirstOrDefault().Position;

      var npc = new NPC(playerAnimations, this)
      {
        Position = doorPosition,
        IsCollidable = false,
        Layer = Building.DefaultLayer + 0.0075f,
        Home = house,
      };

      house.ResidentCount++;

      npc.LoadContent(_content);

      GameComponents.Add(npc);
    }
    

    public override void Draw(GameTime gameTime)
    {
      if(baseScreenSize == Vector2.Zero)
        SetGlobalTransform();

      // Set the render target
      _graphicsDevice.SetRenderTarget(_renderTarget);

      //_graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
      _graphicsDevice.Clear(Color.Black);
      penumbra.BeginDraw();
      //var scale = Matrix.CreateScale(Game1.ScreenWidth / (float)Game1.ScreenHeight, Game1.ScreenWidth / (float)Game1.ScreenHeight, 1);

      _spriteBatch.Begin(
        SpriteSortMode.FrontToBack,
        BlendState.AlphaBlend,
        SamplerState.PointWrap, null, null, null,
       globalTransformation);// * scale);

      foreach (var component in GameComponents)
        component.Draw(gameTime, _spriteBatch);
      

      _spriteBatch.End();
      // Draw the actual lit scene.
      penumbra.Draw(gameTime);

      // Drop the render target
      _graphicsDevice.SetRenderTarget(null);

    

      _graphicsDevice.Clear(Color.Black);

      _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                  SamplerState.LinearClamp, DepthStencilState.None,
                  RasterizerState.CullNone, _effect,  _camera.Transform);

      _spriteBatch.Draw(_renderTarget, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.White);
     
      _spriteBatch.End();


      DrawGui(gameTime);
    }

    public void DrawGui(GameTime gameTime)
    {
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

      foreach (var component in GameComponents)
        component.Update(gameTime);

      for (int i = 0; i < GameComponents.Count; i++)
      {
        for (int j = i + 1; j < GameComponents.Count; j++)
        {
          GameComponents[i].CheckCollision(GameComponents[j]);
        }
      }

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

      _renderTarget = new RenderTarget2D(
        _graphicsDevice,
        _graphicsDevice.PresentationParameters.BackBufferWidth,
        _graphicsDevice.PresentationParameters.BackBufferHeight,
        false,
        _graphicsDevice.PresentationParameters.BackBufferFormat,
        DepthFormat.Depth24);

      //_effect = _content.Load<Effect>("Effect/BlackAndWhite");

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

      Notifications = new Notifications();

      Resources = new Models.Resources();

      var playerAnimations = new Dictionary<string, Animation>()
      {
        { "WalkDown", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkDown"), 4) },
        { "WalkLeft", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkLeft"), 4) },
        { "WalkRight", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkRight"), 4) },
        { "WalkUp", new Animation(_content.Load<Texture2D>("Sprites/Player/WalkUp"), 4) },
      };

      //Player = new TopDown.Sprites.Player(playerAnimations)
      //{
      //  Layer = 0.9f,
      //  Position = new Vector2(512, 320),
      //};

      GameComponents = new List<Component>()
      {
        //Player,
      };

      var dot = new Texture2D(_graphicsDevice, 1, 1);
      dot.SetData(new Color[] { new Color(0, 200, 0), });

      //for (int y = 0; y < 10; y++)
      //{
      //  for (int x = 0; x < 100; x++)
      //  {
      GameComponents.Add(new Sprite(dot)
      {
        IsCollidable = false,
        IsEnabled = false, // So we don't update
        Position = new Vector2(0, 0),
        Scale = 10000,
      });
      //  }
      //}

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
        //new BottomToolbar(this),
        new ResourceView(Resources),
        _buildMenu,
        CraftingMenu,
        InventoryMenu,
        JobMenu,
        ItemMenu,
        Notifications,
      };

      foreach (var component in GameComponents)
        component.LoadContent(_content);

      foreach (var component in _guiComponents)
        component.LoadContent(_content);

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

          blacksmith.State = BuildingStates.Built_Out;
          GameComponents.Add(blacksmith);

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
              var bed = new Furniture(_content.Load<Texture2D>("Furniture/Bed"), this)
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

          smallHouse.State = BuildingStates.Built_Out;
          GameComponents.Add(smallHouse);

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

            GameComponents.Add(npc);
          }

          break;

          default:
          //throw new Exception("Unknown group: " + objectGroup.Name);
          break;
        }
      }

      UpdateMap();


      // Create our lighting component and register it as a service so that subsystems can access it.--------------
      penumbra = new PenumbraComponent(_game)
      {
        AmbientColor = new Color(new Vector3(0.1f))
      };
      // Create sample light source and shadow hull.
      Light light = new PointLight
      {
        Color = new Color(255,140,0),
        Scale = new Vector2(360), // Range of the light source (how far the light will travel)
        ShadowType = ShadowType.Solid, // Will not lit hulls themselves
        Position = new Vector2(550, 240),
        Intensity = 1.5f

      };

      Light light2 = new PointLight
      {
        Scale = new Vector2(360), // Range of the light source (how far the light will travel)
        ShadowType = ShadowType.Solid, // Will not lit hulls themselves
        
      };

      hull = new Hull(new Vector2(1.0f), new Vector2(-1.0f, 1.0f), new Vector2(-1.0f), new Vector2(1.0f, -1.0f))
      {
        Position = new Vector2(400f, 400f),
        Scale = new Vector2(50)
      };


      penumbra.Hulls.Add(hull);

      hull = new Hull(new Vector2(1.0f), new Vector2(-1.0f, 1.0f), new Vector2(-1.0f), new Vector2(1.0f, -1.0f))
      {
        Position = new Vector2(400f, 600f),
        Scale = new Vector2(50)
      };


      penumbra.Hulls.Add(hull);

      hull = new Hull(new Vector2(1.0f), new Vector2(-1.0f, 1.0f), new Vector2(-1.0f), new Vector2(1.0f, -1.0f))
      {
        Position = new Vector2(600f, 600f),
        Scale = new Vector2(50)
      };


      penumbra.Hulls.Add(hull);

      hull = new Hull(new Vector2(1.0f), new Vector2(-1.0f, 1.0f), new Vector2(-1.0f), new Vector2(1.0f, -1.0f))
      {
        Position = new Vector2(400f, 750f),
        Scale = new Vector2(50)
      };


      penumbra.Hulls.Add(hull);

      hull = new Hull(new Vector2(1.0f), new Vector2(-1.0f, 1.0f), new Vector2(-1.0f), new Vector2(1.0f, -1.0f))
      {
        Position = new Vector2(400f, 900f),
        Scale = new Vector2(50)
      };

      //create all the hulls unsing the CollisionRectangles
      foreach (var collisionRectangle in GameComponents.SelectMany(a=> a.CollisionRectangles))
      {
        Hull tempHull = new Hull(new Vector2(collisionRectangle.Right, collisionRectangle.Top), new Vector2(collisionRectangle.Left, collisionRectangle.Top), new Vector2(collisionRectangle.Right, collisionRectangle.Bottom), new Vector2(collisionRectangle.Left, collisionRectangle.Bottom))
        {
          Position = collisionRectangle.Location.ToVector2(),
          Scale = new Vector2(collisionRectangle.Width*collisionRectangle.Height)

        };

        HullList.Add(tempHull);
      }


      penumbra.Lights.Add(light);
      penumbra.Lights.Add(light2);
      penumbra.Hulls.Add(hull);
      penumbra.Hulls.AddRange(HullList);
      _game.Services.AddService(penumbra);
      //-----------------------------------------------------------------------------------------------------------

      // Load penumbra
      penumbra.Initialize();

    }

    private void SetGlobalTransform()
    {
      baseScreenSize = new Vector2(_graphicsDevice.DisplayMode.Width, _graphicsDevice.DisplayMode.Height);
      //-----------------------------------------------------------------------------------------------------------
      //Work out how much we need to scale our graphics to fill the screen
      float horScaling = _graphicsDevice.PresentationParameters.BackBufferWidth / baseScreenSize.X;
      float verScaling = _graphicsDevice.PresentationParameters.BackBufferHeight / baseScreenSize.Y;
      Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);
      globalTransformation = Matrix.CreateScale(screenScalingFactor);
      penumbra.Transform = globalTransformation;
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

    private void PlacingBuildingUpdate(GameTime gameTime)
    {
      Time = Time.AddSeconds(30);

      foreach (var component in _guiComponents)
        component.Update(gameTime);

      foreach (var component in GameComponents)
        component.Update(gameTime);

      foreach (var building in BuildingComponents)
      {
        building.State = BuildingStates.Built_In;
      }

      for (int i = 0; i < GameComponents.Count; i++)
      {
        for (int j = i + 1; j < GameComponents.Count; j++)
        {
          GameComponents[i].CheckCollision(GameComponents[j]);
        }
      }

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

      foreach (var component in GameComponents)
        component.Update(gameTime);

      foreach (var building in BuildingComponents)
      {
        building.State = BuildingStates.Built_In;
      }

      for (int i = 0; i < GameComponents.Count; i++)
      {
        for (int j = i + 1; j < GameComponents.Count; j++)
        {
          GameComponents[i].CheckCollision(GameComponents[j]);
        }
      }

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

      foreach (var component in GameComponents)
        component.Update(gameTime);

      for (int i = 0; i < GameComponents.Count; i++)
      {
        for (int j = i + 1; j < GameComponents.Count; j++)
        {
          GameComponents[i].CheckCollision(GameComponents[j]);
        }
      }

      if (Keyboard.IsKeyPressed(Keys.P))
        State = GameStates.Paused;

      if (Keyboard.IsKeyPressed(Keys.B))
        State = GameStates.BuildMenu;

      if (Keyboard.IsKeyPressed(Keys.J))
        State = GameStates.JobMenu;

      if (Keyboard.IsKeyPressed(Keys.C))
      {
        var selectedItem = CraftingMenu.ComboBox.SelectedItem;
        CraftingMenu.ComboBox.Reset();

        var npcs = NPCComponents.Where(c => c.Workplace != null && c.Workplace.Name == "Blacksmith");

        var items = new List<ComboBoxItem>();
        foreach (var npc in npcs)
        {
          var item = new ComboBoxItem(CraftingMenu.ComboBox);

          item.Content = npc;

          item.LoadContent(_content);

          item.Text = npc.Name;

          items.Add(item);
        }

        CraftingMenu.ComboBox.Items = items;

        if (selectedItem != null)
        {
          if (CraftingMenu.ComboBox.Items.Any(c => c.Text == selectedItem.Text))
          {
            CraftingMenu.ComboBox.SelectedItem = CraftingMenu.ComboBox.Items.Where(c => c.Text == selectedItem.Text).FirstOrDefault();
          }
        }

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

    public override void UnloadContent()
    {
      foreach (var component in GameComponents)
        component.UnloadContent();

      foreach (var component in _guiComponents)
        component.UnloadContent();

      GameComponents.Clear();

      _guiComponents.Clear();
    }

    public Vector2 GetMouseWithCameraPosition()
    {
      Matrix inverseTransform = Matrix.Invert(globalTransformation);

      return Vector2.Transform(new Vector2(Mouse.PositionWithCamera.X, Mouse.PositionWithCamera.Y), inverseTransform);
    }
    
    public override void Update(GameTime gameTime)
    {
      if (Keyboard.IsKeyPressed(Keys.T))
        Notifications.Add(Time, "This is a test notification");

<<<<<<< HEAD
      _camera.Update();

=======

      


      penumbra.Lights[1].Position = GetMouseWithCameraPosition();

      double randomNumber = new Random().NextDouble();
      penumbra.Lights[0].Intensity = MathHelper.Lerp(0, 1.5f, (float)randomNumber);

      
>>>>>>> origin/master
      // TODO: Might be an idea to 'hard-code' the darkness for different times of the day.
      //  I can't see how maths can be used for daylight. Yes.
      float someMaths = (float)Math.Sin((-MathHelper.PiOver2 + 2 * Math.PI * (Time.Hour + (Time.Minute / 60))) / 48);
      float DarknessLevel = Math.Abs(MathHelper.SmoothStep(12f, 2f, someMaths));
<<<<<<< HEAD

      //var Position = new Vector2(50, 50);
      //var Scale = new Vector2(50);
      //var Origin = new Vector2(50.0f, 50.5f);
      //var Rotation = MathHelper.Pi - MathHelper.PiOver2 * 0.75f;
      //Matrix LocalToWorld;
      //Transform(ref Position, ref Origin, ref Scale, Rotation, out LocalToWorld);
      //var cvp = Matrix.Identity;
      //Matrix wvp;
      //Matrix.Multiply(ref LocalToWorld, ref cvp, out wvp);

      //_effect.Parameters["WorldViewProjection"].SetValue(wvp);
      //_effect.Parameters["LightColor"].SetValue(new Vector3(1, 1, 1));
      //_effect.Parameters["LightIntensity"].SetValue(1.5f);
      // _effect.Parameters["DarknessLevel"].SetValue(1f);
=======
      
>>>>>>> origin/master

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
