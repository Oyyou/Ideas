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

namespace TopDown.Controls
{
  public class GameSpeedController : Component
  {
    private Sprite _background;

    private List<Button> _buttons;

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      var x = 5;
      var y = GameEngine.ScreenHeight - (/*(_background.Rectangle.Height / 2) -*/ (_buttons.FirstOrDefault().Rectangle.Height /*/ 2*/));

      foreach (var button in _buttons)
      {
        button.Position = new Vector2(x, y);
        x += button.Rectangle.Width + 5;
      }

      //_background.Position = new Vector2(0, GameEngine.ScreenHeight - _background.Rectangle.Height);

      //_background.Draw(gameTime, spriteBatch);

      foreach (var button in _buttons)
        button.Draw(gameTime, spriteBatch);
    }

    public override void LoadContent(ContentManager content)
    {
      var speed1Button = new Button(content.Load<Texture2D>("Controls/Icons/Speed_1"));

      var speed2Button = new Button(content.Load<Texture2D>("Controls/Icons/Speed_2"));

      var speed3Button = new Button(content.Load<Texture2D>("Controls/Icons/Speed_3"));

      _buttons = new List<Button>()
      {
        speed1Button,
        speed2Button,
        speed3Button,
      };

      foreach (var button in _buttons)
        button.LoadContent(content);
    }

    public override void UnloadContent()
    {

    }

    public override void Update(GameTime gameTime)
    {
      foreach (var button in _buttons)
        button.Update(gameTime);
    }
  }
}
