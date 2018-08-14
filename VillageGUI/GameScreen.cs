using Engine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VillageGUI.Interface;
using VillageGUI.Interface.Windows;
using Engine.Models;
using Engine.Interface.Windows;
using VillageBackend.Models;
using VillageBackend.Managers;
using VillageGUI.Interface.Buttons;
using Microsoft.Xna.Framework.Input;
using Engine.Input;

namespace VillageGUI
{
  public class GameScreen : State
  {
    private MouseState _currentMouseState;

    private MouseState _previousMouseState;

    private Toolbar _toolbar;

    private Resources _resources;

    private GameManagers _gameManagers;

    private List<Button> _buttons;

    public GameScreen()
    {
    }

    public override void LoadContent(GameModel gameModel)
    {
      base.LoadContent(gameModel);

      _resources = new Resources()
      {
        Food = int.MaxValue,
        Gold = int.MaxValue,
        Stone = int.MaxValue,
        Wood = int.MaxValue,
      };

      _gameManagers = new GameManagers();
      _gameManagers.ItemManager = new ItemManager(_gameManagers, _resources);
      _gameManagers.JobManager = new JobManager(_gameManagers);
      _gameManagers.VillagerManager = new VillagerManager(_gameManagers);

      // The reason I load windows like this is so that all of the loading is down at once, rather than when we first call a window
      _windows = new List<Window>()
      {
        new CraftingWindow(_content, _gameManagers.ItemManager),
        new JobsWindow(_content, _gameManagers),
      };

      _toolbar = new Toolbar(this, gameModel.ContentManger);

      var buttonTexture = _content.Load<Texture2D>("Interface/Button");
      var buttonFont = _content.Load<SpriteFont>("Fonts/Font");

      _buttons = new List<Button>()
      {
        new Button(buttonTexture, buttonFont)
        {
          Position = new Vector2(320, 280),
          Text = "Add building",
          Click = AddBuilding,
          Layer = 0.70f,
        },
        new Button(buttonTexture, buttonFont)
        {
          Position = new Vector2(320, 340),
          Text = "Add villager",
          Click = AddVillager,
          Layer = 0.70f,
        },
      };
    }

    private void AddVillager(object obj)
    {
      _gameManagers.VillagerManager.Add(new Villager());
    }

    private void AddBuilding(object obj)
    {
      _gameManagers.JobManager.Add(new Job() { Name = "BlackSmith" });
    }

    public override void OnScreenResize()
    {
      _toolbar.OnScreenResize();
      Window?.OnScreenResize();
    }

    public override void PostUpdate(GameTime gameTime)
    {

    }

    public override void UnloadContent()
    {
      _toolbar.UnloadContent();
      Window?.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      GameMouse.Update();

      var clicked = _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;
      
      foreach (var button in _buttons)
      {
        button.Update(GameMouse.Rectangle, _buttons);
      }

      _toolbar.Update(gameTime);

      Window?.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
      _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

      DrawButtons(gameTime);

      _toolbar.Draw(gameTime, _spriteBatch);

      _spriteBatch.End();

      Window?.Draw(gameTime, _spriteBatch, _graphicsDeviceManager);
    }

    private void DrawButtons(GameTime gameTime)
    {
      foreach (var button in _buttons)
      {
        switch (button.CurrentState)
        {
          case ButtonStates.Nothing:

            button.Color = Color.White;

            button.Scale = 1.0f;

            break;
          case ButtonStates.Hovering:

            button.Color = Color.YellowGreen;

            button.Scale = 1.0f;

            break;
          case ButtonStates.Clicked:

            // This will be removed

            button.Color = Color.YellowGreen;

            button.Scale = 1.2f;

            break;
          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }

        button.Draw(gameTime, _spriteBatch);
      }
    }
  }
}
