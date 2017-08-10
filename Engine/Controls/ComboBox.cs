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
  public class ComboBox : Component
  {
    private Button _button;

    private ComboBoxItem _selectedItem;

    private float _timer;

    public int Height
    {
      get
      {
        return _button.Rectangle.Height;
      }
    }

    public List<ComboBoxItem> Items;

    public override float Layer
    {
      get { return _button.Layer; }
      set { _button.Layer = value; }
    }

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

    public ComboBoxItem SelectedItem
    {
      get { return _selectedItem; }
      set
      {
        _selectedItem = value;

        if (value == null)
          return;

        if (_button == null)
          return;

        _button.Text = _selectedItem.Text;
      }
    }

    public bool Show { get; set; }

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

    private void Click(object sender, EventArgs e)
    {
      Show = !Show;
    }

    public ComboBox()
    {
      Reset();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      _button.Draw(gameTime, spriteBatch);

      if (!Show)
        return;

      foreach (var item in Items)
      {
        item.Draw(gameTime, spriteBatch);
      }
    }

    public override void LoadContent(ContentManager content)
    {
      _button = new Button(content.Load<Texture2D>("Controls/ComboBox"), content.Load<SpriteFont>("Fonts/Font"));
      _button.LoadContent(content);

      _button.Click += Click;

      _button.Text = "Select NPC";

      Items = new List<ComboBoxItem>();
    }

    public void Reset()
    {
      Show = false;
      SelectedItem = null;

      if (_button != null)
        _button.Text = "Select NPC";
    }

    public override void UnloadContent()
    {
      _button.UnloadContent();

      foreach (var item in Items)
      {
        item.UnloadContent();
      }

      Items.Clear();
    }

    public override void Update(GameTime gameTime)
    {
      _button.Update(gameTime);

      if (!Show)
      {
        _timer = 0;
        return;
      }

      var increment = _button.Rectangle.Height + 5;

      var y = _button.Position.Y + increment;

      foreach (var item in Items)
      {
        item.Position = new Vector2(_button.Position.X, y);
        item.Layer = _button.Layer;

        y += increment;

        item.Update(gameTime);
      }
    }
  }
}
