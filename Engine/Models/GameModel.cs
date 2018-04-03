using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Models
{
  public class GameModel
  {
    public ContentManager ContentManger { get; set; }

    public GameEngine Game { get; set; }

    public GraphicsDeviceManager GraphicsDeviceManager { get; set; }

    public SpriteBatch SpriteBatch { get; set; }

    public GameModel()
    {

    }
  }
}