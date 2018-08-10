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
using VillageBackend.Managers;
using GUITest.Interface.Buttons;
using Microsoft.Xna.Framework.Input;

namespace GUITest
{
  public class GameScreen : State
  {
    private MouseState _currentMouseState;

    private MouseState _previousMouseState;

    private Toolbar _toolbar;

    private Resources _resources;

    #region Managers

    private ItemManager _itemManager;

    private JobManager _jobManager;

    private VillagerManager _villagerManager;

    #endregion

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

      _itemManager = new ItemManager(_resources);

      _jobManager = new JobManager();

      _villagerManager = new VillagerManager();

      // The reason I load windows like this is so that all of the loading is down at once, rather than when we first call a window
      _windows = new List<Window>()
      {
        new CraftingWindow(_content, _itemManager),
        new JobsWindow(_content, _jobManager, _villagerManager),
      };

      _toolbar = new Toolbar(this, gameModel.ContentManger);

      var buttonTexture = _content.Load<Texture2D>("Interface/Button");
      var buttonFont = _content.Load<SpriteFont>("Fonts/Font");

      _buttons = new List<Button>()
      {
        new Button(buttonTexture, buttonFont)
        {
          Position = new Vector2(100, 50),
          Text = "Add building",
          Click = AddBuilding
        },
        new Button(buttonTexture, buttonFont)
        {
          Position = new Vector2(100, 110),
          Text = "Add villager",
          Click = AddVillager
        },
      };
    }

    private void AddVillager(object obj)
    {
      _villagerManager.Add(new Villager());
    }

    private void AddBuilding(object obj)
    {
      _jobManager.Add(new Job() { Name = "BlackSmith", VillagerId = null, BuildingId = 1 });
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
      _previousMouseState = _currentMouseState;
      _currentMouseState = Mouse.GetState();

      var clicked = _currentMouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;

      var mouseRectangle = new Rectangle(_currentMouseState.X, _currentMouseState.Y, 1, 1);

      foreach (var button in _buttons)
      {
        switch (button.CurrentState)
        {
          case ButtonStates.Nothing:

            if (mouseRectangle.Intersects(button.Rectangle))
              button.CurrentState = ButtonStates.Hovering;

            break;
          case ButtonStates.Hovering:

            if (!mouseRectangle.Intersects(button.Rectangle))
              button.CurrentState = ButtonStates.Nothing;

            if (clicked)
            {
              foreach (var b in _buttons)
                b.CurrentState = ButtonStates.Nothing;

              button.OnClick();
            }

            break;

          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }
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
