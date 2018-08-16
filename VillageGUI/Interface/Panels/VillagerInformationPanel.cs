using Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBackend.Managers;
using VillageBackend.Models;
using VillageGUI.Interface.Buttons;

namespace VillageGUI.Interface.Panels
{
  public class VillagerInformationPanel : Control
  {
    private List<VillagerInfoButton> _buttons;

    private VillagerInfoButton _addButton;

    private VillagerInfoButton _minusButton;

    private GameManagers _gameManagers;

    private Texture2D _texture;

    private SpriteFont _font;

    public Villager Villager { get; set; }

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

    public override Vector2 Position { get; set; }

    public Color Colour { get; set; }

    public VillagerInformationPanel(ContentManager content, GameManagers gameManagers)
    {
      _gameManagers = gameManagers;

      _texture = content.Load<Texture2D>("Interface/VillagerInfo");

      _font = content.Load<SpriteFont>("Fonts/Font");

      _addButton = new VillagerInfoButton(content.Load<Texture2D>("Interface/Button"), content.Load<SpriteFont>("Fonts/Font"))
      {
        Layer = Layer + 0.1f,
        Text = "+",
        Click = OnAddClick
      };
      _minusButton = new VillagerInfoButton(content.Load<Texture2D>("Interface/Button"), content.Load<SpriteFont>("Fonts/Font"))
      {
        Layer = Layer + 0.1f,
        Text = "-",
        Click = OnMinusClick
      };

      _buttons = new List<VillagerInfoButton>()
      {
        _addButton,
        _minusButton,
      };

      Colour = Color.White;
    }

    private void OnMinusClick(Button obj)
    {
      Villager.JobId = null;
    }

    private void OnAddClick(Button obj)
    {
      if (_gameManagers.JobManager.SelectedJob == null)
        return;

      _gameManagers.VillagerManager.AssignJob(Villager, _gameManagers.JobManager.SelectedJob);
    }

    public void Update(Rectangle mouseRectangle, Rectangle mouseWithCamera, Rectangle windowRectangle)
    {
      _addButton.Position = new Vector2(Rectangle.X + 100 + _addButton.Origin.X, Rectangle.Y + 10 + _addButton.Origin.Y);
      _minusButton.Position = new Vector2(_addButton.Position.X, _addButton.Position.Y + _minusButton.Rectangle.Height + 10);
      _addButton.Layer = this.Layer + 0.01f;
      _minusButton.Layer = this.Layer + 0.01f;

      foreach (var button in _buttons)
      {
        button.Update(mouseRectangle, _buttons, mouseWithCamera, windowRectangle);
      }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      Colour = Color.White;

      if (Villager.JobId != null)
      {
        if (_gameManagers.JobManager.SelectedJob != null)
        {
          if (_gameManagers.JobManager.SelectedJob.Id == Villager.JobId.Value)
            Colour = Color.Green;
          else Colour = Color.Red;
        }
      }

      spriteBatch.Draw(_texture, Position, null, Colour, 0f, new Vector2(_texture.Width / 2, _texture.Height / 2), 1f, SpriteEffects.None, Layer);
      spriteBatch.DrawString(_font, Villager.Name, new Vector2(Rectangle.X + 10, Rectangle.Y + 10), Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, Layer + 0.01f);

      foreach (var button in _buttons)
      {
        button.Draw(gameTime, spriteBatch);
      }
    }

    public override void UnloadContent()
    {
      _addButton.UnloadContent();
      _minusButton.UnloadContent();

      _texture.Dispose();
    }
  }
}
