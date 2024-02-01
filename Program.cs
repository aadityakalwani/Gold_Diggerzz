using System;
using System.Collections.Generic;
using System.Threading;

namespace Gold_Diggerzz
{          
    internal abstract class Program

    {
        
        /*
         * current issues
         * when you 'skip a day' the game stops
            * this is probably because in some switch case i'd need to return a different value or something
         * when you go bankrupt it is in a while true look and infinitely printing "YOU FAILED" - maybe turn to a do while? idfk
         * pre-bankruptcy, it should sell your workers for $100 each
         */
        
        /* to-do ideas
         * (initial inspiration: https://replit.com/@AadityaKalwani/Digging-Simulator#main.py)
         * fancy nice animations eg. to see very clearly that you're in the market or you're digging
         * more resources eg. iron, coal, etc.
         * managers that do shit
         * or you can 'restart' and sacrifice all your $$$ for a better location with better gold payments per day
         * (like prestige in all the idle miner games i played)
         */ 
        
        /*
         * hierarchy
         
           - `Main()`
                 - `CreateResourceDictionary()`
                 - `CreatePricesDictionary()`
                 - `PrintResources(Dictionary<string, double> resources)`
                 - `RunGame(Dictionary<string, double> resourceDictionary, Dictionary<string, double> priceDictionary)`
                       - `UserMenuOption(Dictionary<string, double> resources, Dictionary<string, double> prices)`
                                - `CheckIfInDebt(Dictionary<string, double> resources)`
                                - `CalendarEffects(Dictionary<string, double> prices, DateTime currentDate)`
                       - `DigOneDay(Dictionary<string, double> resources, Dictionary<string, double> prices)`
                                - `PrintResources(Dictionary<string, double> resources)`
                       - `GoToMarket(Dictionary<string, double> resources, Dictionary<string, double> priceDictionary)`
                                - `PrintResources(Dictionary<string, double> resources)`
                       - `PrintRules()`
                       - `QuitGame(Dictionary<string, double> resources)`
                       - `GameFailed(Dictionary<string, double> resources)`
                                - `QuitGame(Dictionary<string, double> resources)`
           - `GetValidInt()`
           - `GetValidDouble()`
           
           This hierarchy shows the flow of your program and how each subroutine is called from its parent subroutine.
         */
        
        /*
         current features:
         chance of finding gold = 65%
         chance of finding iron = 15%
           chance of finding Ancient Artefact = 5%
           
           cost of hiring employee = $100
           gold value =  $15
           iron value = $75
           
           Ancient Artefact has two powerup options:
           $200 instantly, or a 5% chance of finding iron for the next 5 days
           
           the resources you gain are equal to the number of employees you have
           eg. 7 employees = 7 gold found on that day
           
           baseline wage = $10 per employee per day
           
           10% chance an employee is ill and doesnt come in to work
           
           30% pay increase on weekends only
           on the first of every month, employee wage increases by 10%
           
           on the 15th of each month, each employee gets 10% of your current $$$ stash (profit sharing)
           
           one x date every month, there is a stock market crash where gold, iron, and employee hiring prices halve
           
           you can bribe the govt with $150 and not pay any wages for the next 3 days
           
        
           at any time if your $$$ balance goes negative, the govt sells all of your resources for 50% the current market rate
           
           if your $$$ balance is negative and you have no resource, you fail the game
         */
        
        private static void Main()
        {
            // pregame:
            Dictionary<string, double> resourceDictionary = CreateResourceDictionary();
            Dictionary<string, double> priceDictionary = CreatePricesDictionary();
            
            Console.WriteLine("Welcome to Gold Diggerzz!");
            Console.WriteLine("The aim of the game is to survive for as long as possible before bankruptcy");
            Console.WriteLine("We have created your initial resource dictionary, we're cooking:");
            
            PrintResources(resourceDictionary);
            
            RunGame(resourceDictionary, priceDictionary);
        }
        
