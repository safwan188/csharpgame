using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public abstract class Interactable
    {
        public Position Position { get; set; }

        protected Interactable(Position position)
        {
            Position = position;
        }

        public abstract string Interact(Player player);
    }
    public class Enemy : Interactable
    {
        public int Health { get; set; }
        public int Damage { get; set; }
        public bool IsDead => Health <= 0;

        public Enemy(Position position, int level) : base(position)
        {
            Health = 20 + level * 5; // for example
            Damage = 5 + level * 5; // for example
        }

        public override string Interact(Player player)
        {
            StringBuilder sb = new StringBuilder();

            // Variables to keep track of total damage
            int totalPlayerDamage = 0;
            int totalEnemyDamage = 0;

            // Both player and enemy keep dealing damage until one of them is dead.
            while (player.IsAlive && !this.IsDead)
            {
                player.Health -= Damage;
                this.Health -= player.Damage;

                // Accumulate total damage
                totalPlayerDamage += player.Damage;
                totalEnemyDamage += Damage;

                if (!player.IsAlive)
                {
                    sb.AppendLine("You have been defeated!");
                }
                if (this.IsDead)
                {
                    sb.AppendLine("You have defeated the enemy!");
                }
            }

            // Add total damage dealt to the log message
            sb.AppendLine($"Total damage dealt by player: {totalPlayerDamage}");
            sb.AppendLine($"Total damage received by player: {totalEnemyDamage}");

            return sb.ToString();
        }

        public void MoveTowards(Position target)
        {
            
            if (Position.X < target.X)
            {
                Position.X += 1;
            }
            else if (Position.X > target.X)
            {
                Position.X -= 1;
            }
            if(Position.Y < target.Y )
            {
                Position.Y += 1;
            }
            else if (Position.Y > target.Y)
            {
                Position.Y -= 1;
            }
        }

        public string Fight(Player player)
        {
            string result = "";

            // The battle continues until either the player or the enemy is defeated
            while (player.Health > 0 && Health > 0)
            {
                // Damage each other
                player.Health -= Damage;
                Health -= player.Damage;

                result += $"You fought with an enemy. You dealt {player.Damage} damage and received {Damage} damage. ";

                if (player.Health <= 0)
                {
                    result += "You have been defeated by the enemy!";
                }
                else if (Health <= 0)
                {
                    result += "You defeated the enemy!";
                }
            }

            return result;
        }
    }

    public class Shop : Interactable
    {
        
        public int PotionPrice { get; set; } = 50; // Price of a health potion
        public int EnergyDrinkPrice { get; set; } = 75; // Price of an energy drink
        
        public Shop (Position position) : base(position)
        {
           
        }
        public override string Interact(Player player)
        {
            Console.WriteLine("Welcome to the shop! Here's what we have for sale:");
            Console.WriteLine($"1. Health Potion: {PotionPrice} gold");
            Console.WriteLine($"2. Energy Drink: {EnergyDrinkPrice} gold");
            Console.WriteLine($"3. Return to game: press r");
            // Loop until a valid input is received
            while (true)
            {
                // Display a menu for the player to choose what to buy
              

                char choice = Console.ReadKey(true).KeyChar;
                switch (choice)
                {
                    case '1':
                        if (player.gold >= PotionPrice)
                        {
                            player.gold -= PotionPrice;
                            player.HealthPotions += 1; // Increase the player's health potions count.
                            return "You bought a health potion!";
                        }
                        else
                        {
                            Console.WriteLine("You don't have enough gold for a health potion.");
                        }
                        break;
                    case '2':
                        if (player.gold >= EnergyDrinkPrice)
                        {
                            player.gold -= EnergyDrinkPrice;
                            player.Damage += 10; // Increase the player's damage.
                            return $"You bought an energy drink! New damage: {player.Damage}";
                        }
                        else
                        {
                            Console.WriteLine("You don't have enough gold for an energy drink.");
                        }
                        break;
                    case 'r':
                        return "You decided to return to the game.";
                    default:
                        Console.WriteLine("Invalid input. Please try again.");
                        break;
                }
            }
        }

    }

    public class Trap : Interactable
    {
        public int Damage { get; set; }
        public bool Triggered { get; set; }

        public Trap(Position position) : base(position)
        {
            Damage = 20;
            Triggered = false;
        }

        public override string Interact(Player player)
        {
            if (!Triggered)
            {
                player.Health -= Damage;
                Triggered = true;
                return $"You stepped on a trap and took {Damage} damage! Your health is now {player.Health}.";
            }

            return "";
        }
    }

    public class Chest : Interactable
    {
        private Random rand = new Random();
        public bool Opened { get; set; }

        public Chest(Position position) : base(position)
        {
            Opened = false;
        }

        public override string Interact(Player player)
        {
            if (!Opened)
            {
                // Randomly choose a reward
                switch (rand.Next(3))
                {
                    case 0: // Increase player health
                        player.Health += 20;
                        if(player.Health > player.MaxHealth)
                        {
                            player.Health = player.MaxHealth;
                        }
                        Opened = true;
                        return "You opened a chest and found a health potion! Your health is now " + player.Health + ".";
                    case 1: // Increase player damage
                        player.Damage += 5;
                        Opened = true;
                        return "You opened a chest and found a strength potion! Your damage is now " + player.Damage + ".";
                    case 2: // Increase player damage
                        player.MaxHealth += 20;
                        Opened = true;
                        return "You opened a chest and found a maxhealth potion your damage" +
                            "your maxhealth is: " + player.MaxHealth+".";

                }

                
            }
            else
            {
                return "This chest is already opened.";
            }

            return "";
        }
    }




}
