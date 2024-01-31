using System;
using System.Collections.Generic;
using System.Threading;

namespace Gold_Diggerzz
{
    internal abstract class Program

    {
        
        /* to-do ideas
         * (initial inspiration: https://replit.com/@AadityaKalwani/Digging-Simulator#main.py)
         * fancy nice animations eg. to see very clearly that you're in the market or you're digging
         * more resources eg. iron, coal, etc.
         * a power-up item that you can choose your special ability eg. 100% chance of finding gold for three day or flat $200
         * a bribe function:
           bribe the government with $100 and you dont have to pay wages for the next 5 days
         * managers that do shit
         * or you can 'restart' and sacrifice all your $$$ for a better location with better gold payments per day
         * (like prestige in all the idle miner games i played)
         */ 
        
        private static void Main()
        {
            // pregame:
            Dictionary<string, double> resourceDictionary = CreateResourceDictionary();
            Dictionary<string, double> priceDictionary = CreatePricesDictionary();
            Console.WriteLine("Welcome to Gold Diggerzz!");
            Console.WriteLine("Created your initial resource dictionary, we're cooking:");
            
            PrintResources(resourceDictionary);
            
            RunGame(resourceDictionary, priceDictionary);
        }
        
        private static DateTime _currentDate = new DateTime(2024, 1, 1);
        
        private static void RunGame(Dictionary<string, double> resourceDictionary, Dictionary<string, double> priceDictionary)
        {
            int menuOption;
            do
            {
                menuOption = UserMenuOption(resourceDictionary, priceDictionary);
            
                switch (menuOption)
                {
                    case 0:
                        Console.WriteLine("You were in debt, bossman have paid that off so we good now");
                        break;
                    case 1:
                        Console.WriteLine("You have chosen to dig one day");
                        DigOneDay(resourceDictionary, priceDictionary);
                        break;
                    case 2:
                        GoToMarket(resourceDictionary, priceDictionary);
                        break;
                    case 3:
                        PrintRules();
                        break;
                    case 4:
                        QuitGame(resourceDictionary);
                        break;
                    case 5:
                        Console.WriteLine("Skipping one day");
                        Console.WriteLine("You have been charged $30 for the costs of skipping a day");
                        resourceDictionary["Dollars"] -= 30;
                        _currentDate = _currentDate.AddDays(1);
                        break;
                    case 6:
                        Console.WriteLine("Enter number of days to dig in one go");
                        int daysToDig = GetValidInt();
                        for (int i = 0; i <= daysToDig; i++)
                        {
                            DigOneDay(resourceDictionary, priceDictionary);
                        }
                        break;
                    case -1:
                        GameFailed(resourceDictionary);
                        break;
                    default:
                        Console.WriteLine("Please enter a valid option");
                        break;
                }
            } while (menuOption != 4 && menuOption != 5);
            
        }
        
        private static void PrintRules()
        {
            Console.WriteLine("Each employee of yours charges $10 in wages for the day");
            Console.WriteLine("Each day, there is an 60% chance of finding gold");
            Console.WriteLine("Each day, there is a 10% chance of finding diamonds");
            Console.WriteLine("If you find gold or diamonds, you gain the number of employees you have of the resource");
            Console.WriteLine("When you go to the market, you are given the rates of resource conversions");
            Console.WriteLine("If you are in debt, the bossman comes and takes all your resources and sells them for 2/5 the rate");
        }
        
        private static void PrintResources(Dictionary<string, double> resources)
        {
            Console.WriteLine("\nHere are your resources.\n");
            foreach (KeyValuePair<string, double> resource in resources)
            {
                Console.WriteLine($"You have {resource.Value} {resource.Key}");
            }
        }
        
        private static Dictionary<string, double> CreateResourceDictionary()
        {
            /*
             * commands i can use:
             * resources.Add("Rubies", 0);
             * resources.Remove("Rubies");
             * resources["Rubies"] = 0;
             * resources["Rubies"] += 1;
             */
            
            Dictionary<string, double> resources = new Dictionary<string, double>()
            {
                {"Gold", 0},
                {"Diamonds", 0},
                {"Dollars", 100},
                {"Workers", 1},
                {"Worker Level", 1}
            };
            return resources;
        }
        
