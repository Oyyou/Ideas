using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using VillageBackend.Models;

namespace VillageGUI.Interface.Buttons
{
  public class JobButton : Button
  {
    public Job Job { get; set; }

    public JobButton(Texture2D texture, SpriteFont font)
      : base(texture, font)
    {
    }
  }
}
