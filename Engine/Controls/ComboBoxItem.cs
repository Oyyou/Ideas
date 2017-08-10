using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Controls
{
  public class ComboBoxItem : Component
  {
    private Button _button;

    public int Height
    {
      get
      {
        return _button.Rectangle.Height;
      }
    }

    public override float Layer
    {
      get { return _button.Layer; }
      set { _button.Layer = value; }
    }

    public readonly ComboBox Parent;

    public override Vector2 Position
    {
      get { return _button.Position; }
      set { _button.Position = value; }
    }

    public override Rectangle Rectangle
    {
      get { return _button.Rectangle; }
      set { _button.Rectangle = value; }
    }

    public string Text
    {
      get { return _button.Text; }
      set { _button.Text = value; }
    }

    public int Width
    {
      get
      {
        return _button.Rectangle.Width;
      }
    }

    public override void CheckCollision(Component component)
    {

    }

    public ComboBoxItem(ComboBox parent)
    {
      Parent = parent;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      _button.Draw(gameTime, spriteBatch);
    }

    public override void LoadContent(ContentManager content)
    {
      _button = new Button(content.Load<Texture2D>("Controls/ComboBox"), content.Load<SpriteFont>("Fonts/Font"));
      _button.LoadContent(content);
    }

    protected virtual void OnClick()
    {
      Parent.SelectedItem = this;
      Parent.Show = false;
    }

    public override void UnloadContent()
    {
      _button.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
      _button.Update(gameTime);

      if (_button.IsClicked)
        OnClick();
    }
  }
}