        // imagine this as like global variables
        private static int _increasedironChanceDays;
        private static int _noWageDaysLeft;
        private static int _lessWorkerDays;
        
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
                        PrintResources(resourceDictionary);
                        break;
                    case 6:
                        Console.WriteLine("Enter number of days to dig in one go");
                        int daysToDig = GetValidInt();
                        for (int i = 0; i < daysToDig; i++)
                        {
                            if (CheckIfInDebt(resourceDictionary, priceDictionary) !=  "true")
                            {
                                DigOneDay(resourceDictionary, priceDictionary);
                            }
                            else
                            {
                                break;
                                // If the user is in debt, stop the digging process
                            }
                        }
                        break;
                    case 7:
                        Console.WriteLine("You have chosen to bribe the government");
                        Console.WriteLine("You have been charged $150 for the bribe");
                        resourceDictionary["Dollars"] -= 150;
                        Console.WriteLine("You don't have to pay wages for the next three days");
                        _noWageDaysLeft = 3;
                        break;
                    case -1:
                        GameFailed(resourceDictionary);
                        break;
                    default:
                        Console.WriteLine("Please enter a valid option");
                        break;
                }
            } while (menuOption != 4 && menuOption != -1);
            
        }
        
        private static void PrintRules()
        {
            Console.WriteLine("Each employee of yours charges $10 in wages for the day");
            Console.WriteLine("Each day, there is an 60% chance of finding gold");
            Console.WriteLine("Each day, there is a 10% chance of finding iron");
            Console.WriteLine("If you find gold or iron, you gain the number of employees you have of the resource");
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
                { "Gold", 0 },
                { "iron", 0 },
                { "Dollars", 100 },
                { "Workers", 1 },
                { "magicTokens", 0}
            };
            return resources;
        }
        
        private static Dictionary<string, double> CreatePricesDictionary()
        {
            Dictionary<string, double> prices = new Dictionary<string, double>()
            {
                {"Gold", 15},
                {"iron", 75},
                {"Workers", 100},
                {"Wage", 10}
            };
            
            return prices;
        }
        
        private static int UserMenuOption(Dictionary<string, double> resources, Dictionary<string, double> prices)
        {
            string takeUserInput = CheckIfInDebt(resources, prices);
            
            CalendarEffects(prices, _currentDate, resources);
            
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
                Console.WriteLine("7 - Bribe the government");
            
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
            
            bool ironFound;

            Console.WriteLine("We are about to dig, let us cook");

            // ASCII art animation for digging
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
             
            Console.WriteLine("Starting to dig...");                                                                    
                                                                                                                         
            for (int i = 0; i < 10; i++)                                                                                
            {                                                                                                           
                Thread.Sleep(250);                            
                Console.Clear();                                                                                        
                for (int j = 0; j < shovel.Length; j++)                                                                 
                {                                                                                                       
                     if (j < shovel.Length - i)                                                                          
                     {                                                                                                   
                         Console.WriteLine(shovel[j]);                                                                   
                     }                                                                                                   
                     else                                                                                                
                     {                                                                                                   
                         string spaces = "";
                         for (int k = 0; k < i; k++)
                         {
                             spaces += " ";
                         }
                         Console.WriteLine(spaces + shovel[j]);                                            
                     }                                                                                                   
                }

                Console.WriteLine("Progress:");
                for (int j = 0; j < 10; j++)
                {
                    if (j < i)
                    {
                        Console.Write("##");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                Console.WriteLine("|");       
            }      
            
            Thread.Sleep(500);
            
            Console.WriteLine("Digging done for the day");
            Console.WriteLine("Here are the changes to your resources:");
            
            // creating randoms for the chance of finding gold and iron
            Random random = new Random();
            int finalRandom = random.Next(0, 100);
            
            // 65% chance of finding gold
            bool goldFound = finalRandom < 65;
            
            // 5% chance of finding the Ancient Artefact superpower
            bool ancientArtefactFound = finalRandom < 5;
            
            if (ancientArtefactFound)
            {
                Console.Write("\ud83c\udf1f You found the Ancient Artefact power-up \ud83c\udf1f");
                Console.WriteLine("Choose a powerup:");
                Console.WriteLine("1 - 50% chance of finding iron for the next five days");
                Console.WriteLine("2 - $250 instantly");
                int userInput = GetValidInt();

                switch (userInput)
                {
                    case 1:
                        Console.WriteLine("You have chosen the 50% chance of finding iron for the next five days");
                        _increasedironChanceDays = 5;
                        break;
                    case 2:
                        Console.WriteLine("You have chosen the $250 instantly");
                        resources["Dollars"] += 250;
                        break;
                }
                
            }
            
            // if there is a changed chance of finding iron due to the Ancient Artefact powerup
            if (_increasedironChanceDays != 0)
            {
                Console.WriteLine($"You have the Ancient Artefact powerup, you have a 50% chance of finding iron for the next {_increasedironChanceDays} days");
                ironFound = finalRandom < 50;
                _increasedironChanceDays -= 1;
            }
            else
            {
                // 15% chance of finding iron
                ironFound = finalRandom < 15;
            }
            
            // 5% chance of getting a magicToken
            bool magicTokenFound = finalRandom < 5;
            if (magicTokenFound && resources["magicTokens"] < 3)
            {
                resources["magicTokens"] += 1;
                Console.WriteLine($"You've acquired another magic token. You have {resources["magicTokens"]} magic tokens now, increasing selling price by {resources["magicTokens"] * 10}%");
                prices["Gold"] *= 1.1;
                prices["iron"] *= 1.1;
            }

            // update values within the resources dictionary
            if (ironFound)
            {
                Console.WriteLine("OMG bro you found iron \ud83d\udc8e");
                resources["iron"] += resources["Workers"];
            }
            
            if (goldFound)
            {
                Console.WriteLine("OMG bro you found gold \ud83c\udf1f");
                resources["Gold"] += resources["Workers"];
            }

            if (_noWageDaysLeft != 0)
            {
                Console.WriteLine($"You don't have to pay wages today, or for the next {_noWageDaysLeft} days");
                _noWageDaysLeft -= 1;
            }

            else
            {
                double totalWages = resources["Workers"] * prices["Wage"];
                resources["Dollars"] -= totalWages;
            
                Console.WriteLine($"Your {resources["Workers"]} employees charged a wage of ${totalWages} today.");
            }
            
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
                Console.WriteLine("2 - Sell iron for dollars");
                Console.WriteLine("3 - Hire More Employees");
                Console.WriteLine("4 - Exit market");
                Console.WriteLine("5 - Sell all gold and iron for dollars");

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
                            resources["Dollars"] += goldToSell * priceDictionary["Gold"];
                        }

                        Console.WriteLine("Here are your update resources:");
                        PrintResources(resources);

                        break;
                    case 2:
                        Console.WriteLine("Your have chosen to sell iron for dollars");
                        Console.WriteLine($"How many iron do you want to sell?\nEnter '-1' to sell all your iron.\nYou have {resources["iron"]} iron");
                        double ironToSell = GetValidInt();
                        if (ironToSell == -1)
                        {
                            ironToSell = resources["iron"];
                        }
                        if (ironToSell > resources["iron"])
                        {
                            Console.WriteLine("You don't have enough gold to sell that much");
                        }
                        else
                        {
                            resources["iron"] -= ironToSell;
                            resources["Dollars"] += ironToSell * priceDictionary["iron"];
                        }

                        Console.WriteLine("Here are your updated resources:");
                        PrintResources(resources);

                        break;
                    case 3:
                        Console.WriteLine("Enter how many employees you want to hire:");
                        Console.WriteLine($"Remember each employee charges {priceDictionary["Wage"]} in wages per day right now");
                        int employeesToHire = GetValidInt();
                        if (employeesToHire * priceDictionary["Workers"] > resources["Dollars"])
                        {
                            Console.WriteLine("You don't have enough dollars to hire that many employees");
                        }
                        else
                        {
                            Console.WriteLine("You have hired 1 more employee");
                            resources["Workers"] += employeesToHire;
                            resources["Dollars"] -= employeesToHire * priceDictionary["Workers"];
                            Console.WriteLine($"You now have {resources["Workers"]} employees");
                        }
                        break;
                    case 4:
                        Console.WriteLine("Thanks for coming to the market! Goodbye");
                        break;
                    case 5:
                        Console.WriteLine("We're selling all your gold and iron for dollars");
                        resources["Dollars"] += resources["Gold"] * priceDictionary["Gold"];
                        resources["Dollars"] += resources["iron"] * priceDictionary["iron"];
                        resources["Gold"] = 0;
                        resources["iron"] = 0;
                        PrintResources(resources);
                        break;
                }
            } while (marketOption != 4);
        }
        
        private static string CheckIfInDebt(Dictionary<string, double> resources, Dictionary<string, double> prices)
        {
            string inDebt = "false";
            if (resources["Dollars"] < 0)
            {
                inDebt = "true";
                
                if (inDebt == "true" && resources["Gold"] == 0 && resources["iron"] == 0)
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
                
                    Console.WriteLine($"right now you have ${resources["Dollars"]}, {resources["iron"]} iron and {resources["Gold"]} gold");
                
                    Console.WriteLine("After bossman stole your resources, you now have:");

                    resources["Dollars"] += resources["Gold"] * prices["Gold"];
                    resources["Dollars"] += resources["iron"] * prices["Gold"]; 
                
                    resources["Gold"] = 0;
                    resources["iron"] = 0;
                
                    PrintResources(resources);
                }
            }
            
            return inDebt;
        }

        private static void QuitGame(Dictionary<string, double> resources)
        {
            Console.WriteLine("Your final stats were:");
            PrintResources(resources);
            Console.WriteLine($"You lasted until {_currentDate.Date:dddd, d MMMM, yyyy}");
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
        
        private static void CalendarEffects(Dictionary<string, double> prices, DateTime currentDate, Dictionary<string, double> resources)
        {
            
            // +30% pay on weekends - wage is increased on saturday, then reduced again on monday
            if (currentDate.DayOfWeek is DayOfWeek.Saturday)
            {
                Console.WriteLine("It's the weekend, your employees want 30% more pay");
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
            
            if (currentDate.Day == crashDate || currentDate.Day == crashDate + 1)
            {
                Console.WriteLine("The stock market has crashed, your gold and iron prices have plummeted but you can hire employees for cheaper");
                
                prices["Gold"] /= 2;
                prices["iron"] /= 2;
                prices["Workers"] /= 2;
            }

            
            // 10% raise on the first of every month (apart from January)
            if (currentDate.Month != 1 && currentDate.Day == 1)
            {
                Console.WriteLine("It's the first of the month, your employees want a 10% raise");
                prices["Wage"] *= 1.1;
            }
            
            
            // set _lessWorkerDays to like a million days ago so it doesnt affect anything 
            _lessWorkerDays = 0;
            
            // 10% chance an employee is unwell and doesnt come in
            if (random.Next(0, 100) < 10)
            {
                Console.WriteLine("One of your employees is unwell and doesn't come in today");
                resources["Workers"] -= 1;
                _lessWorkerDays = 1;
            }
            
            // to undo the effects of above
            if (_lessWorkerDays == 1)
            {
                resources["Workers"] += 1;
                Console.WriteLine("Your employee is back at work today");
                _lessWorkerDays = 0;
            }
            
            
            // 10% profit sharing to each employee on the 15th of every month
            if (currentDate.Day == 15)
            {
                Console.WriteLine("Profit sharing, each employee gets 10% of your current $$$ stash");
                Console.WriteLine($"Your {resources["Workers"]} employees get ${resources["Dollars"] * 0.1} each");
                double dollarsToLose = resources["Dollars"] * 0.1 * resources["Workers"];
                resources["Dollars"] -= dollarsToLose;
                Console.WriteLine($"Your employees have been paid, you have lost $ {dollarsToLose} in the process");
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
                
