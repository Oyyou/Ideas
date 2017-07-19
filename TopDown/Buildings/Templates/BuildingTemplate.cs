using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopDown.Buildings.Templates
{
  public class BuildingTemplate
  {
    /// <summary>
    /// The position of the door
    /// </summary>
    public Vector2 DoorPosition { get; set; }

    /// <summary>
    /// The width of the door
    /// </summary>
    public int DoorWidth { get; set; }

    /// <summary>
    /// The height difference between the inside and outside texture
    /// </summary>
    public int OutExtraHeight { get; set; }

    /// <summary>
    /// The width difference between the inside and outside texture
    /// </summary>
    public int OutExtraWidth { get; set; }

    /// <summary>
    /// The texture that appears when we're inside the building
    /// </summary>
    public Texture2D TextureIn { get; set; }

    /// <summary>
    /// The texture that appears when we're outside the building
    /// </summary>
    public Texture2D TextureOut { get; set; }
  }
}
