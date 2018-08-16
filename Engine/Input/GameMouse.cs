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
    private static Matrix _transform = Matrix.Identity;

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

    /// <summary>
    /// The single object we're able to click
    /// </summary>
    public static IClickable ValidObject
    {
      get
      {
        return ClickableObjects.OrderBy(c => c.Layer).LastOrDefault();
      }
    }

    public static Rectangle Rectangle
    {
      get
      {
        return new Rectangle((int)CurrentMouse.X, (int)CurrentMouse.Y, 1, 1);
      }
    }

    public static Rectangle RectangleWithCamera
    {
      get
      {
        if (_transform == Matrix.Identity)
          return Rectangle;

        var scale = Matrix.Invert(_transform).Scale;
        var translation = _transform.Translation;

        var rectangle = new Rectangle(
          (int)((CurrentMouse.X - translation.X) * scale.X),
          (int)((CurrentMouse.Y - translation.Y) * scale.Y),
          1,
          1
        );

        return rectangle;
      }
    }

    public static void AddObject(IClickable clickableObject)
    {
      if (!ClickableObjects.Contains(clickableObject))
        ClickableObjects.Add(clickableObject);
    }

    public static void Update()
    {
      PreviousMouse = CurrentMouse;
      CurrentMouse = Mouse.GetState();
    }

    public static void Update(Matrix transform)
    {
      Update();

      _transform = transform;
    }
  }
}
