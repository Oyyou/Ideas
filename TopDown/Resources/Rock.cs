using Engine.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TopDown.States;
using TopDown.FX;
using Microsoft.Xna.Framework.Content;
using Engine;
using Microsoft.Xna.Framework.Audio;

namespace TopDown.Resources
{
  public class Rock : Sprite
  {
    private GameState _gameState;

    private const float _hitTimer = 0.3f;

    private Texture2D _rockParticleTexture;

    private SoundEffect _soundEffect;

    private float _timer = 0;

    public Models.Resources Resources { get; set; }

    private void GenerateRockParticle(float lifeTimer)
    {
      var position = new Vector2(
        GameEngine.Random.Next((int)Position.X, (int)Position.X + Rectangle.Width),
        GameEngine.Random.Next((int)Position.Y, (int)Position.Y + Rectangle.Height)
      );

      Components.Add(
        new RockParticle(_rockParticleTexture)
        {
          Position = position,
          Layer = this.Layer + 0.01f,
          LifeTimer = lifeTimer,
          Rotation = MathHelper.ToRadians(GameEngine.Random.Next(0, 360)),
        }
      );
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      Resources = new Models.Resources()
      {
        Stone = 10,
      };

      _rockParticleTexture = content.Load<Texture2D>("FX/RockParticle");

      _soundEffect = content.Load<SoundEffect>("Sounds/RockHit");
    }

    public Rock(Texture2D texture, GameState gameState) : base(texture)
    {
      _gameState = gameState;
    }

    public override void UnloadContent()
    {
      base.UnloadContent();

      _rockParticleTexture.Dispose();

      _soundEffect.Dispose();
    }

    public override void Update(GameTime gameTime)
    {
      Color = Color.White;

      if (GameState.Mouse.RectangleWithCamera.Intersects(this.Rectangle))
      {
        if (Vector2.Distance(this.Position, _gameState.Player.Position) < 100)
        {
          Color = Color.Yellow;

          if (GameState.Mouse.LeftDown)
          {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer > _hitTimer)
            {
              var positions = new List<Vector2>();

              for (int i = 0; i < 4; i++)
              {
                GenerateRockParticle(_hitTimer);
              }

              _timer = 0f;

              _soundEffect.Play();

              Resources.Stone--;
              _gameState.Resources.Stone++;

              if (Resources.Stone == 0)
                IsRemoved = true;
            }
          }
        }
        else
        {
          Color = Color.Red;
          _timer = 0;
        }
      }
      else
      {
        _timer = 0;
      }

      base.Update(gameTime);
    }
  }
}
