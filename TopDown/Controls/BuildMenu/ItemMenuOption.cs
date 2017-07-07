using Engine.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace TopDown.Controls.BuildMenu
{
  public enum ItemMenuOptionStates
  {
    Clickable,
    Clicked,
    Placed,
  }

  public class ItemMenuOption : Button
  {
    public ItemMenuOptionStates State { get; set; }

    public ItemMenuOption(Texture2D texture) : base(texture)
    {
      State = ItemMenuOptionStates.Clickable;
    }
  }
}
