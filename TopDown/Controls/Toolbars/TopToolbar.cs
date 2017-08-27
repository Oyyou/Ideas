﻿using Engine;
using Engine.Controls;
using Engine.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.States;

namespace TopDown.Controls.Toolbars
{
  public class TopToolbar : Toolbar
  {
    private void BuildButton_Click(object sender, EventArgs e)
    {
      _gameScreen.State = GameStates.BuildMenu;
    }

    private void CraftButton_Click(object sender, EventArgs e)
    {
      _gameScreen.State = GameStates.CraftingMenu;
    }

    private void InventoryButton_Click(object sender, EventArgs e)
    {
      _gameScreen.State = GameStates.InventoryMenu;
    }

    private void JobsButton_Click(object sender, EventArgs e)
    {
      _gameScreen.State = GameStates.JobMenu;
    }

    public override void LoadContent(ContentManager content)
    {
      _toolbarSprite = new Sprite(content.Load<Texture2D>("Controls/Toolbar_Top"));

      var buildButton = new Button(content.Load<Texture2D>("Controls/Icons/Build"));
      buildButton.Click += BuildButton_Click;

      var craftButton = new Button(content.Load<Texture2D>("Controls/Icons/Crafting"));
      craftButton.Click += CraftButton_Click;

      var jobsButton = new Button(content.Load<Texture2D>("Controls/Icons/Jobs"));
      jobsButton.Click += JobsButton_Click;

      var inventoryButton = new Button(content.Load<Texture2D>("Controls/Icons/Inventory"));
      inventoryButton.Click += InventoryButton_Click;

      _icons = new List<Button>()
      {
        buildButton,
        craftButton,
        jobsButton,
        inventoryButton,
      };

      InitializeIcons(content);
    }

    public TopToolbar(GameScreen gameScreen) : base(gameScreen)
    {
    }

    public override void Update(GameTime gameTime)
    {
      if (_gameScreen.State != GameStates.Playing)
        return;

      _toolbarSprite.Update(gameTime);

      foreach (var icon in _icons)
        icon.Update(gameTime);
      
      _toolbarSprite.Position = new Vector2((GameEngine.ScreenWidth / 2) - (_toolbarSprite.Rectangle.Width / 2), 20);
    }
  }
}