        private static Dictionary<string, double> CreatePricesDictionary()
        {
            Dictionary<string, double> prices = new Dictionary<string, double>()
            {
                {"Gold", 15},
                {"Diamonds", 75},
                {"Workers", 100},
                {"Wage", 10}
            };
            
            return prices;
        }
        
        private static int UserMenuOption(Dictionary<string, double> resources, Dictionary<string, double> prices)
        {
            string takeUserInput = CheckIfInDebt(resources);
            
            CalendarEffects(prices, _currentDate);
            
            if (takeUserInput == "false")
            {
                Console.WriteLine($"Today is {_currentDate:dddd, d MMMM, yyyy}");
                Console.WriteLine("Please select an option:");
                Console.WriteLine("_________________________");
                Console.WriteLine("1 - Dig one day");
                Console.WriteLine("2 - Go to market");
                Console.WriteLine("3 - Print game mechanics");
                Console.WriteLine("4 - Quit game");
                Console.WriteLine("5 - Skip one day");
                Console.WriteLine("6 - Dig multiple days");
            
                int userOption = GetValidInt();
                Console.Clear();
                return userOption;
            }

            if(takeUserInput == "bankrupt")
            { 
                return -1;
            }
            
            return 0;
        }
        
        private static void DigOneDay(Dictionary<string, double> resources, Dictionary<string, double> prices)
        {
            int increasedDiamondChanceDays = 0;
            bool diamondFound = true;
            
            Console.WriteLine("We are about to dig, let us cook");
            Console.WriteLine("\nDigging...................\n");
        
            /*
             ascii art animations
             *  string[] diggingMessages = { "Digging through the dirt...", "Found a rock, moving it aside...", "Something shiny up ahead...", "Almost there..." };
                  Random random = new Random();

                  Console.WriteLine("Starting to dig...");

                  for (int i = 0; i < 10; i++)
                  {
                      Thread.Sleep(500); // Wait for half a second to simulate the digging process
                      Console.Write(new string('#', i) + "\r");
                      Console.WriteLine(diggingMessages[random.Next(diggingMessages.Length)]);
                  }

                  Console.WriteLine("Digging complete!");
                  
                  ___________________________
                  
                  
                  ALTERNATIVE OPTION
                  
                  
                  
                  Console.WriteLine("We are about to dig, let us cook");
               Console.WriteLine("\nDigging...................\n");

               string[] shovel = new string[]
               {
                   "    ______",
                   "   /      \\",
                   "  /        \\",
                   "  |        |",
                   "  \\________/",
                   "       ||",
                   "       ||",
                   "       ||"
               };

               Random random = new Random();

               Console.WriteLine("Starting to dig...");

               for (int i = 0; i < 10; i++)
               {
                   Thread.Sleep(500); // Wait for half a second to simulate the digging process
                   Console.Clear();
                   for (int j = 0; j < shovel.Length; j++)
                   {
                       if (j < shovel.Length - i)
                       {
                           Console.WriteLine(shovel[j]);
                       }
                       else
                       {
                           Console.WriteLine(new string(' ', i) + shovel[j]);
                       }
                   }
                   Console.WriteLine("Progress: " + new string('#', i) + new string(' ', 10 - i) + "|");
               }

                
             */
            
            Thread.Sleep(2000);
            Console.WriteLine("Digging done for the day");
            
            Console.WriteLine("Here are the changes to your resources:");
            
            double totalWages = resources["Workers"] * prices["Wage"];
            
            // creating randoms for the chance of finding gold and diamonds
            Random random = new Random();
            int finalRandom = random.Next(0, 100);
            
            // 60% chance of finding gold
            bool goldFound = finalRandom < 60;
            
            // 5% chance of finding the magic star superpower
            bool magicStarFound = finalRandom < 5;
            
            if (magicStarFound)
            {
                Console.Write("\\ud83c\\udf1f You found the magic star power-up \ud83c\udf1f");
                Console.WriteLine("Choose a powerup:");
                Console.WriteLine("1 - 50% chance of finding diamond for the next three days");
                Console.WriteLine("2 - $250 instantly");
                int userInput = GetValidInt();

                switch (userInput)
                {
                    case 1:
                        Console.WriteLine("You have chosen the 50% chance of finding diamond for the next three days");
                        increasedDiamondChanceDays = 3;
                        break;
                    case 2:
                        Console.WriteLine("You have chosen the $250 instantly");
                        resources["Dollars"] += 250;
                        break;
                }
                
            }
            
            // if there is a changed chance of finding diamonds due to the magic star powerup
            if (increasedDiamondChanceDays > 0)
            {
                increasedDiamondChanceDays -= 1;
                Console.WriteLine($"You have the magic star powerup, you have a 50% chance of finding diamonds for the next {increasedDiamondChanceDays} days");
                diamondFound = finalRandom < 50;
            }
            else
            {
                // 15% chance of finding diamonds
                diamondFound = finalRandom < 15;
            }
            
            // update values within the resources dictionary
            if (diamondFound)
            {
                Console.WriteLine("OMG bro you found diamond \ud83d\udc8e");
                resources["Diamonds"] += resources["Workers"];
            }
            
            if (goldFound)
            {
                Console.WriteLine("OMG bro you found gold \ud83c\udf1f");
                resources["Gold"] += resources["Workers"];
            }
            
            resources["Dollars"] -= totalWages;

            Console.WriteLine($"Your {resources["Workers"]} employees charged a wage of ${totalWages} today.");
            PrintResources(resources);
            
            _currentDate = _currentDate.AddDays(1);
        }
        
