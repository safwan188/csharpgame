using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp1.Tile;

namespace ConsoleApp1
{
    internal class Game
    {
        public int Level { get; set; }
        public Player Player { get; set; }
        public Dungeon Dungeon { get; set; }
        public List<string> Log { get; set; }

        public Game()
        {
          
            Level = 1;
            Dungeon = new Dungeon("level1.txt",Level);
            Player = new Player(new Position(Dungeon.Entry.X,Dungeon.Entry.Y));
            Log = new List<string>();
        }

        public void AddLog(string message)
        {
            if (message.Length != 0)
            {
                Log.Add(message);
            }
            // Optionally, limit the log size:
            while (Log.Count >1)
            {
                
                Log.RemoveAt(0);
            }
        }
        public void DrawHUD()
        {
            // Save current cursor position
            int previousCursorLeft = Console.CursorLeft;
            int previousCursorTop = Console.CursorTop;

            // Position the cursor where the HUD should be drawn. You may adjust these values.
            Console.SetCursorPosition(Dungeon.Width+4,0);

            // Write HUD information
            Console.WriteLine($"----HP: {Player.Health},damage: {Player.Damage}maxhealth : {Player.MaxHealth},health Potions :{Player.HealthPotions},gold:{Player.gold}-----");

            // Restore cursor position
            Console.SetCursorPosition(previousCursorLeft, previousCursorTop);
        }
        public void DisplayLog()
        {
            // Save current cursor position
            int previousCursorLeft = Console.CursorLeft;
            int previousCursorTop = Console.CursorTop;

            // Position the cursor where the logs should be written
            // Here it is assumed that you have enough space below the HUD
            Console.SetCursorPosition(0, Dungeon.Height + 4);

            // Erase previous logs
            for (int i = 0; i < Log.Count; i++)
            {
                Console.Write(new string(' ', Console.WindowWidth));
            }

            // Reset the cursor to the beginning of the log area
            Console.SetCursorPosition(0, Dungeon.Height + 4);

            foreach (string log in Log)
            {
                Console.WriteLine(log);
            }

            // Restore cursor position
            Console.SetCursorPosition(previousCursorLeft, previousCursorTop);
        }


        public void Run()
{
    DrawDungeon();
    DisplayLog();


    while (true)
    {



                Thread.Sleep(50);
        char input = Console.ReadKey(true).KeyChar;

        Position oldPosition = new Position(Player.Position.X, Player.Position.Y);
        Position newPosition = new Position(Player.Position.X, Player.Position.Y);
                Console.SetCursorPosition(oldPosition.X, oldPosition.Y);

                switch (input)
        {
            case 'w':
                newPosition.Y -= 1;
                break;
            case 'a':
                newPosition.X -= 1;
                break;
            case 's':
                newPosition.Y += 1;
                break;
            case 'd':
                newPosition.X += 1;
                break;
            case 'h':
                if (Player.HealthPotions > 0)
                {
                    Player.Health = Math.Min(Player.MaxHealth, Player.Health + 50); // or however much you want the potion to heal
                    Player.HealthPotions--;
                    AddLog("You drank a health potion and healed 50 HP!");
                }
                else
                {
                    AddLog("You have no health potions left!");
                }
                break;
            default:
                continue;
        }
                if (Dungeon.Map[newPosition.X, newPosition.Y].Type == TileType.Exit)
                {
                    NextLevel();
                    Run();
                }
                if (Dungeon.Map[newPosition.X, newPosition.Y].Type == TileType.SHOP)
                {
                    Shop sh = (Shop)this.Dungeon.GetInteractableAt(newPosition);

                    Console.Clear();
                    DrawDungeon();
                    Console.SetCursorPosition(0, Dungeon.Height+1);
                    
                    
                    AddLog(sh.Interact(Player));
                    Console.Clear();
                    DrawDungeon();
                    DisplayLog();



                }

                if (Dungeon.IsWalkable(newPosition))
        {
            
            Console.SetCursorPosition(Player.Position.X, Player.Position.Y);
                 
            Player.Move(input,Dungeon.Width,Dungeon.Height);
            DrawTile(oldPosition);
            Console.SetCursorPosition(Player.Position.X, Player.Position.Y);
            DrawTile(Player.Position);
            if (Player.Position.X == Dungeon.Exit.X && Player.Position.Y == Dungeon.Exit.Y)
            {
                NextLevel();
                Console.Clear();
                DrawDungeon();
                DisplayLog();
            }

            Interactable interactable = Dungeon.GetInteractableAt(Player.Position);
            if (interactable != null)
            {
                if (interactable is Chest chest)
                {
                    if (!chest.Opened)
                    {
                        string logMessage = chest.Interact(Player);
                        AddLog(logMessage);
                    }
                }
                else
                {
                    string logMessage = interactable.Interact(Player);
                    AddLog(logMessage);
                }
            }
                    DrawHUD();
                    var deadEnemies = Dungeon.Interactables.Where(i => i is Enemy e).ToList();
            foreach (Enemy enem in deadEnemies)
            {
                if (enem.IsDead)
                {
                            Dungeon.Interactables.Remove(enem);
                            DrawTile(enem.Position);
                        }
                        else
                        {
                            int distanceToPlayer = Math.Abs(Player.Position.X - enem.Position.X) + Math.Abs(Player.Position.Y -     enem.Position.Y);
                            if (distanceToPlayer <= 3)
                            {
                                while (distanceToPlayer >1)
                                {
                                    Position oldEnemyPosition = new Position(enem.Position.X, enem.Position.Y);
                                    enem.MoveTowards(Player.Position);
                                    distanceToPlayer = Math.Abs(Player.Position.X - enem.Position.X) + Math.Abs(Player.Position.Y - enem.Position.Y);
                                    Console.SetCursorPosition(oldEnemyPosition.X, oldEnemyPosition.Y);
                                    DrawTile(oldEnemyPosition);
                                    Console.SetCursorPosition(enem.Position.X, enem.Position.Y);
                                    DrawTile(enem.Position);
                                }
                                Position oldEnemyPosition1= new Position(enem.Position.X, enem.Position.Y);
                                // Check if enemy reached the player
                                if (distanceToPlayer ==1)  
                                {
                                    string logMessage = enem.Interact(Player);
                                    AddLog(logMessage);
                                    Console.SetCursorPosition(oldEnemyPosition1.X, oldEnemyPosition1.Y);
                                    Dungeon.Interactables.Remove(enem);
                                    DrawTile(oldEnemyPosition1);

                                    DrawHUD();

                                }

                              
                              
                            }

                        }

            }
                    


                    Console.SetCursorPosition(0, Dungeon.Height);
                    DisplayLog();
                    if (Player.Health <= 0)
                    {
                        GameOver();
                        return; // This will end the Run method early.
                    }
                }
    }
}


