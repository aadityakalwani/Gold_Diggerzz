using System;
using System.Collections.Generic;
using System.Threading;

namespace Gold_Diggerzz
{
    internal class Program

    {
        
        /* idea dumping ground
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
        
        // "Thread.Sleep(3000);" = time.sleep(3)
        
        private static void Main(string[] args)
        {
            // pregame:
            Dictionary<string,int> resourceDictionary = CreateResourceDictionary();
            Console.WriteLine("Welcome to Gold Diggerzz!");
            Console.WriteLine("Created your initial resource dictionary, we're cooking:");
            
            PrintResources(resourceDictionary);
            
            RunGame(resourceDictionary);
        }

        private static void RunGame(Dictionary<string, int> resourceDictionary)
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
                        GoToMarket(resourceDictionary);
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

        private static int UserMenuOption(Dictionary<string,int> resources)
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

        private static void PrintRules()
        {
            Console.WriteLine("Each employee of yours charges $10 in wages for the day");
            Console.WriteLine("Each day, there is an 80% chance of finding gold");
            Console.WriteLine("If you find gold or diamonds, you gain the number of employees you have of the resource");
            Console.WriteLine("Each day, there is a 20% chance of finding diamonds");
            Console.WriteLine("When you go to the market, you are given the rates of resource conversions");
        }

        private static void DigOneDay(Dictionary<string,int> resources)
        {
            Console.WriteLine("We are about to dig, let us cook");
            Console.WriteLine("\nDigging...................\n");
            Console.WriteLine("Digging done for the day");
            
            Console.WriteLine("Here are the changes to your resources:");
            
            int numberOfEmployees = resources["Workers"];
            int totalWages = numberOfEmployees * 10;
            
            // creating randoms for the chance of finding gold and diamonds
            Random random = new Random();
            int finalRandom = random.Next(0, 10);
            
            // 80% chance of finding gold
            bool goldFound = finalRandom < 8;
            
            // 20% chance of finding diamonds
            bool diamondFound = finalRandom < 2;
            
            // update values within the resources dictionary
            if (diamondFound)
            {
                Console.WriteLine("OMG bro you found diamond \ud83d\udc8e");
                resources["Diamonds"] += 1;
            }
            
            if (goldFound)
            {
                Console.WriteLine("OMG bro you found gold \ud83c\udf1f");
                resources["Gold"] += 1;
            }
            resources["Dollars"] -= totalWages;

            Console.WriteLine($"Your {numberOfEmployees} employees charged a wage of ${totalWages} today.");
            PrintResources(resources);
        }
        
        private static void GoToMarket(Dictionary<string, int> resources)
        {
            Console.WriteLine("You've chosen to go to the market");
            Console.WriteLine("Here are the rates for today:");
            Console.WriteLine("1 gold = 15 dollars");
            Console.WriteLine("1 diamond = 75 dollars");

            int marketOption;
            do
            {
                Console.WriteLine("Here is the menu for the market:");
                Console.WriteLine("1 - Sell Gold for dollars");
                Console.WriteLine("2 - Sell Diamonds for dollars");
                Console.WriteLine("3 - Hire More Employees");
                Console.WriteLine("4 - Exit market");

                marketOption = int.Parse(Console.ReadLine());

                switch (marketOption)
                {
                    case 1:
                        Console.WriteLine("You have chosen to sell gold for dollars");
                        Console.WriteLine($"How much gold do you want to sell? You have {resources["Gold"]} gold");
                        int goldToSell = int.Parse(Console.ReadLine());
                        if (goldToSell > resources["Gold"])
                        {
                            Console.WriteLine("You don't have enough gold to sell that much");
                        }
                        else
                        {
                            resources["Gold"] -= goldToSell;
                            resources["Dollars"] += goldToSell * 15;
                        }

                        Console.WriteLine("Here are your update resources:");
                        PrintResources(resources);

                        break;
                    case 2:
                        Console.WriteLine("Your have chosen to sell diamonds for dollars");
                        Console.WriteLine(
                            $"How many diamonds do you want to sell? You have {resources["Diamonds"]} diamonds");
                        int diamondsToSell = int.Parse(Console.ReadLine());
                        if (diamondsToSell > resources["Diamonds"])
                        {
                            Console.WriteLine("You don't have enough gold to sell that much");
                        }
                        else
                        {
                            resources["Gold"] -= diamondsToSell;
                            resources["Dollars"] += diamondsToSell * 75;
                        }

                        Console.WriteLine("Here are your update resources:");
                        PrintResources(resources);

                        break;
                    case 4:
                        Console.WriteLine("Thanks for coming to the market! Goodbye");
                        break;
                }
            } while (marketOption != 4);
            
        }
        
        private static Dictionary<string,int> CreateResourceDictionary()
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
        
        private static void PrintResources(Dictionary<string,int> resources)
        {
            Console.WriteLine("\nHere are your resources.\n");
            foreach (KeyValuePair<string, int> resource in resources)
            {
                Console.WriteLine($"You have {resource.Value} {resource.Key}");
            }
        }

        private static void QuitGame(Dictionary<string,int> resources)
        {
            Console.WriteLine("You have chosen to quit the game");
            Console.WriteLine($"Your final stats were:");
            PrintResources(resources);
            Console.WriteLine("\nGoodbye!");
        }


    }
}
