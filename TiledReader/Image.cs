using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TiledReader
{
  [XmlRoot(ElementName = "image")]
  public class Image
  {
    [XmlAttribute("source")]
    public string Source;

    [XmlAttribute("width")]
    public int Width;

    [XmlAttribute("height")]
    public int Height;
  }
}
