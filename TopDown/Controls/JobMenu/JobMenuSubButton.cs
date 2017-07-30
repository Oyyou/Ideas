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
    public Button Add;

    public Button Minus;

    protected override Color _colour
    {
      get
      {
        if (IsJobSelected)
          return Color.Green;

        if (NPC.Workplace != null)
          return Color.Red;

        if (!IsEnabled)
          return Color.Black;

        if (IsSelected)
          return Color.Yellow;

        if (IsHovering)
          return Color.Gray;

        return Color.White;
      }
    }

    public NPC NPC { get; private set; }

    /// <summary>
    /// If the NPC has a job, and the job is 'clicked' on the job menu, change colour
    /// </summary>
    public bool IsJobSelected { get; set; }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Draw(_texture, Rectangle, null, _colour, 0, new Vector2(0, 0), SpriteEffects.None, Layer);

      DrawNPCIcon(gameTime, spriteBatch);

      foreach (var component in Components)
        component.Draw(gameTime, spriteBatch);

      if (_font != null)
      {
        spriteBatch.DrawString(_font, NPC.Name, new Vector2(Position.X + 42, Position.Y + 5), Color.Black, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
        spriteBatch.DrawString(_font, NPC.Job, new Vector2(Position.X + 42, Position.Y + 25), Color.Black, 0, new Vector2(0, 0), 1, SpriteEffects.None, Layer + 0.001f);
      }

    }

    private void DrawNPCIcon(GameTime gameTime, SpriteBatch spriteBatch)
    {
      var scale = NPC.DisplaySprite.Scale;

      var width = NPC.DisplaySprite.SourceRectangle.Width * scale;
      var height = NPC.DisplaySprite.SourceRectangle.Height * scale;

      var x = (Position.X + 5) + 16 - (width / 2);
      var y = (Position.Y + 5) + 16 - (height / 2);

      NPC.DisplaySprite.Position = new Vector2(x, y);

      NPC.DisplaySprite.Layer = Layer + 0.001f;

      NPC.DisplaySprite.Draw(gameTime, spriteBatch);
    }

    public JobMenuSubButton(Texture2D texture, SpriteFont font, NPC npc) : base(texture, font)
    {
      NPC = npc;
    }

    public override void LoadContent(ContentManager content)
    {
      base.LoadContent(content);

      var addTexture = content.Load<Texture2D>("Controls/Add");

      var minusTexture = content.Load<Texture2D>("Controls/Minus");

      var position = new Vector2(Rectangle.Right - addTexture.Width - 5, Rectangle.Top + 5);

      Add = new Button(addTexture)
      {
        Position = position,
        Layer = Layer + 0.001f,
      };

      Add.Click += Add_Click;

      Minus = new Button(minusTexture)
      {
        Position = position,
        Layer = Layer + 0.001f,
        IsVisible = false,
      };

      Minus.Click += Minus_Click;

      Components.Add(Add);
      Components.Add(Minus);

      foreach (var component in Components) 
        component.LoadContent(content);
    }

    private void Minus_Click(object sender, EventArgs e)
    {
      NPC.Unemploy();
    }

    private void Add_Click(object sender, EventArgs e)
    {
      NPC.AssignJob();
    }
  }
}
