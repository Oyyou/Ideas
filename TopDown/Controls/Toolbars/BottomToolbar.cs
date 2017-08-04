using Engine;
using Engine.Controls;
using Engine.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.States;

namespace TopDown.Controls.Toolbars
{
  public class BottomToolbar : Toolbar
  {
    public BottomToolbar(GameScreen gameScreen) : base(gameScreen)
    {

    }

    private void FistButton_Click(object sender, EventArgs e)
    {
      GameScreen.Mouse.MouseState = MouseStates.Building;
    }

    public override void LoadContent(ContentManager content)
    {
      _toolbarSprite = new Sprite(content.Load<Texture2D>("Controls/Toolbar"));
      _toolbarSprite.Position = new Vector2((GameEngine.ScreenWidth / 2) - (_toolbarSprite.Rectangle.Width / 2), GameEngine.ScreenHeight - _toolbarSprite.Rectangle.Height - 20);

      var fistButton = new Button(content.Load<Texture2D>("Controls/Icons/Fist"));
      fistButton.Click += FistButton_Click;

      var pickaxeButton = new Button(content.Load<Texture2D>("Controls/Icons/Pickaxe"));
      pickaxeButton.Click += PickaxeButton_Click;

      _icons = new List<Button>()
      {
        fistButton,
        pickaxeButton,
      };

      InitializeIcons(content);
    }

    private void PickaxeButton_Click(object sender, EventArgs e)
    {
      GameScreen.Mouse.MouseState = MouseStates.Mining;
    }

    public override void Update(GameTime gameTime)
    {
      if (_gameScreen.State != GameStates.Playing)
        return;

      if (GameScreen.Keyboard.IsKeyPressed(Keys.D1))
        GameScreen.Mouse.MouseState = MouseStates.Building;
      else if (GameScreen.Keyboard.IsKeyPressed(Keys.D2))
        GameScreen.Mouse.MouseState = MouseStates.Mining;

      _toolbarSprite.Update(gameTime);

      foreach (var icon in _icons)
        icon.Update(gameTime);
    }
  }
}
