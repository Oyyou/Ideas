using Engine.Models;
using Engine.Sprites;
using Engine.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class GameEngine : Game
  {
    protected State _currentState;

    protected GameModel _gameModel;

    protected GraphicsDeviceManager _graphics;

    protected State _nextState;

    protected SpriteBatch _spriteBatch;

    public static Random Random;

    public static int ScreenHeight { get; protected set; }

    public static int ScreenWidth { get; protected set; }

    public static Rectangle ScreenRectangle { get; protected set; }

    public void ChangeState(State state)
    {
      _nextState = state;
    }
  }
}
