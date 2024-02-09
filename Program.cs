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
         * after a few days the employee efficiency is super high (like 70ish)
         * print mechanics and tutorial is a) too long and b) incorrect values
         * test out achievements feature
        */
        
        /* to-do ideas
         * earthquakes that loosen soil and make shit easier to find (+ cool animations possible)
         * achievements - eg. find 10kg total iron
         * tutorial mode (that is actually functional)
         * loans - you can take a loan from the bank and pay it back with interest
         * load/save game by saving the resource dictionary and the current date to a file
         * more power-ups
            * "Resource Rush": This powerup could increase the amount of all resources found for a certain number of days. This would allow the player to gather resources more quickly.
            * "Resource Radar" (for each resource): This powerup could increase the chance of finding a specific resource for a certain number of days. For example, if the powerup is activated for gold, then for the next few days, the chance of finding gold would be increased.
         * stock market feature (kinda done?)
             * Every 5 gold sold, increase gold price and for every 5 gold mined/gained, decrease price? Incentivising selling fast and not holding resources for ages
         * option to invest in the stock market
         * managers that do shit
         * or you can 'restart' and sacrifice all your $$$ for a better location with better iron payments per day
            * (like prestige in all the idle miner games i played)
         
         features i can't do until i have an individual employee stat:
         * per-employee stats
         * workers retire after x days
         * add a 'luck' stat for each employee that changes the probabilities of finding resources
             * when you hire an employee they're given a 'luck' rating between 20-80%
         * send individual number of employees for training course that then boosts their productivity
         */ 
        
        /*
         * hierarchy:
         
         - Main()
               - CreateResourceDictionary()
               - CreatePricesDictionary()
               - CreateProbabilityDictionary()
               - PrintResources(Dictionary<string, double> resources)
               - RunGame(Dictionary<string, double> resourceDictionary, Dictionary<string, double> priceDictionary, Dictionary<string, double> probabilityDictionary)
                     - UserMenuOption(Dictionary<string, double> resources, Dictionary<string, double> prices)
                              - CheckIfInDebt(Dictionary<string, double> resources)
                              - CalendarEffects(Dictionary<string, double> prices, DateTime currentDate)
                     - Dig(Dictionary<string, double> resources, Dictionary<string, double> prices, int daysToDig, Dictionary<string, double> probabilities)
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
        chance of finding coal = 90%
        chance of finding stone = 75%
        chance of finding iron = 65%
        chance of finding gold = 20%
        chance of finding diamond = 5% 
        chance of finding Ancient Artefact = 5%

        cost of hiring employee = $100
        coal value = $4
        stone value = $8
        iron value =  $15
        gold value = $75
        diamond value = $200
        
        resource values fluctuate by upto ±10% per day

        Ancient Artefact has two powerup options:
        $250 instantly, or a 50% chance of finding gold for the next 5 days

        the resources you gain are equal to the number of employees you have times their efficiency
        eg. 7 employees = 7 iron found on that day * efficiency of 1.5 = 10.5iron found

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
        private static int _marketMasterDaysLeft;
        private static int _noWageDaysLeft;
        private static int _lessWorkerDays;
        private static int _crashDaysLeft;
        private static int _badWeatherDaysLeft;
        private static int _hurricaneDaysLeft;
        private static int _beautifulSkyDaysLeft;
        private static int _totalBribes;
        private static int _totalPowerUpsUsed;
        private static double _totalCoalFound;
        private static double _totalStoneFound;
        private static double _totalIronFound;
        private static double _totalGoldFound;
        private static double _totalDiamondFound;
        private static double _totalDaysDug;
        private static double _totalEmployeesHired;
        private static double _totalDollarsEarned;
        private static double _employeeEfficiency = 1;
        private static bool _achievement1;
        private static bool _achievement2;
        private static bool _achievement3;
        private static bool _achievement4;
        private static bool _achievement5;
        private static bool _achievement6;
        private static bool _achievement7;
        private static bool _achievement8;
        private static bool _achievement9;
        private static bool _achievement10;
        private static bool _achievement11;
        private static bool _achievement12;
        private static bool _achievement13;
        private static bool _achievement14;
        private static bool _achievement15;
        private static bool _achievement16;
        private static bool _achievement17;
        private static bool _achievement18;
        private static bool _achievement19;
        private static bool _achievement20;
        private static DateTime _currentDate = new DateTime(2024, 1, 1);
        static Random _random = new Random();
        private static int _crashDate = _random.Next(0, 28);
        
        private static void Main()
        {
            // pregame
            Dictionary<string, double> resourceDictionary = CreateResourceDictionary();
            Dictionary<string, double> priceDictionary = CreatePricesDictionary();
            Dictionary<string, double> probabilityDictionary = CreateProbabilityDictionary();
            Dictionary<string, double> powerUpDictionary = CreatePowerUpDictionary();
            List<string> achievementsList = new List<string>();

            // setting the initial values for the global variables
            _lessWorkerDays = 0;
            _increasedGoldChanceDays = 0;
            _marketMasterDaysLeft = 0;
            _noWageDaysLeft = 0;
            _crashDaysLeft = 0;
            _badWeatherDaysLeft = 0;
            _hurricaneDaysLeft = 0;
            _beautifulSkyDaysLeft = 0;
            _totalBribes = 0;
            _totalCoalFound = 0;
            _totalStoneFound = 0;
            _totalIronFound = 0;
            _totalGoldFound = 0;
            _totalDiamondFound = 0;
            _totalDaysDug = 0;
            _totalEmployeesHired = 1;
            _employeeEfficiency = 1;
            _totalPowerUpsUsed = 0;
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 Welcome to Gold Diggerzz                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            Console.WriteLine("Aim of the game: survive for as long as possible before bankruptcy");
            Console.WriteLine("These are your initial resources...");
            
            PrintResources(resourceDictionary);
            
            Console.WriteLine("Do you want a tutorial? (y/n)");
            if (Console.ReadLine() == "y")
            {
                RunTutorial();
                PrintGameMechanics(priceDictionary, probabilityDictionary);
                Thread.Sleep(2000);
            }
            
            
            // game starts
            Console.WriteLine("The game is about to start, good luck...");
            RunGame(resourceDictionary, priceDictionary, probabilityDictionary, powerUpDictionary, achievementsList);
        }
        
        private static void RunGame(Dictionary<string, double> resourceDictionary, Dictionary<string, double> priceDictionary, Dictionary<string, double> probabilityDictionary, Dictionary<string, double> powerUpDictionary, List<string> achievementsList)
        {
            int menuOption;
            do
            {
                menuOption = UserMenuOption(resourceDictionary, priceDictionary);
            
                switch (menuOption)
                {
                    case -2:
                        Console.WriteLine("You were in debt, bossman have paid that off so we good now");
                        break;
                    case -1:
                        GameFailed();
                        break;
                    case 0:
                        QuitGame();
                        break;
                    case 1:
                        _animation = true;
                        Console.WriteLine("You have chosen to dig one day");
                        Dig(resourceDictionary, priceDictionary, 1, probabilityDictionary, powerUpDictionary, achievementsList);
                        break;
                    case 2:
                        _animation = false;
                        Console.WriteLine("Enter number of days to dig in one go (upto 30)");
                        int daysToDig = GetValidInt(1, 30);
                        Dig(resourceDictionary, priceDictionary, daysToDig, probabilityDictionary, powerUpDictionary, achievementsList);
                        break;
                    case 3:
                        GoToMarket(resourceDictionary, priceDictionary);
                        break;
                    case 4:
                        Console.WriteLine("Skipping one day");
                        Console.WriteLine($"You have been charged ${priceDictionary["SkipDay"]} for the costs of skipping a day");
                        resourceDictionary["Dollars"] -= priceDictionary["SkipDay"];
                        _currentDate = _currentDate.AddDays(1);
                        PrintResources(resourceDictionary);
                        break;
                    case 5:
                        if (powerUpDictionary["Ancient Artefact"] !> 0 && powerUpDictionary["Time Machine"] !> 0)
                        {
                            Console.WriteLine("\u274c You don't have any powerups to use \u274c");
                            break;
                        }
                        Console.WriteLine("What powerup do you want to use?");
                        Console.WriteLine($"You have {powerUpDictionary["Ancient Artefact"]} Ancient Artefacts, {powerUpDictionary["Time Machine"]} Time Machines and {powerUpDictionary["Market Master"]} Market Masters\n");
                        Console.WriteLine("0 - Cancel & Return");
                        Console.WriteLine("1 - Ancient Artefact");
                        Console.WriteLine("2 - Time Machine");
                        Console.WriteLine("3 - Market Master");
                        int powerUpChoice = GetValidInt(0, 3);

                        switch (powerUpChoice)
                        {
                            case 1:
                                if (powerUpDictionary["Ancient Artefact"] != 0)
                                {
                                    UsePowerUp(resourceDictionary, priceDictionary, probabilityDictionary, powerUpChoice, powerUpDictionary, achievementsList);   
                                }
                                else
                                {
                                    Console.WriteLine("You don't have any Ancient Artefacts to use");
                                }
                                break;
                            
                            case 2:
                            {
                                if (powerUpDictionary["Time Machine"] != 0)
                                {
                                    UsePowerUp(resourceDictionary, priceDictionary, probabilityDictionary, powerUpChoice, powerUpDictionary, achievementsList);
                                
                                }
                                else
                                {
                                    Console.WriteLine("You don't have any Time Machines to use");
                                }
                            }
                                break;
                            
                            case 3:
                                if (powerUpDictionary["Market Master"] != 0)
                                {
                                    UsePowerUp(resourceDictionary, priceDictionary, probabilityDictionary, powerUpChoice, powerUpDictionary, achievementsList);
                                }
                                else
                                {
                                    Console.WriteLine("You don't have any Market Masters to use");
                                }

                                break;
                        }
                        break;
                    case 6:
                        PrintGameMechanics(priceDictionary, probabilityDictionary);
                        break;
                    case 7:
                        PrintStats();
                        break;
                    case 8:
                        for (int achievementNumber = 0; achievementNumber < achievementsList.Count; achievementNumber++)
                        {
                            Console.WriteLine($"Achievement {achievementNumber}: {achievementsList[achievementNumber]}");
                        }
                        break;
                    case 9:
                        if (resourceDictionary["Dollars"] > priceDictionary["trainingCourse"] * resourceDictionary["Workers"] && resourceDictionary["Workers"] != 0)
                        {
                            Console.WriteLine("You have chosen to send all employees on a training course");
                            Console.WriteLine($"You have been charged {priceDictionary["trainingCourse"]} per employee");
                            Console.WriteLine("Your employees will be back in 7 days");
                            EmployeeTrainingCourse(resourceDictionary, priceDictionary);
                        }
                        else if (resourceDictionary["Dollars"] > priceDictionary["trainingCourse"] * resourceDictionary["Workers"] && resourceDictionary["Workers"] == 0)
                        {
                            Console.WriteLine("You don't have any employees to send on a training course");
                            Console.WriteLine("This could be because of employee illness - try again later");
                        }
                        else
                        {
                            Console.WriteLine("You don't have enough money to send all employees on a training course");
                        }
                        break;
                        
                    case 10:
                        Console.WriteLine("You've chosen to commit a crime. Choose an option:");
                        Console.WriteLine("1 - Pay $50 for information on the next stock market crash");
                        Console.WriteLine("2 - Bribe the government for $200 to not pay wages for the next 3 days");
                        int crimeChoice = GetValidInt(1, 2);

                        switch (crimeChoice)
                        {
                            case 1:
                                Console.WriteLine(
                                    "You have chosen to pay $50 for information on the next stock market crash");
                                Console.WriteLine("You have been charged $50");
                                resourceDictionary["Dollars"] -= 50;
                                Console.WriteLine("Giving you the information now...");
                                Console.WriteLine($"Expect a stock market crash on the {_crashDate}th of every month");
                                break;
                            case 2:
                                Console.WriteLine("You have chosen to bribe the government");
                                Console.WriteLine($"You have been charged {priceDictionary["bribe"]} for the bribe");
                                resourceDictionary["Dollars"] -= priceDictionary["bribe"];
                                Console.WriteLine("You don't have to pay wages for the next three days");
                                _noWageDaysLeft = 3;
                                _totalBribes += 1;
                                break;
                        }

                        break;

                    default:
                        Console.WriteLine("Please enter a valid option");
                        break;
                }
            } while (menuOption != 0 && menuOption != -1);
            
        }

        private static void PrintGameMechanics(Dictionary<string, double> prices,
            Dictionary<string, double> probabilities)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    GAME MECHANICS                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine($"Chance of finding coal = {probabilities["coal"]}%");
            Console.WriteLine($"Chance of finding stone = {probabilities["stone"]}%");
            Console.WriteLine($"Chance of finding iron = {probabilities["iron"]}%");
            Console.WriteLine($"Chance of finding gold = {probabilities["gold"]}%");
            Console.WriteLine($"Chance of finding diamond = {probabilities["diamond"]}%");
            Console.WriteLine($"Chance of finding Ancient Artefact = {probabilities["AncientArtefact"]}%");

            Console.WriteLine($"\nCost of hiring employee = ${prices["Workers"]}");
            Console.WriteLine($"Coal value = ${prices["coal"]}");
            Console.WriteLine($"Stone value = ${prices["stone"]}");
            Console.WriteLine($"Iron value = ${prices["iron"]}");
            Console.WriteLine($"Gold value = ${prices["gold"]}");
            Console.WriteLine($"Diamond value = ${prices["diamond"]}");

            Console.WriteLine("\nResource values fluctuate by upto ±10% per day");
            Console.WriteLine("\nAncient Artefact has two powerup options:");
            Console.WriteLine("$250 instantly, or a 50% chance of finding gold for the next 5 days");
            Console.WriteLine("\nThe resources you gain are equal to the number of employees you have times their efficiency");
            Console.WriteLine("\nBaseline wage = $10 per employee per day");
            Console.WriteLine("\n10% chance an employee is ill and doesn't come in to work");
            Console.WriteLine("\n30% pay increase on weekends only");
            Console.WriteLine("\nOn the first of every month, employee wage increases by 10%");
            Console.WriteLine("\nOn the 15th of each month, each employee gets 10% of your current $$$ stash (profit sharing)");
            Console.WriteLine("\nOne x date every month, there is a stock market crash where iron, gold, and employee hiring prices halve");
            Console.WriteLine("\nYou can bribe the govt with $150 and not pay any wages for the next 3 days");
            Console.WriteLine("\nAt any time if your $$$ balance goes negative, the govt sells all of your resources for 50% the current market rate");
            Console.WriteLine("\nIf you have no resources to sell, they sell your employees for $100 each");
            Console.WriteLine("\nIf your $$$ balance is negative and you have no resource, you fail the game");

        }

        private static void PrintStats()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                        YOUR STATS                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            Console.WriteLine($"Here are your stats as of {_currentDate.Date: dddd, dd MMMM yyyy}:");
            Console.WriteLine($"Total coal found: {_totalCoalFound}kg");
            Console.WriteLine($"Total stone found: {_totalStoneFound}kg");
            Console.WriteLine($"Total iron found: {_totalIronFound}kg");
            Console.WriteLine($"Total gold found: {_totalGoldFound}kg");
            Console.WriteLine($"Total diamonds found: {_totalDiamondFound}kg");
            Console.WriteLine($"Total powerups used: {_totalPowerUpsUsed}");
            Console.WriteLine($"Total employees hired: {_totalEmployeesHired}");
            Console.WriteLine($"Total bribes paid: {_totalBribes}");
            Console.WriteLine($"\nTotal dollars earned: ${_totalDollarsEarned}");
            Console.WriteLine($"Total days dug: {_totalDaysDug}");
        }
        
        private static void PrintResources(Dictionary<string, double> resources)
        {
            Console.WriteLine("_____________________________________________________________________");
            Console.WriteLine($"                     You have ${resources["Dollars"]}\n");
            Console.WriteLine($"| You have {resources["coal"]}kg of coal         | You have {resources["stone"]}kg of stone");
            Console.WriteLine($"| You have {resources["iron"]}kg of iron         | You have {resources["gold"]}kg of gold");
            Console.WriteLine($"| You have {resources["diamond"]}kg of diamond      | You have {resources["magicTokens"]} magic token");
            Console.WriteLine($"| You have {resources["Workers"]} employees         | Your employees' efficiency is {_employeeEfficiency}");
            Console.WriteLine("_____________________________________________________________________");
        }
        
        private static Dictionary<string, double> CreateResourceDictionary()
        {
            Dictionary<string, double> resources = new Dictionary<string, double>()
            {
                { "coal", 0},
                { "stone", 0},
                { "iron", 0 },
                { "gold", 0 },
                { "diamond", 0},
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
                { "coal", 4},
                { "stone", 8},
                { "iron", 15},
                { "gold", 60},
                { "diamond", 200},
                { "Workers", 100},
                { "Wage", 13},
                { "SkipDay", 50},
                { "bribe", 200},
                { "trainingCourse", 400}
            };
            
            return prices;
        }

        private static Dictionary<string, double> CreateProbabilityDictionary()
        {
            Dictionary<string, double> probabilities = new Dictionary<string, double>()
            {
                { "coal", 90 },
                { "stone", 75 },
                { "iron", 65 },
                { "gold", 20 },
                { "diamond", 5 },
                { "MarketMaster", 5 },
                { "AncientArtefact", 8 },
                { "magicToken", 7 },
                { "TimeMachine", 6 },
                { "employeeIll", 10 },
                { "stockMarketCrash", 5 }
            };
            
            return probabilities;
        }

        private static Dictionary<string, double> CreatePowerUpDictionary()
        {
            Dictionary<string, double> powerUps = new Dictionary<string, double>()
            {
                { "Ancient Artefact", 0 },
                { "Time Machine", 0 },
                { "Market Master", 0 },
                { "magicToken", 0 }
            };
            return powerUps;
        }
        
        private static void RunTutorial()
        {
            Console.WriteLine("Welcome to the tutorial");
            Console.WriteLine("You are a gold digger, and you have to survive for as long as possible before bankruptcy");
            Console.WriteLine("You have a few resources to start with:");
            Console.WriteLine("You have $100, 0kg of coal, 0kg of iron, 0kg of gold, 0kg stone, 0kg diamond and 1 employee");
            Console.WriteLine("You can hire more employees, dig for resources, and sell resources at the market");
            Console.WriteLine("You can also bribe the government to not pay wages for the next three days");
            Console.WriteLine("You can also pay $50 for information on the next stock market crash");
            Console.WriteLine("You can also send all employees for a training course for $400 per employee (+30% efficiency) (7 days)");
            Console.WriteLine("You can also sell all your iron and gold for dollars");
            Console.WriteLine("You can also skip one day for $30");
            Console.WriteLine("You can also quit the game");
            Console.WriteLine("You can also dig for multiple days");
            Console.WriteLine("Here are the game mechanics:");
        }
        
        private static int UserMenuOption(Dictionary<string, double> resources, Dictionary<string, double> prices)
        {
            string takeUserInput = CheckIfInDebt(resources, prices);
            
            if (takeUserInput == "false")
            {
                Console.WriteLine($"Today is {_currentDate:dddd, d MMMM, yyyy}");
                Console.WriteLine("___________________________________\n");
                Console.WriteLine("Main Features:              Print Options:                   Other Features:\n");
                Console.WriteLine("0 - Quit game               6 - Print game mechanics          4 - Skip one day");
                Console.WriteLine("1 - Dig one day             7 - Print stats                   5 - Use a powerup");
                Console.WriteLine("2 - Dig multiple days       8 - Print achievements            9 - Send employees for training");
                Console.WriteLine("3 - Go to market                                              10 - Commit a crime (further options inside)");
                Console.WriteLine("Your choice:");
             
                int userOption = GetValidInt(0, 10);
                Console.Clear();          
                return userOption;
            }

            if(takeUserInput == "bankrupt")
            { 
                return -1;
            }
            
            // -2 if you were previously in debt
            return -2;
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
                    Console.WriteLine($"right now you have ${resources["Dollars"]}, {resources["coal"]}kg of coal, {resources["gold"]}kg of gold, {resources["iron"]}kg of iron and {resources["stone"]}kg of stone and {resources["diamond"]}kg of diamond");
                    Console.WriteLine("Unlucky bro...");
                    Console.WriteLine("After bossman stole your resources, you now have:");

                    resources["Dollars"] += resources["coal"] * prices["coal"];
                    resources["Dollars"] += resources["iron"] * prices["iron"];
                    resources["Dollars"] += resources["gold"] * prices["gold"]; 
                    resources["Dollars"] += resources["stone"] * prices["stone"];
                    resources["Dollars"] += resources["diamond"] * prices["diamond"];
                    _totalDollarsEarned += resources["coal"] * prices["coal"]+ resources["iron"] * prices["iron"] + resources["gold"] * prices["gold"] + resources["stone"] * prices["stone"] + resources["diamond"] * prices["diamond"];
                
                    resources["coal"] = 0;
                    resources["iron"] = 0;
                    resources["gold"] = 0;
                    resources["stone"] = 0;
                    resources["diamond"] = 0;
                
                    PrintResources(resources);
                }

                bool noResources = resources["coal"] == 0 && resources["stone"] == 0 && resources["iron"] == 0 && resources["gold"] == 0 && resources["diamond"] == 0;
                
                if (inDebt == "true" && noResources && resources["Workers"] < 2)
                {
                    Console.WriteLine("Bro you're literally bankrupt.You have failed the game.");
                    return "bankrupt";
                }
                
                if (inDebt == "true" && noResources && resources["Workers"] >= 2)
                {
                    Console.WriteLine("You don't have resources to sell, so we're selling workers for $100 per guy.");
                    resources["Dollars"] += resources["Workers"] * 100;
                    _totalDollarsEarned += resources["Workers"] * 100;
                    resources["Workers"] = 1;
                }
            }
            
            return inDebt;
        }
        
        private static void Dig(Dictionary<string, double> resources, Dictionary<string, double> prices, int daysToDig, Dictionary<string, double> probabilities, Dictionary<string, double> powerUpDictionary, List<string> achievementsList)
        {
            
            for (int days = 0; days < daysToDig; days++)
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
            
                    Console.WriteLine($"Digging done for the day {_currentDate.Date:dddd, dd MMMM, yyyy}");
                    Console.WriteLine("Here are the changes to your resources:");
            
                    // creating randoms for the chance of finding iron and gold and stone
                    Random random = new Random();
                    int randomForCoal = random.Next(0, 90);
                    int randomForStone = random.Next(0, 100);
                    int randomForIron = random.Next(0, 100);
                    int randomForGold = random.Next(0, 100);
                    int randomForDiamond = random.Next(0, 100);
                    int randomForAncientArtefact = _random.Next(0, 100);
                    int randomForMarketMaster = _random.Next(0, 100);
                    int randomForTimeMachine = _random.Next(0, 100);
                    int randomForMagicToken = _random.Next(0, 100);
                    
                    
                    // if there is a changed chance of finding gold due to the Ancient Artefact powerup
                    if (_increasedGoldChanceDays != 0)
                    {
                        Console.WriteLine($"You have the Ancient Artefact powerup, you have a 50% chance of finding gold for the next {_increasedGoldChanceDays} days");
                        probabilities["gold"] = 50;
                        _increasedGoldChanceDays -= 1;
                    }
                    
                    else
                    {
                        // restore 15% chance of finding gold
                        probabilities["gold"] = 15;
                    }
                    
                    bool coalFound = randomForCoal < probabilities["coal"];
                    bool stoneFound = randomForStone < probabilities["stone"];
                    bool ironFound = randomForIron < probabilities["iron"];
                    bool goldFound = randomForGold < probabilities["gold"];
                    bool diamondFound = randomForDiamond < probabilities["diamond"];
                    bool ancientArtefactFound = randomForAncientArtefact < probabilities["AncientArtefact"];
                    bool marketMasterFound = randomForMarketMaster < probabilities["MarketMaster"];
                    bool timeMachineFound = randomForTimeMachine < probabilities["TimeMachine"];
                    bool magicTokenFound = randomForMagicToken < probabilities["magicToken"];
                    
                    // update values within the resources dictionary
                    
                    double newResourcesFound = resources["Workers"] * _employeeEfficiency;

                    if (coalFound)
                    {
                        Console.WriteLine($"You found {newResourcesFound}kg of coal \ud83e\udea8");
                        resources["coal"] += newResourcesFound;
                        _totalCoalFound += newResourcesFound;
                    }
                    
                    if (stoneFound)
                    {
                        Console.WriteLine($"You found {newResourcesFound}kg of stone \ud83e\udea8");
                        resources["stone"] += newResourcesFound;
                        _totalStoneFound += newResourcesFound;
                    }
            
                    if (ironFound)
                    {
                        Console.WriteLine($"You found {newResourcesFound}kg of iron \ud83e\uddbe ");
                        resources["iron"] += newResourcesFound;
                        _totalIronFound += newResourcesFound;
                    }

                    if (goldFound)
                    {
                        Console.WriteLine($"You found {newResourcesFound}kg of gold \ud83d\udc51");
                        resources["gold"] += newResourcesFound;
                        _totalGoldFound += newResourcesFound;
                    }
                    
                    if (diamondFound)
                    {
                        Console.WriteLine($"You found {newResourcesFound}kg of diamond \ud83d\udc8e");
                        resources["diamond"] += newResourcesFound;
                        _totalDiamondFound += newResourcesFound;
                    }
                    
                    if (!coalFound && !stoneFound && !ironFound && !goldFound && !diamondFound && !ancientArtefactFound && !timeMachineFound && !magicTokenFound && !marketMasterFound)
                    {
                        Console.WriteLine("You found nothing today \ud83d\udeab");
                    }
                    
                    if (ancientArtefactFound)
                    {
                        Console.Write("\ud83c\udffa You found the Ancient Artefact power-up \ud83c\udffa");
                        Console.WriteLine("Choose an option:");
                        Console.WriteLine("1 - Use now");
                        Console.WriteLine("2 - Save for later");
                        int userInput = GetValidInt(1, 2);

                        switch (userInput)
                        {
                            case 1:
                                UsePowerUp(resources, prices, probabilities, 1, powerUpDictionary, achievementsList);
                                break;
                            case 2:
                                Console.WriteLine("You have chosen to save the Ancient Artefact for later");
                                powerUpDictionary["Ancient Artefact"] += 1;
                                break;
                        }
                    }
            
                    if (timeMachineFound)
                    {
                        Console.Write("\u23f3 You found the Time Machine power-up \u23f3");
                        Console.WriteLine("Choose an option:");
                        Console.WriteLine("1 - Use now");
                        Console.WriteLine("2 - Save for later");
                        int userInput = GetValidInt(1, 2);

                        switch (userInput)
                        {
                            case 1:
                                UsePowerUp(resources, prices, probabilities, 2, powerUpDictionary, achievementsList);
                                break;
                            case 2:
                                Console.WriteLine("You have chosen to save the Time Machine for later");
                                powerUpDictionary["Time Machine"] += 1;
                                break;
                        }
                    }
                    
                    if (magicTokenFound && resources["magicTokens"] < 4)
                    {
                        resources["magicTokens"] += 1;
                        Console.WriteLine($"You've acquired another magic token. You have {resources["magicTokens"]} magic tokens now");
                        Console.WriteLine($"Selling price increased by a total of {resources["magicTokens"] * 20}%");
                        prices["coal"] *= 1.2;
                        prices["iron"] *= 1.2;
                        prices["gold"] *= 1.2;
                        prices["stone"] *= 1.2;
                        prices["diamond"] *= 1.2;
                    }

                    if (marketMasterFound)
                    {
                        Console.WriteLine("You found the Market Master power up");
                        Console.WriteLine("Choose an option:");
                        Console.WriteLine("1 - Use now");
                        Console.WriteLine("2 - Save for later");
                        int userInput = GetValidInt(1, 2);

                        switch (userInput)
                        {
                            case 1:
                                UsePowerUp(resources, prices, probabilities, 3, powerUpDictionary, achievementsList);
                                break;
                            case 2:
                                Console.WriteLine("You have chosen to save the Market Master for later");
                                powerUpDictionary["Market Master"] += 1;
                                break;
                        }
                    }
                    
                    // calendar/weather etc effects 
                    Console.WriteLine("Here are the current active effects affecting your game:");
                    
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
                        Console.WriteLine($"{_badWeatherDaysLeft} days left of torrential rain");
                        _badWeatherDaysLeft -= 1;
                    }
                    
                    if (_hurricaneDaysLeft != 0)
                    {
                        Console.WriteLine($"{_hurricaneDaysLeft} days left of hurricane");
                        _hurricaneDaysLeft -= 1;
                    }
                    
                    if (_beautifulSkyDaysLeft != 0)
                    {
                        Console.WriteLine($"{_beautifulSkyDaysLeft} days left of beautiful sky");
                        _beautifulSkyDaysLeft -= 1;
                    }

                    if (_marketMasterDaysLeft == 1)
                    {
                        Console.WriteLine("Your Market Master powerup is no longer active");
                        _marketMasterDaysLeft = 0;
                    }
                    
                    else if (_marketMasterDaysLeft > 1)
                    {
                        Console.WriteLine($"{_marketMasterDaysLeft} days left of the Market Master powerup");
                        _marketMasterDaysLeft -= 1;
                    }
                    
                    _currentDate = _currentDate.AddDays(1);
                }
            
                ChangePrices(prices);
                _totalDaysDug += 1;

                if (daysToDig >= 2)
                {
                    Console.WriteLine($"Current balance - {resources["Dollars"]}");
                    Console.WriteLine($"There are {daysToDig - days - 1} days left to dig");
                }
                
                Console.WriteLine("___________________________________");
                
                // change the probabilities of finding resources - including calendar and weather effects
                ChangeProbabilities(prices, _currentDate, resources);
            
                // apply a ±10% fluctuation to the prices of iron and gold
                ChangePrices(prices);
                
                Console.WriteLine("___________________________________");
            }

            CheckAchievements(achievementsList);
            
            Console.WriteLine($"After {daysToDig} days of digging, here are your updated resources:");
            PrintResources(resources);
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
            Console.WriteLine($"| Coal: ${priceDictionary["coal"]} per kg");
            Console.WriteLine($"| Stone: ${priceDictionary["stone"]} per kg");
            Console.WriteLine($"| Iron: ${priceDictionary["iron"]} per kg");
            Console.WriteLine($"| Gold: ${priceDictionary["gold"]} per kg");
            Console.WriteLine($"| Diamond: ${priceDictionary["diamond"]} per kg");
            Console.WriteLine($"| Employees: ${priceDictionary["Workers"]} per employee");
            Console.WriteLine($"| Wages: ${priceDictionary["Wage"]} per employee per day");
            Console.WriteLine("______________________________");
            
            
            int marketOption;
            do
            {
                Console.WriteLine("\nHere is the menu for the market:");
                Console.WriteLine("1 - Sell a specific resource");
                Console.WriteLine("2 - Sell all resources for dollars");
                Console.WriteLine("3 - Hire More Employees");
                Console.WriteLine("4 - Exit market");
                Console.WriteLine("\nChoose Option:");
                marketOption = GetValidInt(1, 4);

                switch (marketOption)
                {
                    case 1:
                        Console.WriteLine("You've chosen to sell a specific resource.\nWhich resource do you want to sell?");
                        // sht here bro add in the emojis
                        Console.WriteLine("1 - Coal\n2 - Stone\n3 - Iron\n4 - Gold\n5 - Diamond");
                        int sellChoice = GetValidInt(1, 5);
                        switch (sellChoice)
                        {
                            case 1:
                        Console.WriteLine("Your have chosen to sell coal for dollars");
                        Console.WriteLine($"How much coal do you want to sell?\nYou have {resources["coal"]}kg of coal");
                        double coalToSell = GetValidDouble(0, 1000000);
                        if (coalToSell > resources["coal"])
                        {
                            Console.WriteLine("You don't have enough coal to sell that much");
                        }
                        else
                        {
                            resources["coal"] -= coalToSell;
                            resources["Dollars"] += coalToSell * priceDictionary["coal"];
                            _totalDollarsEarned += coalToSell * priceDictionary["coal"];
                        }

                        Console.WriteLine("Here are your updated resources:");
                        PrintResources(resources);
                        break;
                    case 2:
                        Console.WriteLine("Your have chosen to sell stone for dollars");
                        Console.WriteLine($"How much stone do you want to sell?\nYou have {resources["stone"]}kg of stone");
                        double stoneToSell = GetValidDouble(0, 1000000);
                        if (stoneToSell > resources["gold"])
                        {
                            Console.WriteLine("You don't have enough stone to sell that much");
                        }
                        else
                        {
                            resources["stone"] -= stoneToSell;
                            resources["Dollars"] += stoneToSell * priceDictionary["stone"];
                            _totalDollarsEarned += stoneToSell * priceDictionary["stone"];
                        }

                        Console.WriteLine("Here are your updated resources:");
                        PrintResources(resources);
                        break;
                    case 3:
                        Console.WriteLine("You have chosen to sell iron for dollars");
                        Console.WriteLine($"How much iron do you want to sell?\nYou have {resources["iron"]}kg of iron");
                        double ironToSell = GetValidDouble(0, 100000000);
                        
                        if (ironToSell > resources["iron"])
                        {
                            Console.WriteLine("You don't have enough iron to sell that much");
                        }
                        else
                        {
                            resources["iron"] -= ironToSell;
                            resources["Dollars"] += ironToSell * priceDictionary["iron"];
                            _totalDollarsEarned += ironToSell * priceDictionary["iron"];
                        }

                        Console.WriteLine("Here are your update resources:");
                        PrintResources(resources);
                        break;
                    case 4:
                        Console.WriteLine("Your have chosen to sell gold for dollars");
                        Console.WriteLine($"How much gold do you want to sell?\nYou have {resources["gold"]}kg of gold");
                        double goldToSell = GetValidDouble(0, 10000000);
                        if (goldToSell > resources["gold"])
                        {
                            Console.WriteLine("You don't have enough gold to sell that much");
                        }
                        else
                        {
                            resources["gold"] -= goldToSell;
                            resources["Dollars"] += goldToSell * priceDictionary["gold"];
                            _totalDollarsEarned += goldToSell * priceDictionary["gold"];
                        }

                        Console.WriteLine("Here are your updated resources:");
                        PrintResources(resources);

                        break;
                    case 5:
                                Console.WriteLine("Your have chosen to sell diamond for dollars");
                                Console.WriteLine($"How much diamond do you want to sell?\nYou have {resources["diamond"]}kg of diamond");
                                double diamondToSell = GetValidDouble(0, 1000000);
                                if (diamondToSell > resources["diamond"])
                                {
                                    Console.WriteLine("You don't have enough diamond to sell that much");
                                }
                                else
                                {
                                    resources["diamond"] -= diamondToSell;
                                    resources["Dollars"] += diamondToSell * priceDictionary["diamond"];
                                    _totalDollarsEarned += diamondToSell * priceDictionary["diamond"];
                                }

                                Console.WriteLine("Here are your updated resources:");
                                PrintResources(resources);
                        break; 
                        }

                    break;
                        
                    case 2:
                        Console.WriteLine("We're selling all your coal and iron and gold and stone and diamond for dollars");
                        resources["Dollars"] += resources["coal"] * priceDictionary["coal"];
                        resources["Dollars"] += resources["iron"] * priceDictionary["iron"];
                        resources["Dollars"] += resources["gold"] * priceDictionary["gold"];
                        resources["Dollars"] += resources["stone"] * priceDictionary["stone"];
                        resources["Dollars"] += resources["diamond"] * priceDictionary["diamond"];
                        _totalDollarsEarned += resources["coal"] * priceDictionary["coal"] + resources["iron"] * priceDictionary["iron"] + resources["gold"] * priceDictionary["gold"] + resources["stone"] * priceDictionary["stone"] + resources["diamond"] * priceDictionary["diamond"];
                        resources["coal"] = 0;
                        resources["iron"] = 0;
                        resources["gold"] = 0;
                        resources["stone"] = 0;
                        resources["diamond"] = 0;
                        
                        Console.WriteLine("Here are your updated resources:");
                        PrintResources(resources);
                        break;
                    
                    case 3:
                        Console.WriteLine("Enter how many employees you want to hire:");
                        Console.WriteLine($"Remember each employee charges {priceDictionary["Wage"]} in wages per day right now");
                        int employeesToHire = GetValidInt(0, 100000);
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
                    
                }
            } while (marketOption != 4);
        }

        private static void UsePowerUp(Dictionary<string, double> resources, Dictionary<string, double> prices, Dictionary<string, double> probabilities, int powerUpChoice, Dictionary<string, double> powerUpDictionary, List<string> achievementsList)
        {
            switch (powerUpChoice)
            {
                case 1:
                {
                    Console.WriteLine("You have chosen to use the Ancient Artefact powerup");
                    Console.WriteLine("Choose your option:");
                    Console.WriteLine("1 - 50% chance of finding gold for the next five days");
                    Console.WriteLine("2 - $250 instantly");
                    int ancientArtefactChoice = GetValidInt(1, 2);
                
                    if (ancientArtefactChoice == 1)
                    {
                        Console.WriteLine("You have chosen the 50% chance of finding gold for the next five days");
                        _increasedGoldChanceDays = 5;
                    }
                    else if (ancientArtefactChoice == 2)
                    {
                        Console.WriteLine("You have chosen the $250 instantly");
                        resources["Dollars"] += 250;
                        _totalDollarsEarned += 250;
                    }
                
                    powerUpDictionary["Ancient Artefact"] -= 1;
                    break;
                }
                
                case 2:
                {
                    Console.WriteLine("You have chosen to use the Time Machine powerup");
                    Console.WriteLine("This will give you 10 days' worth of rewards without costing you anything");
                    _noWageDaysLeft = 10;
                    _animation = false;
                    Dig(resources, prices, 10, probabilities, powerUpDictionary, achievementsList);
                    powerUpDictionary["Time Machine"] -= 1;
                    break;
                }
                
                case 3:
                    Console.WriteLine("Applying the Market Master power up...");
                    Console.WriteLine("The selling price of all resources has increased by 50% for the next 5 days");
                    _marketMasterDaysLeft = 5;
                    
                    prices["coal"] *= 1.5;
                    prices["stone"] *= 1.5;
                    prices["iron"] *= 1.5;
                    prices["gold"] *= 1.5;
                    prices["diamond"] *= 1.5;
                    
                    powerUpDictionary["MarketMaster"] -= 1;
                    
                    break;
            }
            
            _totalPowerUpsUsed += 1;
        }
        
        private static void QuitGame()
        {
            PrintStats();
            Console.WriteLine($"You lasted until {_currentDate.Date:dddd, d MMMM, yyyy}");
            Console.WriteLine("\nGoodbye!");
        }

        private static void GameFailed()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                         YOU FAILED                         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            QuitGame();
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
                prices["Wage"] /= 1.3;
            }
            
            // stock market code below
            // to undo the effects of the crash
            if (_crashDaysLeft > 1)
            {
                Console.WriteLine("The stock market has recovered");
                prices["coal"] *= 2;
                prices["stone"] *= 2;
                prices["iron"] *= 2;
                prices["gold"] *= 2;
                prices["diamond"] *= 2;
                prices["Workers"] *= 2;
                _crashDaysLeft = 0;
            }
            
            if (currentDate.Day == _crashDate && _crashDaysLeft == 0)
            {
                Console.WriteLine("The stock market has crashed, your iron and gold prices have plummeted but you can hire employees for cheaper");
                
                prices["coal"] /= 2;
                prices["stone"] /= 2;
                prices["iron"] /= 2;
                prices["gold"] /= 2;
                prices["diamond"] /= 2;
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

                else
                {
                    Console.WriteLine("Because you have so many employees, 60% of your current $$$ stash is given to them");
                    Console.WriteLine($"This means you'll lose {resources["Dollars"] * 0.6}");
                    resources["Dollars"] *= 0.4;
                }
                
            }
            
            
            // weather effects: rain or hurricane reducing efficiency, beautifulSky increasing efficiency
            
             // undoing weather effects 
            if (_badWeatherDaysLeft == 1)
            {
                Console.WriteLine("The weather has cleared up, your employees are back to normal efficiency");
                _employeeEfficiency /= 0.7;
            }
            
            if (_beautifulSkyDaysLeft == 1)
            {
                Console.WriteLine("The weather is mid, your employees are back to normal efficiency");
                _employeeEfficiency /= 1.5;
                _beautifulSkyDaysLeft = 0;
            }
            
            if (_hurricaneDaysLeft == 1)
            {
                Console.WriteLine("The hurricane has passed, your employees are back to normal efficiency");
                _employeeEfficiency /= 0.4;
            }
            
            bool noActiveWeatherEffects = _badWeatherDaysLeft == 0 && _hurricaneDaysLeft == 0 && _beautifulSkyDaysLeft == 0;
            
            // 5% chance a hurricane that reduces the probability of finding resources by 50% for the next 5 days
            if (_random.Next(0, 100) < 5 && noActiveWeatherEffects)
            {
                Console.WriteLine("A hurricane is coming, efficiency is now 40% the next five days");
                _employeeEfficiency *= 0.4;
                _hurricaneDaysLeft = 6;
            }
            
            // rain reducing efficiency
            else if (_random.Next(0, 100) < 30 && noActiveWeatherEffects)
            {
                Console.WriteLine("Due to torrential rain, your employees are 30% less efficient for the next two days");
                _employeeEfficiency *= 0.7;
                _badWeatherDaysLeft = 3;
            }
            
            // 30% chance beautiful sky increasing efficiency
            else if (_random.Next(0, 100) < 30 && noActiveWeatherEffects)
            {
                Console.WriteLine("The weather is beautiful today, your employees are 20% more efficient for two days");
                _employeeEfficiency *= 1.2;
                _beautifulSkyDaysLeft = 3; 
            }
            
        }

        private static void CheckAchievements(List<string> achievements)
        {
            
            if (_totalCoalFound >= 100 && !_achievement1)
            {
                Console.WriteLine("You've unlocked an achievement: 100kg of coal found milestone");
                _achievement1 = true;
                achievements.Add("100kg of coal found");
                
            }
            
            if (_totalCoalFound >= 1000 && !_achievement2)
            {
                Console.WriteLine("You've unlocked an achievement: 1000kg of coal found milestone");
                _achievement2 = true;
                achievements.Add("1000kg of coal found");
            }
            
            if (_totalCoalFound >= 10000 && !_achievement3)
            {
                Console.WriteLine("You've unlocked an achievement: 10000kg of coal found milestone");
                _achievement3 = true;
                achievements.Add("10000kg of coal found");
            }
            
            if (_totalStoneFound >= 100 && !_achievement4)
            {
                Console.WriteLine("You've unlocked an achievement: 100kg of stone found milestone");
                _achievement4 = true;
                achievements.Add("100kg of stone found");
            }
            
            if (_totalStoneFound >= 1000 && !_achievement5)
            {
                Console.WriteLine("You've unlocked an achievement: 1000kg of stone found milestone");
                _achievement5 = true;
                achievements.Add("1000kg of stone found");
            }
            
            if (_totalStoneFound >= 10000 && !_achievement6)
            {
                Console.WriteLine("You've unlocked an achievement: 10000kg of stone found milestone");
                _achievement6 = true;
                achievements.Add("10000kg of stone found");
            }
            
            if (_totalIronFound >= 75 && !_achievement7)
            {
                Console.WriteLine("You've unlocked an achievement: 75kg of iron found milestone");
                _achievement7 = true;
                achievements.Add("75kg of iron found");
            }
            
            if (_totalIronFound >= 750 && !_achievement8)
            {
                Console.WriteLine("You've unlocked an achievement: 750kg of iron found milestone");
                _achievement8 = true;
                achievements.Add("750kg of iron found");
            }
            
            if (_totalIronFound >= 7500 && !_achievement9)
            {
                Console.WriteLine("You've unlocked an achievement: 7500kg of iron found milestone");
                _achievement9 = true;
                achievements.Add("7500kg of iron found");
            }
            
            if (_totalGoldFound >= 30 && !_achievement10)
            {
                Console.WriteLine("You've unlocked an achievement: 30kg of gold found milestone");
                _achievement10 = true;
                achievements.Add("30kg of gold found");
            }
            
            if (_totalGoldFound >= 300 && !_achievement11)
            {
                Console.WriteLine("You've unlocked an achievement: 300kg of gold found milestone");
                _achievement11 = true;
                achievements.Add("300kg of gold found");
            }
            
            if (_totalGoldFound >= 3000 && !_achievement12)
            {
                Console.WriteLine("You've unlocked an achievement: 3000kg of gold found milestone");
                _achievement12 = true;
                achievements.Add("3000kg of gold found");
            }
            
            if (_totalDiamondFound >= 10 && !_achievement13)
            {
                Console.WriteLine("You've unlocked an achievement: 10kg of diamond found milestone");
                _achievement13 = true;
                achievements.Add("10kg of diamond found");
            }
            
            if (_totalDiamondFound >= 100 && !_achievement14)
            {
                Console.WriteLine("You've unlocked an achievement: 100kg of diamond found milestone");
                _achievement14 = true;
                achievements.Add("100kg of diamond found");
            }
            
            if (_totalDiamondFound >= 1000 && !_achievement15)
            {
                Console.WriteLine("You've unlocked an achievement: 1000kg of diamond found milestone");
                _achievement15 = true;
                achievements.Add("1000kg of diamond found");
            }
            
            if (_totalDollarsEarned >= 300 && !_achievement16)
            {
                Console.WriteLine("You've unlocked an achievement: $300 earned milestone");
                _achievement16 = true;
                achievements.Add("$300 earned");
            }
            
            if (_totalDollarsEarned >= 1000 && !_achievement17)
            {
                Console.WriteLine("You've unlocked an achievement: $1000 earned milestone");
                _achievement17 = true;
                achievements.Add("$1000 earned");
            }
            
            if (_totalDollarsEarned >= 10000 && !_achievement18)
            {
                Console.WriteLine("You've unlocked an achievement: $10000 earned milestone");
                _achievement18 = true;
                achievements.Add("$10000 earned");
            }
            
            if (_totalEmployeesHired >= 10 && !_achievement19)
            {
                Console.WriteLine("You've unlocked an achievement: 10 employees hired milestone");
                _achievement19 = true;
                achievements.Add("10 employees hired");
            }
            
            if (_totalEmployeesHired >= 100 && !_achievement20)
            {
                Console.WriteLine("You've unlocked an achievement: 100 employees hired milestone");
                _achievement20 = true;
                achievements.Add("100 employees hired");
            }
        }

        private static void ChangePrices(Dictionary<string, double> prices)
        {
            // upto a 30% fluctuation in prices based on random probability
            Random random = new Random();
            int randomChange = random.Next(-10, 10);
            
            double percentageChange = randomChange + 100;
            percentageChange /= 100;

            prices["coal"] *= percentageChange;
            prices["stone"] *= percentageChange;
            prices["iron"] *= percentageChange;
            prices["gold"] *= percentageChange;
            prices["diamond"] *= percentageChange;
        }

        private static void EmployeeTrainingCourse(Dictionary<string, double> resources, Dictionary<string, double> prices)
        {
            // to boost the productivity of employees
            Console.WriteLine($"This course charged you {prices["trainingCourse"] * resources["Workers"]} in fees");
            resources["Dollars"] -= prices["trainingCourse"] * resources["Workers"];
            _employeeEfficiency *= 1.3;
            _currentDate.AddDays(7);
            Console.WriteLine("Training employees...");
            Thread.Sleep(1500);
            Console.WriteLine("7 Days have now passed");
        }
        
        private static int GetValidInt(int min, int max)
        {
            if (int.TryParse(Console.ReadLine(), out int validInt))
            {
                if (validInt >= min && validInt <= max)
                {
                    return validInt; 
                }
                
                Console.WriteLine($"No bro enter a number between {min} and {max}");
                GetValidInt(min, max);
            }

            Console.WriteLine("Please enter a valid integer");
            return GetValidInt(min, max);
        }
        
        private static double GetValidDouble(double min, double max)
        {
            if (double.TryParse(Console.ReadLine(), out double validDouble))
            {
                if (validDouble >= min && validDouble <= max)
                {
                    return validDouble;
                }
                
                Console.WriteLine($"No bro enter a number between {min} and {max}");
                GetValidDouble(min, max);
            }

            Console.WriteLine($"Please enter a valid decimal number between {min} and {max}");
            return GetValidDouble(min, max);
        }
    }
}
