using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Models
{
  public class GameModel
  {
    public ContentManager ContentManger { get; set; }

    public GameEngine Game { get; set; }

    public GraphicsDevice GraphicsDevice { get; set; }

    public SpriteBatch SpriteBatch { get; set; }

    public GameModel(ContentManager contentManger, GameEngine game, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
      ContentManger = contentManger;
      Game = game;
      GraphicsDevice = graphicsDevice;
      SpriteBatch = spriteBatch;
    }
  }
}