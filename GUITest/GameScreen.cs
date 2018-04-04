using Engine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GUITest.Interface;
using GUITest.Interface.Windows;
using Engine.Models;
using Engine.Interface.Windows;
using VillageBackend.Models;

namespace GUITest
{
  public class GameScreen : State
  {
    private Toolbar _toolbar;

    public GameScreen()
    {
    }

    public override void Draw(GameTime gameTime)
    {
      _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

      _toolbar.Draw(gameTime, _spriteBatch);

      _spriteBatch.End();

      _window?.Draw(gameTime, _spriteBatch, _graphicsDeviceManager);
    }

    public override void LoadContent(GameModel gameModel)
    {
      base.LoadContent(gameModel);

      _toolbar = new Toolbar(this, new VillageBackend.Managers.ItemManager(new Resources()), gameModel.ContentManger);
    }

    public override void OnScreenResize()
    {
      _toolbar.OnScreenResize();
      _window?.OnScreenResize();
    }

    public override void PostUpdate(GameTime gameTime)
    {

    }

    public override void UnloadContent()
    {
      _toolbar.UnloadContent();
      _window?.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      _toolbar.Update(gameTime);

      _window?.Update(gameTime);
    }
  }
}
