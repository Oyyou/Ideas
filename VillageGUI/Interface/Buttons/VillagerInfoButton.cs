﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace VillageGUI.Interface.Buttons
{
  public class VillagerInfoButton : Button
  {
    public VillagerInfoButton(Texture2D texture, SpriteFont font) : base(texture, font)
    {
    }

    protected override void DrawClicked()
    {
      CurrentState = ButtonStates.Hovering;
    }
  }
}
