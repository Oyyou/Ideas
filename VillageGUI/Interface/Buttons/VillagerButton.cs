using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using VillageBackend.Models;
using static VillageBackend.Enums;

namespace VillageGUI.Interface.Buttons
{
  public class VillagerButton : Button
  {
    public ItemCategories Category { get; set; }

    public Villager Villager { get; set; }

    public VillagerButton(Texture2D texture, SpriteFont font) : base(texture, font)
    {
    }
  }
}
