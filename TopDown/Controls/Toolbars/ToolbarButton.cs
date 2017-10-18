using Engine.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace TopDown.Controls.Toolbars
{
  public class ToolbarButton : Button
  {
    protected override void OnHover()
    {
      Scale = 2.0f;
    }

    protected override void OffHover()
    {
      Scale = 1.0f;
    }

    public ToolbarButton(Texture2D texture) : base(texture)
    {

    }
  }
}
