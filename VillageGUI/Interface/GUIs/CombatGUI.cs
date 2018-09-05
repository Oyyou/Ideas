using Engine;
using Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBackend.Models;
using VillageGUI.Interface.Buttons;
using VillageGUI.Interface.Combat;

namespace VillageGUI.Interface.GUIs
{
  public class CombatGUI
  {
    private Button _endTurnButton;

    private HeroPanel _heroPanel;

    public Action<Button> EndTurnClick;

    public int SelectedHeroIndex
    {
      get
      {
        return _heroPanel.SelectedHeroIndex;
      }
    }

    public CombatGUI(Squad squad)
    {
      _heroPanel = new HeroPanel(squad);
    }

    public void LoadContent(ContentManager content)
    {
      _endTurnButton = new Button(content.Load<Texture2D>("Interface/EndTurn"))
      {
        Layer = 1f,
        Click = EndTurnClick,
      };

      _heroPanel.LoadContent(content);

      SetPositions();
    }

    public void Clear()
    {
      _heroPanel.Clear();
    }

    public void UnloadContent()
    {
      _endTurnButton.UnloadContent();
      _heroPanel.UnloadContent();
    }

    private void SetPositions()
    {
      var screenWidth = GameEngine.ScreenWidth;
      var screenHeight = GameEngine.ScreenHeight;

      _endTurnButton.Position = new Vector2(screenWidth - _endTurnButton.Origin.X - 10, screenHeight - _endTurnButton.Origin.Y - 10);

      _heroPanel.SetPositions();
    }

    public void OnScreenResize()
    {
      SetPositions();
    }

    public void Update(GameTime gameTime)
    {
      _endTurnButton.Update(GameMouse.Rectangle, new List<Button>() { _endTurnButton });

      _heroPanel.Update(gameTime);

      if (_endTurnButton.Clicked)
        Clear();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(SpriteSortMode.FrontToBack);

      _endTurnButton.Draw(gameTime, spriteBatch);

      _heroPanel.Draw(gameTime, spriteBatch);

      spriteBatch.End();
    }
  }
}
