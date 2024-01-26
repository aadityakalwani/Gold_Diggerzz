using System;
using System.Collections.Generic;

namespace Gold_Diggerzz
{
    internal class Program

    {
        
        # region idea dumping ground
        
        /*
         * This is my digging simulator guys
         * initial ideas:
         * (initial inspiration: https://replit.com/@AadityaKalwani/Digging-Simulator#main.py)
         * basically you dig and you get x amount of gold per day with a possibility of getting diamonds too
         * you can use gold to go to the market and upgrade stuff like your shovel and employees
         * it follows the gregorian calendar
         * so for example, on weekends the employees want extra pay and once per few months the stock market crashes and your gold rates plummet
         * you can trade gold and diamonds for $$$ depending on the rates
         * you use your $$$ to buy upgrades for things like your shovel or your employees
         * or you can 'restart' and sacrifice all your $$$ for a better location with better gold payments per day
         * (like prestige in all the idle miner games i played)
         */
        
        /*
         * basic functionality first:
         * menu option:
         * 1 - dig one day
         * simulates 1 day's worth of digging
         * 2 - go to market
         * later, the ability to trade in the market
         */
        
        # endregion
        
        public static void Main(string[] args)
        {
            // pregame:
            Dictionary<string,int> resourceDictionary = CreateResourceDictionary();
            Console.WriteLine("Welcome to Gold Diggerzz!");
            Console.WriteLine("Created your initial resource dictionary, we're cooking:");
            
            PrintResources(resourceDictionary);
            
            RunGame(resourceDictionary);
        }

        public static void RunGame(Dictionary<string, int> resourceDictionary)
        {
            int menuOption;
            do
            {
                menuOption = UserMenuOption(resourceDictionary);
            
                switch (menuOption)
                {
                    case 1:
                        Console.WriteLine("You have chosen to dig one day");
                        DigOneDay(resourceDictionary);
                        break;
                    case 2:
                        GoToMarket();
                        break;
                    case 3:
                        PrintRules();
                        break;
                    case 4:
                        QuitGame(resourceDictionary);
                        break;
                    default:
                        Console.WriteLine("Please enter a valid option");
                        break;
                }
            } while (menuOption != 4);
            
        }

        public static int UserMenuOption(Dictionary<string,int> resources)
        {
            Console.WriteLine("Please select an option:");
            Console.WriteLine("_________________________");
            Console.WriteLine("1 - Dig one day");
            Console.WriteLine("2 - Go to market");
            Console.WriteLine("3 - Quit game");
            Console.WriteLine("4 - Print game mechanics");
            
            int userOption = int.Parse(Console.ReadLine());
            Console.Clear();
            return userOption;
        }

        public static void PrintRules()
        {
            Console.WriteLine("Each employee of yours charges $10 in wages for the day");
            Console.WriteLine("Each day, there is an 80% chance of finding gold");
        }

        public static void DigOneDay(Dictionary<string,int> resources)
        {
            Console.WriteLine("We are about to di, let us cook");
            Console.WriteLine("\nDigging...................\n");
            Console.WriteLine("Digging done for the day");
            
            Console.WriteLine("Here are the changes to your resources:");
            
            
            int numberOfEmployees = resources["Workers"];
            int totalWages = numberOfEmployees * 10;
            
            // 80% chance of finding gold
            Random random = new Random();
            int finalRandom = random.Next(0, 10);
            
            bool goldFound = finalRandom < 8;
            
            // update values within the resources dictionary
            if (goldFound)
            {
                Console.WriteLine("OMG bro you found gold \ud83d\udc8e");
                resources["Gold"] += 1;
                
            }
            resources["Dollars"] -= totalWages;
            
            Console.WriteLine($"Your {numberOfEmployees} employees charged a wage of ${totalWages} today." +
                              $"You now have ${resources["Dollars"]}");
            
            PrintResources(resources);
        }
        
        public static void GoToMarket()
        {
            Console.WriteLine("You've chosen to go to the market");
            // some code goes here
            // probably a fat switch case with all the options
        }
        
        public static Dictionary<string,int> CreateResourceDictionary()
        {
            /*
             * commands i can use:
             * resources.Add("Rubies", 0);
             * resources.Remove("Rubies");
             * resources["Rubies"] = 0;
             * resources["Rubies"] += 1;
             */
            
            Dictionary<string, int> resources = new Dictionary<string, int>()
            {
                {"Gold", 0},
                {"Diamonds", 0},
                {"Dollars", 100},
                {"Workers", 1},
                {"Worker Level", 1}
            };
            return resources;
        }
        
        public static void PrintResources(Dictionary<string,int> resources)
        {
            Console.WriteLine("\nHere are your resources.\n");
            foreach (KeyValuePair<string, int> resource in resources)
            {
                Console.WriteLine($"You have {resource.Value} {resource.Key}");
            }
        }

        // subroutine i can use to get the daily rates of things like employee wage later on
        public static int GetRates()
        {
            int employeeWage = 10;

            return employeeWage;
        }

        public static void QuitGame(Dictionary<string,int> resources)
        {
            Console.WriteLine("You have chosen to quit the game");
            Console.WriteLine($"Your final stats were:");
            PrintResources(resources);
            Console.WriteLine("\nGoodbye!");
        }


    }
}
