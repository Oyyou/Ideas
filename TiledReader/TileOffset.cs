using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TiledReader
{
  [XmlRoot(ElementName = "tileoffset")]
  public class TileOffset
  {
    [XmlAttribute("x")]
    public int X;

    [XmlAttribute("y")]
    public int Y;
  }
}
