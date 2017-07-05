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
  public class Keyboard : Component
  {
    private Microsoft.Xna.Framework.Input.KeyboardState _currentKey;

    private Microsoft.Xna.Framework.Input.KeyboardState _previousKey;
    
    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {

    }

    public bool IsKeyPressed(Microsoft.Xna.Framework.Input.Keys key)
    {
      return _currentKey.IsKeyUp(key) && _previousKey.IsKeyDown(key);
    }

    public override void LoadContent(ContentManager content)
    {

    }

    public override void UnloadContent()
    {

    }

    public override void Update(GameTime gameTime)
    {
      _previousKey = _currentKey;
      _currentKey = Microsoft.Xna.Framework.Input.Keyboard.GetState();
    }
  }
}
