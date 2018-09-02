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
    public string FirstGId;

    [XmlAttribute("source")]
    public string Source;
  }
}
