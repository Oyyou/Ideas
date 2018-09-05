using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Utilities
{
  public class FrameCounter : Component
  {
    private SpriteFont _font;

    public FrameCounter()
    {
      new Vector2(GameEngine.ScreenWidth - 100, GameEngine.ScreenHeight - 20);
    }

    public long TotalFrames { get; private set; }

    public float TotalSeconds { get; private set; }

    public float AverageFramesPerSecond { get; private set; }

    public float CurrentFramesPerSecond { get; private set; }

    public const int MAXIMUM_SAMPLES = 100;

    private Queue<float> _sampleBuffer = new Queue<float>();

    public override void Update(GameTime gameTime)
    {
      var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      CurrentFramesPerSecond = 1.0f / deltaTime;

      _sampleBuffer.Enqueue(CurrentFramesPerSecond);

      if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
      {
        _sampleBuffer.Dequeue();
        AverageFramesPerSecond = _sampleBuffer.Average(i => i);
      }
      else
      {
        AverageFramesPerSecond = CurrentFramesPerSecond;
      }

      TotalFrames++;
      TotalSeconds += deltaTime;
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      var fps = string.Format("FPS: {0:00}", AverageFramesPerSecond);


      spriteBatch.DrawString(_font, fps, Position, Color.Black);

      // other draw code here
    }

    public override void LoadContent(ContentManager content)
    {
      _font = content.Load<SpriteFont>("Fonts/Font");
    }

    public override void UnloadContent()
    {

    }
  }
}
