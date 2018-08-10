using Engine.Interface.Windows;
using Engine.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.States
{
  public abstract class State
  {
    #region Fields

    protected ContentManager _content
    {
      get { return _gameModel.ContentManger; }
    }

    protected GameEngine _game
    {
      get { return _gameModel.Game; }
    }

    protected GameModel _gameModel { get; private set; }

    protected GraphicsDevice _graphicsDevice
    {
      get { return _graphicsDeviceManager.GraphicsDevice; }
    }

    protected GraphicsDeviceManager _graphicsDeviceManager
    {
      get { return _gameModel.GraphicsDeviceManager; }
    }

    protected SpriteBatch _spriteBatch
    {
      get { return _gameModel.SpriteBatch; }
    }

    public Window Window { get; protected set; }

    protected List<Window> _windows;

    public bool IsWindowOpen
    {
      get
      {
        return Window != null;
      }
    }

    public Rectangle WindowRectangle
    {
      get
      {
        return Window != null ? Window.WindowRectangle : new Rectangle(0, 0, 0, 0);
      }
    }

    #endregion

    /// <summary>
    /// How quickly everything goes
    /// </summary>
    public static float GameSpeed { get; protected set; }

    #region Methods

    public void CloseWindow()
    {
      if (Window == null)
        return;

      if (_windows.Any(c => c.GetType() == Window.GetType()))
      {
        for (int i = 0; i < _windows.Count; i++)
        {
          if (_windows[i].GetType() == Window.GetType())
          {
            _windows[i] = Window;
            break;
          }
        }
      }
      else
      {
        _windows.Add(Window.Clone() as Window);
      }

      Window = null;
    }

    public abstract void Draw(GameTime gameTime);

    public virtual void LoadContent(GameModel gameModel)
    {
      _gameModel = gameModel;

      _windows = new List<Window>();
    }

    public abstract void OnScreenResize();

    public void OpenWindow(string name)
    {
      var window = _windows.Where(c => c.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

      Window = window ?? throw new Exception($"Window '{name}' doesn't exist");
    }

    public abstract void PostUpdate(GameTime gameTime);

    public State() { }

    public abstract void UnloadContent();

    public abstract void Update(GameTime gameTime);
    #endregion
  }
}
