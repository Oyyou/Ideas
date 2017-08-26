using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TopDown.Controls
{
  public class Notifications : Component
  {
    public class Notification
    {
      public string BodyText { get; set; }

      public string HeaderText { get; set; }

      public float Timer { get; set; }
    }

    private SpriteFont _font;

    private List<Notification> _notifications;

    private Texture2D _texture;

    public void Add(DateTime time, string text)
    {
      var notification = new Notification()
      {
        BodyText = text,
        HeaderText = time.ToString("dd/MM/yy - hh:mm"),
      };

      _notifications.Add(notification);
    }

    public override void CheckCollision(Component component)
    {

    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      var y = 20;

      var x = Game1.ScreenWidth - _texture.Width - 20;

      // Loop around them backwards so the latest notification is at the top of the screen
      for (int i = _notifications.Count - 1; i >= 0; i--)
      {
        var notification = _notifications[i];

        spriteBatch.Draw(_texture, new Vector2(x, y), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0.99f);

        spriteBatch.DrawString(_font, notification.HeaderText, new Vector2(x + 5, y + 5), Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.991f);

        var words = notification.BodyText.Split(' ');
        var bodyText = "";

        foreach (var word in words)
        {
          if (_font.MeasureString(bodyText + word).X > _texture.Width - 10)
            bodyText += "\n";

          bodyText += word + " ";
        }        

        spriteBatch.DrawString(_font, bodyText, new Vector2(x + 5, y + 25), Color.Black, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0.991f);

        y += _texture.Height + 5;
      }
    }

    public override void LoadContent(ContentManager content)
    {
      _notifications = new List<Notification>();

      _font = content.Load<SpriteFont>("Fonts/Font");

      _texture = content.Load<Texture2D>("Controls/Notification");
    }

    public override void UnloadContent()
    {
      _notifications.Clear();
    }

    public override void Update(GameTime gameTime)
    {
      for (int i = 0; i < _notifications.Count; i++)
      {
        _notifications[i].Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_notifications[i].Timer > 5)
        {
          _notifications.RemoveAt(i);
          i--;
        }
      }
    }
  }
}
