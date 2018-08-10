using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TopDown.Controls
{
  public class ResourceView : Component
  {
    private SpriteFont _font;

    private VillageBackend.Models.Resources _resources;

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.DrawString(_font, "Wood: " + _resources.Wood, new Vector2(10, 5), Color.Red);
      spriteBatch.DrawString(_font, "Stone: " + _resources.Stone, new Vector2(10, 25), Color.Red);
    }

    public override void LoadContent(ContentManager content)
    {
      _font = content.Load<SpriteFont>("Fonts/Font");
    }

    public ResourceView(VillageBackend.Models.Resources resources)
    {
      _resources = resources;
    }

    public override void UnloadContent()
    {
      
    }

    public override void Update(GameTime gameTime)
    {

    }
  }
}
