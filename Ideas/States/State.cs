using Ideas.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ideas.States
{
  public abstract class State
  {
    #region Fields

    protected ContentManager _content
    {
      get { return _gameModel.ContentManger; }
    }

    protected Game1 _game
    {
      get { return _gameModel.Game; }
    }

    protected GameModel _gameModel { get; private set; }

    protected GraphicsDevice _graphicsDevice
    {
      get { return _gameModel.GraphicsDevice; }
    }

    protected SpriteBatch _spriteBatch
    {
      get { return _gameModel.SpriteBatch; }
    }

    #endregion

    #region Methods

    public abstract void Draw(GameTime gameTime);

    public virtual void LoadContent(GameModel gameModel)
    {
      _gameModel = gameModel;
    }

    public State() { }

    public abstract void UnloadContent();

    public abstract void Update(GameTime gameTime);
    #endregion
  }
}
