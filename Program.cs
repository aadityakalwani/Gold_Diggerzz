using System;
using System.Collections.Generic;
using System.Threading;

namespace Gold_Diggerzz
{          
    internal abstract class Program

    {
        // initial inspiration: https://replit.com/@AadityaKalwani/Digging-Simulator#main.py
        
        /*
         * current issues
        */
        
        /* to-do ideas
         * more resources eg. diamonds, coal, etc.
         * tutorial mode
         * loans - you can take a loan from the bank and pay it back with interest
         * option to invest in the stock market
         * load/save game by saving the dictionaries to a file
         * more power-ups
         * add a weather system that changes the probabilities of finding resources
         * stock market feature (kinda done?)
         * managers that do shit
         * or you can 'restart' and sacrifice all your $$$ for a better location with better iron payments per day
         * (like prestige in all the idle miner games i played)
         
         * can't do until i have an individual employee stat:
         * per-employee stats
         * workers retire after x days
         * add a 'luck' stat for each employee that changes the probabilities of finding resources
         * send individual number of employees for training course that then boosts their productivity
         */ 
        
        /*
         * hierarchy
         
             - Main()
                   - CreateResourceDictionary()
                   - CreatePricesDictionary()
                   - PrintResources(Dictionary<string, double> resources)
                   - RunGame(Dictionary<string, double> resourceDictionary, Dictionary<string, double> priceDictionary)
                         - UserMenuOption(Dictionary<string, double> resources, Dictionary<string, double> prices)
                                  - CheckIfInDebt(Dictionary<string, double> resources)
                                  - CalendarEffects(Dictionary<string, double> prices, DateTime currentDate)
                         - Dig(Dictionary<string, double> resources, Dictionary<string, double> prices)
                                  - PrintResources(Dictionary<string, double> resources)
                         - GoToMarket(Dictionary<string, double> resources, Dictionary<string, double> priceDictionary)
                                  - PrintResources(Dictionary<string, double> resources)
                         - PrintGameMechanics()
                         - QuitGame(Dictionary<string, double> resources)
                         - GameFailed(Dictionary<string, double> resources)
                                  - QuitGame(Dictionary<string, double> resources)
             - GetValidInt()
             - GetValidDouble()

           This hierarchy shows the flow of your program and how each subroutine is called from its parent subroutine.
         */
        
        /*
        current features: (also in PrintMechanics())
        chance of finding iron = 65%
        chance of finding gold = 15%
        chance of finding Ancient Artefact = 5%

        cost of hiring employee = $100
        iron value =  $15
        gold value = $75
        
        iron and gold values fluctuate by upto ±10% per day

        Ancient Artefact has two powerup options:
        $250 instantly, or a 50% chance of finding gold for the next 5 days

        the resources you gain are equal to the number of employees you have times their efficiency
        eg. 7 employees = 7 iron found on that day

        baseline wage = $10 per employee per day

        10% chance an employee is ill and doesnt come in to work

        30% pay increase on weekends only
        
        on the first of every month, employee wage increases by 10%

        on the 15th of each month, each employee gets 10% of your current $$$ stash (profit sharing)

        one x date every month, there is a stock market crash where iron, gold, and employee hiring prices halve

        you can bribe the govt with $150 and not pay any wages for the next 3 days

        at any time if your $$$ balance goes negative, the govt sells all of your resources for 50% the current market rate
        
        if you have no resources to sell, they sell your employees for $100 each

        if your $$$ balance is negative and you have no resource, you fail the game
        */
        
        // imagine these as like global variables
        // the ints are for the number of days left for the effect to wear off - set to 0 in Main() during pre-game
        private static bool _animation = true;
        private static int _increasedGoldChanceDays;
        private static int _noWageDaysLeft;
        private static int _lessWorkerDays;
        private static int _crashDaysLeft;
        private static int _badWeatherDaysLeft;
        private static int _hurricaneDaysLeft;
        private static int _beautifulSkyDaysLeft;
        private static int _totalBribes;
        private static double _totalIronFound;
        private static double _totalGoldFound;
        private static double _totalDaysDug;
        private static double _totalEmployeesHired;
        private static double _employeeEfficiency = 1;
        private static DateTime _currentDate = new DateTime(2024, 1, 1);
        static Random _random = new Random();
        private static int _crashDate = _random.Next(0, 28);
        
