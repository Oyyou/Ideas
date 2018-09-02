using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TiledReader
{
  [XmlRoot(ElementName = "layer")]
  public class Layer
  {
    [XmlAttribute("name")]
    public string Name;

    [XmlAttribute("width")]
    public string Width;

    [XmlAttribute("height")]
    public string Height;
  }
}