        private static void GoToMarket(Dictionary<string, double> resources, Dictionary<string, double> priceDictionary)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    WELCOME TO THE MARKET                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine($"Here are the rates for {_currentDate:dddd dd MMMM, yyyy}:");
            
            foreach (KeyValuePair<string, double> item in priceDictionary)
            {
                Console.WriteLine($"1 {item.Key} = ${item.Value}");
            }
            
            
            int marketOption;
            do
            {
                Console.WriteLine("Here is the menu for the market:");
                Console.WriteLine("1 - Sell Gold for dollars");
                Console.WriteLine("2 - Sell Diamonds for dollars");
                Console.WriteLine("3 - Hire More Employees");
                Console.WriteLine("4 - Exit market");
                Console.WriteLine("5 - Sell all gold and diamonds for dollars");

                marketOption = GetValidInt();

                switch (marketOption)
                {
                    case 1:
                        Console.WriteLine("You have chosen to sell gold for dollars");
                        Console.WriteLine($"How much gold do you want to sell?\nEnter '-1' to sell all your gold\nYou have {resources["Gold"]} gold");
                        double goldToSell = GetValidDouble();
                        if (goldToSell == -1)
                        {
                            goldToSell = resources["Gold"];
                        }
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
                        Console.WriteLine($"How many diamonds do you want to sell?\nEnter '-1' to sell all your diamonds.\nYou have {resources["Diamonds"]} diamonds");
                        double diamondsToSell = GetValidDouble();
                        if (diamondsToSell == -1)
                        {
                            diamondsToSell = resources["Diamonds"];
                        }
                        if (diamondsToSell > resources["Diamonds"])
                        {
                            Console.WriteLine("You don't have enough gold to sell that much");
                        }
                        else
                        {
                            resources["Diamonds"] -= diamondsToSell;
                            resources["Dollars"] += diamondsToSell * 75;
                        }

                        Console.WriteLine("Here are your update resources:");
                        PrintResources(resources);

                        break;
                    case 3:
                        Console.WriteLine("Enter how many employees you want to hire:");
                        Console.WriteLine($"Remember each employee charges {priceDictionary["Wage"]} in wages per day");
                        int employeesToHire = GetValidInt();
                        if (employeesToHire * 100 > resources["Dollars"])
                        {
                            Console.WriteLine("You don't have enough dollars to hire that many employees");
                        }
                        else
                        {
                            resources["Workers"] += employeesToHire;
                            resources["Dollars"] -= employeesToHire * 100;
                        }
                        break;
                    case 4:
                        Console.WriteLine("Thanks for coming to the market! Goodbye");
                        break;
                    case 5:
                        Console.WriteLine("We're selling all your gold and diamonds for dollars");
                        resources["Dollars"] += resources["Gold"] * 15;
                        resources["Dollars"] += resources["Diamonds"] * 75;
                        resources["Gold"] = 0;
                        resources["Diamonds"] = 0;
                        PrintResources(resources);
                        break;
                }
            } while (marketOption != 4);
        }
        
