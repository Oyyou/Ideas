using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine;
using Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using VillageBackend.Models;
using VillageGUI.Interface.Buttons;

namespace VillageGUI.Interface.Combat
{
  public class HeroPanel : Control
  {
    private Texture2D _texture;

    private Texture2D _defaultAvatar;

    private SpriteFont _font;

    private Squad _squad;

    private List<HeroButton> _heroButtons = new List<HeroButton>();

    public override Rectangle Rectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
      }
    }

    public int SelectedHeroIndex { get; set; }

    public override Vector2 Position { get; set; }

    public float Layer { get; set; }

    public HeroPanel(Squad squad)
    {
      _squad = squad;
    }

    public void LoadContent(ContentManager content)
    {
      _texture = content.Load<Texture2D>("Interface/Combat/HeroPanel");

      _defaultAvatar = content.Load<Texture2D>("Avatars/Default");

      _font = content.Load<SpriteFont>("Fonts/Calibri_12pt");

      for (int i = 0; i < _squad.Villagers.Count; i++)
      {
        _heroButtons.Add(new HeroButton(_defaultAvatar)
        {
          Layer = this.Layer + 0.01f,
          Click = HeroButtonClicked,
          SelectedHeroIndex = i,
        });
      }

      SetPositions();
    }

    public void SetPositions()
    {
      Position = new Vector2(10, GameEngine.ScreenHeight - 10 - Rectangle.Height);

      for (int i = 0; i < _squad.Villagers.Count; i++)
      {
        _heroButtons[i].Position = Position + new Vector2(4 + ((_defaultAvatar.Width + 2) * i), 20) + _heroButtons[i].Origin;
      }
    }

    private void HeroButtonClicked(Button button)
    {
      var heroButton = button as HeroButton;

      SelectedHeroIndex = heroButton.SelectedHeroIndex;
    }

    public override void UnloadContent()
    {
      _texture.Dispose();

      foreach (var button in _heroButtons)
        button.UnloadContent();
    }

    public void Update(GameTime gameTime)
    {
      foreach (var button in _heroButtons)
        button.Update(GameMouse.Rectangle, _heroButtons);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Position, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer);

      spriteBatch.DrawString(_font, _squad.Name, Position + new Vector2(4, 4), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer + 0.01f);

      spriteBatch.DrawString(_font, "Abilities", Position + new Vector2(300, 20), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer + 0.01f);

      foreach (var button in _heroButtons)
        button.Draw(gameTime, spriteBatch);
    }
  }
}
