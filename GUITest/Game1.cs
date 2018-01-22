using GUITest.Interface;
using GUITest.Interface.Windows;
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

    public bool IsWindowOpen
    {
      get
      {
        return _window != null;
      }
    }

    public static int ScreenHeight { get; private set; }

    public static int ScreenWidth { get; private set; }

    public static Rectangle ScreenRectangle { get; private set; }

    public Rectangle WindowRectangle
    {
      get
      {
        return _window != null ? _window.Rectangle : new Rectangle(0, 0, 0, 0);
      }
    }

    public void CloseWindow()
    {
      _window = null;
    }

    public Game1()
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";

      //graphics.PreferredBackBufferWidth = 1280;
      //graphics.PreferredBackBufferHeight = 720;
      //graphics.ApplyChanges();
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

      ScreenRectangle = new Rectangle(0, 0, ScreenWidth, ScreenHeight);

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

      ScreenRectangle = new Rectangle(0, 0, ScreenWidth, ScreenHeight);

      _toolbar.OnScreenResize();
      _window?.OnScreenResize();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      spriteBatch = new SpriteBatch(GraphicsDevice);

      _toolbar = new Toolbar(this, Content);

      //_window = new Interface.Window(Content);
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

      _window?.Update(gameTime);

      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.CornflowerBlue);
      
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

      _toolbar.Draw(gameTime, spriteBatch);

      spriteBatch.End();

      _window?.Draw(gameTime, spriteBatch, graphics);

      base.Draw(gameTime);
    }

    public void OpenWindow(Window window)
    {
      _window = window;
    }
  }
}
