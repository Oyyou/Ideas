using System.Xml.Serialization;

namespace Engine.TmxSharp.Collisions
{
    [XmlRoot(ElementName = "objectgroup")]
    public class CollisionObjectGroup
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("draworder")]
        public string DrawOrder;

        [XmlAttribute("opacity")]
        public float Opacity;

        [XmlAttribute("offsetx")]
        public int OffsetX;

        [XmlAttribute("offsety")]
        public int OffsetY;

        [XmlAttribute("visible")]
        public byte Visible = 1;

        [XmlElement("object")]
        public CollisionObject[] CollisionObjects;

        [XmlArray("properties")]
        [XmlArrayItem(ElementName = "property")]
        public Property[] Properties;
    }
}
