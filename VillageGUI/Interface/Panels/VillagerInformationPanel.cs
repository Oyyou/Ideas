using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageGUI.Interface.Buttons;

namespace VillageGUI.Interface.Panels
{
  public class VillagerInformationPanel : Control
  {
    private Button _addButton;

    private Button _minusButton;

    private Texture2D _texture;

    public float Layer { get; set; }

    public override Rectangle Rectangle
    {
      get
      {
        var width = _texture.Width;
        var height = _texture.Height;

        return new Rectangle((int)(Position.X - (width / 2)), (int)(Position.Y - (height / 2)), (int)width, (int)height);
      }
    }

    public VillagerInformationPanel(ContentManager content)
    {
      _texture = content.Load<Texture2D>("Interface/VillagerInfo");

      _addButton = new Button(content.Load<Texture2D>("Interface/Button"), content.Load<SpriteFont>("Fonts/Font"))
      {
        Layer = Layer + 0.1f,
        Text = "+",
      };
      _minusButton = new Button(content.Load<Texture2D>("Interface/Button"), content.Load<SpriteFont>("Fonts/Font"))
      {
        Layer = Layer + 0.1f,
        Text = "-",
      };
    }

    public override Vector2 Position { get; set; }

    public void Update()
    {
      _addButton.Position = new Vector2(Rectangle.X + 10 + _addButton.Origin.X, Rectangle.Y + 10 + _addButton.Origin.Y);
      _minusButton.Position = new Vector2(_addButton.Position.X, _addButton.Position.Y + _minusButton.Rectangle.Height + 10);
      _addButton.Layer = this.Layer + 0.01f;
      _minusButton.Layer = this.Layer + 0.01f;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Position, null, Color.White, 0f, new Vector2(_texture.Width / 2, _texture.Height / 2), 1f, SpriteEffects.None, Layer);

      _addButton.Draw(gameTime, spriteBatch);
      _minusButton.Draw(gameTime, spriteBatch);
    }

    public override void UnloadContent()
    {
      _addButton.UnloadContent();
      _minusButton.UnloadContent();

      _texture.Dispose();
    }
  }
}
