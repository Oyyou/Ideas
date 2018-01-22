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
using TopDown.Builders;
using TopDown.Buildings;
using TopDown.Controls.ItemMenu;
using TopDown.Buildings.Housing;
using TopDown.Buildings.Labour;
using TopDown.Sprites;
using TopDown.Furnitures;

namespace TopDown.Controls.BuildMenu
{
  public class BuildMenuWindow : MenuWindow
  {
    private List<BuildMenuSubButton> _buildSubOptions;

    private List<Button> _buttons;

    private Texture2D _mainButtonTexture;

    private Texture2D _subButtonTexture;

    private void ArtsButton_Click(object sender, EventArgs e)
    {
      _buildSubOptions = new List<BuildMenuSubButton>()
      {
        new BuildMenuSubButton(_subButtonTexture, _font)
        {
          Text = "Library",
          Layer =  0.99f,
          GameScreenSetValue = States.GameStates.PlacingBuilding,
          ResourceCost = new VillageBackend.Models.Resources()
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

    public BuildMenuWindow(GameScreen gameScreen)
      : base(gameScreen)
    {

    }

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      if (_gameScreen.State != States.GameStates.BuildMenu)
        return;

      base.Draw(gameTime, spriteBatch);

      foreach (var component in Components)
        component.Draw(gameTime, spriteBatch);

      var buttonY = _windowSprite.Position.Y + 56;

      foreach (var component in _buttons)
      {
        component.Position = new Vector2(_windowSprite.Position.X + 11, buttonY);

        buttonY += component.Rectangle.Height + 5;

        component.Draw(gameTime, spriteBatch);
      }

      var x = _windowSprite.Position.X + 196;
      var xIncrement = 0;

      var y = _windowSprite.Position.Y + 56;

      foreach (var component in _buildSubOptions)
      {
        component.Position = new Vector2(x + xIncrement, y);

        xIncrement += component.Rectangle.Width + 5;

        if (x + xIncrement > _windowSprite.Rectangle.Right - 12)
        {
          y += component.Rectangle.Height + 5;
          xIncrement = 0;
        }

        component.Draw(gameTime, spriteBatch);
      }

      spriteBatch.DrawString(_font, "Build", _fontPosition, Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.99f);
    }

    private void Item_Click(object sender, EventArgs e)
    {
      var itemOption = sender as ItemMenuButton;

      if (_gameScreen.State == States.GameStates.PlacingItems)
      {
        if (itemOption.CurrentState == ItemMenuButtonStates.Clicked)
        {
          if (_gameScreen.SelectedBuilding != null)
            _gameScreen.SelectedBuilding.Components.Last().IsRemoved = true;

          if (_gameScreen.SelectedPathBuilder != null)
          {
            _gameScreen.SelectedPathBuilder.Path = null;
            _gameScreen.SelectedPathBuilder.State = PathBuilderStates.Selecting;
            _gameScreen.SelectedPathBuilder.Paths.Last().IsRemoved = true;
          }

          itemOption.CurrentState = ItemMenuButtonStates.Clickable;
          _gameScreen.State = States.GameStates.ItemMenu;

          foreach (var item in itemOption.Parent.Items)
          {
            item.CanClick = true;
          }
        }

        return;
      }

      //if (itemOption.Components == null)
      //  throw new Exception($"No items to add for the '{itemOption.Text}' option.");

      if (_gameScreen.SelectedBuilding != null)
      {
        ((Furniture)itemOption.PlacingObject).Building = _gameScreen.SelectedBuilding;
        itemOption.PlacingObject.Layer = _gameScreen.SelectedBuilding.Layer + 0.01f;

        _gameScreen.SelectedBuilding.Components.Add((Furniture)itemOption.PlacingObject.Clone());
      }
      else if (_gameScreen.SelectedPathBuilder != null)
      {
        _gameScreen.SelectedPathBuilder.Path = (Path)itemOption.PlacingObject;
        _gameScreen.SelectedPathBuilder.State = PathBuilderStates.Placing;
      }

      _gameScreen.ItemMenu.CurrentButton = itemOption;
      //CurrentButton = itemOption;
      _gameScreen.State = States.GameStates.PlacingItems;

      foreach (var item in itemOption.Parent.Items)
        item.CanClick = false;
    }

    private void HousingButton_Click(object sender, EventArgs e)
    {
      _buildSubOptions = new List<BuildMenuSubButton>()
      {
        GetSmallHouse(),
        //GetTavern(),
      };

      foreach (var component in _buildSubOptions)
        component.LoadContent(_content);
    }

    private BuildMenuSubButton GetSmallHouse()
    {
      var smallHouse = new BuildMenuSubButton(_subButtonTexture, _font)
      {
        Building = new SmallHouse(_gameScreen, _content.Load<Texture2D>("Buildings/SmallHouse/In"), _content.Load<Texture2D>("Buildings/SmallHouse/Out_Top"), _content.Load<Texture2D>("Buildings/SmallHouse/Out_Bottom"))
        {
          State = Buildings.BuildingStates.Placing,
        },
        Text = "Small House",
        Layer = 0.99f,
        GameScreenSetValue = States.GameStates.PlacingBuilding,
        ResourceCost = new VillageBackend.Models.Resources()
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
        IsRequired = true,
        PlacingObject = new Bed(_content.Load<Texture2D>("Furniture/Bed"), _gameScreen)
        {
          State = PlacableObjectStates.Placing,
          Position = GameScreen.Mouse.PositionWithCamera,
        },
      };

      bed.Click += Item_Click;

      var toilet = new ItemMenuButton(_mainButtonTexture, _font)
      {
        Text = "Toilet",
        Parent = smallHouse,
        PlacingObject = new Furniture(_content.Load<Texture2D>("Furniture/Toilet"), _gameScreen)
        {
          State = PlacableObjectStates.Placing,
          Position = GameScreen.Mouse.PositionWithCamera,
        },
      };

      toilet.Click += Item_Click;

      smallHouse.Items = new List<ItemMenuButton>()
      {
        bed,
        toilet,
      };

      smallHouse.Click += SubButton_Click;

      return smallHouse;
    }

    private BuildMenuSubButton GetTavern()
    {
      var tavern = new BuildMenuSubButton(_subButtonTexture, _font)
      {
        Building = new Tavern(_gameScreen, _content.Load<Texture2D>("Buildings/Tavern/In"), _content.Load<Texture2D>("Buildings/Tavern/Out_Top"), _content.Load<Texture2D>("Buildings/Tavern/Out_Bottom"))
        {
          State = Buildings.BuildingStates.Placing,
        },
        Text = "Tavern",
        Layer = 0.99f,
        GameScreenSetValue = States.GameStates.PlacingBuilding,
        ResourceCost = new VillageBackend.Models.Resources()
        {
          Food = 5,
          Gold = 1,
          Wood = 10,
          Stone = 10,
        },
      };

      var bar = new ItemMenuButton(_mainButtonTexture, _font)
      {
        Amount = 1,
        Text = "Bar",
        Parent = tavern,
        PlacingObject = new Bar(_content.Load<Texture2D>("Furniture/Bar"), _gameScreen)
        {
          State = PlacableObjectStates.Placing,
          Position = GameScreen.Mouse.PositionWithCamera,
        },
        CollisionRectangles = new List<Rectangle>()
        {
          new Rectangle()
        },
      };

      bar.Click += Item_Click;

      var stool = new ItemMenuButton(_mainButtonTexture, _font)
      {
        Amount = 9,
        Text = "Stool",
        Parent = tavern,
        PlacingObject = new Furniture(_content.Load<Texture2D>("Furniture/Stool"), _gameScreen)
        {
          State = PlacableObjectStates.Placing,
          Position = GameScreen.Mouse.PositionWithCamera,
        },
      };

      stool.Click += Item_Click;

      var booth = new ItemMenuButton(_mainButtonTexture, _font)
      {
        Amount = 2,
        Text = "Booth",
        Parent = tavern,
        PlacingObject = new Furniture(_content.Load<Texture2D>("Furniture/Booth"), _gameScreen)
        {
          State = PlacableObjectStates.Placing,
          Position = GameScreen.Mouse.PositionWithCamera,
        },
      };

      booth.Click += Item_Click;

      tavern.Items = new List<ItemMenuButton>()
      {
        bar,
        stool,
        booth,
      };

      tavern.Click += SubButton_Click;

      return tavern;
    }

    private void SubButton_Click(object sender, EventArgs e)
    {
      if (!(sender is BuildMenuSubButton))
        throw new Exception("This event can only be applied to a 'BuildMenuSubButton'.");

      var button = sender as BuildMenuSubButton;

      if (button.Building == null)
        throw new Exception($"Button '{button.Text}' doesn't have a building set.");

      _gameScreen.AddComponent(button.Building);
    }

    private void LabourButton_Click(object sender, EventArgs e)
    {
      var blacksmith = new BuildMenuSubButton(_subButtonTexture, _font)
      {
        Building = new Blacksmith(_gameScreen, _content.Load<Texture2D>("Buildings/Blacksmith/In"), _content.Load<Texture2D>("Buildings/Blacksmith/Out_Top"), _content.Load<Texture2D>("Buildings/Blacksmith/Out_Bottom"))
        {
          State = Buildings.BuildingStates.Placing,
        },
        Text = "Blacksmith",
        Layer = 0.99f,
        GameScreenSetValue = States.GameStates.PlacingBuilding,
        ResourceCost = new VillageBackend.Models.Resources()
        {
          Food = 15,
          Gold = 10,
          Wood = 30,
          Stone = 40,
        },
      };

      blacksmith.Click += SubButton_Click;

      var anvil = new ItemMenuButton(_mainButtonTexture, _font)
      {
        Text = "Anvil",
        Parent = blacksmith,
        //Layer = _background.Layer + 0.01f,
        PlacingObject = new Furniture(_content.Load<Texture2D>("Furniture/Anvil"), _gameScreen)
        {
          State = PlacableObjectStates.Placing,
          Position = GameScreen.Mouse.PositionWithCamera,
        },
      };

      anvil.Click += Item_Click;

      blacksmith.Items = new List<ItemMenuButton>()
      {
        anvil,
      };

      var farm = new BuildMenuSubButton(_subButtonTexture, _font)
      {
        Building = new Farm(_gameScreen, _content.Load<Texture2D>("Buildings/Farm/In"), _content.Load<Texture2D>("Buildings/Farm/Out_Top"), _content.Load<Texture2D>("Buildings/Farm/Out_Bottom"))
        {
          State = BuildingStates.Placing,
        },
        Text = "Farm",
        Layer = 0.99f,
        GameScreenSetValue = GameStates.PlacingBuilding,
        ResourceCost = new VillageBackend.Models.Resources()
        {
          Food = 15,
          Gold = 10,
          Wood = 30,
          Stone = 40,
        },
      };

      farm.Click += SubButton_Click;

      var mine = new BuildMenuSubButton(_subButtonTexture, _font)
      {
        Building = new Mine(_gameScreen, _content.Load<Texture2D>("Buildings/Mine/In"), _content.Load<Texture2D>("Buildings/Mine/Out_Top"), _content.Load<Texture2D>("Buildings/Mine/Out_Bottom"))
        {
          State = Buildings.BuildingStates.Placing,
        },
        Text = "Mine",
        Layer = 0.99f,
        GameScreenSetValue = States.GameStates.PlacingBuilding,
        ResourceCost = new VillageBackend.Models.Resources()
        {
          Food = 15,
          Gold = 10,
          Wood = 30,
          Stone = 40,
        },
      };

      mine.Click += SubButton_Click;

      _buildSubOptions = new List<BuildMenuSubButton>()
      {
        blacksmith,
        farm,
        new BuildMenuSubButton(_subButtonTexture, _font)
        {
          Text = "Lumber Mill",
          Layer =  0.99f,
          GameScreenSetValue = States.GameStates.PlacingBuilding,
          ResourceCost = new VillageBackend.Models.Resources()
          {
            Food = 15,
            Gold = 10,
            Wood = 30,
            Stone = 40,
          },
        },
        mine,
      };

      foreach (var component in _buildSubOptions)
        component.LoadContent(_content);
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      _mainButtonTexture = content.Load<Texture2D>("Controls/BuildMenuMainOptionButton");
      _subButtonTexture = content.Load<Texture2D>("Controls/BuildMenuSubOptionButton");

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

      _buttons = new List<Button>()
      {
        housingButton,
        labourButton,
        artsButton,
        miscButton,
      };

      foreach (var button in _buttons)
        button.LoadContent(content);

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
        ResourceCost = new VillageBackend.Models.Resources()
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
        PlacingObject = new Path(_content.Load<Texture2D>("Sprites/Paths/StonePath"))
        {
          IsCollidable = false,
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
      _gameScreen.AddComponent(new PathBuilder(_gameScreen)
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
      if (_gameScreen.State != States.GameStates.BuildMenu)
        return;

      foreach (var component in Components)
        component.Update(gameTime);

      foreach (var button in _buttons)
        button.Update(gameTime);

      foreach (var component in _buildSubOptions)
      {
        component.Update(gameTime);

        if (component.IsClicked)
        {
          _gameScreen.ItemMenu.Open(component);
        }
      }
    }
  }
}
