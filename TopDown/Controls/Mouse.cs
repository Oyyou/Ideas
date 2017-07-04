using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TopDown.Core;
using Engine.Sprites;

namespace TopDown.Controls
{
  public class Mouse : Component
  {
    private Camera _camera;

    private Microsoft.Xna.Framework.Input.MouseState _currentMouse;

    private Sprite _pickaxeCursor;

    private Microsoft.Xna.Framework.Input.MouseState _previousMouse;

    public bool LeftClicked
    {
      get
      {
        return _currentMouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released &&
          _previousMouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
      }
    }

    public bool LeftDown
    {
      get
      {
        return _currentMouse.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
      }
    }

    public Rectangle Rectangle
    {
      get
      {
        return new Rectangle(_currentMouse.Position, new Point(1, 1));
      }
    }
    public Rectangle RectangleWithCamera
    {
      get
      {
        return new Rectangle(
          _currentMouse.X + ((int)_camera.Position.X - (GameEngine.ScreenWidth / 2)),
          _previousMouse.Y + ((int)_camera.Position.Y - (GameEngine.ScreenHeight / 2)),
          1,
          1
        );
      }
    }

    public bool RightClicked
    {
      get
      {
        return _currentMouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Released &&
          _previousMouse.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed;
      }
    }

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      _pickaxeCursor.Draw(gameTime, spriteBatch);
    }

    public override void LoadContent(ContentManager content)
    {
      var pickaxeTexture = content.Load<Texture2D>("Cursors/Piackaxe");

      _pickaxeCursor = new Sprite(pickaxeTexture)
      {
        Origin = new Vector2(pickaxeTexture.Width / 2, pickaxeTexture.Height / 2),
        Layer = 1f,
      };
    }

    public Mouse(Camera camera)
    {
      _camera = camera;
    }

    public override void UnloadContent()
    {

    }

    public override void Update(GameTime gameTime)
    {
      _previousMouse = _currentMouse;
      _currentMouse = Microsoft.Xna.Framework.Input.Mouse.GetState();

      _pickaxeCursor.Position = _currentMouse.Position.ToVector2();
    }
  }
}
