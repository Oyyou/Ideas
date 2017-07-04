using System.IO;
using System.Xml.Serialization;
using Engine.TmxSharp.Collisions;
using Engine.TmxSharp.Images;
using Engine.TmxSharp.Layers;
using Engine.TmxSharp.Tilesets;
using System;
using System.Threading.Tasks;

namespace Engine.TmxSharp
{
  [XmlRoot(ElementName = "map")]
  public class TmxMap
  {
    [XmlAttribute("version")]
    public string Version;

    [XmlAttribute("orientation")]
    public string Orientation;

    [XmlAttribute("renderorder")]
    public string RenderOrder;

    [XmlAttribute("width")]
    public int Width;

    [XmlAttribute("height")]
    public int Height;

    [XmlAttribute("backgroundcolor")]
    public string BackgroundColor;

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

    [XmlElement("objectgroup")]
    public CollisionObjectGroup[] ObjectGroups;

    [XmlElement("imagelayer")]
    public ImageLayer ImageLayer;

    [XmlArray("properties")]
    [XmlArrayItem(ElementName = "property")]
    public Property[] Properties;

    [XmlIgnore]
    public TmxMap Map = null;

    public void Save(string path)
    {
      var xml = new XmlSerializer(typeof(TmxMap));
      using (var stream = new StreamWriter(path))
      {
        xml.Serialize(stream, this);
      }
    }

    public static TmxMap Load(string path)
    {
      // Deserialize it, and return the TmxMap instance.
      var xml = new XmlSerializer(typeof(TmxMap));
      using (var stream = new StreamReader(path))
      {
        var instance = (TmxMap)xml.Deserialize(stream);
        return instance;
      }
    }
  }
}
