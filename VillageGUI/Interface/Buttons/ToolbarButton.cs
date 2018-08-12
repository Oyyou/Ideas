using VillageGUI.Interface.Buttons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageGUI.Interface.Buttons
{
  public enum ButtonStates
  {
    Nothing,
    Hovering,
    Clicked,
  }

  public class ToolbarButton : Button
  {
    /// <summary>
    /// The key that activates the button
    /// </summary>
    public readonly Keys OpenKey;

    public ToolbarButton(Texture2D texture, Keys openKey)
      : base(texture)
    {
      OpenKey = openKey;
    }
  }
}
