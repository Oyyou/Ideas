using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDown.Buildings.Templates
{
  public class SmallHouseTemplate : BuildingTemplate
  {
    public SmallHouseTemplate(ContentManager content)
    {
      OutExtraHeight = 128;

      OutExtraWidth = 40;

      TextureIn = content.Load<Texture2D>("Buildings/SmallHouse/In");

      TextureOut = content.Load<Texture2D>("Buildings/SmallHouse/Out");
    }
  }
}
