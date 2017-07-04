using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ideas.Models
{
  public class GameModel
  {
    public ContentManager ContentManger { get; set; }

    public Game1 Game { get; set; }

    public GraphicsDevice GraphicsDevice { get; set; }

    public SpriteBatch SpriteBatch { get; set; }

    public GameModel(ContentManager contentManger, Game1 game, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
      ContentManger = contentManger;
      Game = game;
      GraphicsDevice = graphicsDevice;
      SpriteBatch = spriteBatch;
    }
  }
}