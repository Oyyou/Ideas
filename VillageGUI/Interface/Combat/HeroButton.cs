using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageGUI.Interface.Buttons;

namespace VillageGUI.Interface.Combat
{
  public class HeroButton : Button
  {
    public int SelectedHeroIndex;

    public HeroButton(Texture2D texture) : base(texture)
    {
    }
  }
}
