using System.Xml.Serialization;

namespace Engine.TmxSharp.Images
{
    [XmlRoot(ElementName = "image")]
    public class Image
    {
        [XmlAttribute("source")]
        public string Source;

        [XmlAttribute("trans")]
        public string Trans;

        [XmlAttribute("width")]
        public int Width;

        [XmlAttribute("height")]
        public int Height;
    }
}