        private static void Main()
        {
            // pregame
            Dictionary<string, double> resourceDictionary = CreateResourceDictionary();
            Dictionary<string, double> priceDictionary = CreatePricesDictionary();

            _lessWorkerDays = 0;
            _increasedGoldChanceDays = 0;
            _noWageDaysLeft = 0;
            _crashDaysLeft = 0;
            _badWeatherDaysLeft = 0;
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 Welcome to Gold Diggerzz                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            Console.WriteLine("The aim of the game is to survive for as long as possible before bankruptcy");
            Console.WriteLine("This are your initial resources...Good luck!");
            
            PrintResources(resourceDictionary);
            // pregame ends
            
            // game starts
            RunGame(resourceDictionary, priceDictionary);
        }
        
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
                        _animation = true;
                        Console.WriteLine("You have chosen to dig one day");
                        Dig(resourceDictionary, priceDictionary, 1);
                        break;
                    case 2:
                        GoToMarket(resourceDictionary, priceDictionary);
                        break;
                    case 3:
                        PrintGameMechanics();
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
                        _animation = false;
                        Console.WriteLine("Enter number of days to dig in one go");
                        int daysToDig = GetValidInt();
                        Dig(resourceDictionary, priceDictionary, daysToDig);
                        break;
                    case 7:
                        Console.WriteLine("You have chosen to bribe the government");
                        Console.WriteLine("You have been charged $150 for the bribe");
                        resourceDictionary["Dollars"] -= 150;
                        Console.WriteLine("You don't have to pay wages for the next three days");
                        _noWageDaysLeft = 3;
                        _totalBribes += 1;
                        break;
                    case 8:
                        Console.WriteLine("Giving you the information now...");
                        Console.WriteLine($"Expect a stock market crash on the {_crashDate}th of every month");
                        break;
                    case 9:
                        PrintStats();
                        break;
                    case 10:
                        if (resourceDictionary["Dollars"] > 200 * resourceDictionary["Workers"])
                        {
                            Console.WriteLine("You have chosen to send all employees on a training course");
                            Console.WriteLine("You have been charged $200 per employee");
                            Console.WriteLine("Your employees will be back in 7 days");
                            EmployeeTrainingCourse(resourceDictionary);
                        }
                        else
                        {
                            Console.WriteLine("You don't have enough money to send all employees on a training course");
                        }
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
        
        private static void PrintGameMechanics()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    GAME MECHANICS                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            Console.WriteLine("Chance of finding iron = 65%");
            Console.WriteLine("Chance of finding gold = 15%");
            Console.WriteLine("Chance of finding Ancient Artefact = 5%");
            Console.WriteLine("Chance of finding a magic token = 5%");
            Console.WriteLine("\nCost of hiring employee = $100");
            Console.WriteLine("Iron value = $15");
            Console.WriteLine("Gold value = $75");
            Console.WriteLine("Iron and gold values fluctuate by upto ± 10% per day");
            Console.WriteLine("Each magic token increases selling price by 20%, and you can obtain upto 3 of these");
            Console.WriteLine("\nAncient Artefact has two powerup options:");
            Console.WriteLine("$250 instantly, or a 50% chance of finding gold for the next 5 days");
            Console.WriteLine("\nThe resources you gain are equal to the number of employees you have times their efficiency");
            Console.WriteLine("Sending an employee on a training course increases their efficiency by 50%");
            Console.WriteLine("Eg. 7 employees = 7 iron found on that day");
            Console.WriteLine("\nBaseline wage = $10 per employee per day");
            Console.WriteLine("10% chance an employee is ill and doesn't come in to work");
            Console.WriteLine("30% pay increase on weekends only");
            Console.WriteLine("On the first of every month, employee wage increases by 10%");
            Console.WriteLine("On the 15th of each month, each employee gets 10% of your current $$$ stash (profit sharing)");
            Console.WriteLine("One x date every month, there is a stock market crash where iron, gold, and employee hiring prices halve");
            Console.WriteLine("\nYou can bribe the govt with $150 and not pay any wages for the next 3 days");
            Console.WriteLine("\nAt any time if your $$$ balance goes negative, the govt sells all of your resources for 50% the current market rate");
            Console.WriteLine("If you have no resources to sell, they sell your employees for $100 each");
            Console.WriteLine("If your $$$ balance is negative and you have no resource, you fail the game");
        }
        
