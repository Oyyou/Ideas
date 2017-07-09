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
using Microsoft.Xna.Framework.Input;
using Engine.Controls;
using TopDown.States;
using TopDown.Buildings.Templates;

namespace TopDown.Controls.BuildMenu
{
  public class BuildMenuWindow : Component
  {
    private List<Button> _buildSubOptions;

    private GameScreen _gameState;

    private Texture2D _mainButtonTexture;

    private ContentManager _content;

    private SpriteFont _font;

    private Vector2 _fontPosition;

    private Texture2D _subButtonTexture;

    public Vector2 Position { get; private set; }

    private void ArtsButton_Click(object sender, EventArgs e)
    {
      _buildSubOptions = new List<Button>()
      {
        new BuildMenuSubItem(_subButtonTexture, _font)
        {
          Text = "Library",
          Layer =  0.99f,
          ResourceCost = new Models.Resources()
          {
            Food = 30,
            Gold = 10,
            Wood = 50,
            Stone = 75,
          },
        },
      };

      foreach (var component in _buildSubOptions)
        component.LoadContent(_content);
    }

    public BuildMenuWindow(GameScreen gameState)
    {
      _gameState = gameState;
    }

    public override void CheckCollision(Component component)
    {

    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
      _gameState.State = States.States.Playing;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameState.State != States.States.BuildMenu)
        return;

      foreach (var component in Components)
        component.Draw(gameTime, spriteBatch);

      foreach (var component in _buildSubOptions)
        component.Draw(gameTime, spriteBatch);

      spriteBatch.DrawString(_font, "Build", _fontPosition, Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.99f);
    }

    private void HousingButton_Click(object sender, EventArgs e)
    {
      var smallHouse = new BuildMenuSubItem(_subButtonTexture, _font)
      {
        Text = "Small House",
        Layer = 0.99f,
        ResourceCost = new Models.Resources()
        {
          Food = 5,
          Gold = 1,
          Wood = 10,
          Stone = 10,
        },
      };

      smallHouse.Click += SmallHouse_Click;

      _buildSubOptions = new List<Button>()
      {
        smallHouse,
        new BuildMenuSubItem(_subButtonTexture, _font)
        {
          Text = "Large House",
          Layer =  0.99f,
          ResourceCost = new Models.Resources()
          {
            Food = 15,
            Gold = 10,
            Wood = 30,
            Stone = 40,
          },
        },
      };

      foreach (var component in _buildSubOptions)
        component.LoadContent(_content);
    }

    private void SmallHouse_Click(object sender, EventArgs e)
    {
      _gameState.AddComponent(new Buildings.Building(_gameState, new SmallHouseTemplate())
      {
        BuildingState = Buildings.BuildingStates.Placing,
      });
    }

    private void LabourButton_Click(object sender, EventArgs e)
    {
      _buildSubOptions = new List<Button>()
      {
        new BuildMenuSubItem(_subButtonTexture, _font)
        {
          Text = "Blacksmith",
          Layer =  0.99f,
          ResourceCost = new Models.Resources()
          {
            Food = 15,
            Gold = 10,
            Wood = 30,
            Stone = 40,
          },
        },
        new BuildMenuSubItem(_subButtonTexture, _font)
        {
          Text = "Farm",
          Layer =  0.99f,
          ResourceCost = new Models.Resources()
          {
            Food = 15,
            Gold = 10,
            Wood = 30,
            Stone = 40,
          },
        },
        new BuildMenuSubItem(_subButtonTexture, _font)
        {
          Text = "Lumber Mill",
          Layer =  0.99f,
          ResourceCost = new Models.Resources()
          {
            Food = 15,
            Gold = 10,
            Wood = 30,
            Stone = 40,
          },
        },
        new BuildMenuSubItem(_subButtonTexture, _font)
        {
          Text = "Mine",
          Layer =  0.99f,
          ResourceCost = new Models.Resources()
          {
            Food = 15,
            Gold = 10,
            Wood = 30,
            Stone = 40,
          },
        },
      };

      foreach (var component in _buildSubOptions)
        component.LoadContent(_content);
    }

    public override void LoadContent(ContentManager content)
    {
      _content = content;

      var backgroundTexture = content.Load<Texture2D>("Controls/BuildMenu");
      _mainButtonTexture = content.Load<Texture2D>("Controls/BuildMenuMainOptionButton");
      _subButtonTexture = content.Load<Texture2D>("Controls/BuildMenuSubOptionButton");
      var closeButtonTexture = content.Load<Texture2D>("Controls/Close");

      Position = new Vector2(
            (GameEngine.ScreenWidth / 2) - (backgroundTexture.Width / 2),
            25f);

      var background = new Sprite(backgroundTexture)
      {
        Position = Position,
        Layer = 0.98f,
      };

      var closeButton = new Button(closeButtonTexture)
      {
        Position = new Vector2(
          background.Rectangle.Right - closeButtonTexture.Width - 5,
          background.Rectangle.Top + 5
        ),
        Layer = 0.99f
      };

      closeButton.Click += CloseButton_Click;

      _font = content.Load<SpriteFont>("Fonts/Font");
      _fontPosition = new Vector2(Position.X + 5, Position.Y + 5);

      var housingButton = new Button(_mainButtonTexture, _font)
      {
        Text = "Housing",
        Position = new Vector2(Position.X + 11, Position.Y + 56),
        Layer = 0.99f
      };

      housingButton.Click += HousingButton_Click;

      var labourButton = new Button(_mainButtonTexture, _font)
      {
        Text = "Labour",
        Position = new Vector2(housingButton.Position.X, housingButton.Rectangle.Bottom + 5),
        Layer = 0.99f
      };

      labourButton.Click += LabourButton_Click;

      var artsButton = new Button(_mainButtonTexture, _font)
      {
        Text = "Arts",
        Position = new Vector2(labourButton.Position.X, labourButton.Rectangle.Bottom + 5),
        Layer = 0.99f
      };

      artsButton.Click += ArtsButton_Click;

      Components = new List<Component>()
      {
        background,
        closeButton,
        housingButton,
        labourButton,
        artsButton,
      };

      foreach (var component in Components)
        component.LoadContent(content);

      _buildSubOptions = new List<Button>();
    }

    public override void UnloadContent()
    {
      foreach (var component in Components)
        component.UnloadContent();

      foreach (var component in _buildSubOptions)
        component.UnloadContent();

      Components?.Clear();

      _buildSubOptions?.Clear();
    }

    public override void Update(GameTime gameTime)
    {
      if (_gameState.State != States.States.BuildMenu)
        return;

      foreach (var component in Components)
        component.Update(gameTime);

      var x = Position.X + 196;
      var xIncrement = 0;

      var y = Position.Y + 56;

      foreach (var component in _buildSubOptions)
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
          _gameState.State = States.States.PlacingBuilding;
      }
    }
  }
}
