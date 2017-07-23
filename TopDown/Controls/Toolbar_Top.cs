using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Sprites;
using Engine.Controls;
using TopDown.States;
using Microsoft.Xna.Framework.Input;

namespace TopDown.Controls
{
  public class Toolbar_Top : Component
  {
    private List<Button> _icons;

    private GameScreen _gameScreen;

    private Sprite _toolbarSprite;

    private void BuildButton_Click(object sender, EventArgs e)
    {
      _gameScreen.State = GameStates.BuildMenu;
    }

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameScreen.State != GameStates.Playing)
        return;

      _toolbarSprite.Draw(gameTime, spriteBatch);

      foreach (var icon in _icons)
        icon.Draw(gameTime, spriteBatch);
    }

    private void JobsButton_Click(object sender, EventArgs e)
    {
      _gameScreen.State = GameStates.JobMenu;
    }

    public override void LoadContent(ContentManager content)
    {
      _toolbarSprite = new Sprite(content.Load<Texture2D>("Controls/Toolbar_Top"));
      _toolbarSprite.Position = new Vector2((GameEngine.ScreenWidth / 2) - (_toolbarSprite.Rectangle.Width / 2), 20);

      var buildButton = new Button(content.Load<Texture2D>("Controls/Icons/Build"));
      buildButton.Click += BuildButton_Click;

      var jobsButton = new Button(content.Load<Texture2D>("Controls/Icons/Jobs"));
      jobsButton.Click += JobsButton_Click;

      _icons = new List<Button>()
      {
        buildButton,
        jobsButton,
      };

      var x = _toolbarSprite.Position.X;

      foreach (var icon in _icons)
      {
        icon.Position = new Vector2(x, _toolbarSprite.Position.Y);

        x += icon.Rectangle.Width;
      }
    }

    public Toolbar_Top(GameScreen gameScreen)
    {
      _gameScreen = gameScreen;
    }

    public override void UnloadContent()
    {
      _toolbarSprite.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      if(_gameScreen.State != GameStates.Playing)
        return;

      _toolbarSprite.Update(gameTime);

      foreach (var icon in _icons)
        icon.Update(gameTime);
    }
  }
}
