using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TiledReader
{
  [XmlRoot(ElementName = "data")]
  public class Data
  {
    [XmlAttribute("encoding")]
    public string Encoding;

    //public string Value;
  }
}
