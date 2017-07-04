using System.Xml.Serialization;
using Engine.TmxSharp.Collisions;
using Engine.TmxSharp.Tilesets.Animation;

namespace Engine.TmxSharp.Tilesets
{
    [XmlRoot(ElementName = "tile")]
    public class TilesetTile
    {
        [XmlAttribute("id")]
        public int ID;

        [XmlAttribute("terrain")]
        public string Terrain;

        [XmlAttribute("probability")]
        public float Probability = 1.0f;

        [XmlElement("objectgroup")]
        public CollisionObjectGroup[] ObjectGroup;

        [XmlArray("animation")]
        [XmlArrayItem(ElementName = "frame")]
        public Frame[] Frame;

        [XmlArray("properties")]
        [XmlArrayItem(ElementName = "property")]
        public Property[] Properties;
    }
}
