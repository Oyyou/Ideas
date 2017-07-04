using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ideas.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using Ideas.Sprites;
using Ideas.Controls;

namespace Ideas.States
{
  public class MainMenu : State
  {
    private List<Component> _guiComponents;

    public override void Draw(GameTime gameTime)
    {
      _spriteBatch.Begin();

      foreach (var component in _guiComponents)
        component.Draw(gameTime, _spriteBatch);

      _spriteBatch.End();
    }

    public override void LoadContent(GameModel gameModel)
    {
      base.LoadContent(gameModel);

      var buttonTexture = gameModel.ContentManger.Load<Texture2D>("Controls/Button");
      var buttonFont = gameModel.ContentManger.Load<SpriteFont>("Fonts/Font");

      var quitButton = new Button(buttonTexture, buttonFont)
      {
        Position = new Vector2(325, 200),
        Text = "Quit",
      };
      quitButton.Click += QuitButton_Click;

      _guiComponents = new List<Component>()
      {
        quitButton,
      };
      
      foreach (var component in _guiComponents)
        component.LoadContent(_content);
    }

    public MainMenu()
    {

    }

    private void QuitButton_Click(object sender, EventArgs e)
    {
      _game.Exit();
    }

    public override void UnloadContent()
    {
      foreach (var component in _guiComponents)
        component.UnloadContent();

      _guiComponents.Clear();
    }

    public override void Update(GameTime gameTime)
    {
      foreach (var component in _guiComponents)
        component.Update(gameTime);
    }
  }
}
