using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
  public abstract class Component
  {
    public virtual List<Rectangle> CollisionRectangles { get; protected set; }

    public List<Component> Components;

    public bool IsEnabled { get; set; }

    public bool IsRemoved { get; set; }

    public abstract void CheckCollision(Component component);

    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    public abstract void LoadContent(ContentManager content);

    public void PostUpdate(GameTime gameTime)
    {
      for (int i = 0; i < Components.Count; i++)
      {
        if (Components[i].IsRemoved)
        {
          Components.RemoveAt(i);
          i--;
        }
      }
    }

    public abstract void UnloadContent();

    public abstract void Update(GameTime gameTime);
  }
}
