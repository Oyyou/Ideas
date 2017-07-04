using System.Xml.Serialization;

namespace Engine.TmxSharp.Images
{
    [XmlRoot(ElementName = "imagelayer")]
    public class ImageLayer
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("x")]
        public int X;

        [XmlAttribute("y")]
        public int Y;

        [XmlAttribute("visible")]
        public byte Visible = 1;

        [XmlAttribute("opacity")]
        public float Opacity;

        [XmlElement("image")]
        public Image Image;

        [XmlArray("properties")]
        [XmlArrayItem(ElementName = "property")]
        public Property[] Properties;
    }
}
