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
    private List<JobMenuButton> _buttons;

    private Texture2D _buttonTexture;

    private List<JobMenuSubButton> _subButtons;

    private Texture2D _subButtonTexture;

    /// <summary>
    /// The job currently clicked
    /// </summary>
    public JobMenuButton JobButton;

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameScreen.State != States.GameStates.JobMenu)
        return;

      foreach (var component in Components)
        component.Draw(gameTime, spriteBatch);

      foreach (var component in _buttons)
        component.Draw(gameTime, spriteBatch);

      foreach (var component in _subButtons)
        component.Draw(gameTime, spriteBatch);

      spriteBatch.DrawString(base._font, "Jobs", _fontPosition, Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.99f);
    }

    public JobMenuWindow(GameScreen gameScreen)
      : base(gameScreen)
    {

    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      _buttons = new List<JobMenuButton>();

      _subButtons = new List<JobMenuSubButton>();

      _buttonTexture = content.Load<Texture2D>("Controls/BuildMenuMainOptionButton");
      _subButtonTexture = content.Load<Texture2D>("Controls/JobWindow/JobWindowSubButton");
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
      {
        _buttons.Clear();
        _subButtons.Clear();

        return;
      }

      SetButtons();

      SetSubButton();

      foreach (var component in Components)
        component.Update(gameTime);

      foreach (var button in _buttons)
        button.Update(gameTime);

      foreach (var subButton in _subButtons)
      {
        subButton.Update(gameTime);
        subButton.IsJobSelected = JobButton != null && JobButton.JobBuilding == subButton.NPC.Workplace;
        subButton.IsHovering = false;

        var condition1 = JobButton != null && JobButton.JobBuilding != subButton.NPC.Workplace;
        var condition2 = JobButton != null && JobButton.JobBuilding == subButton.NPC.Workplace;

        subButton.Add.IsVisible = condition1;
        subButton.Add.IsEnabled = condition1;

        subButton.Minus.IsVisible = condition2;
        subButton.Minus.IsEnabled = condition2;
      }
    }

    private void SetButtons()
    {
      if (_buttons.Count > 0)
        return;

      var x = Position.X + 11;
      var y = Position.Y + 56;

      foreach (var component in _gameScreen.Workplaces)
      {
        var button = new JobMenuButton(_buttonTexture, base._font)
        {
          Layer = 0.99f,
          Text = component.Name,
          Position = new Vector2(x, y),
          JobBuilding = component,
        };

        button.Click += Button_Click;

        button.LoadContent(_content);

        _buttons.Add(button);

        y += button.Rectangle.Height + 5;
      }
    }

    private void Button_Click(object sender, EventArgs e)
    {
      foreach (var component in _buttons)
        component.IsSelected = false;

      var button = sender as JobMenuButton;

      button.IsSelected = true;

      JobButton = button;
    }

    private void SetSubButton()
    {
      if (_subButtons.Count > 0)
        return;

      var x = Position.X + 196;
      var xIncrement = 0;

      var y = Position.Y + 56;

      foreach (var npc in _gameScreen.NPCComponents)
      {
        var button = new JobMenuSubButton(_subButtonTexture, _font, (NPC)npc)
        {
          Layer = 0.99f,
          Position = new Vector2(x + xIncrement, y)
        };

        xIncrement += button.Rectangle.Width + 5;

        if (x + xIncrement > 708)
        {
          y += button.Rectangle.Height + 5;
          xIncrement = 0;
        }

        button.LoadContent(_content);

        _subButtons.Add(button);
      }
    }
  }
}
