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
using TopDown.Sprites;

namespace TopDown.Controls.JobMenu
{
  public class JobMenuWindow : MenuWindow
  {
    private SpriteFont font;

    private List<JobMenuSubButton> _subButtons;

    private Texture2D subButtonTexture;

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameScreen.State != States.GameStates.JobMenu)
        return;

      foreach (var component in Components)
        component.Draw(gameTime, spriteBatch);

      foreach (var component in _subButtons)
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

      _subButtons = new List<JobMenuSubButton>();

      font = content.Load<SpriteFont>("Fonts/Font");
      subButtonTexture = content.Load<Texture2D>("Controls/JobWindow/JobWindowSubButton");

      foreach (var component in _subButtons)
        component.LoadContent(_content);
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

      _subButtons.Clear();
      foreach (var npc in _gameScreen.NPCComponents)
      {
        _subButtons.Add(new JobMenuSubButton(subButtonTexture, font, (NPC)npc)
        {
          Layer = 0.99f,
        });
      }

      foreach (var component in Components)
        component.Update(gameTime);

      var x = Position.X + 196;
      var xIncrement = 0;

      var y = Position.Y + 56;

      foreach (var component in _subButtons)
      {
        component.Position = new Vector2(x + xIncrement, y);

        xIncrement += component.Rectangle.Width + 5;

        if (x + xIncrement > 708)
        {
          y += component.Rectangle.Height + 5;
          xIncrement = 0;
        }

        component.Update(gameTime);

        if (component.IsClicked)
        {

        }
      }
    }
  }
}
