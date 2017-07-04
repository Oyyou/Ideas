using System.Xml.Serialization;
using Engine.TmxSharp.Images;

namespace Engine.TmxSharp.Tilesets
{
    [XmlRoot(ElementName = "tileset")]
    public class Tileset
    {
        [XmlAttribute("firstgid")]
        public int FirstGID;

        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("tilewidth")]
        public int TileWidth;

        [XmlAttribute("tileheight")]
        public int TileHeight;

        [XmlAttribute("tilecount")]
        public int TileCount;

        [XmlElement("image")]
        public Image Image;

        [XmlElement("tile", Type = typeof(TilesetTile))]
        public TilesetTile[] Tile;

        [XmlArray("properties")]
        [XmlArrayItem(ElementName = "property")]
        public Property[] Properties;

        [XmlArray("terraintypes")]
        [XmlArrayItem(ElementName = "terrain")]
        public Terrain[] TerrainTypes;
    }
}
