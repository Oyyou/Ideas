using System.Xml.Serialization;

namespace Engine.TmxSharp.Tilesets.Animation
{
    [XmlRoot(ElementName = "frame")]
    public class Frame
    {
        [XmlAttribute("tileid")]
        public int TileID;

        [XmlAttribute("duration")]
        public int Duration;
    }
}
