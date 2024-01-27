using System;
using System.Collections.Generic;
using System.Threading;

namespace Gold_Diggerzz
{
    internal abstract class Program

    {
        
        /* to-do ideas
         * (initial inspiration: https://replit.com/@AadityaKalwani/Digging-Simulator#main.py)
         * it follows the gregorian calendar
         * so for example, on weekends the employees want extra pay and once per few months the stock market crashes and your gold rates plummet
         * or you can 'restart' and sacrifice all your $$$ for a better location with better gold payments per day
         * (like prestige in all the idle miner games i played)
         */
        
        // "Thread.Sleep(3000);" = time.sleep(3)

        private static void Main()
        {
            // pregame:
            Dictionary<string,int> resourceDictionary = CreateResourceDictionary();
            Dictionary<string,int> priceDictionary = CreatePricesDictionary();
            Console.WriteLine("Welcome to Gold Diggerzz!");
            Console.WriteLine("Created your initial resource dictionary, we're cooking:");
            
            PrintResources(resourceDictionary);
            
            RunGame(resourceDictionary, priceDictionary);
        }
        
        private static DateTime currentDate = new DateTime(2024, 1, 1);
        
        private static void RunGame(Dictionary<string, int> resourceDictionary, Dictionary<string, int> priceDictionary)
        {
            int menuOption;
            do
            {
                menuOption = UserMenuOption(resourceDictionary);
            
                switch (menuOption)
                {
                    case 0:
                        Console.WriteLine("You were in debt, bossman have paid that off so we good now");
                        break;
                    case 1:
                        Console.WriteLine("You have chosen to dig one day");
                        DigOneDay(resourceDictionary);
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
                        GameFailed(resourceDictionary);
                        break;
                    default:
                        Console.WriteLine("Please enter a valid option");
                        break;
                }
            } while (menuOption != 4);
            
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
        
        private static void PrintResources(Dictionary<string,int> resources)
        {
            Console.WriteLine("\nHere are your resources.\n");
            foreach (KeyValuePair<string, int> resource in resources)
            {
                Console.WriteLine($"You have {resource.Value} {resource.Key}");
            }
            Console.WriteLine();
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
        
        private static Dictionary<string,int> CreatePricesDictionary()
        {
            Dictionary<string, int> prices = new Dictionary<string, int>()
            {
                { "Gold", 15 },
                { "Diamonds", 75 },
                { "Workers", 100 }
            };
            
            return prices;
        }
        
        private static int UserMenuOption(Dictionary<string,int> resources)
        {
            string takeUserInput = CheckIfInDebt(resources);
            if (takeUserInput == "false")
            {
                Console.WriteLine($"Today is {currentDate:dddd, d MMMM, yyyy}");
                Console.WriteLine("Please select an option:");
                Console.WriteLine("_________________________");
                Console.WriteLine("1 - Dig one day");
                Console.WriteLine("2 - Go to market");
                Console.WriteLine("3 - Print game mechanics");
                Console.WriteLine("4 - Quit game");
            
                int userOption = int.Parse(Console.ReadLine());
                Console.Clear();
                return userOption;
            }

            if(takeUserInput == "bankrupt")
            { 
                return 5;
            }
            
            return 0;
        }
        
        private static void DigOneDay(Dictionary<string,int> resources)
        {
            Console.WriteLine("We are about to dig, let us cook");
            Console.WriteLine("\nDigging...................\n");
            Console.WriteLine("Digging done for the day");
            
            Console.WriteLine("Here are the changes to your resources:");
            
            int totalWages = resources["Workers"] * 10;
            
            // creating randoms for the chance of finding gold and diamonds
            Random random = new Random();
            int finalRandom = random.Next(0, 10);
            
            // 60% chance of finding gold
            bool goldFound = finalRandom < 6;
            
            // 10% chance of finding diamonds
            bool diamondFound = finalRandom < 1;
            
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
            
            currentDate = currentDate.AddDays(1);
        }
        
        private static void GoToMarket(Dictionary<string, int> resources, Dictionary<string, int> priceDictionary)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    WELCOME TO THE MARKET                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine($"Here are the rates for {currentDate:dddd dd MMMM, yyyy}:");
            
            foreach (KeyValuePair<string, int> item in priceDictionary)
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

                marketOption = int.Parse(Console.ReadLine());

                switch (marketOption)
                {
                    case 1:
                        Console.WriteLine("You have chosen to sell gold for dollars");
                        Console.WriteLine($"How much gold do you want to sell?\nEnter '-1' to sell all your gold\nYou have {resources["Gold"]} gold");
                        int goldToSell = int.Parse(Console.ReadLine());
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
                        int diamondsToSell = int.Parse(Console.ReadLine());
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
                        Console.WriteLine("Remember each employee charges $10 in wages per day");
                        int employeesToHire = int.Parse(Console.ReadLine());
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
        
        private static string CheckIfInDebt(Dictionary<string,int> resources)
        {
            string inDebt = "false";
            bool gameFailed = false;
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

        private static void QuitGame(Dictionary<string,int> resources)
        {
            Console.WriteLine("You have chosen to quit the game");
            Console.WriteLine("Your final stats were:");
            PrintResources(resources);
            Console.WriteLine("\nGoodbye!");
        }

        private static void GameFailed(Dictionary<string,int> resources)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                         YOU FAILED                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            QuitGame(resources);
        }

    }
}
