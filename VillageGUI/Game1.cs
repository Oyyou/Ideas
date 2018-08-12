using Engine;
using Engine.Models;
using VillageGUI.Interface;
using VillageGUI.Interface.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using VillageBackend.Models;
using static VillageBackend.Enums;

namespace VillageGUI
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class Game1 : GameEngine
  {
    private GameScreen _gameScreen;

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
      Window.AllowUserResizing = true;

      var height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2;

      var width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2;

      _graphics.PreferredBackBufferHeight = Math.Max(height, 480);

      _graphics.PreferredBackBufferWidth = Math.Max(width, 800);

      _graphics.ApplyChanges();

      ScreenHeight = _graphics.PreferredBackBufferHeight;

      ScreenWidth = _graphics.PreferredBackBufferWidth;

      ScreenRectangle = new Rectangle(0, 0, ScreenWidth, ScreenHeight);

      Window.ClientSizeChanged += Window_ClientSizeChanged;

      IsMouseVisible = true;

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

      _gameScreen.OnScreenResize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      _gameModel = new GameModel()
      {
        ContentManger = Content,
        Game = this,
        GraphicsDeviceManager = _graphics,
        SpriteBatch = _spriteBatch,
      };
      
      _gameScreen = new GameScreen();
      _gameScreen.LoadContent(_gameModel);
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {
      _gameScreen.UnloadContent();
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      _gameScreen.Update(gameTime);

      _gameScreen.PostUpdate(gameTime);

      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      _gameScreen.Draw(gameTime);

      base.Draw(gameTime);
    }
  }
}
