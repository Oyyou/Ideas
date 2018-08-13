using Engine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Interface.Windows
{
  public abstract class Window : ICloneable, IClickable
  {
    protected ContentManager _content;

    protected SpriteFont _font;

    protected bool _hasUpdated;

    public bool Close { get; set; }

    public string Name { get; protected set; }

    public Vector2 Position { get; set; }

    public Rectangle Rectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
      }
    }

    public abstract Rectangle WindowRectangle { get; }

    public Texture2D Texture { get; protected set; }

    public float Layer { get; set; }

    public Window(ContentManager content)
    {
      Close = false;

      _content = content;

      Name = "Window";

      _font = content.Load<SpriteFont>("Fonts/Font");

      _hasUpdated = false;

      Texture = content.Load<Texture2D>("Interface/Window");

      Layer = 0.9f;
    }

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDeviceManager graphics);

    public void OnScreenResize()
    {
      SetPositions();
    }

    public abstract void SetPositions();

    public abstract void UnloadContent();

    public abstract void Update(GameTime gameTime);

    public object Clone()
    {
      return this.MemberwiseClone();
    }
  }
}