        private static void PrintStats()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                        YOUR STATS                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            Console.WriteLine($"Here are your stats as of {_currentDate.Date: dddd, dd MMMM yyyy}:");
            Console.WriteLine($"Total iron found {_totalIronFound}");
            Console.WriteLine($"Total gold found {_totalGoldFound}");
            Console.WriteLine($"Total employees hired: {_totalEmployeesHired}");
            Console.WriteLine($"Total days dug: {_totalDaysDug}");
            Console.WriteLine($"Total bribes paid: {_totalBribes}");
        }
        
        private static void PrintResources(Dictionary<string, double> resources)
        {
            Console.WriteLine("__________________________________");
            Console.WriteLine($"| You have ${resources["Dollars"]}                   |");
            Console.WriteLine($"| You have {resources["iron"]}kg of iron            |");
            Console.WriteLine($"| You have {resources["gold"]}kg of gold            |");
            Console.WriteLine($"| You have {resources["Workers"]} employees            |");
            Console.WriteLine($"| You have {resources["magicTokens"]} magic tokens         |");
            Console.WriteLine($"| Your employees' efficiency is {_employeeEfficiency} |");
            Console.WriteLine("_________________________________\n");
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
                { "iron", 0 },
                { "gold", 0 },
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
                {"iron", 15},
                {"gold", 75},
                {"Workers", 100},
                {"Wage", 10}
            };
            
            return prices;
        }

        private static Dictionary<string, double> CreateProbabilityDictionary()
        {
            Dictionary<string, double> probabilities = new Dictionary<string, double>()
            {
                { "iron", 65 },
                { "gold", 15 },
                { "AncientArtefact", 5 },
                { "magicToken", 5 },
                { "employeeIll", 10 },
                { "stockMarketCrash", 5 }
            };
            
            return probabilities;
        }
        
        private static int UserMenuOption(Dictionary<string, double> resources, Dictionary<string, double> prices)
        {
            string takeUserInput = CheckIfInDebt(resources, prices);
            
            ChangeProbabilities(prices, _currentDate, resources);
            
            if (takeUserInput == "false")
            {
                Console.WriteLine($"Today is {_currentDate:dddd, d MMMM, yyyy}");
                Console.WriteLine("___________________________________");
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1 - Dig one day");
                Console.WriteLine("2 - Go to market");
                Console.WriteLine("3 - Print game mechanics");
                Console.WriteLine("4 - Quit game");
                Console.WriteLine("5 - Skip one day - $30 cost");
                Console.WriteLine("6 - Dig multiple days");
                Console.WriteLine("7 - Bribe the government for $150 to not pay wages for the next three days");
                Console.WriteLine("8 - Pay $50 for information on the next stock market crash");
                Console.WriteLine("9 - Print stats");
                Console.WriteLine("10 - Send all employees for a training course for $200 per employee (7 days)");
                Console.WriteLine("___________________________________");
                Console.WriteLine("Your choice:");
             
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
        
        private static string CheckIfInDebt(Dictionary<string, double> resources, Dictionary<string, double> prices)
        {
            string inDebt = "false";
            if (resources["Dollars"] < 0)
            {
                inDebt = "true";
                
                if (inDebt == "true")
                {
                    Console.WriteLine("\n\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31");
                    Console.WriteLine("You are in debt, bossman is coming for you");
                    Console.WriteLine("The government will come and sell all your resources for 2/5 the rate");
                    Console.WriteLine("They're also reducing your percentage chances of finding resources by 30% for the next three days");
                    Console.WriteLine($"right now you have ${resources["Dollars"]}, {resources["gold"]} gold and {resources["iron"]} iron");
                    Console.WriteLine("Unlucky bro...");
                    Console.WriteLine("After bossman stole your resources, you now have:");

                    resources["Dollars"] += resources["iron"] * prices["iron"];
                    resources["Dollars"] += resources["gold"] * prices["iron"]; 
                
                    resources["iron"] = 0;
                    resources["gold"] = 0;
                
                    PrintResources(resources);
                }
                
                if (inDebt == "true" && resources["iron"] == 0 && resources["gold"] == 0 && resources["Workers"] == 0)
                {
                    Console.WriteLine("Bro you're literally bankrupt. You have failed the game.");
                    return "bankrupt";
                }
                
                if (inDebt == "true" && resources["iron"] == 0 && resources["gold"] == 0 && resources["Workers"] != 0)
                {
                    Console.WriteLine("You don't have resources to sell, so we're selling workers for $100 per guy.");
                    resources["Dollars"] += resources["Workers"] * 100;
                    resources["Workers"] = 0;
                }
            }
            
            return inDebt;
        }
        
        private static void Dig(Dictionary<string, double> resources, Dictionary<string, double> prices, int daysToDig)
        {
            for (int days = 0; days <= daysToDig; days++)
            {
                if (CheckIfInDebt(resources, prices) !=  "true")
                {
                    if (_animation)
                    {
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
                        
                        Thread.Sleep(1000);
                                                                                                                                     
                        for (int i = 0; i < 10; i++)                                                                                
                        {                                                                                                           
                            Thread.Sleep(150);                            
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
                                    Console.Write(" ");
                                }
                            }
                            Console.WriteLine("|");       
                        }      
                    
                        Thread.Sleep(500); 
                    }

                    double newResourcesFound = resources["Workers"] * _employeeEfficiency;
            
                    Console.WriteLine("Digging done for the day");
                    Console.WriteLine("Here are the changes to your resources:");
            
                    // creating randoms for the chance of finding iron and gold
                    Random random = new Random();
                    int finalRandom = random.Next(0, 100);
            
                    // 65% chance of finding iron
                    bool ironFound = finalRandom < 65;
            
                    // 5% chance of finding the Ancient Artefact superpower
                    bool ancientArtefactFound = finalRandom < 5;
            
                    if (ancientArtefactFound)
                    {
                        Console.Write("\ud83c\udffa You found the Ancient Artefact power-up \ud83c\udffa");
                        Console.WriteLine("Choose a powerup:");
                        Console.WriteLine("1 - 50% chance of finding gold for the next five days");
                        Console.WriteLine("2 - $250 instantly");
                        int userInput = GetValidInt();

                        switch (userInput)
                        {
                            case 1:
                                Console.WriteLine("You have chosen the 50% chance of finding gold for the next five days");
                                _increasedGoldChanceDays = 5;
                                break;
                            case 2:
                                Console.WriteLine("You have chosen the $250 instantly");
                                resources["Dollars"] += 250;
                                break;
                        }
                
                    }
            
                    // if there is a changed chance of finding gold due to the Ancient Artefact powerup
                
                    bool goldFound;
                
                    if (_increasedGoldChanceDays != 0)
                    {
                        Console.WriteLine($"You have the Ancient Artefact powerup, you have a 50% chance of finding gold for the next {_increasedGoldChanceDays} days");
                        goldFound = finalRandom < 50;
                        _increasedGoldChanceDays -= 1;
                    }
                    else
                    {
                        // 15% chance of finding gold
                        goldFound = finalRandom < 15;
                    }
            
                    // 5% chance of getting a magicToken
                    bool magicTokenFound = finalRandom < 5;
                    if (magicTokenFound && resources["magicTokens"] < 3)
                    {
                        resources["magicTokens"] += 1;
                        Console.WriteLine($"You've acquired another magic token. You have {resources["magicTokens"]} magic tokens now");
                        Console.WriteLine($"Selling price increased by a total of {resources["magicTokens"] * 20}%");
                        prices["iron"] *= 1.2;
                        prices["gold"] *= 1.2;
                    }

                    // update values within the resources dictionary
                    if (goldFound)
                    {
                        Console.WriteLine($"You found {newResourcesFound}kg of gold \ud83d\udc51");
                        resources["gold"] += newResourcesFound;
                        _totalGoldFound += newResourcesFound;
                    }
            
                    if (ironFound)
                    {
                        Console.WriteLine($"You found {newResourcesFound}kg of iron \ud83e\uddbe ");
                        resources["iron"] += newResourcesFound;
                        _totalIronFound += newResourcesFound;
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

                    if (_badWeatherDaysLeft != 0)
                    {
                        _badWeatherDaysLeft -= 1;
                    }
                    
                    if (_hurricaneDaysLeft != 0)
                    {
                        _hurricaneDaysLeft -= 1;
                    }
                    
                    if (_beautifulSkyDaysLeft != 0)
                    {
                        _beautifulSkyDaysLeft -= 1;
                    }
                    
                    if (daysToDig == 1)
                    { 
                        PrintResources(resources);
                    }
            
                    _currentDate = _currentDate.AddDays(1);
                }
            
                ChangePrices(prices);
                _totalDaysDug += 1;
            }
            
            if (daysToDig > 1)
            {
                PrintResources(resources);
            }
            
        }
        
        private static void GoToMarket(Dictionary<string, double> resources, Dictionary<string, double> priceDictionary)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    WELCOME TO THE MARKET                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            Console.WriteLine($"Here are the rates for {_currentDate:dddd dd MMMM, yyyy}:");
            
            Console.WriteLine("______________________________");
            Console.WriteLine($"| Iron: ${priceDictionary["iron"]} per kg");
            Console.WriteLine($"| Gold: ${priceDictionary["gold"]} per kg");
            Console.WriteLine($"| Employees: ${priceDictionary["Workers"]} per employee");
            Console.WriteLine($"| Wages: ${priceDictionary["Wage"]} per employee per day");
            Console.WriteLine("______________________________");
            
            
            int marketOption;
            do
            {
                Console.WriteLine("Here is the menu for the market:");
                Console.WriteLine("1 - Sell iron for dollars");
                Console.WriteLine("2 - Sell gold for dollars");
                Console.WriteLine("3 - Hire More Employees");
                Console.WriteLine("4 - Exit market");
                Console.WriteLine("5 - Sell all iron and gold for dollars");

                marketOption = GetValidInt();

                switch (marketOption)
                {
                    case 1:
                        Console.WriteLine("You have chosen to sell iron for dollars");
                        Console.WriteLine($"How much iron do you want to sell?\nYou have {resources["iron"]}kg of iron");
                        double ironToSell = GetValidDouble();
                        
                        if (ironToSell > resources["iron"])
                        {
                            Console.WriteLine("You don't have enough iron to sell that much");
                        }
                        else
                        {
                            resources["iron"] -= ironToSell;
                            resources["Dollars"] += ironToSell * priceDictionary["iron"];
                        }

                        Console.WriteLine("Here are your update resources:");
                        PrintResources(resources);

                        break;
                    case 2:
                        Console.WriteLine("Your have chosen to sell gold for dollars");
                        Console.WriteLine($"How much gold do you want to sell?\nYou have {resources["gold"]}kg of gold");
                        double goldToSell = GetValidInt();
                        if (goldToSell > resources["gold"])
                        {
                            Console.WriteLine("You don't have enough gold to sell that much");
                        }
                        else
                        {
                            resources["gold"] -= goldToSell;
                            resources["Dollars"] += goldToSell * priceDictionary["gold"];
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
                            _totalEmployeesHired += employeesToHire;
                        }
                        break;
                    case 4:
                        Console.WriteLine("Thanks for coming to the market!");
                        break;
                    case 5:
                        Console.WriteLine("We're selling all your iron and gold for dollars");
                        resources["Dollars"] += resources["iron"] * priceDictionary["iron"];
                        resources["Dollars"] += resources["gold"] * priceDictionary["gold"];
                        resources["iron"] = 0;
                        resources["gold"] = 0;
                        PrintResources(resources);
                        break;
                }
            } while (marketOption != 4);
        }

        private static void QuitGame(Dictionary<string, double> resources)
        {
            Console.WriteLine("Your final stats were:");
            PrintResources(resources);
            PrintStats();
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
        
        private static void ChangeProbabilities(Dictionary<string, double> prices, DateTime currentDate, Dictionary<string, double> resources)
        {
            
            // calendar effects: weekend pay, stock market crash, wage increase, employee illness, profit sharing
            
            
            // +30% pay on weekends - wage is increased on saturday, then reduced again on monday
            if (currentDate.DayOfWeek is DayOfWeek.Saturday)
            {
                Console.WriteLine("It's the weekend, your employees want 30% more pay");
                prices["Wage"] *= 1.3;
            }
            
            // to undo the effect of weekend pay
            else if (currentDate.DayOfWeek is DayOfWeek.Monday)
            {
                if (currentDate.Month != 1)
                {
                    prices["Wage"] = prices["Wage"] * 10/13;
                }
            }
            
            // stock market code below
            // to undo the effects of the crash
            if (_crashDaysLeft > 1)
            {
                Console.WriteLine("The stock market has recovered");
                prices["iron"] *= 2;
                prices["gold"] *= 2;
                prices["Workers"] *= 2;
                _crashDaysLeft = 0;
            }
            
            if (currentDate.Day == _crashDate && _crashDaysLeft == 0)
            {
                Console.WriteLine("The stock market has crashed, your iron and gold prices have plummeted but you can hire employees for cheaper");
                
                prices["iron"] /= 2;
                prices["gold"] /= 2;
                prices["Workers"] /= 2;
                _crashDaysLeft = 2;
            }
            
            // 10% raise on the first of every month (apart from January)
            if (currentDate.Month != 1 && currentDate.Day == 1)
            {
                Console.WriteLine("It's the first of the month, your employees want a 10% raise");
                prices["Wage"] *= 1.1;
            } 
            
            // to undo the effects of unwell workers
            if (_lessWorkerDays == 1)
            {
                resources["Workers"] += 1;
                Console.WriteLine("Your employee is back at work today");
                _lessWorkerDays = 0;
            }
            
            // 10% chance an employee is unwell and doesn't come in
            if (_random.Next(0, 100) < 10)
            {
                Console.WriteLine("One of your employees is unwell and doesn't come in today");
                resources["Workers"] -= 1;
                _lessWorkerDays = 1;
            }
            
            // 10% profit sharing to each employee on the 15th of every month
            if (currentDate.Day == 15)
            {
                Console.WriteLine("Profit sharing time!");
                    
                if (resources["Workers"] < 8)
                {
                    Console.WriteLine("Each employee gets 10% of your current $$$ stash");
                    Console.WriteLine($"Your {resources["Workers"]} employees get ${resources["Dollars"] * 0.1} each");
                    double dollarsToLose = resources["Dollars"] * 0.1 * resources["Workers"];
                    resources["Dollars"] -= dollarsToLose;
                    Console.WriteLine($"Your employees have been paid, you have lost $ {dollarsToLose} in the process");
                }
                
                Console.WriteLine("Because you have so many employees, 60% of your current $$$ stash is given to them");
                Console.WriteLine($"This means you'll lose {resources["Dollars"] * 0.6}");
                resources["Dollars"] *= 0.4;
            }
            
            
            // weather effects: rain reducing efficiency, hurricane, beautifulSky increasing efficiency
            

            // rain reducing efficiency
            // undo the effects of rain reduced efficiency
            if (_badWeatherDaysLeft == 0)
            {
                Console.WriteLine("The weather has cleared up, your employees are back to normal efficiency");
                _employeeEfficiency = _employeeEfficiency * 10/7;
            }
            
            if (_random.Next(0, 100) < 30)
            {
                Console.WriteLine("Due to torrential rain, your employees are 30% less efficient for the next two days");
                _employeeEfficiency *= 0.7;
                _badWeatherDaysLeft = 2;
            }
            
            
            // to undo effects of hurricane
            if (_hurricaneDaysLeft == 0)
            {
                Console.WriteLine("The hurricane has passed, your employees are back to normal efficiency");
                _employeeEfficiency = _employeeEfficiency * 10/3;
            }
            
            // 5% chance a hurricane that reduces the probability of finding resources by 50% for the next 5 days
            if (_random.Next(0, 100) < 5)
            {
                Console.WriteLine("A hurricane is coming, efficiency is now 30% the next five days");
                _employeeEfficiency *= 0.3;
                _hurricaneDaysLeft = 5;
            }

            if (_random.Next(0, 100) < 40)
            {
                Console.WriteLine("The weather is beautiful today, your employees are 50% more efficient for two days");
                _employeeEfficiency *= 1.5;
                _beautifulSkyDaysLeft = 2;
            }
            
        }

        private static void ChangePrices(Dictionary<string, double> prices)
        {
            // upto a 30% fluctuation in prices based on random probability
            Random random = new Random();
            int randomChange = random.Next(-10, 10);

            prices["iron"] += randomChange;
            prices["gold"] += randomChange;
            
            Console.WriteLine("The prices of iron and Gold have been changed in line with stock market movements");

        }

        private static void EmployeeTrainingCourse(Dictionary<string, double> resources)
        {
            // to boost the productivity of employees
            Console.WriteLine($"This course charged you {200 * resources["Workers"]} in fees");
            resources["Dollars"] -= 200 * resources["Workers"];
            _employeeEfficiency *= 1.5;
            
            _currentDate.AddDays(7);
            Console.WriteLine("Training employees...");
            Thread.Sleep(1500);
            Console.WriteLine("7 Days have now passed");
        }
        
        private static int GetValidInt()
        {
            if (int.TryParse(Console.ReadLine(), out int validInt))
            {
                if (validInt >= 0)
                {
                    return validInt;
                }
            
                Console.WriteLine("No bro dont try and enter negative numbers");
                GetValidInt();
               
                
            }

            Console.WriteLine("Please enter a valid integer");
            return GetValidInt();
        }
        
        private static double GetValidDouble()
        {
            if (double.TryParse(Console.ReadLine(), out double validDouble)) 
            {
                if (validDouble >= 0)
                {
                    return validDouble;
                }
            
                Console.WriteLine("No bro dont try and enter negative numbers");
                GetValidDouble();
                
            }
        
            Console.WriteLine("Please enter a valid double");
            return GetValidDouble();
        }
    }
}
