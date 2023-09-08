using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Player
    {
        public Position Position { get; set; }
        public int Health { get; set; }
        public int Damage { get; set; }
        public int MaxHealth { get; set; }
        public int HealthPotions { get; set; }
        public int gold { get; set; }

        public Player(Position position)
        {
            gold = 50;
            HealthPotions = 3;
            Position = position;
            Health = 100; // initial health
            Damage = 10; // initial damage
            MaxHealth = Health;
        }
        public bool IsAlive => Health > 0;
        // Movement logic
        public void Move(char direction, int width, int height)
        {
            switch (direction)
            {
                case 'w': // move up
                    if (Position.Y > 1)
                        Position.Y -= 1;
                    break;
                case 'a': // move left
                    if (Position.X > 1)
                        Position.X -= 1;
                    break;
                case 's': // move down
                    if (Position.Y < height -1)
                        Position.Y += 1;
                    break;
                case 'd': // move right
                    if (Position.X < width - 1)
                        Position.X += 1;
                    break;
            }
        }
        public string Interact(Interactable interactable)
        {
            // If the interactable object is an enemy and is adjacent to the player,
            // start a battle
            if (interactable is Enemy enemy && IsAdjacent(enemy.Position))
            {
                return enemy.Fight(this);
            }

            // Otherwise, just interact normally
            return interactable.Interact(this);
        }
        private bool IsAdjacent(Position position)
        {
            int dx = Math.Abs(Position.X - position.X);
            int dy = Math.Abs(Position.Y - position.Y);

            // If the position is one tile away horizontally or vertically,
            // it is considered adjacent
            return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
        }
    }
}
