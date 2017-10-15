using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.Buildings;
using TopDown.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TopDown.Controls.InspectMenu
{
  public class InspectMenuWindow : MenuWindow
  {
    /// <summary>
    /// The building that is being inspected
    /// </summary>
    public Building Building { get; set; }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameScreen.State != States.GameStates.InspectMenu)
        return;

      base.Draw(gameTime, spriteBatch);

      foreach (var component in Components)
        component.Draw(gameTime, spriteBatch);

      spriteBatch.DrawString(_font, "Inspecting", _fontPosition, Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.99f);
    }

    public InspectMenuWindow(GameScreen gameScreen) : base(gameScreen)
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

      Components?.Clear();
    }

    public override void Update(GameTime gameTime)
    {
      if (_gameScreen.State != States.GameStates.InspectMenu)
      {
        return;
      }

      foreach (var component in Components)
        component.Update(gameTime);
    }
  }
}
