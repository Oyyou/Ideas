using Ideas.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ideas.States
{
  public class SideScroller : State
  {
    private List<Component> _guiComponents;

    public override void Draw(GameTime gameTime)
    {
      foreach (var component in _guiComponents)
        component.Draw(gameTime, _spriteBatch);
    }

    public override void LoadContent(GameModel gameModel)
    {
      base.LoadContent(gameModel);

      foreach (var component in _guiComponents)
        component.LoadContent(_content);
    }

    public SideScroller()
    {

    }

    public override void UnloadContent()
    {
      foreach (var component in _guiComponents)
        component.UnloadContent();

      _guiComponents.Clear();
    }

    public override void Update(GameTime gameTime)
    {
      foreach (var component in _guiComponents)
        component.Update(gameTime);
    }
  }
}
