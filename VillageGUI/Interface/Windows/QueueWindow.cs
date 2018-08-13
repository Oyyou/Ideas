using Engine.Interface.Windows;
using VillageGUI.Interface.Buttons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBackend.Managers;
using VillageBackend.Models;
using Engine.Input;

namespace VillageGUI.Interface.Windows
{
  internal class QueueWindow : Window
  {
    private ItemManager _itemManager;

    private List<ItemButton> _queuedItemButtons = new List<ItemButton>();

    public override Rectangle WindowRectangle { get => this.Rectangle; }

    public QueueWindow(ContentManager content, ItemManager itemManager) : base(content)
    {
      _itemManager = itemManager;

      Name = "Queue";

      Texture = content.Load<Texture2D>("Interface/Window250x360");

      _itemManager.QueuedItems.CollectionChanged += QueuedItems_CollectionChanged;

      SetPositions();
    }

    private void QueuedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      var x = Position.X + 10 + 25;
      var y = Position.Y + 38 + 25;

      var max = ((int)Math.Floor(((Rectangle.Width) / 70d) * // X
        (((Rectangle.Height) + 38d) / 70d)) - 1); // Y

      _queuedItemButtons = new List<ItemButton>();

      for (int i = 0; i < MathHelper.Min(max, _itemManager.QueuedItems.Count); i++)
      {
        var item = _itemManager.QueuedItems[i];

        _queuedItemButtons.Add(GetItemButton(item));
      }

      SetButtonPositions();
    }

    private void SetButtonPositions()
    {
      if (_queuedItemButtons == null ||
         _queuedItemButtons.Count == 0)
        return;

      var x = Position.X + 10 + (_queuedItemButtons.FirstOrDefault().Rectangle.Width / 2);
      var y = Position.Y + 38 + (_queuedItemButtons.FirstOrDefault().Rectangle.Height / 2);

      foreach (var button in _queuedItemButtons)
      {
        button.Position = new Vector2(x, y);

        x += button.Rectangle.Width + 10;

        if (x + (button.Rectangle.Width / 2) >= Position.X + Rectangle.Width)
        {
          x = Position.X + 10 + 25;
          y += button.Rectangle.Height + 10;
        }
      }
    }

    private ItemButton GetItemButton(ItemV2 item)
    {
      var fullPath = $"{Directory.GetCurrentDirectory()}\\Content\\Interface\\ItemIcons\\{item.Name}.xnb";

      string content = "Interface/NoImage";

      if (File.Exists(fullPath))
        content = "Interface/ItemIcons/" + item.Name;

      var button = new ItemButton(_content.Load<Texture2D>(content), item)
      {
        Click = ItemClick,
      };

      return button;
    }

    /// <summary>
    /// Removes the item from the queue when clicked
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ItemClick(object sender)
    {
      var button = sender as ItemButton;

      for (int i = 0; i < _queuedItemButtons.Count; i++)
      {
        if (_queuedItemButtons[i] == button)
        {
          _itemManager.QueuedItems.RemoveAt(i);
          return;
        }
      }
    }

    public override void SetPositions()
    {
      SetButtonPositions();
    }

    public override void UnloadContent()
    {

    }

    public override void Update(GameTime gameTime)
    {
      _hasUpdated = true;

      var mouseRectangle = GameMouse.Rectangle;

      foreach (var button in _queuedItemButtons)
      {
        switch (button.CurrentState)
        {
          case ButtonStates.Nothing:

            if (Keyboard.GetState().IsKeyDown(Keys.T))
            {
              Console.WriteLine(mouseRectangle);
              Console.WriteLine(button.Rectangle);
              throw new Exception("");
            }

            if (mouseRectangle.Intersects(button.Rectangle) && mouseRectangle.Intersects(WindowRectangle))
              button.CurrentState = ButtonStates.Hovering;

            break;
          case ButtonStates.Hovering:

            if (!mouseRectangle.Intersects(button.Rectangle) || !mouseRectangle.Intersects(WindowRectangle))
              button.CurrentState = ButtonStates.Nothing;

            if (GameMouse.Clicked)
              button.OnClick();

            break;

          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }
      }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {
      if (!_hasUpdated)
        return;

      DrawWindow(gameTime, spriteBatch);

      DrawQueueButtons(gameTime, spriteBatch);
    }

    private void DrawQueueButtons(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

      foreach (var button in _queuedItemButtons)
      {
        switch (button.CurrentState)
        {
          case ButtonStates.Nothing:

            button.Color = Color.White;

            button.Scale = 1.0f;

            break;
          case ButtonStates.Hovering:

            button.Color = Color.YellowGreen;

            button.Scale = 1.0f;

            break;
          default:
            throw new Exception("Unknown ToolbarButtonState: " + button.CurrentState.ToString());
        }

        button.Draw(gameTime, spriteBatch);
      }

      var diff = _itemManager.QueuedItems.Count - _queuedItemButtons.Count;

      if (diff > 0)
      {
        var text = "+" + diff;

        var lastButton = _queuedItemButtons.Last();
        var x = (lastButton.Rectangle.X + lastButton.Rectangle.Width + (lastButton.Rectangle.Width / 2) + 10) - (_font.MeasureString(text).X / 2);
        var y = (lastButton.Rectangle.Y + (lastButton.Rectangle.Height / 2)) - (_font.MeasureString(text).Y / 2);

        spriteBatch.DrawString(_font, text, new Vector2(x, y), Color.White);
      }

      spriteBatch.End();
    }

    protected void DrawWindow(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

      spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

      //_itemSection.Scrollbar.Draw(gameTime, spriteBatch);

      //_categorySection.Scrollbar.Draw(gameTime, spriteBatch);

      spriteBatch.DrawString(_font, Name, new Vector2(Position.X + 10, Position.Y + 10), Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.1f);

      spriteBatch.End();
    }
  }
}
