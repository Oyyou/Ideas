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
using TopDown.Builders;
using TopDown.Buildings;
using TopDown.Controls.ItemMenu;
using TopDown.Buildings.Housing;
using TopDown.Buildings.Labour;

namespace TopDown.Controls.BuildMenu
{
  public class BuildMenuWindow : Component
  {
    private List<BuildMenuSubButton> _buildSubOptions;

    private GameScreen _gameState;

    private Texture2D _mainButtonTexture;

    private ContentManager _content;

    private SpriteFont _font;

    private Vector2 _fontPosition;

    private Texture2D _subButtonTexture;

    public Vector2 Position { get; private set; }

    private void ArtsButton_Click(object sender, EventArgs e)
    {
      _buildSubOptions = new List<BuildMenuSubButton>()
      {
        new BuildMenuSubButton(_subButtonTexture, _font)
        {
          Text = "Library",
          Layer =  0.99f,
          GameScreenSetValue = States.GameStates.PlacingBuilding,
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
      _gameState.State = States.GameStates.Playing;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameState.State != States.GameStates.BuildMenu)
        return;

      foreach (var component in Components)
        component.Draw(gameTime, spriteBatch);

      foreach (var component in _buildSubOptions)
        component.Draw(gameTime, spriteBatch);

      spriteBatch.DrawString(_font, "Build", _fontPosition, Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.99f);
    }

    private void Item_Click(object sender, EventArgs e)
    {
      var itemOption = sender as ItemMenuButton;

      if (_gameState.State == States.GameStates.PlacingItems)
      {
        if (itemOption.CurrentState == ItemMenuButtonStates.Clicked)
        {
          if (_gameState.SelectedBuilding != null)
            _gameState.SelectedBuilding.Components.Last().IsRemoved = true;

          if (_gameState.SelectedPathBuilder != null)
          {
            _gameState.SelectedPathBuilder.Path = null;
            _gameState.SelectedPathBuilder.State = PathBuilderStates.Selecting;
            _gameState.SelectedPathBuilder.Furniture.Last().IsRemoved = true;
          }

          // 

          itemOption.CurrentState = ItemMenuButtonStates.Clickable;
          _gameState.State = States.GameStates.ItemMenu;

          foreach (var item in itemOption.Parent.Items)
          {
            item.CanClick = true;
          }
        }

        return;
      }

      if (itemOption.Furniture == null)
        throw new Exception($"Furniture hasn't been set for '{itemOption.Text}' option.");

      if (_gameState.SelectedBuilding != null)
      {
        itemOption.Furniture.Building = _gameState.SelectedBuilding;
        itemOption.Furniture.Layer = _gameState.SelectedBuilding.Layer + 0.01f;

        _gameState.SelectedBuilding.Components.Add((Furniture)itemOption.Furniture.Clone());
      }
      else if (_gameState.SelectedPathBuilder != null)
      {
        itemOption.Furniture.Building = _gameState.SelectedPathBuilder;
        itemOption.Furniture.Layer = _gameState.SelectedPathBuilder.Layer + 0.01f;

        _gameState.SelectedPathBuilder.Path = (Furniture)itemOption.Furniture.Clone();
        _gameState.SelectedPathBuilder.State = PathBuilderStates.Placing;
      }

      _gameState.ItemMenu.CurrentButton = itemOption;
      //CurrentButton = itemOption;
      _gameState.State = States.GameStates.PlacingItems;

      foreach (var item in itemOption.Parent.Items)
        item.CanClick = false;
    }

    private void HousingButton_Click(object sender, EventArgs e)
    {
      var smallHouse = new BuildMenuSubButton(_subButtonTexture, _font)
      {
        Text = "Small House",
        Layer = 0.99f,
        GameScreenSetValue = States.GameStates.PlacingBuilding,
        ResourceCost = new Models.Resources()
        {
          Food = 5,
          Gold = 1,
          Wood = 10,
          Stone = 10,
        },
      };

      var bed = new ItemMenuButton(_mainButtonTexture, _font)
      {
        Text = "Bed",
        Parent = smallHouse,
        //Layer = _background.Layer + 0.01f,
        Furniture = new Furniture(_content.Load<Texture2D>("Furniture/Bed"), _gameState)
        {
          State = FurnatureStates.Placing,
          Position = GameScreen.Mouse.PositionWithCamera,
        },
      };

      bed.Click += Item_Click;

      var toilet = new ItemMenuButton(_mainButtonTexture, _font)
      {
        Text = "Toilet",
        Parent = smallHouse,
        // Layer = _background.Layer + 0.01f,
        Furniture = new Furniture(_content.Load<Texture2D>("Furniture/Toilet"), _gameState)
        {
          State = FurnatureStates.Placing,
          Position = GameScreen.Mouse.PositionWithCamera,
        },
      };

      toilet.Click += Item_Click;

      smallHouse.Items = new List<ItemMenuButton>()
      {
        bed,
        toilet,
      };

      smallHouse.Click += SmallHouse_Click;

      _buildSubOptions = new List<BuildMenuSubButton>()
      {
        smallHouse,
        new BuildMenuSubButton(_subButtonTexture, _font)
        {
          Text = "Large House",
          Layer =  0.99f,
          GameScreenSetValue = States.GameStates.PlacingBuilding,
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
      _gameState.AddComponent(new SmallHouse(_gameState, new SmallHouseTemplate(_content))
      {
        BuildingState = Buildings.BuildingStates.Placing,
      });
    }

    private void LabourButton_Click(object sender, EventArgs e)
    {
      var blacksmith = new BuildMenuSubButton(_subButtonTexture, _font)
      {
        Text = "Blacksmith",
        Layer = 0.99f,
        GameScreenSetValue = States.GameStates.PlacingBuilding,
        ResourceCost = new Models.Resources()
        {
          Food = 15,
          Gold = 10,
          Wood = 30,
          Stone = 40,
        },
      };

      blacksmith.Click += Blacksmith_Click;

      var anvil = new ItemMenuButton(_mainButtonTexture, _font)
      {
        Text = "Anvil",
        Parent = blacksmith,
        //Layer = _background.Layer + 0.01f,
        Furniture = new Furniture(_content.Load<Texture2D>("Furniture/Anvil"), _gameState)
        {
          State = FurnatureStates.Placing,
          Position = GameScreen.Mouse.PositionWithCamera,
        },
      };

      anvil.Click += Item_Click;

      blacksmith.Items = new List<ItemMenuButton>()
      {
        anvil,
      };

      _buildSubOptions = new List<BuildMenuSubButton>()
      {
        blacksmith,
        new BuildMenuSubButton(_subButtonTexture, _font)
        {
          Text = "Farm",
          Layer =  0.99f,
          GameScreenSetValue = States.GameStates.PlacingBuilding,
          ResourceCost = new Models.Resources()
          {
            Food = 15,
            Gold = 10,
            Wood = 30,
            Stone = 40,
          },
        },
        new BuildMenuSubButton(_subButtonTexture, _font)
        {
          Text = "Lumber Mill",
          Layer =  0.99f,
          GameScreenSetValue = States.GameStates.PlacingBuilding,
          ResourceCost = new Models.Resources()
          {
            Food = 15,
            Gold = 10,
            Wood = 30,
            Stone = 40,
          },
        },
        new BuildMenuSubButton(_subButtonTexture, _font)
        {
          Text = "Mine",
          Layer =  0.99f,
          GameScreenSetValue = States.GameStates.PlacingBuilding,
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

    private void Blacksmith_Click(object sender, EventArgs e)
    {
      _gameState.AddComponent(new Blacksmith(_gameState, new BlacksmithTemplate(_content))
      {
        BuildingState = Buildings.BuildingStates.Placing,
      });
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

      var miscButton = new Button(_mainButtonTexture, _font)
      {
        Text = "Misc",
        Position = new Vector2(artsButton.Position.X, artsButton.Rectangle.Bottom + 5),
        Layer = 0.99f
      };

      miscButton.Click += MiscButton_Click;

      Components = new List<Component>()
      {
        background,
        closeButton,
        housingButton,
        labourButton,
        artsButton,
        miscButton
      };

      foreach (var component in Components)
        component.LoadContent(content);

      _buildSubOptions = new List<BuildMenuSubButton>();
    }

    private void MiscButton_Click(object sender, EventArgs e)
    {
      var path = new BuildMenuSubButton(_subButtonTexture, _font)
      {
        Text = "Path",
        Layer = 0.99f,
        GameScreenSetValue = States.GameStates.ItemMenu,
        ResourceCost = new Models.Resources()
        {
          Food = 0,
          Gold = 0,
          Wood = 0,
          Stone = 1,
        },
      };

      path.Click += Path_Click;

      var stonePath = new ItemMenuButton(_mainButtonTexture, _font)
      {
        Text = "Stone",
        Parent = path,
        //Layer = _background.Layer + 0.01f,
        Furniture = new Furniture(_content.Load<Texture2D>("Sprites/Paths/StonePath"), _gameState)
        {
          IsCollidable = false,
          State = FurnatureStates.Placing,
          Position = GameScreen.Mouse.PositionWithCamera,
        },
      };

      stonePath.Click += Item_Click;

      path.Items = new List<ItemMenuButton>()
      {
        stonePath,
      };

      _buildSubOptions = new List<BuildMenuSubButton>()
      {
        path,
      };

      foreach (var component in _buildSubOptions)
        component.LoadContent(_content);
    }

    private void StonePath_Click(object sender, EventArgs e)
    {
      var item = sender as ItemMenuButton;
    }

    private void Path_Click(object sender, EventArgs e)
    {
      _gameState.AddComponent(new PathBuilder(_gameState)
      {
        State = PathBuilderStates.Selecting,
      });
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
      if (_gameState.State != States.GameStates.BuildMenu)
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
        {
          _gameState.ItemMenu.Open(component);
        }
      }
    }
  }
}
