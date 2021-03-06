﻿using Engine;
using Engine.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using TopDown.States;

namespace TopDown
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class Game1 : GameEngine
  {
    public Game1()
    {
      Random = new System.Random();

      _graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";

      //Window.AllowUserResizing = true;
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
      //IsMouseVisible = true;
      //base.IsFixedTimeStep = false;

      //this.graphics.SynchronizeWithVerticalRetrace = false;
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

      base.Initialize();
    }

    private void Window_ClientSizeChanged(object sender, EventArgs e)
    {
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

      _currentState = new GameScreen();

      _currentState.LoadContent(_gameModel);
    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// game-specific content.
    /// </summary>
    protected override void UnloadContent()
    {
      _currentState.UnloadContent();
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
