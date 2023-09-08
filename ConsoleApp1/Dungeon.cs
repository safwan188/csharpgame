using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp1.Tile;

namespace ConsoleApp1
{
    internal class Dungeon
    {
        public List<Interactable> Interactables { get; set; }
        public Tile[,] Map { get; set; }
        private Random rand = new Random();
        public Position Entry { get; set; }
        public Position Exit { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Dungeon(string filePath, int level)
        {
            Interactables = new List<Interactable>();

            string[] lines = File.ReadAllLines(filePath);
            Height = lines.Length;
            Width = lines[0].Length;
            Map = new Tile[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Position currentPosition = new Position(x, y);
                    switch (lines[y][x])
                    {
                        case '|':
                            Map[x, y] = new Tile(TileType.Wall, currentPosition);
                            break;
                        case '-':
                            Map[x, y] = new Tile(TileType.Walkable, currentPosition);
                            break;
                        case 'E':
                            Entry = currentPosition;
                            Map[x, y] = new Tile(TileType.Entry, currentPosition);
                            break;
                        case 'X':
                            Exit = currentPosition;
                            Map[x, y] = new Tile(TileType.Exit, currentPosition);
                            break;
                        case 'C':
                            Interactables.Add(new Chest(currentPosition));
                            Map[x, y] = new Tile(TileType.Walkable, currentPosition);
                            break;
                        case 'T':
                            Interactables.Add(new Trap(currentPosition));
                            Map[x, y] = new Tile(TileType.Walkable, currentPosition);
                            break;
                        case 'S':
                            Interactables.Add(new Shop(currentPosition));
                            Map[x, y] = new Tile(TileType.SHOP, currentPosition);
                            break;

                        default:
                            Map[x, y] = new Tile(TileType.Wall, currentPosition);
                            break;
                    }
                }
            }

            // Spawn enemies randomly
            for (int i = 0; i < level * 2; i++)
            {
                Position enemyPos= new Position(rand.Next(1, Width - 1), rand.Next(Height / 4, Height - 1)); ;
               while (!IsWalkable(enemyPos) || IsNearOtherInteractable(enemyPos))
                {
                     enemyPos = new Position(rand.Next(1, Width - 1), rand.Next(Height / 4, Height - 1)); ;
                }

                Interactables.Add(new Enemy(enemyPos, level));
            }
        }

        public bool IsNearOtherInteractable(Position position)
        {
            foreach (Interactable interactable in Interactables)
            {
                if (Math.Abs(interactable.Position.X - position.X) <= 1 && Math.Abs(interactable.Position.Y - position.Y) <= 1)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsWalkable(Position position)
        
        {   
            if (position.X < 1 || position.X >= Width || position.Y < 1 || position.Y >= Height)
            {
                return false;
            }
          

            return Map[position.X, position.Y].Type == TileType.Walkable;
        }

        public Interactable GetInteractableAt(Position position)
        {
            return Interactables.Find(i => i.Position.X == position.X && i.Position.Y == position.Y);
        }
    }
}
