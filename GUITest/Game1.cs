using GUITest.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GUITest
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class Game1 : Game
  {
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    private Toolbar _toolbar;

    private Window _window;

    public static int ScreenHeight { get; private set; }

    public static int ScreenWidth { get; private set; }

    public Game1()
    {
      graphics = new GraphicsDeviceManager(this);
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
      IsMouseVisible = true;

      ScreenHeight = graphics.PreferredBackBufferHeight;
      ScreenWidth = graphics.PreferredBackBufferWidth;

      Window.AllowUserResizing = true;
      Window.ClientSizeChanged += Window_ClientSizeChanged;

      base.Initialize();
    }

    private void Window_ClientSizeChanged(object sender, System.EventArgs e)
    {
      graphics.PreferredBackBufferHeight = MathHelper.Clamp(graphics.PreferredBackBufferHeight, 480, 1400);
      graphics.PreferredBackBufferWidth = MathHelper.Clamp(graphics.PreferredBackBufferWidth, 800, 2560);
      graphics.ApplyChanges();

      ScreenHeight = graphics.PreferredBackBufferHeight;
      ScreenWidth = graphics.PreferredBackBufferWidth;

      _toolbar.OnScreenResize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      spriteBatch = new SpriteBatch(GraphicsDevice);

      _toolbar = new Toolbar(Content);

      _window = new Interface.Window(Content);
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
      _toolbar.Update(gameTime);

      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);

      var original = graphics.GraphicsDevice.Viewport;

      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

      _toolbar.Draw(gameTime, spriteBatch);

      spriteBatch.End();

      graphics.GraphicsDevice.Viewport = new Viewport(10, 10, 250, 360);

      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

      _window.Draw(gameTime, spriteBatch);

      spriteBatch.End();

      graphics.GraphicsDevice.Viewport = original;

      base.Draw(gameTime);
    }
  }
}
