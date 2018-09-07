using CombatTest.Screens;
using Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using VillageBackend.Managers;

namespace CombatTest
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class Game1 : GameEngine
  {
    private GameManagers _gameManagers;

    public Game1()
    {
      Random = new Random();

      _graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      var height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2;

      var width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2;

      _graphics.PreferredBackBufferHeight = Math.Max(height, 480);

      _graphics.PreferredBackBufferWidth = Math.Max(width, 800);

      _graphics.ApplyChanges();

      ScreenHeight = _graphics.PreferredBackBufferHeight;

      ScreenWidth = _graphics.PreferredBackBufferWidth;

      ScreenRectangle = new Rectangle(0, 0, ScreenWidth, ScreenHeight);

      Window.ClientSizeChanged += Window_ClientSizeChanged;

      Window.AllowUserResizing = false;

      IsMouseVisible = true;

      // sets fps to monitor Hz 
      //_graphics.SynchronizeWithVerticalRetrace = false;
      //this.IsFixedTimeStep = false;

      base.Initialize();
    }

    private void Window_ClientSizeChanged(object sender, System.EventArgs e)
    {
      _graphics.PreferredBackBufferHeight = MathHelper.Clamp(_graphics.PreferredBackBufferHeight, 480, 1400);
      _graphics.PreferredBackBufferWidth = MathHelper.Clamp(_graphics.PreferredBackBufferWidth, 800, 2560);
      _graphics.ApplyChanges();

      ScreenHeight = _graphics.PreferredBackBufferHeight;
      ScreenWidth = _graphics.PreferredBackBufferWidth;

      ScreenRectangle = new Rectangle(0, 0, ScreenWidth, ScreenHeight);

      _currentState.OnScreenResize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      _gameModel = new Engine.Models.GameModel()
      {
        ContentManger = Content,
        Game = this,
        GraphicsDeviceManager = _graphics,
        SpriteBatch = _spriteBatch,
      };
      
      _gameManagers = new GameManagers();
      _gameManagers.SquadManager = new SquadManager(_gameManagers);
      _gameManagers.VillagerManager = new VillagerManager(_gameManagers);

      _gameManagers.VillagerManager.Add(new VillageBackend.Models.Villager() { Speed = 3, });
      _gameManagers.VillagerManager.Add(new VillageBackend.Models.Villager() { Speed = 3, });
      _gameManagers.VillagerManager.Add(new VillageBackend.Models.Villager() { Speed = 3, });
      _gameManagers.VillagerManager.Add(new VillageBackend.Models.Villager() { Speed = 3, });

      _gameManagers.SquadManager.Add("Beta",
        _gameManagers.VillagerManager.Villagers[0],
        _gameManagers.VillagerManager.Villagers[1],
        _gameManagers.VillagerManager.Villagers[2],
        _gameManagers.VillagerManager.Villagers[3]);
      
      _currentState = new CombatScreen(_gameManagers.SquadManager.Squads[0]);

      _currentState.LoadContent(_gameModel);
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: Unload any non ContentManager content here
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      _currentState.Update(gameTime);

      _currentState.PostUpdate(gameTime);

      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      _currentState.Draw(gameTime);

      base.Draw(gameTime);
    }
  }
}
