using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TiledReader
{
  [XmlRoot(ElementName = "map")]
  public class TiledMap
  {
    [XmlAttribute("version")]
    public string Version;

    [XmlAttribute("tiledversion")]
    public string TiledVersion;

    [XmlAttribute("orientation")]
    public string Orientation;

    [XmlAttribute("renderorder")]
    public string RenderOrder;

    [XmlAttribute("width")]
    public int Width;

    [XmlAttribute("height")]
    public int Height;

    //[XmlAttribute("backgroundcolor")]
    //public string BackgroundColor;

    [XmlAttribute("tilewidth")]
    public int TileWidth;

    [XmlAttribute("tileheight")]
    public int TileHeight;

    [XmlAttribute("nextobjectid")]
    public int NextObjectID;

    [XmlElement("tileset")]
    public Tileset[] Tileset;

    [XmlElement("layer")]
    public Layer[] Layer;

    [XmlAttribute("data")]
    public string Data;

    [XmlElement("objectgroup")]
    public CollisionObjectGroup[] ObjectGroups;

    //[XmlElement("imagelayer")]
    //public ImageLayer ImageLayer;

    //[XmlArray("properties")]
    //[XmlArrayItem(ElementName = "property")]
    //public Property[] Properties;

    [XmlIgnore]
    public TiledMap Map = null;

    public void Save(string path)
    {
      var xml = new XmlSerializer(typeof(TiledMap));
      using (var stream = new StreamWriter(path))
      {
        xml.Serialize(stream, this);
      }
    }

    public static TiledMap Load(string path, string fileName)
    {
      // Deserialize it, and return the TmxMap instance.
      var xml = new XmlSerializer(typeof(TiledMap));
      using (var stream = new StreamReader($"{path}/{fileName}"))
      {
        var instance = (TiledMap)xml.Deserialize(stream);

        return instance;
      }
    }
  }
}

