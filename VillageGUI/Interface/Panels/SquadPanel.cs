using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using VillageBackend.Managers;
using VillageBackend.Models;
using VillageGUI.Interface.Buttons;
using VillageGUI.Interface.Windows;

namespace VillageGUI.Interface.Panels
{
  public class SquadPanel : Control
  {
    private Texture2D _texture;

    private SpriteFont _font;

    private Squad _squad;

    private Texture2D _avatarTexture;

    private Button _plusButton;

    public override Rectangle Rectangle
    {
      get
      {
        var width = _texture.Width;
        var height = _texture.Height;

        return new Rectangle((int)(Position.X - (width / 2)), (int)(Position.Y - (height / 2)), (int)width, (int)height);
      }
    }

    public float Layer { get; set; }

    public override Vector2 Position { get; set; }

    public SquadPanel(ContentManager content, GraphicsDevice graphicsDevice, Squad squad, WindowSection section)
    {
      _squad = squad;

      _font = content.Load<SpriteFont>("Fonts/Font");

      _avatarTexture = content.Load<Texture2D>("Avatars/Default");
      _plusButton = new Button(content.Load<Texture2D>("Interface/Plus"));

      _texture = new Texture2D(graphicsDevice, section.Area.Width - 50, _avatarTexture.Height + 8);
      Helpers.SetTexture(_texture, new Color(43, 43, 43), new Color(0, 0, 0));
    }

    public override void UnloadContent()
    {
      _texture.Dispose();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Position, null, Color.White, 0, new Vector2(_texture.Width / 2, _texture.Height / 2), 1f, SpriteEffects.None, Layer);

      if (_squad != null)
      {
        spriteBatch.DrawString(_font, _squad.Name, new Vector2(Rectangle.Left + 5, Rectangle.Top + 5), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, Layer + 0.01f);

        DrawAvatars(spriteBatch);
      }
      else
      {
        _plusButton.Position = this.Position;
        _plusButton.Draw(gameTime, spriteBatch);
      }
    }

    private void DrawAvatars(SpriteBatch spriteBatch)
    {
      int count = 4;

      var totalWidth = (_avatarTexture.Width + 2) * 4;

      var position = new Vector2((Rectangle.Right - totalWidth) + (_avatarTexture.Width / 2), Position.Y + 4 - (_texture.Height / 2) + (_avatarTexture.Height / 2));

      for (int i = 0; i < count; i++)
      {
        if (_squad.Villagers.Count < i + 1)
          continue;

        spriteBatch.Draw(_avatarTexture, position, null, Color.White, 0f, new Vector2(_avatarTexture.Width / 2, _avatarTexture.Height / 2), 1f, SpriteEffects.None, Layer + 0.01f);
        position.X += _avatarTexture.Width + 1;
      }
    }
  }
}
