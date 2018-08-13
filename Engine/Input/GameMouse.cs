using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Input
{
  public static class GameMouse
  {
    /// <summary>
    /// These are objects the mouse is currently hovering over
    /// </summary>
    public static List<IClickable> ClickableObjects = new List<IClickable>();

    public static MouseState CurrentMouse;
    public static MouseState PreviousMouse;

    public static bool Clicked
    {
      get
      {
        return CurrentMouse.LeftButton == ButtonState.Released && PreviousMouse.LeftButton == ButtonState.Pressed;
      }
    }

    public static IClickable ClickedObject
    {
      get
      {
        if (!Clicked)
          return null;

        return ClickableObjects.OrderBy(c => c.Layer).Last();
      }
    }

    public static Rectangle Rectangle
    {
      get
      {
        return new Rectangle((int)CurrentMouse.X, (int)CurrentMouse.Y, 1, 1);
      }
    }

    public static void Update()
    {
      PreviousMouse = CurrentMouse;
      CurrentMouse = Mouse.GetState();
    }
  }
}
