﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Gold_Diggerzz
{
    // initial inspiration: https://replit.com/@AadityaKalwani/Digging-Simulator#main.py
    
    /* program structure + hierarchy

       - Resource
             - Coal
             - Stone
             - Iron
             - Gold
             - Diamond
       - Dollars
       - Workers
       - PowerUp
             - MagicTokens
             - TimeMachine
             - AncientArtefact
             - MarketMaster
       - Program
             - Main()
                 - CreatePricesDictionary()
                 - CreateProbabilityDictionary()
                 - RunGame(Dictionary<string, double> priceDictionary, Dictionary<string, double> probabilityDictionary, List<string> achievementsList)
                     - UserMenuOption()
                         - CheckIfInDebt()
                     - Dig(int daysToDig, List<string> achievementsList, bool skipDay)
                         - PrintResources()
                     - GoToMarket()
                         - PrintResources()
                     - PrintGameMechanics()
                     - QuitGame()
                     - GameFailed()
                     - ChangeProbabilities(DateTime currentDate)
                     - CheckAchievements(List<string> achievements)
                     - ChangePrices()
                     - EmployeeTrainingCourse()
                 - GetValidInt(int min, int max)
                 - GetValidDouble(double min, double max)
      * /

   /*
    * current issues
    * inconsistent between weather effect printing and actual
       * eg "6 days left of bad weather" but then it's only 5 days
    * uhm the bankruptcy doesnt reduce price to 40%? does it? confirm
    * magic tokens are x1.2 every time, which is not mathematically correct
    */
    
    /* current issues:
     * somehow going to have to create each employee separately and then have a list of them + option to print them out
     * inconsistency with the weather effect printing and actual
        * eg "6 days left of bad weather" but then it's only 5 days
     */

    /* to-do ideas
     * OOP Workers
     * OOP Powerups
     * OOP 
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
    
    // in the pregame i create a new instance of each resource and pass in the probability, price, quantity and totalFound
    // this is then just updated throughout the game
    // essentially removing coal, gold, etc from every dictionary that its currently in
    // and instead of operation on the dictionary, i just call the method for each resource object when needed
    // and then just deleting the dictionary and replacing it with the object everywhere
    
    class Resource
    {
        public double Probability;
        public double Price;
        public double Quantity;
        public double TotalFound;

        public Resource(double initialProbability, double initialPrice, double initialQuantity, double totalFound)
        {
            Probability = initialProbability;
            Price = initialPrice;
            Quantity = initialQuantity;
            TotalFound = totalFound;
        }
    }
    
    
    class Worker
    {
        public string Name;
        public double Quantity;
        public double Wage;
        public double Price;
        public double EmployeeIllProbability;
        
        public Worker(string name, double initialQuantity, double initialWage, double initialPrice, double employeeIllProbability)
        {
            Name = name;
            Quantity = initialQuantity; // = 1
            Wage = initialWage; // = 13
            Price = initialPrice; // = 100
            EmployeeIllProbability = employeeIllProbability; // = 10
        }
    }

    class PowerUp
    {
        public double Quantity;
        public double Probability;
        
        public PowerUp(double initialQuantity, double initialProbability)

        {
            Quantity = initialQuantity; // = 0
            Probability = initialProbability; // = 2-10 depending on the powerup
        }
    }
    
    internal abstract class Program

    {
        
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
        private static Random _random = new Random();
        private static int _crashDate = _random.Next(0, 28);
        
        // Declare your variables at the class level
        static Resource coal;
        static Resource stone;
        static Resource iron;
        static Resource gold;
        static Resource diamond;
        static Resource dollars;
        static Worker workers;
        static PowerUp magicTokens;
        static PowerUp timeMachine;
        static PowerUp ancientArtefact;
        static PowerUp marketMaster;
        
        // to stop screaming at me for names it doesnt recognise/think are typos
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        
        private static void Main()
        {
            // pregame
            Dictionary<string, double> priceDictionary = CreatePricesDictionary();
            Dictionary<string, double> probabilityDictionary = CreateProbabilityDictionary();
            
            // possible names for the workers

            List<string> possibleNames = new List<string>()
            {
                "Sir Reginald Thunderpants III", "Bartholomew McFancyPants", "Benedict Cucumberbatch",
                "Chadwick von Snugglesworth", "Montgomery Fiddlesticks", "Floyd Wibblebottom",
                "Buford McFluffybutt", "Percival von Wigglesworth", "Gertrude Puddingpop",
                "Beatrice von Schnauzer", "Rutherford McTinkleton", "Winston Fuzzyface",
                "Penelope Fluffernutter", "Sir Fluffington Fluffykins", "Barbara Biscuitbottom",
                "Clarence Piddlesworth", "Rufus von Purrington", "Chester McNibblebutt",
                "Prudence McSquiggles", "Cuthbert Fuzzyboots", "Millicent von Cuddlebug",
                "Dudley von Floofenstein", "Hilda Muffinfluff", "Percy von Snugglepuss",
                "Barnaby McSnoozytoes", "Mortimer Fluffernoodle", "Eugene von Fuzzywump",
                "Agnes Piddlepants", "Milton McWhiskers", "Edna von Waggletail",
                "Humphrey Fuzzymuffin", "Geraldine Purrwhiskers", "Reginald McFluffernugget",
                "Gertrude von Bumblefluff", "Clarence Purrington", "Tabitha Snickerdoodle",
                "Percival von Cuddlebug", "Mildred Puddlejumper", "Archibald von Cuddlebutt",
                "Esmeralda McSquishyface", "Reginald von Fluffykins", "Beatrice Bumbleflop",
                "Algernon McFuzzyface", "Wanda von Snugglebug", "Horace McSnoozleton",
                "Geraldine von Wigglesworth", "Felix Purrpants", "Matilda von Cuddlewhiskers",
                "Reginald Fluffybottom", "Gwendolyn McFuzzytoes", "Bartholomew von Snuggleton",
                "Clarence McPurrington", "Millicent von Waggletail", "Archibald Fiddlesticks",
                "Esmeralda McWhiskerpants", "Rupert von Fuzzyface", "Mildred Purrwhiskers",
                "Percival von Squigglepaws", "Reginald Fluffypants", "Eugene von Puddlejumper",
                "Gertrude McSnugglepuss", "Humphrey von Purrington", "Prudence Fluffernoodle",
                "Barnaby von Snugglewhiskers", "Hilda McSquigglebottom", "Benedict von Cuddlebug",
                "Penelope McFluffernugget", "Rufus von Cuddlewhiskers", "Matilda McSquishyface",
                "Winston von Fluffytoes", "Esmeralda McFuzzywump", "Montgomery von Squigglepaws",
                "Tabitha McSnickerdoodle", "Buford von Fluffernutter", "Clarence McWaggletail",
                "Beatrice von Fuzzywhiskers", "Rupert McFluffypants", "Gertrude von Cuddlebug",
                "Chester McSquiggles", "Wanda von Fuzzytoes", "Percy McWhiskerpants",
                "Gwendolyn von Purrwhiskers", "Milton McSnoozytoes", "Matilda von Bumbleflop",
                "Archibald McFluffyface", "Prudence von Puddlejumper", "Barbara McFluffernugget",
                "Algernon von Snugglebug", "Esmeralda McSquishytoes", "Rufus von Cuddlewhiskers",
                "Millicent McFuzzywump", "Winston von Snuggleton", "Geraldine McFluffernutter",
                "Reginald von Snugglepuss", "Rupert McFuzzybottom", "Clarence von Cuddlewhiskers",
                "Tabitha McFuzzyface", "Benedict von Purrwhiskers", "Humphrey McSquigglebottom",
                "Percival von McSnoozleton", "Gwendolyn von Fluffypaws", "Chester von Cuddlebug",
                "Prudence McSnugglewhiskers", "Millicent von Fluffernutter", "Esmeralda McPurrington",
                "Rufus von Squigglewhiskers", "Wanda McFuzzytoes", "Algernon von Snuggleton",
                "Matilda McFluffypants", "Rupert von Snugglebug", "Tabitha McPurrwhiskers",
                "Benedict von Fluffypaws", "Gwendolyn McSquiggletoes", "Reginald von Fuzzywhiskers"
            };

            
            coal = new Resource(90, 4, 0, 0);
            stone = new Resource(75, 8, 0, 0);
            iron = new Resource(65, 15, 0, 0);
            gold = new Resource(20, 75, 0, 0);
            diamond = new Resource(5, 200, 0, 0);
            dollars = new Resource(0, 0, 100, 0);
            workers = new Worker(possibleNames[_random.Next(0, possibleNames.Count)], 1, 10, 100, 10);
            magicTokens = new PowerUp(0, 6);
            timeMachine = new PowerUp(0, 3);
            ancientArtefact = new PowerUp(0, 7);
            marketMaster = new PowerUp(0, 4);
            
            List<string> achievementsList = new List<string>();


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 Welcome to Gold Diggerzz                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine("Aim of the game: survive for as long as possible before bankruptcy");
            Console.WriteLine("These are your initial resources...");

            PrintResources();

            // game starts
            Console.WriteLine("The game is about to start, good luck...");
            RunGame(priceDictionary, probabilityDictionary, achievementsList);
        }

        private static void RunGame(Dictionary<string, double> priceDictionary, Dictionary<string, double> probabilityDictionary, List<string> achievementsList)
        {
            int menuOption;
            do
            {
                menuOption = UserMenuOption();

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
                        Dig(priceDictionary, 1, probabilityDictionary, 
                            achievementsList, false);
                        break;
                    case 2:
                        _animation = false;
                        Console.WriteLine("Enter number of days to dig in one go (upto 30)");
                        int daysToDig = GetValidInt(1, 30);
                        Dig(priceDictionary, daysToDig, probabilityDictionary,
                            achievementsList, false);
                        break;
                    case 3:
                        GoToMarket();
                        break;
                    case 4:
                        Console.WriteLine("Skipping one day");
                        Console.WriteLine(
                            $"You have been charged ${priceDictionary["SkipDay"]} for the costs of skipping a day");
                        dollars.Quantity -= priceDictionary["SkipDay"];
                        Dig(priceDictionary, 1, probabilityDictionary,
                            achievementsList, true);
                        PrintResources();
                        break;
                    case 5:
                        if (ancientArtefact.Quantity ! > 0 && timeMachine.Quantity ! > 0)
                        {
                            Console.WriteLine("\u274c You don't have any powerups to use \u274c");
                            break;
                        }

                        Console.WriteLine("What powerup do you want to use?");
                        Console.WriteLine($"You have {ancientArtefact.Quantity} Ancient Artefacts, {timeMachine.Quantity} Time Machines and {marketMaster.Quantity} Market Masters\n");
                        Console.WriteLine("0 - Cancel & Return");
                        Console.WriteLine("1 - Ancient Artefact");
                        Console.WriteLine("2 - Time Machine");
                        Console.WriteLine("3 - Market Master");
                        int powerUpChoice = GetValidInt(0, 3);

                        switch (powerUpChoice)
                        {
                            case 1:
                                if (ancientArtefact.Quantity >= 0)
                                {
                                    UsePowerUp(priceDictionary, probabilityDictionary, powerUpChoice, achievementsList);
                                }
                                else
                                {
                                    Console.WriteLine("You don't have any Ancient Artefacts to use");
                                }

                                break;

                            case 2:
                            {
                                if (timeMachine.Quantity >= 0)
                                {
                                    UsePowerUp(priceDictionary, probabilityDictionary,
                                        powerUpChoice, achievementsList);

                                }
                                else
                                {
                                    Console.WriteLine("You don't have any Time Machines to use");
                                }
                            }
                                break;

                            case 3:
                                if (marketMaster.Quantity >= 0)
                                {
                                    UsePowerUp(priceDictionary, probabilityDictionary,
                                        powerUpChoice, achievementsList);
                                }
                                else
                                {
                                    Console.WriteLine("You don't have any Market Masters to use");
                                }

                                break;
                        }

                        break;
                    case 6:
                        PrintGameMechanics(priceDictionary);
                        break;
                    case 7:
                        PrintStats();
                        break;
                    case 8:
                        for (int achievementNumber = 0; achievementNumber < achievementsList.Count; achievementNumber++)
                        {
                            Console.WriteLine(
                                $"Achievement {achievementNumber}: {achievementsList[achievementNumber]}");
                        }

                        break;
                    case 9:
                        if (dollars.Quantity >
                            priceDictionary["trainingCourse"] * workers.Quantity &&
                            workers.Quantity != 0)
                        {
                            Console.WriteLine("You have chosen to send all employees on a training course");
                            Console.WriteLine(
                                $"You have been charged {priceDictionary["trainingCourse"]} per employee");
                            Console.WriteLine("Your employees will be back in 7 days");
                            EmployeeTrainingCourse(priceDictionary);
                        }
                        else if (dollars.Quantity >
                                 priceDictionary["trainingCourse"] * workers.Quantity &&
                                 workers.Quantity == 0)
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
                        Console.WriteLine(
                            $"1 - Pay ${priceDictionary["stockMarketDate"]} for information on the next stock market crash");
                        Console.WriteLine(
                            $"2 - Bribe the government for ${priceDictionary["bribe"]} to not pay wages for the next 3 days");
                        int crimeChoice = GetValidInt(1, 2);

                        switch (crimeChoice)
                        {
                            case 1:
                                Console.WriteLine(
                                    $"You have chosen to pay ${priceDictionary["stockMarketDate"]} for information on the next stock market crash");
                                dollars.Quantity -= priceDictionary["stockMarketDate"];
                                Console.WriteLine("Giving you the information now...");
                                Console.WriteLine($"Expect a stock market crash on the {_crashDate}th of every month");
                                break;
                            case 2:
                                Console.WriteLine("You have chosen to bribe the government");
                                Console.WriteLine($"You have been charged {priceDictionary["bribe"]} for the bribe");
                                dollars.Quantity -= priceDictionary["bribe"];
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

        private static void PrintGameMechanics(Dictionary<string, double> prices)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    GAME MECHANICS                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine($"Chance of finding coal = {Math.Round(coal.Probability, 2)}%");
            Console.WriteLine($"Chance of finding stone = {Math.Round(coal.Probability, 2)}%");
            Console.WriteLine($"Chance of finding iron = {Math.Round(iron.Probability, 2)}%");
            Console.WriteLine($"Chance of finding gold = {Math.Round(gold.Probability, 2)}%");
            Console.WriteLine($"Chance of finding diamond = {Math.Round(diamond.Probability, 2)}%");
            Console.WriteLine($"Chance of finding Ancient Artefact = {Math.Round(ancientArtefact.Probability, 2)}%");

            Console.WriteLine($"\nCost of hiring employee = ${workers.Wage}");
            Console.WriteLine($"Coal value = ${Math.Round(coal.Price, 2)}");
            Console.WriteLine($"Stone value = ${Math.Round(stone.Price, 2)}");
            Console.WriteLine($"Iron value = ${Math.Round(iron.Price, 2)}");
            Console.WriteLine($"Gold value = ${Math.Round(gold.Price, 2)}");
            Console.WriteLine($"Diamond value = ${Math.Round(diamond.Price, 2)}");

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
            Console.WriteLine($"Total coal found: {Math.Round(coal.TotalFound, 2)}kg");
            Console.WriteLine($"Total stone found: {Math.Round(stone.TotalFound, 2)}kg");
            Console.WriteLine($"Total iron found: {Math.Round(iron.TotalFound, 2)}kg");
            Console.WriteLine($"Total gold found: {Math.Round(gold.TotalFound, 2)}kg");
            Console.WriteLine($"Total diamond found: {Math.Round(diamond.TotalFound, 2)}kg");
            Console.WriteLine($"Total powerups used: {_totalPowerUpsUsed}");
            Console.WriteLine($"Total employees hired: {_totalEmployeesHired}");
            Console.WriteLine($"Total bribes paid: {_totalBribes}");
            Console.WriteLine($"\nTotal dollars earned: ${Math.Round(_totalDollarsEarned, 2)}");
            Console.WriteLine($"Total days dug: {_totalDaysDug}");
        }

        private static void PrintResources()
        {
            Console.WriteLine("_____________________________________________________________________");
            Console.WriteLine($"                     You have ${Math.Round(dollars.Quantity, 2)}\n");
            Console.WriteLine($"| You have {Math.Round(coal.Quantity, 2)}kg of coal         | You have {Math.Round(stone.Quantity, 2)}kg of stone");
            Console.WriteLine($"| You have {Math.Round(iron.Quantity, 2)}kg of iron         | You have {Math.Round(gold.Quantity, 2)}kg of gold");
            Console.WriteLine($"| You have {Math.Round(diamond.Quantity, 2)}kg of diamond      | You have {Math.Round(magicTokens.Quantity, 2)} magic tokens");
            Console.WriteLine($"| You have {workers.Quantity} employees         | Your employees' efficiency is {Math.Round(_employeeEfficiency, 2)}");
            Console.WriteLine("_____________________________________________________________________");
        }

        private static Dictionary<string, double> CreatePricesDictionary()
        {
            Dictionary<string, double> prices = new Dictionary<string, double>()
            {
                { "SkipDay", 50 },
                { "bribe", 200 },
                { "stockMarketDate", 50 },
                { "trainingCourse", 400 }
            };

            return prices;
        }

        private static Dictionary<string, double> CreateProbabilityDictionary()
        {
            Dictionary<string, double> probabilities = new Dictionary<string, double>()
            {
                { "stockMarketCrash", 7 }
            };

            return probabilities;
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

        private static int UserMenuOption()
        {
            string takeUserInput = CheckIfInDebt();

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

            if (takeUserInput == "bankrupt")
            {
                return -1;
            }

            // -2 if you were previously in debt
            return -2;
        }

        private static string CheckIfInDebt()
        {
            string inDebt = "false";
            if (dollars.Quantity < 0)
            {
                inDebt = "true";
                bool noResources = coal.Quantity == 0 && stone.Quantity == 0 && iron.Quantity == 0 &&
                                   gold.Quantity == 0 && diamond.Quantity == 0;


                if (inDebt == "true")
                {
                    Console.WriteLine("\n\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31");
                    Console.WriteLine("You are in debt, bossman is coming for you");
                    Console.WriteLine("The government will come and sell all your resources for 2/5 the rate");
                    Console.WriteLine($"right now you have ${dollars.Quantity}, {coal.Quantity}kg of coal, {stone.Quantity}kg of stone, {iron.Quantity}kg of iron, {gold.Quantity}kg of gold, and {diamond.Quantity}kg of diamond");
                    Console.WriteLine("Unlucky bro...");
                    Console.WriteLine("After bossman stole your resources, you now have:");
                    
                    dollars.Quantity += coal.Quantity * coal.Price + stone.Quantity * stone.Price +
                                           iron.Quantity * iron.Price + gold.Quantity * gold.Price +
                                           diamond.Quantity * diamond.Price;
                    
                    _totalDollarsEarned += coal.Quantity * coal.Price + stone.Quantity * stone.Price +
                                           iron.Quantity * iron.Price + gold.Quantity * gold.Price +
                                           diamond.Quantity * diamond.Price;

                    coal.Quantity = 0;
                    stone.Quantity = 0;
                    iron.Quantity = 0;
                    gold.Quantity = 0;
                    diamond.Quantity = 0;

                    PrintResources();
                }

                if (inDebt == "true" && noResources && workers.Quantity < 2)
                {
                    Console.WriteLine("Bro you're literally bankrupt.You have failed the game.");
                    return "bankrupt";
                }

                if (inDebt == "true" && noResources && workers.Quantity >= 2)
                {
                    Console.WriteLine("You don't have resources to sell, so we're selling workers for $50 per guy.");
                    dollars.Quantity += workers.Quantity * 50;
                    _totalDollarsEarned += workers.Quantity * 50;
                    workers.Quantity = 1;
                }
            }

            return inDebt;
        }

        private static void Dig(Dictionary<string, double> prices, int daysToDig,
            Dictionary<string, double> probabilities,
            List<string> achievementsList, bool skipDay)
        {

            for (int days = 0; days < daysToDig; days++)
            {

                if (CheckIfInDebt() != "true")
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
                            gold.Probability = 50;
                            _increasedGoldChanceDays -= 1;
                        }

                        else
                        {
                            // restore 15% chance of finding gold
                            gold.Probability = 15;
                        }

                        bool coalFound = randomForCoal < coal.Probability;
                        bool stoneFound = randomForStone < stone.Probability;
                        bool ironFound = randomForIron < iron.Probability;
                        bool goldFound = randomForGold < gold.Probability;
                        bool diamondFound = randomForDiamond < diamond.Probability;
                        bool ancientArtefactFound = randomForAncientArtefact < ancientArtefact.Probability;
                        bool marketMasterFound = randomForMarketMaster < marketMaster.Probability;
                        bool timeMachineFound = randomForTimeMachine < timeMachine.Probability;
                        bool magicTokenFound = randomForMagicToken < magicTokens.Probability;

                        // update values within the resources dictionary

                        double newResourcesFound = workers.Quantity * _employeeEfficiency;

                        if (coalFound)
                        {
                            Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of coal \ud83e\udea8");
                            coal.Quantity += newResourcesFound;
                            coal.TotalFound += newResourcesFound;
                        }

                        if (stoneFound)
                        {
                            Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of stone \ud83e\udea8");
                            stone.Quantity += newResourcesFound;
                            stone.TotalFound += newResourcesFound;
                        }

                        if (ironFound)
                        {
                            Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of iron \ud83e\uddbe ");
                            iron.Quantity += newResourcesFound;
                            iron.TotalFound += newResourcesFound;
                        }

                        if (goldFound)
                        {
                            Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of gold \ud83d\udc51");
                            gold.Quantity += newResourcesFound;
                            gold.TotalFound += newResourcesFound;
                        }

                        if (diamondFound)
                        {
                            Console.WriteLine(
                                $"You found {Math.Round(newResourcesFound, 2)}kg of diamond \ud83d\udc8e");
                            diamond.Quantity += newResourcesFound;
                            diamond.TotalFound += newResourcesFound;
                        }

                        if (!coalFound && !stoneFound && !ironFound && !goldFound && !diamondFound &&
                            !ancientArtefactFound && !timeMachineFound && !magicTokenFound && !marketMasterFound)
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
                                    UsePowerUp(prices, probabilities, 1, achievementsList);
                                    break;
                                case 2:
                                    Console.WriteLine("You have chosen to save the Ancient Artefact for later");
                                    ancientArtefact.Quantity += 1;
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
                                    UsePowerUp(prices, probabilities, 2, 
                                        achievementsList);
                                    break;
                                case 2:
                                    Console.WriteLine("You have chosen to save the Time Machine for later");
                                    timeMachine.Quantity += 1;
                                    break;
                            }
                        }

                        if (magicTokenFound && magicTokens.Quantity < 4)
                        {
                            magicTokens.Quantity += 1;
                            Console.WriteLine($"You've acquired another magic token. You have {magicTokens.Quantity} magic tokens now");
                            Console.WriteLine($"Selling price increased by a total of {magicTokens.Quantity * 20}%");
                            coal.Price *= 1.2;
                            stone.Price *= 1.2;
                            iron.Price *= 1.2;
                            gold.Price *= 1.2;
                            diamond.Price *= 1.2;
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
                                    UsePowerUp(prices, probabilities, 3,
                                        achievementsList);
                                    break;
                                case 2:
                                    Console.WriteLine("You have chosen to save the Market Master for later");
                                    marketMaster.Quantity += 1;
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
                        double totalWages = workers.Quantity * workers.Wage;
                        dollars.Quantity -= totalWages;

                        Console.WriteLine($"Your {workers.Quantity} employees charged a wage of ${Math.Round(totalWages, 2)} today.");
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

                ChangePrices();
                _totalDaysDug += 1;

                if (daysToDig >= 2)
                {
                    Console.WriteLine($"Current balance = {dollars.Quantity}");
                    Console.WriteLine($"There are {daysToDig - days - 1} days left to dig");
                }

                Console.WriteLine("___________________________________");

                // change the probabilities of finding resources - including calendar and weather effects
                ChangeProbabilities(_currentDate);

                // apply a ±10% fluctuation to the prices of iron and gold
                ChangePrices();

                Console.WriteLine("___________________________________");
            }

            CheckAchievements(achievementsList);

            Console.WriteLine($"After {daysToDig} days of digging, here are your updated resources:");
            PrintResources();
        }

        private static void GoToMarket()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    WELCOME TO THE MARKET                   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine($"Here are the rates for {_currentDate:dddd dd MMMM, yyyy}:");

            Console.WriteLine("______________________________");
            Console.WriteLine($"| Coal: ${Math.Round(coal.Price, 2)} per kg");
            Console.WriteLine($"| Stone: ${Math.Round(stone.Price, 2)} per kg");
            Console.WriteLine($"| Iron: ${Math.Round(iron.Price, 2)} per kg");
            Console.WriteLine($"| Gold: ${Math.Round(gold.Price, 2)} per kg");
            Console.WriteLine($"| Diamond: ${Math.Round(diamond.Price, 2)} per kg");
            Console.WriteLine($"| Employees: ${Math.Round(workers.Price, 2)} per employee");
            Console.WriteLine($"| Wages: ${Math.Round(workers.Wage, 2)} per employee per day");
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
                                Console.WriteLine($"How much coal do you want to sell?\nYou have {coal.Quantity}kg of coal");
                                double coalToSell = GetValidDouble(0, 100000000000);
                                if (coalToSell > coal.Quantity)
                                {
                                    Console.WriteLine("You don't have enough coal to sell that much");
                                }
                                else
                                {
                                    coal.Quantity -= coalToSell;
                                    dollars.Quantity += coal.Quantity * coal.Price;
                                    _totalDollarsEarned += coal.Quantity * coal.Price;
                                }

                                Console.WriteLine("Here are your updated resources:");
                                PrintResources();
                                break;

                            case 2:
                                Console.WriteLine("Your have chosen to sell stone for dollars");
                                Console.WriteLine($"How much stone do you want to sell?\nYou have {stone.Quantity}kg of stone");
                                double stoneToSell = GetValidDouble(0, 100000000000);
                                if (stoneToSell > stone.Quantity) 
                                {
                                    Console.WriteLine("You don't have enough stone to sell that much");
                                }
                                else
                                {
                                    stone.Quantity -= stoneToSell;
                                    dollars.Quantity += stone.Quantity * stone.Price;
                                    _totalDollarsEarned += stone.Quantity * stone.Price;
                                }

                                Console.WriteLine("Here are your updated resources:");
                                PrintResources();
                                break;
                            case 3:
                                Console.WriteLine("Your have chosen to sell iron for dollars");
                                Console.WriteLine($"How much iron do you want to sell?\nYou have {iron.Quantity}kg of iron");
                                double ironToSell = GetValidDouble(0, 100000000000);
                                if (ironToSell > iron.Quantity)
                                {
                                    Console.WriteLine("You don't have enough iron to sell that much");
                                }
                                else
                                {
                                    iron.Quantity -= ironToSell;
                                    dollars.Quantity += iron.Quantity * iron.Price;
                                    _totalDollarsEarned += iron.Quantity * iron.Price;
                                }

                                Console.WriteLine("Here are your updated resources:");
                                PrintResources();
                                break;
                            case 4:
                                Console.WriteLine("Your have chosen to sell gold for dollars");
                                Console.WriteLine($"How much gold do you want to sell?\nYou have {gold.Quantity}kg of gold");
                                double goldToSell = GetValidDouble(0, 100000000000);
                                if (goldToSell > gold.Quantity)
                                {
                                    Console.WriteLine("You don't have enough gold to sell that much");
                                }
                                else
                                {
                                    gold.Quantity -= goldToSell;
                                    dollars.Quantity += gold.Quantity * gold.Price;
                                    _totalDollarsEarned += gold.Quantity * gold.Price;
                                }

                                Console.WriteLine("Here are your updated resources:");
                                PrintResources();
                                break;
                            case 5:
                                Console.WriteLine("Your have chosen to sell diamond for dollars");
                                Console.WriteLine($"How much diamond do you want to sell?\nYou have {diamond.Quantity}kg of diamond");
                                double diamondToSell = GetValidDouble(0, 100000000000);
                                if (diamondToSell > diamond.Quantity)
                                {
                                    Console.WriteLine("You don't have enough diamond to sell that much");
                                }
                                else
                                {
                                    diamond.Quantity -= diamondToSell;
                                    dollars.Quantity += diamond.Quantity * diamond.Price;
                                    _totalDollarsEarned += diamond.Quantity * diamond.Price;
                                }

                                Console.WriteLine("Here are your updated resources:");
                                PrintResources();
                                break;
                        }

                        break;

                    case 2:
                        Console.WriteLine("We're selling all your coal and iron and gold and stone and diamond for dollars");
                        dollars.Quantity += coal.Quantity * coal.Price;
                        dollars.Quantity += stone.Quantity * stone.Price;
                        dollars.Quantity += iron.Quantity * iron.Price;
                        dollars.Quantity += gold.Quantity * gold.Price;
                        dollars.Quantity += diamond.Quantity * diamond.Price;
                        _totalDollarsEarned += coal.Quantity * coal.Price + stone.Quantity * stone.Price +
                                               iron.Quantity * iron.Price + gold.Quantity * gold.Price +
                                               diamond.Quantity * diamond.Price;
                        coal.Quantity = 0;
                        stone.Quantity = 0;
                        iron.Quantity = 0;
                        gold.Quantity = 0;
                        diamond.Quantity = 0;

                        Console.WriteLine("Here are your updated resources:");
                        PrintResources();
                        break;

                    case 3:
                        Console.WriteLine("Enter how many employees you want to hire:");
                        Console.WriteLine($"Remember each employee charges {workers.Wage} in wages per day right now");
                        int employeesToHire = GetValidInt(0, 100000);
                        if (employeesToHire * workers.Price > dollars.Quantity)
                        {
                            Console.WriteLine("You don't have enough dollars to hire that many employees");
                        }
                        else
                        {
                            Console.WriteLine($"You have hired {employeesToHire} more employee");
                            workers.Quantity += employeesToHire;
                            dollars.Quantity -= employeesToHire * workers.Price;
                            Console.WriteLine($"You now have {workers.Quantity} employees");
                            _totalEmployeesHired += employeesToHire;
                        }
                        break;

                    case 4:
                        Console.WriteLine("Thanks for coming to the market!");
                        break;

                }
            } while (marketOption != 4);
        }

        private static void UsePowerUp(Dictionary<string, double> prices, Dictionary<string, double> probabilities, int powerUpChoice, List<string> achievementsList)
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
                        dollars.Quantity += 250;
                        _totalDollarsEarned += 250;
                    }

                    ancientArtefact.Quantity -= 1;
                    break;
                }

                case 2:
                {
                    Console.WriteLine("You have chosen to use the Time Machine powerup");
                    Console.WriteLine("This will give you 5 days' worth of rewards without costing you anything");
                    _noWageDaysLeft = 10;
                    _animation = false;
                    Dig(prices, 5, probabilities, achievementsList, false);
                    timeMachine.Quantity -= 1;
                    break;
                }

                case 3:
                    Console.WriteLine("Applying the Market Master power up...");
                    Console.WriteLine("The selling price of all resources has increased by 50% for the next 5 days");
                    _marketMasterDaysLeft = 5;

                    coal.Price *= 1.5;
                    stone.Price *= 1.5;
                    iron.Price *= 1.5;
                    gold.Price *= 1.5;
                    diamond.Price *= 1.5;

                    marketMaster.Quantity -= 1;
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

        private static void ChangeProbabilities(DateTime currentDate)
        {

            // calendar effects: weekend pay, stock market crash, wage increase, employee illness, profit sharing, reduced probability of finding resources

            // every 10 days, probability of finding resources is reduced by 5%
            if (currentDate.Day % 10 == 0)
            {
                Console.WriteLine("Congratulations for surviving for another 10 days. The game is now getting even harder...");
                Console.WriteLine("\ud83d\udc22 The probability of finding resources has reduced by 5% \ud83d\udc22");
                coal.Probability *= 0.95;
                stone.Probability *= 0.95;
                iron.Probability *= 0.95;
                gold.Probability *= 0.95;
                diamond.Probability *= 0.95;
            }

            // +30% pay on weekends - wage is increased on saturday, then reduced again on monday
            if (currentDate.DayOfWeek is DayOfWeek.Saturday)
            {
                Console.WriteLine("It's the weekend, your employees want 30% more pay");
                workers.Wage *= 1.3;
            }

            // to undo the effect of weekend pay
            else if (currentDate.DayOfWeek is DayOfWeek.Monday)
            {
                workers.Wage /= 1.3;
            }

            // stock market code below
            // to undo the effects of the crash
            if (_crashDaysLeft > 1)
            {
                Console.WriteLine("The stock market has recovered");
                coal.Price *= 2;
                stone.Price *= 2;
                iron.Price *= 2;
                gold.Price *= 2;
                diamond.Price *= 2;
                workers.Wage *= 2;
                _crashDaysLeft = 0;
            }

            if (currentDate.Day == _crashDate && _crashDaysLeft == 0)
            {
                Console.WriteLine("The stock market has crashed, your iron and gold prices have plummeted but you can hire employees for cheaper");

                coal.Price /= 2;
                stone.Price /= 2;
                iron.Price /= 2;
                gold.Price /= 2;
                diamond.Price /= 2;
                workers.Wage /= 2;
                _crashDaysLeft = 2;
            }

            // 10% raise on the first of every month (apart from January)
            if (currentDate.Month != 1 && currentDate.Day == 1)
            {
                Console.WriteLine("It's the first of the month, your employees want a 10% raise");
                workers.Wage *= 1.1;
            }

            // to undo the effects of unwell workers
            if (_lessWorkerDays == 1)
            {
                workers.Quantity += 1;
                Console.WriteLine("Your employee is back at work today");
                _lessWorkerDays = 0;
            }

            // 10% chance an employee is unwell and doesn't come in
            if (_random.Next(0, 100) < workers.EmployeeIllProbability && workers.Quantity > 1)
            {
                Console.WriteLine("One of your employees is unwell and doesn't come in today");
                workers.Quantity -= 1;
                _lessWorkerDays = 1;
            }

            // 10% profit sharing to each employee on the 15th of every month
            if (currentDate.Day == 15)
            {
                Console.WriteLine("Profit sharing time!");

                if (workers.Quantity < 8)
                {
                    Console.WriteLine("Each employee gets 10% of your current $$$ stash");
                    Console.WriteLine($"Your {workers.Quantity} employees get ${dollars.Quantity * 0.1} each");
                    double dollarsToLose = dollars.Quantity * 0.1 * workers.Quantity;
                    dollars.Quantity -= dollarsToLose;
                    Console.WriteLine($"Your employees have been paid, you have lost $ {dollarsToLose} in the process");
                }

                else
                {
                    Console.WriteLine("Because you have so many employees, 60% of your current $$$ stash is given to them");
                    Console.WriteLine($"This means you'll lose {dollars.Quantity * 0.6}");
                    dollars.Quantity *= 0.4;
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

            bool noActiveWeatherEffects =
                _badWeatherDaysLeft == 0 && _hurricaneDaysLeft == 0 && _beautifulSkyDaysLeft == 0;

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

            if (coal.TotalFound >= 100 && !_achievement1)
            {
                Console.WriteLine("You've unlocked an achievement: 100kg of coal found milestone");
                _achievement1 = true;
                achievements.Add("100kg of coal found");

            }

            if (coal.TotalFound >= 1000 && !_achievement2)
            {
                Console.WriteLine("You've unlocked an achievement: 1000kg of coal found milestone");
                _achievement2 = true;
                achievements.Add("1000kg of coal found");
            }

            if (coal.TotalFound >= 10000 && !_achievement3)
            {
                Console.WriteLine("You've unlocked an achievement: 10000kg of coal found milestone");
                _achievement3 = true;
                achievements.Add("10000kg of coal found");
            }

            if (stone.TotalFound >= 100 && !_achievement4)
            {
                Console.WriteLine("You've unlocked an achievement: 100kg of stone found milestone");
                _achievement4 = true;
                achievements.Add("100kg of stone found");
            }

            if (stone.TotalFound >= 1000 && !_achievement5)
            {
                Console.WriteLine("You've unlocked an achievement: 1000kg of stone found milestone");
                _achievement5 = true;
                achievements.Add("1000kg of stone found");
            }

            if (stone.TotalFound >= 10000 && !_achievement6)
            {
                Console.WriteLine("You've unlocked an achievement: 10000kg of stone found milestone");
                _achievement6 = true;
                achievements.Add("10000kg of stone found");
            }

            if (iron.TotalFound >= 75 && !_achievement7)
            {
                Console.WriteLine("You've unlocked an achievement: 75kg of iron found milestone");
                _achievement7 = true;
                achievements.Add("75kg of iron found");
            }

            if (iron.TotalFound >= 750 && !_achievement8)
            {
                Console.WriteLine("You've unlocked an achievement: 750kg of iron found milestone");
                _achievement8 = true;
                achievements.Add("750kg of iron found");
            }

            if (iron.TotalFound >= 7500 && !_achievement9)
            {
                Console.WriteLine("You've unlocked an achievement: 7500kg of iron found milestone");
                _achievement9 = true;
                achievements.Add("7500kg of iron found");
            }

            if (gold.TotalFound >= 30 && !_achievement10)
            {
                Console.WriteLine("You've unlocked an achievement: 30kg of gold found milestone");
                _achievement10 = true;
                achievements.Add("30kg of gold found");
            }

            if (gold.TotalFound >= 300 && !_achievement11)
            {
                Console.WriteLine("You've unlocked an achievement: 300kg of gold found milestone");
                _achievement11 = true;
                achievements.Add("300kg of gold found");
            }

            if (gold.TotalFound >= 3000 && !_achievement12)
            {
                Console.WriteLine("You've unlocked an achievement: 3000kg of gold found milestone");
                _achievement12 = true;
                achievements.Add("3000kg of gold found");
            }

            if (diamond.TotalFound >= 10 && !_achievement13)
            {
                Console.WriteLine("You've unlocked an achievement: 10kg of diamond found milestone");
                _achievement13 = true;
                achievements.Add("10kg of diamond found");
            }

            if (diamond.TotalFound >= 100 && !_achievement14)
            {
                Console.WriteLine("You've unlocked an achievement: 100kg of diamond found milestone");
                _achievement14 = true;
                achievements.Add("100kg of diamond found");
            }

            if (diamond.TotalFound >= 1000 && !_achievement15)
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

        private static void ChangePrices()
        {
            // upto a 20% fluctuation in prices based on random probability
            Random random = new Random();
            double randomChange = random.Next(-10, 10) / 100.0 + 1;

            coal.Price *= randomChange;
            stone.Price *= randomChange;
            iron.Price *= randomChange;
            gold.Price *= randomChange;
            diamond.Price *= randomChange;
        }

        private static void EmployeeTrainingCourse(Dictionary<string, double> prices)
        {
            // to boost the productivity of employees
            Console.WriteLine("Training employees...");
            Console.WriteLine($"This course charged you {prices["trainingCourse"] * workers.Quantity} in fees");
            dollars.Quantity -= prices["trainingCourse"] * workers.Quantity;
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
}
