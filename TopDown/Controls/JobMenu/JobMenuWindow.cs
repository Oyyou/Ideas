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
using TopDown.States;
using Engine.Controls;

namespace TopDown.Controls.JobMenu
{
  public class JobMenuWindow : MenuWindow
  {
    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameScreen.State != States.GameStates.JobMenu)
        return;

      foreach (var component in Components)
        component.Draw(gameTime, spriteBatch);

      spriteBatch.DrawString(_font, "Jobs", _fontPosition, Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.99f);
    }

    public JobMenuWindow(GameScreen gameScreen)
      : base(gameScreen)
    {

    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);
    }

    public override void UnloadContent()
    {
      foreach (var component in Components)
        component.UnloadContent();

      Components.Clear();
    }

    public override void Update(GameTime gameTime)
    {
      if (_gameScreen.State != States.GameStates.JobMenu)
        return;

      foreach (var component in Components)
        component.Update(gameTime);
    }
  }
}
