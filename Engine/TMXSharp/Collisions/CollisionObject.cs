using System.Xml.Serialization;

namespace Engine.TmxSharp.Collisions
{
    [XmlRoot(ElementName = "object")]
    public class CollisionObject
    {
        [XmlAttribute("id")]
        public int ID;

        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("type")]
        public string Type;

        [XmlAttribute("x")]
        public float X;

        [XmlAttribute("y")]
        public float Y;

        [XmlAttribute("width")]
        public float Width;

        [XmlAttribute("height")]
        public float Height;

        [XmlAttribute("rotation")]
        public float Rotation;

        [XmlAttribute("visible")]
        public byte Visible = 1;

        [XmlArray("properties")]
        [XmlArrayItem(ElementName = "property")]
        public Property[] Properties;

        [XmlElement("ellipse")]
        public Ellipse Ellipse;

        [XmlElement("polyline")]
        public Polyline Polyline;
    }
}
