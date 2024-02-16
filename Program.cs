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
         * inconsistent between weather effect printing and actual
            * eg "6 days left of bad weather" but then it's only 5 days
        */
        
        /* to-do ideas
         * OOPIFYING IN PROGRESS
         * uhm the bankruptcy doesnt reduce price to 40%? does it? confirm
         * magic tokens are x1.2 every time, which is not mathematically correct
         *
         *
         * 
         * reorder the menu options to be more flowy and logical
         * earthquakes that loosen soil and make shit easier to find (+ cool animations possible)
         * tutorial mode (that is actually functional)
         * loans - you can take a loan from the bank and pay it back with interest
         * load/save game by saving the resource dictionary and the current date to a file
         * more power-ups
            * "Resource Rush": This powerup could increase the amount of all resources found for a certain number of days.
            * "Resource Radar" (for each resource): This powerup could increase the chance of finding a specific resource for a certain number of days. For example, if the powerup is activated for gold, then for the next few days, the chance of finding gold would be increased.
         * stock market feature (kinda done?)
             * Every 5 gold sold, increase gold price and for every 5 gold mined/gained, decrease price? Incentivising selling fast and not holding resources for ages
         * option to invest in the stock market
         * managers that do shit
            * eg a 'gold' manager that improves chance of finding gold but is hired for a week
            * or a 'diamond' manager to double chance of finding gold for 10 days
         * or you can 'restart' and sacrifice all your $$$ for a better location with better iron payments per day
            * (like prestige in all the idle miner games i played)
         
         features i can't do until i have an individual employee stat: (employee class/object shit)
         * printing per-employee stats
         * workers retire after x days
         * add a 'luck' stat for each employee that changes the probabilities of finding resources
             * when you hire an employee they're given a 'luck' rating between 20-80%
         * send individual number of employees for training course that then boosts their productivity
         */ 
        
        /*
           * hierarchy:
           * 
           * - Main()
           *     - CreateResourceDictionary()
           *     - CreatePricesDictionary()
           *     - CreateProbabilityDictionary()
           *     - PrintResources(Dictionary<string, double> resources)
           *     - RunGame(Dictionary<string, double> resourceDictionary, Dictionary<string, double> priceDictionary, Dictionary<string, double> probabilityDictionary)
           *            - UserMenuOption(Dictionary<string, double> resources, Dictionary<string, double> prices)
           *                    - CheckIfInDebt(Dictionary<string, double> resources) 
           *                    - CalendarEffects(Dictionary<string, double> prices, DateTime currentDate)
           *            - Dig(Dictionary<string, double> resources, Dictionary<string, double> prices, int daysToDig, Dictionary<string, double> probabilities)
           *                    - PrintResources(Dictionary<string, double> resources)
           *            - GoToMarket(Dictionary<string, double> resources, Dictionary<string, double> priceDictionary)
           *                    - PrintResources(Dictionary<string, double> resources)
           *            - PrintGameMechanics()
           *            - QuitGame(Dictionary<string, double> resources)
           *            - GameFailed(Dictionary<string, double> resources)
           *                    - QuitGame(Dictionary<string, double> resources)
           *     - GetValidInt()
           *     - GetValidDouble()
           * 
           * This hierarchy shows the flow of your program and how each subroutine is called from its parent subroutine.
           * /
        
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
            
            Coal coal = new Coal(90, 4, 0, 0);
            Stone stone = new Stone(75, 8, 0, 0);
            Iron iron = new Iron(65, 15, 0, 0);
            Gold gold = new Gold(20, 75, 0, 0);
            Diamond diamond = new Diamond(5, 200, 0, 0);
            
            List<string> achievementsList = new List<string>();
            
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 Welcome to Gold Diggerzz                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            Console.WriteLine("Aim of the game: survive for as long as possible before bankruptcy");
            Console.WriteLine("These are your initial resources...");
            
            PrintResources(resourceDictionary);
            
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
                        Dig(resourceDictionary, priceDictionary, 1, probabilityDictionary, powerUpDictionary, achievementsList, false);
                        break;
                    case 2:
                        _animation = false;
                        Console.WriteLine("Enter number of days to dig in one go (upto 30)");
                        int daysToDig = GetValidInt(1, 30);
                        Dig(resourceDictionary, priceDictionary, daysToDig, probabilityDictionary, powerUpDictionary, achievementsList, false);
                        break;
                    case 3:
                        GoToMarket(resourceDictionary, priceDictionary);
                        break;
                    case 4:
                        Console.WriteLine("Skipping one day");
                        Console.WriteLine($"You have been charged ${priceDictionary["SkipDay"]} for the costs of skipping a day");
                        resourceDictionary["Dollars"] -= priceDictionary["SkipDay"];
                       Dig(resourceDictionary, priceDictionary, 1, probabilityDictionary, powerUpDictionary, achievementsList, true);
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
                                if (powerUpDictionary["Ancient Artefact"] >= 0)
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
                                if (powerUpDictionary["Time Machine"] >= 0)
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
                                if (powerUpDictionary["Market Master"] >= 0)
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
                        Console.WriteLine($"1 - Pay ${priceDictionary["stockMarketDate"]} for information on the next stock market crash");
                        Console.WriteLine($"2 - Bribe the government for ${priceDictionary["bribe"]} to not pay wages for the next 3 days");
                        int crimeChoice = GetValidInt(1, 2);

                        switch (crimeChoice)
                        {
                            case 1:
                                Console.WriteLine(
                                    $"You have chosen to pay ${priceDictionary["stockMarketDate"]} for information on the next stock market crash");
                                resourceDictionary["Dollars"] -= priceDictionary["stockMarketDate"];
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

                    case 11:
                        RunTutorial(priceDictionary);
                        break;
                    
                    default:
                        Console.WriteLine("Please enter a valid option");
                        break;
                }
            } while (menuOption != 0 && menuOption != -1);
            
        }

        private static void PrintGameMechanics(Dictionary<string, double> prices, Dictionary<string, double> probabilities)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    GAME MECHANICS                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine($"Chance of finding coal = {Coal.Probability}%");
            Console.WriteLine($"Chance of finding stone = {Stone.Probability}%");
            Console.WriteLine($"Chance of finding iron = {Iron.Probability}%");
            Console.WriteLine($"Chance of finding gold = {Gold.Probability}%");
            Console.WriteLine($"Chance of finding diamond = {Diamond.Probability}%");
            Console.WriteLine($"Chance of finding Ancient Artefact = {probabilities["AncientArtefact"]}%");

            Console.WriteLine($"\nCost of hiring employee = ${prices["Workers"]}");
            Console.WriteLine($"Coal value = ${Coal.Price}");
            Console.WriteLine($"Stone value = ${Stone.Price}");
            Console.WriteLine($"Iron value = ${Iron.Price}");
            Console.WriteLine($"Gold value = ${Gold.Price}");
            Console.WriteLine($"Diamond value = ${Diamond.Price}");

            Console.WriteLine("Resource values fluctuate by upto ±10% per day");
            Console.WriteLine("You can find powerups that have different effects");
            Console.WriteLine("The resources you gain are equal to the number of employees you have times their efficiency");
            Console.WriteLine("Baseline wage = $10 per employee per day");
            Console.WriteLine("10% chance an employee is ill and doesn't come in to work");
            Console.WriteLine("30% pay increase on weekends only");
            Console.WriteLine("On the first of every month, employee wage increases by 10%");
            Console.WriteLine("On the 15th of each month, each employee gets 10% of your current $$$ stash (profit sharing)");
            Console.WriteLine("One x date every month, there is a stock market crash where iron, gold, and employee hiring prices halve");
            Console.WriteLine("every 10 days, the probabilities of finding resources is reduced by 5%");
            Console.WriteLine($"You can bribe the govt with ${prices["bribe"]} and not pay any wages for the next 3 days");
            Console.WriteLine("At any time if your $$$ balance goes negative, the govt sells all of your resources for 50% the current market rate");
            Console.WriteLine("If you have no resources to sell, they sell your employees for $100 each until you have 1 employee left");
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
            Console.WriteLine($"Total coal found: {Coal.TotalFound}kg");
            Console.WriteLine($"Total stone found: {Stone.TotalFound}kg");
            Console.WriteLine($"Total iron found: {Iron.TotalFound}kg");
            Console.WriteLine($"Total gold found: {Gold.TotalFound}kg");
            Console.WriteLine($"Total diamond found: {Diamond.TotalFound}kg");
            Console.WriteLine($"Total powerups used: {_totalPowerUpsUsed}");
            Console.WriteLine($"Total employees hired: {_totalEmployeesHired}");
            Console.WriteLine($"Total bribes paid: {_totalBribes}");
            Console.WriteLine($"\nTotal dollars earned: ${_totalDollarsEarned}");
            Console.WriteLine($"Total days dug: {_totalDaysDug}");
        }
        
        private static void PrintResources(Dictionary<string, double> resources)
        {
            Console.WriteLine("_____________________________________________________________________");
            Console.WriteLine($"                     You have ${Math.Round(resources["Dollars"], 2)}\n");
            Console.WriteLine($"| You have {Math.Round(Coal.Quantity, 2)}kg of coal         | You have {Math.Round(Stone.Quantity, 2)}kg of stone");
            Console.WriteLine($"| You have {Math.Round(Iron.Quantity, 2)}kg of iron         | You have {Math.Round(Gold.Quantity, 2)}kg of gold");
            Console.WriteLine($"| You have {Math.Round(Diamond.Quantity, 2)}kg of diamond      | You have {Math.Round(resources["magicTokens"], 2)} magic tokens");
            Console.WriteLine($"| You have {resources["Workers"]} employees         | Your employees' efficiency is {Math.Round(_employeeEfficiency, 2)}");
            Console.WriteLine("_____________________________________________________________________");
        }
        
        private static Dictionary<string, double> CreateResourceDictionary()
        {
            Dictionary<string, double> resources = new Dictionary<string, double>()
            {
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
                { "Workers", 100},
                { "Wage", 13},
                { "SkipDay", 50},
                { "bribe", 200},
                { "stockMarketDate", 50},
                { "trainingCourse", 400}
            };
            
            return prices;
        }

        private static Dictionary<string, double> CreateProbabilityDictionary()
        {
            Dictionary<string, double> probabilities = new Dictionary<string, double>()
            {
                { "MarketMaster", 4 },
                { "AncientArtefact", 7 },
                { "magicToken", 6 },
                { "TimeMachine", 3 },
                { "employeeIll", 10 },
                { "stockMarketCrash", 7 }
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
        
        private static void RunTutorial(Dictionary<string, double> prices)
        {
            Console.WriteLine("Welcome to the tutorial");
            Console.WriteLine("You are a gold digger, and you have to survive for as long as possible before bankruptcy");
            Console.WriteLine("You have a few resources to start with:");
            Console.WriteLine("You have $100, 0kg of coal, 0kg of iron, 0kg of gold, 0kg stone, 0kg diamond and 1 employee");
            Console.WriteLine("You can hire more employees, dig for resources, and sell resources at the market");
            Console.WriteLine("You can also bribe the government to not pay wages for the next three days");
            Console.WriteLine($"You can also pay ${prices["stockMarketDate"]} for information on the next stock market crash");
            Console.WriteLine("You can also send all employees for a training course for $400 per employee (+30% efficiency) (7 days)");
            Console.WriteLine("You can also sell all your iron and gold for dollars");
            Console.WriteLine($"You can also skip one day for ${prices["SkipDay"]}");
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
                Console.WriteLine("Main Features:              Print Options:                  Other Features:\n");
                Console.WriteLine("0 - Quit game               6 - Print game mechanics        4 - Skip one day");
                Console.WriteLine("1 - Dig one day             7 - Print stats                 5 - Use a powerup");
                Console.WriteLine("2 - Dig multiple days       8 - Print achievements          9 - Send employees for training");
                Console.WriteLine("3 - Go to market            11 - Print tutorial             10 - Commit a crime (further options inside)");
                Console.WriteLine("Your choice:");
             
                int userOption = GetValidInt(0, 11);
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
                bool noResources = Coal.Quantity == 0 && Stone.Quantity == 0 && Iron.Quantity == 0 && Gold.Quantity == 0 && Diamond.Quantity == 0;

                
                if (inDebt == "true")
                {
                    Console.WriteLine("\n\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31");
                    Console.WriteLine("You are in debt, bossman is coming for you");
                    Console.WriteLine("The government will come and sell all your resources for 2/5 the rate");
                    Console.WriteLine("They're also reducing your percentage chances of finding resources by 30% for the next three days");
                    Console.WriteLine($"right now you have ${resources["Dollars"]}, {Coal.Quantity}kg of coal, {Stone.Quantity}kg of stone, {Iron.Quantity}kg of iron, {Gold.Quantity}kg of gold, and {Diamond.Quantity}kg of diamond");
                    Console.WriteLine("Unlucky bro...");
                    Console.WriteLine("After bossman stole your resources, you now have:");

                    resources["Dollars"] += Coal.Quantity * Coal.Price;
                    resources["Dollars"] += Stone.Quantity * Stone.Price;
                    resources["Dollars"] += Iron.Quantity * Iron.Price;
                    resources["Dollars"] += Gold.Quantity * Gold.Price;
                    resources["Dollars"] += Diamond.Quantity * Diamond.Price;
                    _totalDollarsEarned += Coal.Quantity * Coal.Price+ Stone.Quantity * Stone.Price + Iron.Quantity * Iron.Price + Gold.Quantity * Gold.Price + Diamond.Quantity * Diamond.Price;
                
                    Coal.Quantity = 0;
                    Stone.Quantity = 0;
                    Iron.Quantity = 0;
                    Gold.Quantity = 0;
                    Diamond.Quantity = 0;
                
                    PrintResources(resources);
                }
                
                if (inDebt == "true" && noResources && resources["Workers"] < 2)
                {
                    Console.WriteLine("Bro you're literally bankrupt.You have failed the game.");
                    return "bankrupt";
                }
                
                if (inDebt == "true" && noResources && resources["Workers"] >= 2)
                {
                    Console.WriteLine("You don't have resources to sell, so we're selling workers for $50 per guy.");
                    resources["Dollars"] += resources["Workers"] * 50;
                    _totalDollarsEarned += resources["Workers"] * 50;
                    resources["Workers"] = 1;
                }
            }
            
            return inDebt;
        }
        
        private static void Dig(Dictionary<string, double> resources, Dictionary<string, double> prices, int daysToDig, Dictionary<string, double> probabilities, Dictionary<string, double> powerUpDictionary, List<string> achievementsList, bool skipDay)
        {
            
            for (int days = 0; days < daysToDig; days++)
            {
                
                if (CheckIfInDebt(resources, prices) !=  "true")
                {
                    if (!skipDay)
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
                        int randomForAncientArtefact = random.Next(0, 100);
                        int randomForMarketMaster = random.Next(0, 100);
                        int randomForTimeMachine = random.Next(0, 100);
                        int randomForMagicToken = random.Next(0, 100);


                        // if there is a changed chance of finding gold due to the Ancient Artefact powerup
                        if (_increasedGoldChanceDays != 0)
                        {
                            Console.WriteLine(
                                $"You have the Ancient Artefact powerup, you have a 50% chance of finding gold for the next {_increasedGoldChanceDays} days");
                            Gold.Probability = 50;
                            _increasedGoldChanceDays -= 1;
                        }

                        else
                        {
                            // restore 15% chance of finding gold
                            Gold.Probability = 15;
                        }

                        bool coalFound = randomForCoal < Coal.Probability;
                        bool stoneFound = randomForStone < Stone.Probability;
                        bool ironFound = randomForIron < Iron.Probability;
                        bool goldFound = randomForGold < Gold.Probability;
                        bool diamondFound = randomForDiamond < Diamond.Probability;
                        bool ancientArtefactFound = randomForAncientArtefact < probabilities["AncientArtefact"];
                        bool marketMasterFound = randomForMarketMaster < probabilities["MarketMaster"];
                        bool timeMachineFound = randomForTimeMachine < probabilities["TimeMachine"];
                        bool magicTokenFound = randomForMagicToken < probabilities["magicToken"];

                        // update values within the resources dictionary

                        double newResourcesFound = resources["Workers"] * _employeeEfficiency;

                        if (coalFound)
                        {
                            Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of coal \ud83e\udea8");
                            Coal.Quantity += newResourcesFound;
                            Coal.TotalFound += newResourcesFound;
                        }

                        if (stoneFound)
                        {
                            Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of stone \ud83e\udea8");
                            Stone.Quantity += newResourcesFound;
                            Stone.TotalFound += newResourcesFound;
                        }

                        if (ironFound)
                        {
                            Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of iron \ud83e\uddbe ");
                            Iron.Quantity += newResourcesFound;
                            Iron.TotalFound += newResourcesFound;
                        }

                        if (goldFound)
                        {
                            Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of gold \ud83d\udc51");
                            Gold.Quantity += newResourcesFound;
                            Gold.TotalFound += newResourcesFound;
                        }

                        if (diamondFound)
                        {
                            Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of diamond \ud83d\udc8e");
                            Diamond.Quantity += newResourcesFound;
                            Diamond.TotalFound += newResourcesFound;
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
                                    UsePowerUp(resources, prices, probabilities, 1, powerUpDictionary,
                                        achievementsList);
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
                                    UsePowerUp(resources, prices, probabilities, 2, powerUpDictionary,
                                        achievementsList);
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
                            Console.WriteLine(
                                $"You've acquired another magic token. You have {resources["magicTokens"]} magic tokens now");
                            Console.WriteLine(
                                $"Selling price increased by a total of {resources["magicTokens"] * 20}%");
                            Coal.Price *= 1.2;
                            Stone.Price *= 1.2;
                            Iron.Price *= 1.2;
                            Gold.Price *= 1.2;
                            Diamond.Price *= 1.2;
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
                                    UsePowerUp(resources, prices, probabilities, 3, powerUpDictionary,
                                        achievementsList);
                                    break;
                                case 2:
                                    Console.WriteLine("You have chosen to save the Market Master for later");
                                    powerUpDictionary["Market Master"] += 1;
                                    break;
                            }
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
            
                        Console.WriteLine($"Your {resources["Workers"]} employees charged a wage of ${Math.Round(totalWages, 2)} today.");
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
                    Console.WriteLine($"Current balance = {resources["Dollars"]}");
                    Console.WriteLine($"There are {daysToDig - days - 1} days left to dig");
                }
                
                Console.WriteLine("___________________________________");
                
                // change the probabilities of finding resources - including calendar and weather effects
                ChangeProbabilities(prices, _currentDate, resources, probabilities);
            
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
            Console.WriteLine($"| Coal: ${Math.Round(Coal.Price, 2)} per kg");
            Console.WriteLine($"| Stone: ${Math.Round(Stone.Price, 2)} per kg");
            Console.WriteLine($"| Iron: ${Math.Round(Iron.Price, 2)} per kg");
            Console.WriteLine($"| Gold: ${Math.Round(Gold.Price, 2)} per kg");
            Console.WriteLine($"| Diamond: ${Math.Round(Diamond.Price, 2)} per kg");
            Console.WriteLine($"| Employees: ${Math.Round(priceDictionary["Workers"], 2)} per employee");
            Console.WriteLine($"| Wages: ${Math.Round(priceDictionary["Wage"], 2)} per employee per day");
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
                            Console.WriteLine($"How much coal do you want to sell?\nYou have {Coal.Quantity}kg of coal");
                            double coalToSell = GetValidDouble(0, 100000000000);
                            if (coalToSell > Coal.Quantity)
                            {
                                Console.WriteLine("You don't have enough coal to sell that much");
                            }
                            else
                            {
                                Coal.Quantity -= coalToSell;
                                resources["Dollars"] += Coal.Quantity * Coal.Price;
                                _totalDollarsEarned += Coal.Quantity * Coal.Price;
                            }

                            Console.WriteLine("Here are your updated resources:"); 
                            PrintResources(resources);
                            break;
                        
                        case 2:
                            Console.WriteLine("Your have chosen to sell stone for dollars");
                            Console.WriteLine($"How much stone do you want to sell?\nYou have {Stone.Quantity}kg of stone");
                            double stoneToSell = GetValidDouble(0, 100000000000);
                            if (stoneToSell > Stone.Quantity)
                            {
                                Console.WriteLine("You don't have enough stone to sell that much");
                            }
                            else
                            {
                                Stone.Quantity -= stoneToSell;
                                resources["Dollars"] += Stone.Quantity * Stone.Price;
                                _totalDollarsEarned += Stone.Quantity * Stone.Price;
                            }

                            Console.WriteLine("Here are your updated resources:"); 
                            PrintResources(resources);
                            break;
                        case 3:
                            Console.WriteLine("Your have chosen to sell iron for dollars");
                            Console.WriteLine($"How much iron do you want to sell?\nYou have {Iron.Quantity}kg of iron");
                            double ironToSell = GetValidDouble(0, 100000000000);
                            if (ironToSell > Iron.Quantity)
                            {
                                Console.WriteLine("You don't have enough iron to sell that much");
                            }
                            else
                            {
                                Iron.Quantity -= ironToSell;
                                resources["Dollars"] += Iron.Quantity * Iron.Price;
                                _totalDollarsEarned += Iron.Quantity * Iron.Price;
                            }

                            Console.WriteLine("Here are your updated resources:"); 
                            PrintResources(resources);
                            break;
                        case 4:
                            Console.WriteLine("Your have chosen to sell gold for dollars");
                            Console.WriteLine($"How much gold do you want to sell?\nYou have {Gold.Quantity}kg of gold");
                            double goldToSell = GetValidDouble(0, 100000000000);
                            if (goldToSell > Gold.Quantity)
                            {
                                Console.WriteLine("You don't have enough gold to sell that much");
                            }
                            else
                            {
                                Gold.Quantity -= goldToSell;
                                resources["Dollars"] += Gold.Quantity * Gold.Price;
                                _totalDollarsEarned += Gold.Quantity * Gold.Price;
                            }

                            Console.WriteLine("Here are your updated resources:"); 
                            PrintResources(resources);
                            break;
                        case 5:
                            Console.WriteLine("Your have chosen to sell diamond for dollars");
                            Console.WriteLine($"How much diamond do you want to sell?\nYou have {Diamond.Quantity}kg of diamond");
                            double diamondToSell = GetValidDouble(0, 100000000000);
                            if (diamondToSell > Diamond.Quantity)
                            {
                                Console.WriteLine("You don't have enough diamond to sell that much");
                            }
                            else
                            {
                                Diamond.Quantity -= diamondToSell;
                                resources["Dollars"] += Diamond.Quantity * Diamond.Price;
                                _totalDollarsEarned += Diamond.Quantity * Diamond.Price;
                            }

                            Console.WriteLine("Here are your updated resources:"); 
                            PrintResources(resources);
                            break; 
                        }

                    break;
                        
                    case 2:
                        Console.WriteLine("We're selling all your coal and iron and gold and stone and diamond for dollars");
                        resources["Dollars"] += Coal.Quantity * Coal.Price;
                        resources["Dollars"] += Stone.Quantity * Stone.Price;
                        resources["Dollars"] += Iron.Quantity * Iron.Price;
                        resources["Dollars"] += Gold.Quantity * Gold.Price;
                        resources["Dollars"] += Diamond.Quantity * Diamond.Price;
                        _totalDollarsEarned += Coal.Quantity * Coal.Price + Stone.Quantity * Stone.Price + Iron.Quantity * Iron.Price + Gold.Quantity * Gold.Price + Diamond.Quantity * Diamond.Price;
                        Coal.Quantity = 0;
                        Stone.Quantity = 0;
                        Iron.Quantity = 0;
                        Gold.Quantity = 0;
                        Diamond.Quantity = 0;
                        
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
                            Console.WriteLine($"You have hired {employeesToHire} more employee");
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
                    Console.WriteLine("This will give you 5 days' worth of rewards without costing you anything");
                    _noWageDaysLeft = 10;
                    _animation = false;
                    Dig(resources, prices, 5, probabilities, powerUpDictionary, achievementsList, false);
                    powerUpDictionary["Time Machine"] -= 1;
                    break;
                }
                
                case 3:
                    Console.WriteLine("Applying the Market Master power up...");
                    Console.WriteLine("The selling price of all resources has increased by 50% for the next 5 days");
                    _marketMasterDaysLeft = 5;
                    
                    Coal.Price *= 1.5;
                    Stone.Price *= 1.5;
                    Iron.Price *= 1.5;
                    Gold.Price *= 1.5;
                    Diamond.Price *= 1.5;
                    
                    powerUpDictionary["Market Master"] -= 1;
                    
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
        
        private static void ChangeProbabilities(Dictionary<string, double> prices, DateTime currentDate, Dictionary<string, double> resources, Dictionary<string, double> probabilities)
        {
            
            // calendar effects: weekend pay, stock market crash, wage increase, employee illness, profit sharing, reduced probability of finding resources
            
            // every 10 days, probability of finding resources is reduced by 5%
            if (currentDate.Day % 10 == 0)
            {
                Console.WriteLine("Congratulations for surviving for another 10 days. The game is now getting even harder...");
                Console.WriteLine("\ud83d\udc22 The probability of finding resources has reduced by 5% \ud83d\udc22");
                Coal.Probability *= 0.95;
                Stone.Probability *= 0.95;
                Iron.Probability *= 0.95;
                Gold.Probability *= 0.95;
                Diamond.Probability *= 0.95;
            }
            
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
                Coal.Price *= 2;
                Stone.Price *= 2;
                Iron.Price *= 2;
                Gold.Price *= 2;
                Diamond.Price *= 2;
                prices["Workers"] *= 2;
                _crashDaysLeft = 0;
            }
            
            if (currentDate.Day == _crashDate && _crashDaysLeft == 0)
            {
                Console.WriteLine("The stock market has crashed, your iron and gold prices have plummeted but you can hire employees for cheaper");
                
                Coal.Price /= 2;
                Stone.Price /= 2;
                Iron.Price /= 2;
                Gold.Price /= 2;
                Diamond.Price /= 2;
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
            if (_random.Next(0, 100) < 10 && resources["Workers"] > 1)
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
                _employeeEfficiency *= 1.3;
            }
            
            if (_beautifulSkyDaysLeft == 1)
            {
                Console.WriteLine("The weather is mid, your employees are back to normal efficiency");
                _employeeEfficiency /= 1.2;
                _beautifulSkyDaysLeft = 0;
            }
            
            if (_hurricaneDaysLeft == 1)
            {
                Console.WriteLine("The hurricane has passed, your employees are back to normal efficiency");
                _employeeEfficiency *= 1.4;
            }
            
            bool noActiveWeatherEffects = _badWeatherDaysLeft == 0 && _hurricaneDaysLeft == 0 && _beautifulSkyDaysLeft == 0;
            
            // 5% chance a hurricane that reduces the probability of finding resources by 50% for the next 5 days
            if (_random.Next(0, 100) < 5 && noActiveWeatherEffects)
            {
                Console.WriteLine("A hurricane is coming, efficiency is now 40% less the next five days");
                _employeeEfficiency /= 1.4;
                _hurricaneDaysLeft = 6;
            }
            
            // rain reducing efficiency
            else if (_random.Next(0, 100) < 30 && noActiveWeatherEffects)
            {
                Console.WriteLine("Due to torrential rain, your employees are 30% less efficient for the next two days");
                _employeeEfficiency /= 1.3;
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
            
            if (Coal.TotalFound >= 100 && !_achievement1)
            {
                Console.WriteLine("You've unlocked an achievement: 100kg of coal found milestone");
                _achievement1 = true;
                achievements.Add("100kg of coal found");
                
            }
            
            if (Coal.TotalFound >= 1000 && !_achievement2)
            {
                Console.WriteLine("You've unlocked an achievement: 1000kg of coal found milestone");
                _achievement2 = true;
                achievements.Add("1000kg of coal found");
            }
            
            if (Coal.TotalFound >= 10000 && !_achievement3)
            {
                Console.WriteLine("You've unlocked an achievement: 10000kg of coal found milestone");
                _achievement3 = true;
                achievements.Add("10000kg of coal found");
            }
            
            if (Stone.TotalFound >= 100 && !_achievement4)
            {
                Console.WriteLine("You've unlocked an achievement: 100kg of stone found milestone");
                _achievement4 = true;
                achievements.Add("100kg of stone found");
            }
            
            if (Stone.TotalFound >= 1000 && !_achievement5)
            {
                Console.WriteLine("You've unlocked an achievement: 1000kg of stone found milestone");
                _achievement5 = true;
                achievements.Add("1000kg of stone found");
            }
            
            if (Stone.TotalFound >= 10000 && !_achievement6)
            {
                Console.WriteLine("You've unlocked an achievement: 10000kg of stone found milestone");
                _achievement6 = true;
                achievements.Add("10000kg of stone found");
            }
            
            if (Iron.TotalFound >= 75 && !_achievement7)
            {
                Console.WriteLine("You've unlocked an achievement: 75kg of iron found milestone");
                _achievement7 = true;
                achievements.Add("75kg of iron found");
            }
            
            if (Iron.TotalFound >= 750 && !_achievement8)
            {
                Console.WriteLine("You've unlocked an achievement: 750kg of iron found milestone");
                _achievement8 = true;
                achievements.Add("750kg of iron found");
            }
            
            if (Iron.TotalFound >= 7500 && !_achievement9)
            {
                Console.WriteLine("You've unlocked an achievement: 7500kg of iron found milestone");
                _achievement9 = true;
                achievements.Add("7500kg of iron found");
            }
            
            if (Gold.TotalFound >= 30 && !_achievement10)
            {
                Console.WriteLine("You've unlocked an achievement: 30kg of gold found milestone");
                _achievement10 = true;
                achievements.Add("30kg of gold found");
            }
            
            if (Gold.TotalFound >= 300 && !_achievement11)
            {
                Console.WriteLine("You've unlocked an achievement: 300kg of gold found milestone");
                _achievement11 = true;
                achievements.Add("300kg of gold found");
            }
            
            if (Gold.TotalFound >= 3000 && !_achievement12)
            {
                Console.WriteLine("You've unlocked an achievement: 3000kg of gold found milestone");
                _achievement12 = true;
                achievements.Add("3000kg of gold found");
            }
            
            if (Diamond.TotalFound >= 10 && !_achievement13)
            {
                Console.WriteLine("You've unlocked an achievement: 10kg of diamond found milestone");
                _achievement13 = true;
                achievements.Add("10kg of diamond found");
            }
            
            if (Diamond.TotalFound >= 100 && !_achievement14)
            {
                Console.WriteLine("You've unlocked an achievement: 100kg of diamond found milestone");
                _achievement14 = true;
                achievements.Add("100kg of diamond found");
            }
            
            if (Diamond.TotalFound >= 1000 && !_achievement15)
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
            // upto a 20% fluctuation in prices based on random probability
            Random random = new Random();
            int randomChange = random.Next(-10, 10);
            
            double percentageChange = (randomChange + 100) / 100;
            
            Coal.Price *= percentageChange;
            Stone.Price *= percentageChange;
            Iron.Price *= percentageChange;
            Gold.Price *= percentageChange;
            Diamond.Price *= percentageChange;
        }

        private static void EmployeeTrainingCourse(Dictionary<string, double> resources, Dictionary<string, double> prices)
        {
            // to boost the productivity of employees
            Console.WriteLine("Training employees...");
            Console.WriteLine($"This course charged you {prices["trainingCourse"] * resources["Workers"]} in fees");
            resources["Dollars"] -= prices["trainingCourse"] * resources["Workers"];
            _employeeEfficiency *= 1.3;
            _currentDate.AddDays(7);
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

    
    // in the pregame i create a new instance of each resource and pass in the probability, price, quantity and totalFound
    // this is then just updated throughout the game
    // essentially removing coal from every dictionary that its currently in
    // and instead of operation on the dictionary, i just call the method for each resource object when needed
    class Coal
    {
        public static double Probability;
        public static double Price;
        public static double Quantity;
        public static double TotalFound;

        public Coal(double initialProbability, double initialPrice, double initialQuantity, double totalFound)
        {
            Probability = initialProbability;
            Price = initialPrice;
            Quantity = initialQuantity;
            TotalFound = totalFound;
        }

        public void ChangeProbability(string increaseOrDecrease, double reductionMultiplier)
        {
            if (increaseOrDecrease == "increase")
            {
                Probability *= reductionMultiplier;
            }
            else if (increaseOrDecrease == "decrease")
            {
               Probability /= reductionMultiplier;
            }
            else
            {
                Console.WriteLine("Invalid passing in, either type increase or decrease bro");
            }
            
        }

        public void ChangePrice(string increaseOrDecrease, double increaseMultiplier)
        {
            if (increaseOrDecrease == "increase")
            {
                Price *= increaseMultiplier;
            }
            else if (increaseOrDecrease == "decrease")
            {
                Price /= increaseMultiplier;
            }
            else
            {
                Console.WriteLine("Invalid passing in, either type increase or decrease bro");
            }
        }
        
        public void ChangeQuantity(string increaseOrDecrease, double changeAmount)
        {
            if (increaseOrDecrease == "increase")
            {
                Quantity += changeAmount;
            }
            else if (increaseOrDecrease == "decrease")
            {
                Quantity /= changeAmount;
            }
            else
            {
                Console.WriteLine("Invalid passing in, either type increase or decrease bro");
            }
        }
    }
    
    class Stone
    {
        public static double Probability;
        public static double Price;
        public static double Quantity;
        public static double TotalFound;

        public Stone(double initialProbability, double initialPrice, double initialQuantity, double totalFound)
        {
            Probability = initialProbability;
            Price = initialPrice;
            Quantity = initialQuantity;
            TotalFound = totalFound;
        }

        public void ChangeProbability(string increaseOrDecrease, double reductionMultiplier)
        {
            if (increaseOrDecrease == "increase")
            {
                Probability *= reductionMultiplier;
            }
            else if (increaseOrDecrease == "decrease")
            {
                Probability /= reductionMultiplier;
            }
            else
            {
                Console.WriteLine("Invalid passing in, either type increase or decrease bro");
            }
            
        }

        public void ChangePrice(string increaseOrDecrease, double increaseMultiplier)
        {
            if (increaseOrDecrease == "increase")
            {
                Price *= increaseMultiplier;
            }
            else if (increaseOrDecrease == "decrease")
            {
                Price /= increaseMultiplier;
            }
            else
            {
                Console.WriteLine("Invalid passing in, either type increase or decrease bro");
            }
        }
        
        public void ChangeQuantity(string increaseOrDecrease, double changeAmount)
        {
            if (increaseOrDecrease == "increase")
            {
                Quantity += changeAmount;
            }
            else if (increaseOrDecrease == "decrease")
            {
                Quantity /= changeAmount;
            }
            else
            {
                Console.WriteLine("Invalid passing in, either type increase or decrease bro");
            }
        }
    }
    
    class Iron
    {
        public static double Probability;
        public static double Price;
        public static double Quantity;
        public static double TotalFound;

        public Iron(double initialProbability, double initialPrice, double initialQuantity, double totalFound)
        {
            Probability = initialProbability;
            Price = initialPrice;
            Quantity = initialQuantity;
            TotalFound = totalFound;
        }

        public void ChangeProbability(string increaseOrDecrease, double reductionMultiplier)
        {
            if (increaseOrDecrease == "increase")
            {
                Probability *= reductionMultiplier;
            }
            else if (increaseOrDecrease == "decrease")
            {
                Probability /= reductionMultiplier;
            }
            else
            {
                Console.WriteLine("Invalid passing in, either type increase or decrease bro");
            }
            
        }

        public void ChangePrice(string increaseOrDecrease, double increaseMultiplier)
        {
            if (increaseOrDecrease == "increase")
            {
                Price *= increaseMultiplier;
            }
            else if (increaseOrDecrease == "decrease")
            {
                Price /= increaseMultiplier;
            }
            else
            {
                Console.WriteLine("Invalid passing in, either type increase or decrease bro");
            }
        }
        
        public void ChangeQuantity(string increaseOrDecrease, double changeAmount)
        {
            if (increaseOrDecrease == "increase")
            {
                Quantity += changeAmount;
            }
            else if (increaseOrDecrease == "decrease")
            {
                Quantity /= changeAmount;
            }
            else
            {
                Console.WriteLine("Invalid passing in, either type increase or decrease bro");
            }
        }
    }
    
    class Gold
    {
        public static double Probability;
        public static double Price;
        public static double Quantity;
        public static double TotalFound;

        public Gold(double initialProbability, double initialPrice, double initialQuantity, double totalFound)
        {
            Probability = initialProbability;
            Price = initialPrice;
            Quantity = initialQuantity;
            TotalFound = totalFound;
        }

        public void ChangeProbability(string increaseOrDecrease, double reductionMultiplier)
        {
            if (increaseOrDecrease == "increase")
            {
                Probability *= reductionMultiplier;
            }
            else if (increaseOrDecrease == "decrease")
            {
                Probability /= reductionMultiplier;
            }
            else
            {
                Console.WriteLine("Invalid passing in, either type increase or decrease bro");
            }
            
        }

        public void ChangePrice(string increaseOrDecrease, double increaseMultiplier)
        {
            if (increaseOrDecrease == "increase")
            {
                Price *= increaseMultiplier;
            }
            else if (increaseOrDecrease == "decrease")
            {
                Price /= increaseMultiplier;
            }
            else
            {
                Console.WriteLine("Invalid passing in, either type increase or decrease bro");
            }
        }
        
        public void ChangeQuantity(string increaseOrDecrease, double changeAmount)
        {
            if (increaseOrDecrease == "increase")
            {
                Quantity += changeAmount;
            }
            else if (increaseOrDecrease == "decrease")
            {
                Quantity /= changeAmount;
            }
            else
            {
                Console.WriteLine("Invalid passing in, either type increase or decrease bro");
            }
        }
    }
    
    class Diamond
    {
        public static double Probability;
        public static double Price;
        public static double Quantity;
        public static double TotalFound;

        public Diamond(double initialProbability, double initialPrice, double initialQuantity, double totalFound)
        {
            Probability = initialProbability;
            Price = initialPrice;
            Quantity = initialQuantity;
            TotalFound = totalFound;
        }

        public void ChangeProbability(string increaseOrDecrease, double reductionMultiplier)
        {
            if (increaseOrDecrease == "increase")
            {
                Probability *= reductionMultiplier;
            }
            else if (increaseOrDecrease == "decrease")
            {
                Probability /= reductionMultiplier;
            }
            else
            {
                Console.WriteLine("Invalid passing in, either type increase or decrease bro");
            }
            
        }

        public void ChangePrice(string increaseOrDecrease, double increaseMultiplier)
        {
            if (increaseOrDecrease == "increase")
            {
                Price *= increaseMultiplier;
            }
            else if (increaseOrDecrease == "decrease")
            {
                Price /= increaseMultiplier;
            }
            else
            {
                Console.WriteLine("Invalid passing in, either type increase or decrease bro");
            }
        }
        
        public void ChangeQuantity(string increaseOrDecrease, double changeAmount)
        {
            if (increaseOrDecrease == "increase")
            {
                Quantity += changeAmount;
            }
            else if (increaseOrDecrease == "decrease")
            {
                Quantity /= changeAmount;
            }
            else
            {
                Console.WriteLine("Invalid passing in, either type increase or decrease bro");
            }
        }
    }
}

