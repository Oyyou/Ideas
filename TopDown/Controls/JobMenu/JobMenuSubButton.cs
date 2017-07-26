using Engine.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDown.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace TopDown.Controls.JobMenu
{
  public class JobMenuSubButton : Button
  {
    private Button _add;

    private NPC _npc;

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Rectangle, null, _colour, 0, new Vector2(0, 0), SpriteEffects.None, Layer);

      DrawNPCIcon(gameTime, spriteBatch);

      foreach (var component in Components)
        component.Draw(gameTime, spriteBatch);

      if (_font != null)
      {
        spriteBatch.DrawString(_font, _npc.Name, new Vector2(Position.X + 42, Position.Y + 5), Color.Black, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
        spriteBatch.DrawString(_font, _npc.Job, new Vector2(Position.X + 42, Position.Y + 25), Color.Black, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
      }

    }

    private void DrawNPCIcon(GameTime gameTime, SpriteBatch spriteBatch)
    {
      var scale = _npc.DisplaySprite.Scale;

      var width = _npc.DisplaySprite.SourceRectangle.Width * scale;
      var height = _npc.DisplaySprite.SourceRectangle.Height * scale;

      var x = (Position.X + 5) + 16 - (width / 2);
      var y = (Position.Y + 5) + 16 - (height / 2);

      _npc.DisplaySprite.Position = new Vector2(x, y);

      _npc.DisplaySprite.Layer = Layer + 0.001f;

      _npc.DisplaySprite.Draw(gameTime, spriteBatch);
    }

    public JobMenuSubButton(Texture2D texture, SpriteFont font, NPC npc) : base(texture, font)
    {
      _npc = npc;
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      var addTexture = content.Load<Texture2D>("Controls/Add");

      _add = new Button(addTexture)
      {
        Position = new Vector2(Rectangle.Right - addTexture.Width - 5, Rectangle.Top + 5),
        Layer = Layer + 0.001f,
      };

      _add.Click += Add_Click;

      Components.Add(_add);

      foreach (var component in Components)
        component.LoadContent(content);
    }

    private void Add_Click(object sender, EventArgs e)
    {
      _npc.AssignJob();
    }
  }
}
