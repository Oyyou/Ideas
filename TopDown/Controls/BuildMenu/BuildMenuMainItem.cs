using Engine.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TopDown.Controls.BuildMenu
{
  public class BuildMenuMainItem : Button
  {
    public List<BuildMenuSubItem> SubItems;
    
    public BuildMenuMainItem(Texture2D texture, SpriteFont font)
      : base(texture, font)
    {
      SubItems = new List<BuildMenuSubItem>();
    }
  }
}
