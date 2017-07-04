using System.Xml.Serialization;

namespace Engine.TmxSharp.Collisions
{
    [XmlRoot(ElementName = "polyline")]
    public class Polyline
    {
        [XmlAttribute("points")]
        public string Points;
    }
}