        private static string CheckIfInDebt(Dictionary<string, double> resources)
        {
            string inDebt = "false";
            if (resources["Dollars"] < 0)
            {
                inDebt = "true";
                
                if (inDebt == "true" && resources["Gold"] == 0 && resources["Diamonds"] == 0)
                {
                    Console.WriteLine("Bro you're literally bankrupt. You have failed the game.");
                    return "bankrupt";
                }

                if (inDebt == "true")
                {
                    Console.WriteLine("\n\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31");
                    Console.WriteLine("You are in debt, bossman is coming for you");
                    Console.WriteLine("The government will come and sell all your resources for 2/5 the rate");
                    Console.WriteLine("They're also reducing your percentage chances of finding resources by 30% for the next three days");
                    Console.WriteLine("Bossman is coming for ur shit, unlucky bro...");
                
                    Console.WriteLine($"right now you have ${resources["Dollars"]}, {resources["Diamonds"]} diamonds and {resources["Gold"]} gold");
                
                    Console.WriteLine("After bossman stole your resources, you now have:");

                    resources["Dollars"] += resources["Gold"] * 6;
                    resources["Dollars"] += resources["Diamonds"] * 30; 
                
                    resources["Gold"] = 0;
                    resources["Diamonds"] = 0;
                
                    PrintResources(resources);
                }
            }
            
            return inDebt;
        }

        private static void QuitGame(Dictionary<string, double> resources)
        {
            Console.WriteLine("Your final stats were:");
            PrintResources(resources);
            Console.WriteLine("\nGoodbye!");
        }

        private static void GameFailed(Dictionary<string, double> resources)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                         YOU FAILED                         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            QuitGame(resources);
        }
        
        private static void CalendarEffects(Dictionary<string, double> prices, DateTime currentDate)
        {
            
            // +30% pay on weekends
            // wage is increased on saturday, then reduced again on monday
            if (currentDate.DayOfWeek is DayOfWeek.Saturday)
            {
                Console.WriteLine("It's the weekend, your employees want 50% more pay");
                prices["Wage"] *= 1.3;
            }
            
            // to undo the effect of above
            else if (currentDate.DayOfWeek is DayOfWeek.Monday)
            {
                if (currentDate.Month != 1)
                {
                    prices["Wage"] = prices["Wage"] * 10/13;
                }
            }
            
            // stock market crash once per month
            Random random = new Random();
            int crashDate = random.Next(0, 28);
            
            if (currentDate.Day == crashDate)
            {
                Console.WriteLine("The stock market has crashed, your gold and diamond prices have plummeted but you can hire employees for cheaper");
                
                prices["Gold"] /= 2;
                prices["Diamonds"] /= 2;
                prices["Workers"] /= 2;
            }

            if (currentDate.Month != 1)
            {
                if (currentDate.Day == 1)
                {
                    Console.WriteLine("It's the first of the month, your employees want a 10% raise");
                    prices["Wage"] *= 1.1;
                }
            }
            
        }

        private static int GetValidInt()
        {
            if (int.TryParse(Console.ReadLine(), out int validInt))
            {
                return validInt;
            }
            else
            {
                Console.WriteLine("Please enter a valid integer");
                return GetValidInt();
            }
        }
        
        private static double GetValidDouble()
        {
            if (double.TryParse(Console.ReadLine(), out double validDouble))
            {
                return validDouble;
            }
            else
            {
                Console.WriteLine("Please enter a valid double");
                return GetValidDouble();
            }
        }
        
    }
}
