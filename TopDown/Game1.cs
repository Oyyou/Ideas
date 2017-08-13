using Engine;
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

    // Create a new render target
    public RenderTarget2D renderTarget;
    private Effect pPEffect;

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

      ScreenHeight = _graphics.PreferredBackBufferHeight;

      ScreenWidth = _graphics.PreferredBackBufferWidth;

      renderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);

      base.Initialize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      _gameModel = new Engine.Models.GameModel(Content, this, _graphics.GraphicsDevice, _spriteBatch);

      _currentState = new GameScreen();
      _currentState.LoadContent(_gameModel);

      pPEffect = Content.Load<Effect>("Effect/BlackAndWhite");
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
      var gs = (GameScreen)_currentState;
      var time = gs.Time;
      float someMaths = (float)Math.Sin((-MathHelper.PiOver2 + 2*Math.PI*time.Hour)/48);
      float DarknessLevel = Math.Abs(MathHelper.SmoothStep(12f, 2f, someMaths));

      pPEffect.Parameters["DarknessLevel"].SetValue(DarknessLevel);

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
      DrawSceneToTexture(renderTarget, gameTime);

      GraphicsDevice.Clear(Color.Black);

      _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                  SamplerState.LinearClamp, DepthStencilState.Default,
                  RasterizerState.CullNone,pPEffect);

      _spriteBatch.Draw(renderTarget, new Rectangle(0, 0, 800, 480), Color.White);

      _spriteBatch.End();
      
      var curstate = _currentState as GameScreen;
      curstate.DrawGui(gameTime);

      base.Draw(gameTime);
    }

    /// <summary>
    /// Draws the entire scene in the given render target.
    /// </summary>
    /// <returns>A texture2D with the scene drawn in it.</returns>
    protected void DrawSceneToTexture(RenderTarget2D renderTarget, GameTime gameTime)
    {
      // Set the render target
      GraphicsDevice.SetRenderTarget(renderTarget);

      GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
 
      GraphicsDevice.Clear(Color.Green);

      _currentState.Draw(gameTime);

      // Drop the render target
      GraphicsDevice.SetRenderTarget(null);
    }
  }
}
