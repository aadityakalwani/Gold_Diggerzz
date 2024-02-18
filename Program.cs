using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Gold_Diggerzz
{
    // as of sunday 18feb 1pm, 27hours 45 minutes spent on digging sim as of google calendar
    // initial inspiration: https://replit.com/@AadityaKalwani/Digging-Simulator#main.py
    
    /* program structure + hierarchy

       - Program
           - Main()
               - RunGame()
                   - UserMenuOption()
                       - CheckIfInDebt()
                   - Dig(int daysToDig)
                       - DisplayResources()
                   - GoToMarket()
                       - DisplayResources()
                   - DisplayStuff.DisplayGameMechanics(this);
                   - DisplayStuff.DisplayStats(this);
                   - DisplayResources()
                   - QuitGame()
                   - GameFailed()
                   - ChangeProbabilities(DateTime currentDate)
                   - CheckAchievements(List<string> achievements)
                   - ChangePrices()
                   - EmployeeTrainingCourse()
               - GetValidInt(int min, int max)
               - GetValidDouble(double min, double max)
               - HireNewWorker(int numberOfWorkers)
               - DisplayStuff.DisplayEmployees(this);
       - Resource
           - Coal
           - Stone
           - Iron
           - Gold
           - Diamond
       - Dollars
       - Worker
           - Name
           - Wage
           - Price
           - EmployeeIllProbability
       - PowerUp
           - MagicTokens
           - TimeMachine
           - AncientArtefact
           - MarketMaster
       - PayForStuff
           - StockMarketCrash
           - SkipDay
           - Bribe
           - TrainingCourse
       - DisplayStuff
           - DisplayGameMechanics(Program program)
           - DisplayStats(Program program)
           - DisplayResources(Program program)
           - DisplayEmployees(Program program)
      */

   /*
    * current issues
    * inconsistent between weather effect Displaying and actual
       * eg "6 days left of bad weather" but then it's only 5 days
    */

    /* to-do ideas
     * achievements are OOP-ed? idk
     * reorder the menu options to be more flowy and logical
     * earthquakes that loosen soil and make shit easier to find (+ cool animations possible)
     * a "mine collapse" event could temporarily reduce the player's digging efficiency and kill some employees
     * tutorial mode (that is actually functional) 
     * loans - you can take a loan from the bank and pay it back with interest
     * load/save game
     * more power-ups
        * "Resource Rush": This powerup could increase the amount of all resources found for a certain number of days.
        * "Resource Radar" (for each resource): This powerup could increase the chance of finding a specific resource for a certain number of days. For example, if the powerup is activated for gold, then for the next few days, the chance of finding gold would be increased.
     * stock market feature (kinda done?)
         * Every 5 gold sold, increase gold price and for every 5 gold mined/gained, decrease price? Incentivising selling fast and not holding resources for ages
     * option to invest in the stock market
     * managers that do shit
        * eg a 'gold' manager that improves chance of finding gold but is hired for a week
        * or a 'diamond' manager to double chance of finding gold for 10 days
     * per-employee stuff
         * workers retire after x days
         * send individual number of employees for training course that then boosts their productivity
     */

    class GameState
    {
        
    }
    
    class Resource
    {
        public double InitialPrice;
        public double Probability;
        public double Price;
        public double Quantity;
        public double TotalFound;

        public Resource(double initialProbability, double initialPrice, double initialQuantity, double totalFound)
        {
            Probability = initialProbability;
            InitialPrice = initialPrice;
            Price = initialPrice;
            Quantity = initialQuantity;
            TotalFound = totalFound;
        }
    }
    
    class Worker
    {
        public string Name;
        public double Wage;
        public double Price;
        public double EmployeeIllProbability;
        public double DefaultEfficiency;
        public int DaysUntilRetirement;
        
        public Worker(string name, double wage, double price, double employeeIllProbability, double defaultEfficiency)
        {
            Name = name;
            Wage = wage;
            Price = price;
            EmployeeIllProbability = employeeIllProbability;
            DefaultEfficiency = defaultEfficiency;
            DaysUntilRetirement = 30;
        }
    }

    class PowerUp
    {
        public double Quantity;
        public double MaxQuantity;
        public double Probability;
        
        public PowerUp(double initialQuantity, double initialProbability, double maxQuantity)
        {
            Quantity = initialQuantity;
            MaxQuantity = maxQuantity;
            Probability = initialProbability;
        }
    }

    class PayForStuff
    {
        public int Price;
        public bool skipDayOrNot;
        
        public PayForStuff(int price)
        {
            Price = price;
            skipDayOrNot = false;
        }
    }

    class DisplayStuff
    {
        
        public static void DisplayGameMechanics(Program program)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    GAME MECHANICS                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine($"Chance of finding coal = {Math.Round(program.coal.Probability, 2)}%");
            Console.WriteLine($"Chance of finding stone = {Math.Round(program.coal.Probability, 2)}%");
            Console.WriteLine($"Chance of finding iron = {Math.Round(program.iron.Probability, 2)}%");
            Console.WriteLine($"Chance of finding gold = {Math.Round(program.gold.Probability, 2)}%");
            Console.WriteLine($"Chance of finding diamond = {Math.Round(program.diamond.Probability, 2)}%");
            Console.WriteLine($"Chance of finding Ancient Artefact = {Math.Round(program.ancientArtefact.Probability, 2)}%");

            Console.WriteLine($"\nCost of hiring employee = ${program._currentEmployeePrice}");
            Console.WriteLine($"Coal value = ${Math.Round(program.coal.Price, 2)}");
            Console.WriteLine($"Stone value = ${Math.Round(program.stone.Price, 2)}");
            Console.WriteLine($"Iron value = ${Math.Round(program.iron.Price, 2)}");
            Console.WriteLine($"Gold value = ${Math.Round(program.gold.Price, 2)}");
            Console.WriteLine($"Diamond value = ${Math.Round(program.diamond.Price, 2)}");

            Console.WriteLine("Resource values fluctuate by upto ±10% per day");
            Console.WriteLine("You can find powerups that have different effects");
            Console.WriteLine("The resources you gain are equal to the number of employees you have times their efficiency");
            Console.WriteLine($"Baseline wage = ${program._currentWageRate} per employee per day");
            Console.WriteLine("10% chance an employee is ill and doesn't come in to work");
            Console.WriteLine("30% pay increase on weekends only");
            Console.WriteLine("On the first of every month, employee wage increases by 10%");
            Console.WriteLine("On the 15th of each month, each employee gets 10% of your current $$$ stash (profit sharing)");
            Console.WriteLine("One x date every month, there is a stock market crash where iron, gold, and employee hiring prices halve");
            Console.WriteLine("every 10 days, the probabilities of finding resources is reduced by 5%");
            Console.WriteLine($"You can bribe the govt with ${program.bribe.Price} and not pay any wages for the next 3 days");
            Console.WriteLine("At any time if your $$$ balance goes negative, the govt sells all of your resources for 50% the current market rate");
            Console.WriteLine("If you have no resources to sell, they sell your employees for $100 each until you have 1 employee left");
            Console.WriteLine("If your $$$ balance is negative and you have no resource, you fail the game");

        }

        public static void DisplayStats(Program program)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                        YOUR STATS                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine($"Here are your stats as of {program._currentDate.Date: dddd, dd MMMM yyyy}:");
            Console.WriteLine($"Total coal found: {Math.Round(program.coal.TotalFound, 2)}kg");
            Console.WriteLine($"Total stone found: {Math.Round(program.stone.TotalFound, 2)}kg");
            Console.WriteLine($"Total iron found: {Math.Round(program.iron.TotalFound, 2)}kg");
            Console.WriteLine($"Total gold found: {Math.Round(program.gold.TotalFound, 2)}kg");
            Console.WriteLine($"Total diamond found: {Math.Round(program.diamond.TotalFound, 2)}kg");
            Console.WriteLine($"Total powerups used: {program._totalPowerUpsUsed}");
            Console.WriteLine($"Total employees hired: {program._totalEmployeesHired}");
            Console.WriteLine($"Total bribes paid: {program._totalBribes}");
            Console.WriteLine($"\nTotal dollars earned: ${Math.Round(program._totalDollarsEarned, 2)}");
            Console.WriteLine($"Total days dug: {program._totalDaysDug}");
        }

        public static void DisplayResources(Program program)
        {
            Console.WriteLine("_____________________________________________________________________");
            Console.WriteLine($"                     You have ${Math.Round(program.dollars.Quantity, 2)}\n");
            Console.WriteLine($"| You have {Math.Round(program.coal.Quantity, 2)}kg of coal         | You have {Math.Round(program.stone.Quantity, 2)}kg of stone");
            Console.WriteLine($"| You have {Math.Round(program.iron.Quantity, 2)}kg of iron         | You have {Math.Round(program.gold.Quantity, 2)}kg of gold");
            Console.WriteLine($"| You have {Math.Round(program.diamond.Quantity, 2)}kg of diamond      | You have {Math.Round(program.magicTokens.Quantity, 2)} magic tokens");
            Console.WriteLine($"| You have {program.workersList.Count} employees         | Your employees' efficiency is {Math.Round(program._averageEmployeeEfficiency, 2)}");
            Console.WriteLine("_____________________________________________________________________");
        }
        
        public static void DisplayEmployees(Program program)
        {
            Console.WriteLine("Here are your employees:");
            int i = 0;
            foreach (Worker worker in program.workersList)
            {
                i++;
                Console.WriteLine($"Employee Number {i} - {worker.Name}, Efficiency {worker.DefaultEfficiency}, Retiring in {worker.DaysUntilRetirement} days \ud83e\uddcd\u200d\u2642\ufe0f");
            }
        }
        
    }

    class Program
    {
        
        // global variables
        // the ints are for the number of days left for the effect to wear off - set to 0 in Main() during pre-game
        public  bool _animation = true;
        public int _increasedGoldChanceDays;
        public int _marketMasterDaysLeft;
        public int _noWageDaysLeft;
        public int _lessWorkerDays;
        public int _crashDaysLeft;
        public int _badWeatherDaysLeft;
        public int _hurricaneDaysLeft;
        public int _beautifulSkyDaysLeft;
        public int _totalBribes;
        public int _totalPowerUpsUsed;
        public double _totalDaysDug;
        public double _totalEmployeesHired;
        public double _totalDollarsEarned;
        public double _averageEmployeeEfficiency = 1;
        public bool _achievement1;
        public bool _achievement2;
        public bool _achievement3;
        public bool _achievement4;
        public bool _achievement5;
        public bool _achievement6;
        public bool _achievement7;
        public bool _achievement8;
        public bool _achievement9;
        public bool _achievement10;
        public bool _achievement11;
        public bool _achievement12;
        public bool _achievement13;
        public bool _achievement14;
        public bool _achievement15;
        public bool _achievement16;
        public bool _achievement17;
        public bool _achievement18;
        public bool _achievement19;
        public bool _achievement20;
        public double _currentWageRate = 10;
        public double _currentEmployeeIllProbability = 10;
        public double _currentEmployeePrice = 100;
        public DateTime _currentDate = new DateTime(2024, 1, 1);
        public static Random _random = new Random();
        public int _crashDate = _random.Next(0, 28);
        public List<Worker> workersList = new List<Worker>();
        
        // Declare your variables at the class level
        public Resource coal;
        public Resource stone;
        public Resource iron;
        public Resource gold;
        public Resource diamond;
        public Resource dollars;
        public PowerUp magicTokens;
        public PowerUp timeMachine;
        public PowerUp ancientArtefact;
        public PowerUp marketMaster;
        public PayForStuff stockMarketCrash;
        public PayForStuff skipDay;
        public PayForStuff bribe;
        public PayForStuff trainingCourse;

        private static List<string> achievementsList = new List<string>();
        
        // possible names for the workers
        // to stop screaming at me for names it doesnt recognise/think are typos
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private static List<string> _possibleNames = new List<string>()
            {
               "Elon Tusk", "Taylor Shift", "Jeff Bezosaurus", "Barack O'Banana", "Lady GooGoo", "Michael Jackhammer", "Tom Crouton", "Beyoncé Know-all", "Albert Eggstein", "Dwayne 'The Rocking Chair' Johnson",
                "Marilyn Mon-roll", "Mark Zuckerburger", "Oprah Win-free", "Steve Jobsite", "Hillary Cling-on", "Emma Stoned", "Donald Duck Trump", "Bruce Spring-chicken", "Bill Gateskeeper", "Justin Beaver",
                "Kim Car-dashing", "Shakira-law", "Charlie Sheen-bean", "Ellen DeGenerous", "Jennifer Lawn-mower", "Will Smithereens", "Cristiano Armani", "Serena Williams-son", "Vladimir Poutine", "Ariana Grand-piano",
                "Jackie Chan-delier", "Scarlett Johandsome", "Vin Diesel-truck", "Harrison Ford F-150", "Gordon Rams-lamb", "Ryan Gooseling", "Nicolas Cage-free", "Johnny Depp-fryer", "Arnold Schwarzenegger-schnitzel", "Jessica Al-bacon",
                "Keanu Reefs", "Denzel Washing-done", "Samuel L. Vodka", "Matt Damon-salad", "Angelina Jolie-ox", "Tom Cruise-control", "Kate Wins-lit", "Julia Robberies", "Robert Downey Jr. High", "Chris Prattfall",
                "Jennifer Aniston-bake", "George Clooney-tunes", "Leonardo DiCapriSun", "Kanye East", "Benedict Cucumberbatch", "Taylor Swiftboat", "Morgan Free-wheeling", "Jimmy Fallon-over", "Nicole Kidneybean", "Hugh Jackman-go",
                "John Lemonade", "Jennifer Lawrence-of-arabia", "Jennifer Garner-den", "Daniel Radish-cliffe", "Angelina Jolie-ox", "Ryan Goose-down", "Emma Watson-burger", "Justin Timberlake-up", "Robert Downey Jr. High", "Tom Hanks-giving",
                "Leonardo DiCaprio-daVinci", "Jack Black-hole", "Miley Cyrus-virus", "Katy Perry-corn", "Hugh Grant-ed", "Anne Hathaway with Words", "Sandra Bullockbuster", "Jim Carrey-on", "Eddie Murphy-bed", "Bruce Willis-tower",
                "Johnny Cash-flow", "Celine Dion-osaur", "Jennifer Lopez-ing", "Ellen DeGeneres-erator", "Chris Hemsworth-the-clock", "Halle Berry-good", "Julia Roberts-rule", "Tom Cruise-control", "Zach Galifianakis-bar", "Kate Wins-lit",
                "Denzel Washing-done", "Brad Pitt-stop", "Eva Longoria-lunch", "Julianne Moore-or-less", "Chris Evans-sive", "Reese Witherspoonful", "Charlize Thereon", "Amy Wine-handy", "Tommy Lee Bones", "Kurt Russell Sprouts",
                "Alicia Keys-to-the-city", "Adam Sand-dollar", "Bruce Spring-clean", "George Clooney-tunes", "Jennifer Aniston-the-pants", "Hugh Jacked-man", "Johnny Deep-fry", "Rihanna-na-na", "Bruce Lee-sure", "Julianne Moore-or-less",
                "Chris Pineapple", "Leonardo DiCapri-pants", "Jackie Chain-reaction", "Morgan Freeman-dle", "Robert Downey Jr. Low", "Chris Rocking Chair", "Helen Mirren-aged", "Jamie Foxx-in-the-box", "Dwayne 'The Flocking Chair' Johnson", "Arnold Schwarzenegger-salad",
                "Will Ferrell-y-good", "Gwyneth Paltrow-lint", "Bradley Cooper-ate", "Liam Neeson-light", "Tom Hardy-har-har", "Daniel Day-Lewis and Clark", "Johnny Depp-o", "Ben Affleck-tion", "Anne Hathaway with Words", "Julia Roberts-ized",
                "Russell Crow-bar", "Hugh Grant-ed", "Reese Witherspoon-fed", "Jennifer Garner-ner", "Ben Stiller-ware", "Halle Berry-licious", "John Travolted", "Denzel Washing-done", "Amy Adams-apples", "Kevin Bacon-ator",
                "Will Smithen", "Owen Wilson-you-over", "Jake Gyllen-hall", "Matthew McConaughey-mind", "Cate Blanchett-et", "Sandra Bullockbuster", "Natalie Port-man", "Steve Carell-ing", "Sylvester Stall-own", "Emily Blunt-ed",
                "Emma Stone-throw", "Mel Gibson-soup", "Ryan Reynolds-wrap", "Nicole Kid-man", "Amanda Seyfried-rice", "James Franco-american", "Kate Wins-lit", "Angelina Jolie-ox", "Harrison Ford Focus", "Julianne Moore-or-less",
                "Scarlett Johandsome", "Leonardo DiCapriSun", "Johnny Deep-fryer", "Jim Carrey-on", "Cameron Diaz-up", "Vin Diesel-truck", "Jennifer Garner-den", "Will Smithereens", "Sandra Bullockbuster", "Brad Pitt-stop",
                "Ryan Gosling-along", "Tom Cruise-control", "Russell Crow-bar", "Matt Damon-salad", "Jennifer Lawrence-of-arabia", "Reese Witherspoon-fed", "Angelina Jolie-ox", "Dwayne 'The Rocking Chair' Johnson", "George Clooney-tunes", "Robert Downey Jr. High",
                "Keanu Reeves and Butthead", "Meryl Streep-ing", "Jessica Al-bacon", "Liam Neeson-light", "Tom Hanks-giving", "Kate Wins-lit", "Bradley Cooper-ate", "Charlize Theron-ament", "Julia Roberts-rule", "Natalie Port-man",
                "Jennifer Aniston-bake", "Kevin Space-jam", "Daniel Radishcliffe", "Amy Wine-house", "Brad Pitt-stop", "Katy Perry-corn", "Hugh Grant-ed", "Anne Hathaway with Words", "Sandra Bullockbuster", "Jim Carrey-on",
                "Eddie Murphy-bed", "Bruce Willis-tower", "Johnny Cash-flow", "Celine Dion-osaur", "Jennifer Lopez-ing", "Ellen DeGeneres-erator", "Chris Hemsworth-the-clock", "Halle Berry-good", "Julia Roberts-rule", "Tom Cruise-control",
                "Zach Galifianakis-bar", "Kate Wins-lit", "Denzel Washing-done", "Brad Pitt-stop", "Eva Longoria-lunch", "Julianne Moore-or-less", "Chris Evans-sive", "Reese Witherspoonful", "Charlize Thereon", "Amy Wine-handy",
                "Tommy Lee Bones", "Kurt Russell Sprouts", "Alicia Keys-to-the-city", "Adam Sand-dollar", "Bruce Spring-clean", "George Clooney-tunes", "Jennifer Aniston-the-pants"
            };
        
        private static List<string>_usedNames = new List<string>();
        
        
        public static void Main()
        {
            Program program = new Program();
            
            // pre-game
            
            program.HireNewWorker(1);
            
            program.coal = new Resource(90, 4, 0, 0);
            program.stone = new Resource(75, 8, 0, 0);
            program.iron = new Resource(65, 15, 0, 0);
            program.gold = new Resource(20, 75, 0, 0);
            program.diamond = new Resource(5, 200, 0, 0);
            program.dollars = new Resource(0, 0, 100, 0);
            program.magicTokens = new PowerUp(0, 6, 3);
            program.timeMachine = new PowerUp(0, 3, 3);
            program.ancientArtefact = new PowerUp(0, 7, 3);
            program.marketMaster = new PowerUp(0, 4, 3);
            program.stockMarketCrash = new PayForStuff(100);
            program.skipDay = new PayForStuff(50);
            program.bribe = new PayForStuff(200);
            program.trainingCourse = new PayForStuff(400);
            
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  ______             __        __        _______   __                                                   \n /      \\           |  \\      |  \\      |       \\ |  \\                                                  \n|  $$$$$$\\  ______  | $$  ____| $$      | $$$$$$$\\ \\$$  ______    ______    ______    ______    _______ \n| $$ __\\$$ /      \\ | $$ /      $$      | $$  | $$|  \\ /      \\  /      \\  /      \\  /      \\  /       \\\n| $$|    \\|  $$$$$$\\| $$|  $$$$$$$      | $$  | $$| $$|  $$$$$$\\|  $$$$$$\\|  $$$$$$\\|  $$$$$$\\|  $$$$$$$\n| $$ \\$$$$| $$  | $$| $$| $$  | $$      | $$  | $$| $$| $$  | $$| $$  | $$| $$    $$| $$   \\$$ \\$$    \\ \n| $$__| $$| $$__/ $$| $$| $$__| $$      | $$__/ $$| $$| $$__| $$| $$__| $$| $$$$$$$$| $$       _\\$$$$$$\\\n \\$$    $$ \\$$    $$| $$ \\$$    $$      | $$    $$| $$ \\$$    $$ \\$$    $$ \\$$     \\| $$      |       $$\n  \\$$$$$$   \\$$$$$$  \\$$  \\$$$$$$$       \\$$$$$$$  \\$$ _\\$$$$$$$ _\\$$$$$$$  \\$$$$$$$ \\$$       \\$$$$$$$ \n                                                      |  \\__| $$|  \\__| $$                              \n                                                       \\$$    $$ \\$$    $$                              \n                                                        \\$$$$$$   \\$$$$$$                               \n");
            Console.ResetColor();

            Console.WriteLine("Aim of the game: survive for as long as possible before bankruptcy");
            Console.WriteLine("These are your initial resources...");

            DisplayStuff.DisplayResources(program);

            // game starts
            Console.WriteLine("The game is about to start, good luck...");
            program.RunGame();
        }

        public void RunGame()
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
                        Dig(1);
                        break;
                    case 2:
                        _animation = false;
                        Console.WriteLine("\n___  ___        _  _    _         _        ______               ______  _        \n|  \\/  |       | || |  (_)       | |       |  _  \\              |  _  \\(_)       \n| .  . | _   _ | || |_  _  _ __  | |  ___  | | | | __ _  _   _  | | | | _   __ _ \n| |\\/| || | | || || __|| || '_ \\ | | / _ \\ | | | |/ _` || | | | | | | || | / _` |\n| |  | || |_| || || |_ | || |_) || ||  __/ | |/ /| (_| || |_| | | |/ / | || (_| |\n\\_|  |_/ \\__,_||_| \\__||_|| .__/ |_| \\___| |___/  \\__,_| \\__, | |___/  |_| \\__, |\n                          | |                             __/ |             __/ |\n                          |_|                            |___/             |___/ \n");
                        Console.WriteLine("Enter number of days to dig in one go (upto 30)");
                        int daysToDig = GetValidInt(1, 30);
                        Dig(daysToDig);
                        break;
                    case 3:
                        GoToMarket();
                        break;
                    case 4:
                        Console.WriteLine("Skipping one day");
                        Console.WriteLine($"You have been charged ${skipDay.Price} for the costs of skipping a day");
                        dollars.Quantity -= skipDay.Price;
                        skipDay.skipDayOrNot = true;
                        Dig(1);
                        DisplayStuff.DisplayResources(this);
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
                                    UsePowerUp(powerUpChoice);
                                }
                                else
                                {
                                    Console.WriteLine("You don't have any Ancient Artefacts to use");
                                }

                                break;

                            case 2:
                            
                                if (timeMachine.Quantity >= 0)
                                {
                                    UsePowerUp(powerUpChoice);
                                }
                                else
                                {
                                    Console.WriteLine("You don't have any Time Machines to use");
                                }
                                break;

                            case 3:
                                if (marketMaster.Quantity >= 0)
                                {
                                    UsePowerUp( powerUpChoice);
                                }
                                else
                                {
                                    Console.WriteLine("You don't have any Market Masters to use");
                                }
                                break;
                        }

                        break;
                    case 6:
                        DisplayStuff.DisplayGameMechanics(this);
                        break;
                    case 7:
                        DisplayStuff.DisplayStats(this);
                        break;
                    case 8:
                        for (int achievementNumber = 0; achievementNumber < achievementsList.Count; achievementNumber++)
                        {
                            Console.WriteLine(
                                $"Achievement {achievementNumber}: {achievementsList[achievementNumber]}");
                        }
                        break;
                    
                    case 9:
                        if (dollars.Quantity > trainingCourse.Price * workersList.Count && workersList.Count != 0)
                        {
                            Console.WriteLine("You have chosen to send all employees on a training course");
                            Console.WriteLine($"You have been charged {trainingCourse.Price} per employee");
                            Console.WriteLine("Your employees will be back in 7 days");
                            EmployeeTrainingCourse();
                        }
                        else if (dollars.Quantity > trainingCourse.Price * workersList.Count && workersList.Count == 0)
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
                        Console.WriteLine($"1 - Pay ${stockMarketCrash.Price} for information on the next stock market crash");
                        Console.WriteLine($"2 - Bribe the government for ${bribe.Price} to not pay wages for the next 3 days");
                        int crimeChoice = GetValidInt(1, 2);

                        switch (crimeChoice)
                        {
                            case 1:
                                Console.WriteLine($"You have chosen to pay ${stockMarketCrash.Price} for information on the next stock market crash");
                                dollars.Quantity -= stockMarketCrash.Price;
                                Console.WriteLine("Giving you the information now...");
                                Console.WriteLine($"Expect a stock market crash on the {_crashDate}th of every month");
                                break;
                            case 2:
                                Console.WriteLine("You have chosen to bribe the government");
                                Console.WriteLine($"You have been charged {bribe.Price} for the bribe");
                                dollars.Quantity -= bribe.Price;
                                Console.WriteLine("You don't have to pay wages for the next three days");
                                _noWageDaysLeft = 3;
                                _totalBribes += 1;
                                break;
                        }

                        break;

                    case 11:
                        RunTutorial();
                        break;
                    
                    case 12:
                        DisplayStuff.DisplayEmployees(this);
                        break;

                    default:
                        Console.WriteLine("Please enter a valid option");
                        break;
                }
            } while (menuOption != 0 && menuOption != -1);

        }
        
        public void RunTutorial()
        {
            Console.WriteLine("Welcome to the tutorial");
            Console.WriteLine("You are a gold digger, and you have to survive for as long as possible before bankruptcy");
            Console.WriteLine("You have a few resources to start with:");
            Console.WriteLine("You have $100, 0kg of coal, 0kg of iron, 0kg of gold, 0kg stone, 0kg diamond and 1 employee");
            Console.WriteLine("You can hire more employees, dig for resources, and sell resources at the market");
            Console.WriteLine("You can also bribe the government to not pay wages for the next three days");
            Console.WriteLine($"You can also pay ${stockMarketCrash.Price} for information on the next stock market crash");
            Console.WriteLine("You can also send all employees for a training course for $400 per employee (+30% efficiency) (7 days)");
            Console.WriteLine("You can also sell all your iron and gold for dollars");
            Console.WriteLine($"You can also skip one day for ${skipDay.Price}");
            Console.WriteLine("You can also quit the game");
            Console.WriteLine("You can also dig for multiple days");
            Console.WriteLine("Here are the game mechanics:");
        }

        public int UserMenuOption()
        {
            string takeUserInput = CheckIfInDebt();

            if (takeUserInput == "false")
            {
                Console.WriteLine($"Today is {_currentDate:dddd, d MMMM, yyyy}");
                Console.WriteLine("___________________________________\n");
                Console.WriteLine("Main Features:              Display Options:                  Other Features:\n");
                Console.WriteLine("0 - Quit game               6 - Display game mechanics        4 - Skip one day");
                Console.WriteLine("1 - Dig one day             7 - Display stats                 5 - Use a powerup");
                Console.WriteLine("2 - Dig multiple days       8 - Display achievements          9 - Send employees for training");
                Console.WriteLine("3 - Go to market            11 - Display tutorial             10 - Commit a crime (further options inside)");
                Console.WriteLine("                            12 - Display employees\n");
                Console.WriteLine("Your choice:");

                int userOption = GetValidInt(0, 12);
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

        public string CheckIfInDebt()
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
                                           diamond.Quantity * diamond.Price * 0.4;
                    
                    _totalDollarsEarned += coal.Quantity * coal.Price + stone.Quantity * stone.Price +
                                           iron.Quantity * iron.Price + gold.Quantity * gold.Price +
                                           diamond.Quantity * diamond.Price * 0.4;

                    coal.Quantity = 0;
                    stone.Quantity = 0;
                    iron.Quantity = 0;
                    gold.Quantity = 0;
                    diamond.Quantity = 0;

                    DisplayStuff.DisplayResources(this);
                }

                if (inDebt == "true" && noResources && workersList.Count < 2)
                {
                    Console.WriteLine("Bro you're literally bankrupt.You have failed the game.");
                    return "bankrupt";
                }

                if (inDebt == "true" && noResources && workersList.Count >= 2)
                {
                    Console.WriteLine("You don't have resources to sell, so we're selling workers for $50 per guy.");
                    dollars.Quantity += workersList.Count * 50;
                    _totalDollarsEarned += workersList.Count * 50;
                    
                    while (workersList.Count > 1)
                    {
                        workersList.RemoveAt(0);
                    }
                }
            }

            return inDebt;
        }

        public void Dig(int daysToDig)
        {

            double totalEfficiency = 0;
            foreach (Worker worker in workersList)
            {
                totalEfficiency += worker.DefaultEfficiency;
            }
            _averageEmployeeEfficiency = totalEfficiency / workersList.Count;

            for (int days = 0; days < daysToDig; days++)
            {

                if (CheckIfInDebt() != "true")
                {
                    if (!skipDay.skipDayOrNot)
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

                        // creating randoms for the chance of finding all the stuff
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

                        double newResourcesFound = workersList.Count * _averageEmployeeEfficiency;

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
                            Console.WriteLine($"2 - Save for later (max {ancientArtefact.MaxQuantity})");
                            int userInput = GetValidInt(1, 2);

                            switch (userInput)
                            {
                                case 1:
                                    UsePowerUp(1);
                                    break;
                                case 2:
                                    if (ancientArtefact.Quantity < ancientArtefact.MaxQuantity)
                                    {
                                        Console.WriteLine("You have chosen to save the Ancient Artefact for later");
                                        ancientArtefact.Quantity += 1;
                                        break;
                                    }
                                    
                                    Console.WriteLine("You have reached the maximum quantity of Ancient Artefacts");
                                    break;
                            }
                        }

                        if (timeMachineFound)
                        {
                            Console.Write("\u23f3 You found the Time Machine power-up \u23f3");
                            Console.WriteLine("Choose an option:");
                            Console.WriteLine("1 - Use now");
                            Console.WriteLine($"2 - Save for later (max {timeMachine.MaxQuantity})");
                            int userInput = GetValidInt(1, 2);

                            switch (userInput)
                            {
                                case 1:
                                    UsePowerUp(2);
                                    break;
                                case 2:
                                    if (timeMachine.Quantity < timeMachine.MaxQuantity)
                                    {
                                        Console.WriteLine("You have chosen to save the Time Machine for later");
                                        marketMaster.Quantity += 1;
                                        break;
                                    }
                                    
                                    Console.WriteLine("You have reached the maximum quantity of Time Machines");
                                    break;
                            }
                        }

                        if (magicTokenFound && magicTokens.Quantity < magicTokens.MaxQuantity)
                        {
                            magicTokens.Quantity += 1;
                            Console.WriteLine($"You've acquired another magic token. You have {magicTokens.Quantity} magic tokens now");
                            Console.WriteLine($"Selling price increased by a total of {magicTokens.Quantity * 20}%");
                            coal.Price += coal.InitialPrice * 0.2;
                            stone.Price += stone.InitialPrice * 0.2;
                            iron.Price += iron.InitialPrice * 0.2;
                            gold.Price += gold.InitialPrice * 0.2;
                            diamond.Price += diamond.InitialPrice * 0.2;
                        }

                        if (marketMasterFound)
                        {
                            Console.WriteLine("You found the Market Master power up");
                            Console.WriteLine("Choose an option:");
                            Console.WriteLine("1 - Use now");
                            Console.WriteLine($"2 - Save for later (max {marketMaster.MaxQuantity})");
                            int userInput = GetValidInt(1, 2);

                            switch (userInput)
                            {
                                case 1:
                                    UsePowerUp(3);
                                    break;
                                case 2:
                                    if (marketMaster.Quantity < marketMaster.MaxQuantity)
                                    {
                                        Console.WriteLine("You have chosen to save the Market Master for later");
                                        marketMaster.Quantity += 1;
                                        break;
                                    }
                                    
                                    Console.WriteLine("You have reached the maximum quantity of Market Master");
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
                        double totalWages = workersList.Count * _currentWageRate;
                        dollars.Quantity -= totalWages;

                        Console.WriteLine($"Your {workersList.Count} employees charged a wage of ${Math.Round(totalWages, 2)} today.");
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
            DisplayStuff.DisplayResources(this);
            
            foreach (Worker worker in workersList)
            {
                if (worker.DaysUntilRetirement != 0)
                {
                    worker.DaysUntilRetirement -= 1;
                }

                if (worker.DaysUntilRetirement == 0)
                {
                    Console.WriteLine($"Employee {worker.Name} has retired. Goodbye!");
                    Console.WriteLine($"You now have {workersList.Count - 1} employees");
                    workersList.Remove(worker);
                }

            }
            
            skipDay.skipDayOrNot = false;
        }

        public void GoToMarket()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(
                "\n __       __                      __                   __     \n|  \\     /  \\                    |  \\                 |  \\    \n| $$\\   /  $$  ______    ______  | $$   __   ______  _| $$_   \n| $$$\\ /  $$$ |      \\  /      \\ | $$  /  \\ /      \\|   $$ \\  \n| $$$$\\  $$$$  \\$$$$$$\\|  $$$$$$\\| $$_/  $$|  $$$$$$\\\\$$$$$$  \n| $$\\$$ $$ $$ /      $$| $$   \\$$| $$   $$ | $$    $$ | $$ __ \n| $$ \\$$$| $$|  $$$$$$$| $$      | $$$$$$\\ | $$$$$$$$ | $$|  \\\n| $$  \\$ | $$ \\$$    $$| $$      | $$  \\$$\\ \\$$     \\  \\$$  $$\n \\$$      \\$$  \\$$$$$$$ \\$$       \\$$   \\$$  \\$$$$$$$   \\$$$$ \n\n");
            Console.ResetColor();

            Console.WriteLine($"Here are the rates for {_currentDate:dddd dd MMMM, yyyy}:");

            Console.WriteLine("______________________________");
            Console.WriteLine($"| Coal: ${Math.Round(coal.Price, 2)} per kg");
            Console.WriteLine($"| Stone: ${Math.Round(stone.Price, 2)} per kg");
            Console.WriteLine($"| Iron: ${Math.Round(iron.Price, 2)} per kg");
            Console.WriteLine($"| Gold: ${Math.Round(gold.Price, 2)} per kg");
            Console.WriteLine($"| Diamond: ${Math.Round(diamond.Price, 2)} per kg");
            Console.WriteLine($"| Employees: ${Math.Round(_currentEmployeePrice, 2)} per employee");
            Console.WriteLine($"| Wages: ${Math.Round(_currentWageRate, 2)} per employee per day");
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
                                DisplayStuff.DisplayResources(this);
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
                                DisplayStuff.DisplayResources(this);
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
                                DisplayStuff.DisplayResources(this);
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
                                DisplayStuff.DisplayResources(this);
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
                                DisplayStuff.DisplayResources(this);
                                break;
                        }

                        break;

                    case 2:
                        Console.WriteLine("We're selling all your coal and iron and gold and stone and diamond for dollars");
                        
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

                        Console.WriteLine("Here are your updated resources:");
                        DisplayStuff.DisplayResources(this);
                        break;

                    case 3:
                        Console.Clear();
                        Console.WriteLine("  _    _   _                      ______                       _                                       \n | |  | | (_)                    |  ____|                     | |                                      \n | |__| |  _   _ __    ___       | |__     _ __ ___    _ __   | |   ___    _   _    ___    ___   ___   \n |  __  | | | | '__|  / _ \\      |  __|   | '_ ` _ \\  | '_ \\  | |  / _ \\  | | | |  / _ \\  / _ \\ / __|  \n | |  | | | | | |    |  __/      | |____  | | | | | | | |_) | | | | (_) | | |_| | |  __/ |  __/ \\__ \\  \n |_|  |_| |_| |_|     \\___|      |______| |_| |_| |_| | .__/  |_|  \\___/   \\__, |  \\___|  \\___| |___/  \n                                                      | |                   __/ |                      \n                                                      |_|                  |___/                     ");
                        Console.WriteLine($"Each employee charges {_currentWageRate} in wages per day right now");
                        Console.WriteLine("Enter how many employees you want to hire:");
                        int employeesToHire = GetValidInt(0, 100000);
                        if (employeesToHire * _currentEmployeePrice > dollars.Quantity)
                        {
                            Console.WriteLine("You don't have enough dollars to hire that many employees");
                        }
                        else
                        {
                            Console.WriteLine($"You have hired {employeesToHire} more employees.\nSay hello to:");
                            
                            HireNewWorker(employeesToHire);
                            
                            dollars.Quantity -= employeesToHire * _currentEmployeePrice;
                            Console.WriteLine($"You now have {workersList.Count} employees");
                            _totalEmployeesHired += employeesToHire;
                        }
                        break;

                    case 4:
                        Console.WriteLine("Thanks for coming to the market!");
                        break;

                }
            } while (marketOption != 4);
        }

        public void UsePowerUp(int powerUpChoice)
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
                        Console.WriteLine("You have chosen the $200 instantly");
                        dollars.Quantity += 200;
                        _totalDollarsEarned += 200;
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
                    Dig(5);
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

        public void QuitGame()
        {
            DisplayStuff.DisplayStats(this);
            Console.WriteLine($"You lasted until {_currentDate.Date:dddd, d MMMM, yyyy}");
            Console.WriteLine("\nGoodbye!");
        }

        public void GameFailed()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                         YOU FAILED                         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            QuitGame();
        }

        public void ChangeProbabilities(DateTime currentDate)
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
                _currentWageRate *= 1.3;
                foreach (Worker workers in workersList)
                {
                    workers.Wage *= 1.3;
                }
            }

            // to undo the effect of weekend pay
            else if (currentDate.DayOfWeek is DayOfWeek.Monday)
            {
                _currentWageRate /= 1.3;
                foreach (Worker workers in workersList)
                {
                    workers.Wage /= 1.3;
                }
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
                _currentEmployeePrice *= 2;
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
                _currentEmployeePrice /= 2;
                _crashDaysLeft = 2;
            }

            // 10% raise on the first of every month (apart from January)
            if (currentDate.Month != 1 && currentDate.Day == 1)
            {
                Console.WriteLine("It's the first of the month, your employees get a 10% raise for the rest of time");
                _currentWageRate *= 1.1;
                foreach (Worker workers in workersList)
                {
                    workers.Wage *= 1.1;
                }
            }

            // to undo the effects of unwell workers
            if (_lessWorkerDays == 1)
            {
                HireNewWorker(1);
                Console.WriteLine("Your employee is back at work today");
                _lessWorkerDays = 0;
            }

            // 10% chance an employee is unwell and doesn't come in
            if (_random.Next(0, 100) < _currentEmployeeIllProbability && workersList.Count > 1)
            {
                Console.WriteLine("One of your employees is unwell and doesn't come in today");
                workersList.RemoveAt(0);
                _lessWorkerDays = 1;
            }

            // 10% profit sharing to each employee on the 15th of every month
            if (currentDate.Day == 15)
            {
                Console.WriteLine("Profit sharing time!");

                if (workersList.Count < 7)
                {
                    Console.WriteLine("Each employee gets 10% of your current $$$ stash");
                    Console.WriteLine($"Your {workersList.Count} employees get ${dollars.Quantity * 0.1} each");
                    double dollarsToLose = dollars.Quantity * 0.1 * workersList.Count;
                    dollars.Quantity -= dollarsToLose;
                    Console.WriteLine($"Your employees have been paid, you have lost $ {dollarsToLose} in the process");
                }

                else
                {
                    Console.WriteLine("Because you have so many employees, 70% of your current $$$ stash is given to them");
                    Console.WriteLine($"This means you'll lose {dollars.Quantity * 0.7}");
                    dollars.Quantity -= dollars.Quantity * 0.7;
                }
            }


            // weather effects: rain or hurricane reducing efficiency, beautifulSky increasing efficiency

            // undoing weather effects 
            if (_badWeatherDaysLeft == 1)
            {
                Console.WriteLine("The weather has cleared up, your employees are back to normal efficiency");
                _averageEmployeeEfficiency *= 1.3;
            }

            if (_beautifulSkyDaysLeft == 1)
            {
                Console.WriteLine("The weather is mid, your employees are back to normal efficiency");
                _averageEmployeeEfficiency /= 1.2;
                _beautifulSkyDaysLeft = 0;
            }

            if (_hurricaneDaysLeft == 1)
            {
                Console.WriteLine("The hurricane has passed, your employees are back to normal efficiency");
                _averageEmployeeEfficiency *= 1.4;
            }

            bool noActiveWeatherEffects =
                _badWeatherDaysLeft == 0 && _hurricaneDaysLeft == 0 && _beautifulSkyDaysLeft == 0;

            // 5% chance a hurricane that reduces the probability of finding resources by 50% for the next 5 days
            if (_random.Next(0, 100) < 5 && noActiveWeatherEffects)
            {
                Console.WriteLine("A hurricane is coming, efficiency is now 40% less the next five days");
                _averageEmployeeEfficiency /= 1.4;
                _hurricaneDaysLeft = 6;
            }

            // rain reducing efficiency
            else if (_random.Next(0, 100) < 30 && noActiveWeatherEffects)
            {
                Console.WriteLine("Due to torrential rain, your employees are 30% less efficient for the next two days");
                _averageEmployeeEfficiency /= 1.3;
                _badWeatherDaysLeft = 3;
            }

            // 30% chance beautiful sky increasing efficiency
            else if (_random.Next(0, 100) < 30 && noActiveWeatherEffects)
            {
                Console.WriteLine("The weather is beautiful today, your employees are 20% more efficient for two days");
                _averageEmployeeEfficiency *= 1.2;
                _beautifulSkyDaysLeft = 3;
            }

        }

        public void CheckAchievements(List<string> achievements)
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

        public void ChangePrices()
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

        public void EmployeeTrainingCourse()
        {
            // to boost the productivity of employees
            Console.WriteLine("Training employees...");
            Console.WriteLine($"This course charged you {trainingCourse.Price * workersList.Count} in fees");
            dollars.Quantity -= trainingCourse.Price * workersList.Count;
            _averageEmployeeEfficiency *= 1.3;
            _currentDate.AddDays(7);
            Thread.Sleep(1500);
            Console.WriteLine("7 Days have now passed");
        }

        public void HireNewWorker(int numberOfWorkers)
        {
            if (_possibleNames.Count > 0)
            {
                for (int i = 0; i < numberOfWorkers; i++)
                {
                    int randomName = _random.Next(0, _possibleNames.Count);
                    double efficiency = _random.Next(70, 130);
                    efficiency /= 100;
                    
                    Worker newWorker = new Worker(_possibleNames[randomName], _currentWageRate, _currentEmployeePrice, _currentEmployeeIllProbability, efficiency);
                    workersList.Add(newWorker);
                    _usedNames.Add(newWorker.Name);
                    _possibleNames.Remove(newWorker.Name);
                    Console.WriteLine($"{newWorker.Name}, Efficiency {newWorker.DefaultEfficiency}\ud83e\uddcd\u200d\u2642\ufe0f");
                }
            }
            else
            {
                Console.WriteLine("You've run out of names to give to your employees");
            }
        }

        public int GetValidInt(int min, int max)
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

        public double GetValidDouble(double min, double max)
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
