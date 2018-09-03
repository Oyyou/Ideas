using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TiledReader
{
  [XmlRoot(ElementName = "tileset")]
  public class Tileset
  {
    [XmlAttribute("firstgid")]
    public int FirstGId;

    [XmlAttribute("name")]
    public string Name;

    [XmlAttribute("tilewidth")]
    public int TileWidth;

    [XmlAttribute("tileheight")]
    public int TileHeight;

    [XmlAttribute("tilecount")]
    public int TileCount;

    [XmlAttribute("columns")]
    public int Columns;

    [XmlElement("tileoffset")]
    public TileOffset TileOffset;

    [XmlElement("image")]
    public Image Image;
  }
}
