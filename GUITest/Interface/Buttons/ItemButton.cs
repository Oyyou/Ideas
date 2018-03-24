using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using VillageBackend.Models;

namespace GUITest.Interface.Buttons
{
  public class ItemButton : Button
  {
    public readonly ItemV2 Item;

    public ItemButton(Texture2D texture, ItemV2 item) : base(texture)
    {
      Item = item;
    }
  }
}