        private void DrawTile(Position position)
        {
            if (Player.Position.X == position.X && Player.Position.Y == position.Y)
            {
                Console.Write('@');
            }
            else if (Dungeon.Entry.X == position.X && Dungeon.Entry.Y == position.Y)
            {
                Console.Write('E');
            }
            else if (Dungeon.Exit.X == position.X && Dungeon.Exit.Y == position.Y)
            {
                Console.Write('X');
            }
            else
            {
                Interactable interactable = Dungeon.GetInteractableAt(position);

                if (interactable is Enemy)

                {
                    Enemy temo = (Enemy)interactable;
                    if(!temo.IsDead)
                    Console.Write('M');
                    else {Console.Write(' '); }
                }
                else if (interactable is Trap && ((Trap)interactable).Triggered)
                {
                    Console.Write('T');
                }
                else if (interactable is Chest && !((Chest)interactable).Opened)
                {
                    Console.Write('C');
                }
                else
                {
                    switch (Dungeon.Map[position.X, position.Y].Type)
                    {
                        case TileType.Wall:
                            Console.Write('|');
                            break;
                        case TileType.Walkable:
                            Console.Write(' ');
                            break;
                    }
                }
            }
        }

        public void NextLevel()
        {
           
            Level++;
            Dungeon = new Dungeon("level"+Level.ToString()+".txt",Level );
            Player.Position = new Position(Dungeon.Entry.X, Dungeon.Entry.Y); // Assuming player always starts at (1, 1)
            AddLog($"You reached the exit and advanced to level {Level}!");
            Console.Clear();
            Console.SetCursorPosition(0, 0);
        }
        public void GameOver()
        {
            Console.WriteLine("GAME OVER");
            Console.WriteLine("You have died.");
            Console.WriteLine($"You reached level {Level}.");
            // Display other relevant game stats here if necessary.

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            // This will terminate the application
            Environment.Exit(0);
        }

        private void DrawDungeon()
        {
            for (int y = 0; y < Dungeon.Height; y++)
            {
                for (int x = 0; x < Dungeon.Width; x++)
                {
                    Position currentPosition = new Position(x, y);

                    if (Player.Position.X == x && Player.Position.Y == y)
                    {
                        Console.Write('@');
                    }
                    else if (Dungeon.Entry.X == x && Dungeon.Entry.Y == y)
                    {
                        Console.Write('E');
                    }
                    else if (Dungeon.Exit.X == x && Dungeon.Exit.Y == y)
                    {
                        Console.Write('X');
                    }
                    else
                    {
                        Interactable interactable = Dungeon.GetInteractableAt(currentPosition);

                        if (interactable is Enemy)
                        {
                            Enemy temo = (Enemy)interactable;
                            if (!temo.IsDead)
                                Console.Write('M');
                        }
                        else if (interactable is Trap && ((Trap)interactable).Triggered)
                        {
                            Console.Write('T');
                        }
                        else if (interactable is Chest && !((Chest)interactable).Opened)
                        {
                            Console.Write('C');
                        }
                        else if (interactable is Shop)
                        {
                            Console.Write('S');
                        }
                        else
                        {
                            switch (Dungeon.Map[x, y].Type)
                            {
                                case TileType.Wall:
                                    Console.Write('|');
                                    break;
                                case TileType.Walkable:
                                    Console.Write('-');
                                    break;
                            }
                        }
                    }
                }
                Console.Write('\n');
            }
        }
    }
}
