using Engine.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using TopDown.Buildings;

namespace TopDown.Controls.JobMenu
{
  public class JobMenuButton : Button
  {
    public Building JobBuilding { get; set; }

    public JobMenuButton(Texture2D texture, SpriteFont font) : base(texture, font)
    {
    }
  }
}
