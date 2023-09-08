using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Tile
    {
        public enum TileType
        {
            Wall, // '|', '-'
            Walkable, // ' '
            Player, // '@'
            Enemy, // 'M'
            Treasure, // 'C'
            Entry, // 'E'
            Exit, // 'X'
            SHOP//'S'
        }
        public TileType Type { get; set; }
        public Position Position { get; set; }

        public Tile(TileType type, Position position)
        {
            Type = type;
            Position = position;
        }
    }
}
