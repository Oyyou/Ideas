﻿using Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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

  public class Button : Control, IClickable
  {
    public Action<Button> Click;

    public bool Clicked;

    public Color Color;

    public ButtonStates CurrentState;

    public Func<bool> IsClickable;

    public readonly SpriteFont Font;

    public readonly Vector2 Origin;

    public Color PenColor;

    public override Vector2 Position { get; set; }

    public ButtonStates PreviousState;

    public override Rectangle Rectangle
    {
      get
      {
        var width = Texture.Width * Scale;
        var height = Texture.Height * Scale;

        return new Rectangle((int)(Position.X - (width / 2)), (int)(Position.Y - (height / 2)), (int)width, (int)height);
      }
    }

    public float Scale;

    public string Text { get; set; }

    public float Layer { get; set; }

    public readonly Texture2D Texture;

    public Button(Texture2D texture)
    {
      Texture = texture;

      Color = Color.White;

      Origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

      PenColor = Color.Black;

      Scale = 1f;

      CurrentState = ButtonStates.Nothing;

      Layer = 0.1f;
    }

    public Button(Texture2D texture, SpriteFont font) : this(texture)
    {
      Font = font;
    }

    public virtual void Update(Rectangle mouseRectangle, IEnumerable<Button> buttons)
    {
      this.Update(mouseRectangle, buttons, null, null);
    }

    public void Update(Rectangle mouseRectangle, IEnumerable<Button> buttons, Rectangle? mouseRectangleWithCamera = null, Rectangle? windowRectangle = null)
    {
      Clicked = false;

      bool isHovering = mouseRectangle.Intersects(this.Rectangle);

      if (mouseRectangleWithCamera != null && windowRectangle != null)
        isHovering = mouseRectangleWithCamera.Value.Intersects(this.Rectangle) && mouseRectangle.Intersects(windowRectangle.Value);

      switch (this.CurrentState)
      {
        case ButtonStates.Nothing:

          if (isHovering)
          {
            GameMouse.AddObject(this);
            if (GameMouse.ValidObject == this)
            {
              this.CurrentState = ButtonStates.Hovering;
            }
            else
            {
              GameMouse.ClickableObjects.Remove(this);
            }
          }

          break;
        case ButtonStates.Hovering:

          if (!isHovering || GameMouse.ValidObject != this)
          {
            GameMouse.ClickableObjects.Remove(this);
            this.CurrentState = ButtonStates.Nothing;
            break;
          }

          if (GameMouse.Clicked && (IsClickable != null && IsClickable()))
          {
            foreach (var b in buttons)
            {
              GameMouse.ClickableObjects.Remove(this);
              b.CurrentState = ButtonStates.Nothing;
            }

            this.OnClick();
            Clicked = true;
          }

          break;

        case ButtonStates.Clicked:

          if (isHovering)
          {
            GameMouse.AddObject(this);
          }
          else
          {
            GameMouse.ClickableObjects.Remove(this);
          }

          break;

        default:
          throw new Exception("Unknown ButtonState: " + this.CurrentState.ToString());
      }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      switch (this.CurrentState)
      {
        case ButtonStates.Nothing:

          this.Color = Color.White;

          this.Scale = 1.0f;

          break;
        case ButtonStates.Hovering:

          this.Color = Color.YellowGreen;

          this.Scale = 1.0f;

          break;
        case ButtonStates.Clicked:

          DrawClicked();

          break;
        default:
          throw new Exception("Unknown ToolbarButtonState: " + this.CurrentState.ToString());
      }

      spriteBatch.Draw(Texture, Position, null, Color, 0f, Origin, Scale, SpriteEffects.None, Layer);

      DrawText(spriteBatch);
    }

    protected virtual void DrawClicked()
    {
      this.Color = Color.YellowGreen;

      this.Scale = 1.05f;
    }

    protected virtual void DrawText(SpriteBatch spriteBatch)
    {
      if (string.IsNullOrEmpty(Text) || Font == null)
        return;

      float x = (Rectangle.X + (Rectangle.Width / 2)) - (Font.MeasureString(Text).X / 2);
      float y = (Rectangle.Y + (Rectangle.Height / 2)) - (Font.MeasureString(Text).Y / 2);

      spriteBatch.DrawString(Font, Text, new Vector2(x, y), PenColor, 0, new Vector2(0, 0), Scale, SpriteEffects.None, Layer + 0.01f);
    }

    public void OnClick()
    {
      CurrentState = ButtonStates.Clicked;
      Click?.Invoke(this);
    }

    public override void UnloadContent()
    {
      Texture.Dispose();
    }
  }
}
