using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBackend.Models;
using VillageGUI.Interface.Buttons;

namespace VillageGUI.Interface.Combat
{
  public class HeroButton : Button
  {
    public int SelectedHeroIndex;

    public Keys OpenKey { get; private set; }

    public Villager Villager;

    public HeroButton(Texture2D texture, Keys openKey) : base(texture)
    {
      OpenKey = openKey;
    }
  }
}
