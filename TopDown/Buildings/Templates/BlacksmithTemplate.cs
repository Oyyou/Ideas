using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDown.Buildings.Templates
{
  public class BlacksmithTemplate : BuildingTemplate
  {
    public BlacksmithTemplate(ContentManager content)
    {
      DoorPosition = new Microsoft.Xna.Framework.Vector2(1, 64);

      DoorWidth = 62;

      OutExtraHeight = 64;

      OutExtraWidth = 40;

      TextureIn = content.Load<Texture2D>("Buildings/Blacksmith/In");

      TextureOut = content.Load<Texture2D>("Buildings/Blacksmith/Out");
    }
  }
}
