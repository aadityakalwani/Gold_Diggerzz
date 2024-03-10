namespace Gold_Diggerzz
{
    // as of sunday 18feb 1pm, 27hours 45 minutes spent on digging sim as of google calendar
    // now probably closer to 45, will recalculate soon
    // initial inspiration: https://replit.com/@AadityaKalwani/Digging-Simulator-1#main.py

    /* current issues
     * LOAD GAME STATE
     * you are allowed to make multiple trades per day
     * You can have negative power ups
        * when you use a powerup straight away, it shouldn't reduce the amount by one but it does 
     * you can not load a game state because of either casting issues or enumeration operation errors
        * why not, at the beginning of the game, load in a game state that has 100 dollars, 0 resources, 1 worker, 0 powerups, 0 real estate, 0 achievements, etc
            * because then i'm just updating the values rather than..shit yeah that'll work?
     */

    /* to-do ideas
     * better understand what to do with morale - right now its just a multiplier for efficiency
     * convert to proper traditional OOP via getters and setters
     * Sell/fire employees code - initial menu option and subroutine created within the Worker class
     * move UsePowerUp to PowerUp class? and other such offloading of tasks from the main class - this causes major static non-static etc issues
     * Game Progression
        * Resource Discovery: Add a feature where players can discover new resources as they dig deeper. These new resources could be more valuable but also more difficult to extract. also based on achievements unlocked
        * new mine also means new resources??? like a dinosaur mine that has dinosaur bones as well as stone, gold, etc. a space mine that has space rocks, etc
        * goals to reach
            * if you reach _______ income you can find ______
        * option to print all achievements and show if unlocked or not
     * option to upgrade your shovel to unlock more materials
     * mine more shit Eg. Mine space rocks, T-rex bones
     * Exploration: Allow the player to explore new areas or mines. This could involve a risk/reward system where exploring new areas could lead to finding more valuable resources but also has the potential for more dangerous events.
     * create reputation (do i need to? ++ how will this work with morale?)
     * a morale-boosting powerup
     * Allow employees to specialize in certain areas, making them more efficient at gathering certain resources. This could add another layer of strategy to the game as players decide how to best allocate their workforce.
     * achievements are OOP-ed? idk about this one - give it some thought
     * hence, or otherwise, an option to print all achievements as an incentive to work towards them/keep playing
     * loans - you can take a loan from the bank and pay it back with interest
     * competition / fake in some other mining companies (or your dad's company) and you're also trying to beat (give them a quadratic rate of growth?)
     */

    class Program
    {
        # region global variables

        public bool _animation = true;
        public double _upgradeInventoryCost = 1000;
        public int _increasedGoldChanceDays;
        public int _marketMasterDaysLeft;
        public int _noWageDaysLeft;
        public int _crashDaysLeft;
        public int _totalBribes;
        public int _totalPowerUpsUsed;
        public double _totalDaysDug;
        public double _totalEmployeesHired;
        public double _totalDollarsEarned;
        public double _averageEmployeeEfficiency = 1;
        public double _averageEmployeeMorale = 1;


        public static Achievements achievement1 = new Achievements(0, "100kg of coal mined");
        public static Achievements achievement2 = new Achievements(1, "1000kg of coal found");
        public static Achievements achievement3 = new Achievements(2, "10000kg of coal found");
        public static Achievements achievement4 = new Achievements(3, "100kg of stone found");
        public static Achievements achievement5 = new Achievements(4, "1000kg of stone found");
        public static Achievements achievement6 = new Achievements(5, "10000kg of stone found");
        public static Achievements achievement7 = new Achievements(6, "75kg of iron found");
        public static Achievements achievement8 = new Achievements(7, "750kg of iron found");
        public static Achievements achievement9 = new Achievements(8, "7500kg of iron found");
        public static Achievements achievement10 = new Achievements(9, "30kg of gold found");
        public static Achievements achievement11 = new Achievements(10, "300kg of gold found");
        public static Achievements achievement12 = new Achievements(11, "3000kg of gold found");
        public static Achievements achievement13 = new Achievements(12, "10kg of diamond found");
        public static Achievements achievement14 = new Achievements(13, "100kg of diamond found");
        public static Achievements achievement15 = new Achievements(14, "1000kg of diamond found");
        public static Achievements achievement16 = new Achievements(15, "$300 total earned");
        public static Achievements achievement17 = new Achievements(16, "$1000 total earned");
        public static Achievements achievement18 = new Achievements(17, "$10000 total earned");
        public static Achievements achievement19 = new Achievements(18, "10 employees hired");
        public static Achievements achievement20 = new Achievements(19, "100 employees hired");

        public List<Achievements> achievementsList = new()
        {
            achievement1, achievement2, achievement3, achievement4, achievement5, achievement6, achievement7, achievement8, achievement9, achievement10,
            achievement11, achievement12, achievement13, achievement14, achievement15, achievement16, achievement17, achievement18, achievement19, achievement20
        };
        
        public double minePercentageFullness = 100;
        public double _currentWageRate = 10;
        public double _currentEmployeeIllProbability = 5;
        public double _currentEmployeePrice = 200;
        public DateTime _currentDate = new DateTime(2024, 1, 1);
        private static Random _random = new();
        public int _crashDate = _random.Next(0, 28);

        public List<Specialist> managersList = new();

        public List<Worker> illWorkersList = new();
        public List<Worker> retiredWorkersList = new();
        public List<Worker> deadWorkersList = new();
        public List<Worker> trainingWorkersList = new();
        public List<Worker> toSendToTrainingList = new();
        public List<Trade> tradeList = new();
        public List<WeatherEffectsClass> ActiveWeatherEffectsList = new();
        public List<Worker> workersList = new() { new Worker("mid", "Bob Smith The OG Worker", 10, 100, 10, 1, 1) };
        public List<RealEstate> buildingRealEstateList = new();
        public List<RealEstate> activeRealEstate = new();
        public List<Resource> resourcesList = new();

        MiningOperation miningOperation = new();
        MarketOperation marketOperation = new();
        DayToDayOperations dayToDayOperations = new();

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
        public PayForStuff bonus;
        public PayForStuff retirementPackage;
        public Trade coalToStone;
        public Trade coalToIron;
        public Trade coalToGold;
        public Trade coalToDiamond;
        public Trade stoneToIron;
        public Trade stoneToGold;
        public Trade stoneToDiamond;
        public Trade ironToGold;
        public Trade ironToDiamond;
        public Trade goldToDiamond;


        // 307 possible names for the workers
        // to stop screaming at me for names it doesn't recognise/think are typos
        public List<string> _possibleNames = new()
        {
            // ReSharper disable StringLiteralTypo
            
            "Elon Tusk", "Taylor Shift", "Jeff Bezosaurus",
            "Barack O, Banana", "Lady GooGoo", "Michael Jackhammer",
            "Tom Crouton", "Beyoncé Know-all", "Albert Eggstein",
            "Dwayne 'The Rocking Chair, Johnson", "Marilyn Mon-roll", "Mark Zuckerburger",
            "Oprah Win-free", "Steve Jobsite", "Hillary Cling-on",
            "Emma Stoned", "Donald Duck Trump", "Bruce Spring-chicken",
            "Bill Gateskeeper", "Justin Beaver", "Kim Car-dashing",
            "Shakira-law", "Charlie Sheen-bean", "Ellen DeGenerous",
            "Jennif er Lawn-mower", "Will Smithereens", "Cristiano Armani",
            "Serena Williams-son", "Vladimir Poutine", "Ariana Grand-piano",
            "Jackie Chan-deliep", "Scarlett Johandsome", "Vin Diesel-truck",
            "Harrison Ford F-150", "Gordon Rams-lamb", "Ryan Gooseling",
            "Nicolas Cage-f ree", "Johnny Depp-fryer", "Arnold Schwarzenegger-schnitzel",
            "Jessica Al-bacon", "Keanu Reefs", "Denzel Washing-done",
            "Samuel L. Vodka", "Matt Damon-salad", "Angelina Jolie-ox",
            "Tom Cruise-control", "Kate Wins-lit", "Julia Robberies",
            "Robert Downey Jp. High", "Chris Prattfall", "Jennif er Aniston-bake",
            "George Clooney-tunes", "Leonardo DiCapriSun", "Kanye East",
            "Benedict Cucumberbatch", "Taylor Swiftboat", "Morgan Free-wheeling",
            "Jimmy Fallon-over", "Nicole Kidneybean", "Hugh Jackman-go",
            "John Lemonade", "Miley Virus", "Katy Perry-able",
            "Jennifer Lawrence-of-arabia", "Jennifer Garner-den", "Daniel Radish-cliffe",
            "Ryan Goose-down", "Emma Watson-burgep", "Justin Timberlake-up",
            "Tom Hanks-giving", "Leonardo DiCaprio-daVinci", "Jack Black-hole",
            "Miley Cyrus-virus", "Katy Perry-corn", "Hugh Grant-ed",
            "Anne Hathaway with Words", "Sandra Bullockbuster", "Jim Carrey-on",
            "Eddie Murphy-bed", "Bruce Willis-tower", "Johnny Cash-flow",
            "Celine Dion-osaur", "Jennif er Lopez-ing", "Ellen DeGeneres-erator",
            "Chris Hemsworth-the-clock", "Halle Berry-good", "Julia Roberts-rule",
            "Zach Galif ianakis-bar", "Brad Pitt-stop", "Eva Longoria-lunch",
            "Julianne Moore-or-less", "Chris Evans-sive", "Reese Witherspoonful",
            "Charlize Thereon", "Amy Wine-handy", "Tommy Lee Bones",
            "Kurt Russell Sprouts", "Alicia Keys-to-the-city", "Adam Sand-dollar",
            "Bruce Spring-clean", "Jennif er Aniston-the-pants", "Hugh Jacked-man",
            "Johnny Deep-fry", "Rihanna-na-na", "Bruce Lee-sure",
            "Chris Pineapple", "Leonardo DiCapri-pants", "Jackie Chain-reaction",
            "Morgan Freeman-dle", "Robert Downey Jr. Low", "Chris Rocking Chair",
            "Helen Mirren-aged", "Jamie Foxx-in-the-box", "Dwayne 'The Flocking Chair'  Johnson",
            "Arnold Schwarzenegger-salad", "Will Ferrell-y-good", "Gwyneth Paltrow-lint",
            "Bradley Cooper-ate", "Liam Neeson-light", "Tom Hardy-har-har",
            "Daniel Day-Lewis and Clark", "Johnny Depp-o", "Ben Affleck-tion",
            "Julia Roberts-ized", "Russell Crow-bar", "Reese Witherspoon-fed",
            "Jennif er Garner-ner", "Ben Stiller-ware", "Halle Berry-licious",
            "John Travolted", "Amy Adams-apples", "Kevin Bacon-ator",
            "Will Smithen", "Owen Wilson-you-over", "Jake Gyllen-hall",
            "Matthew McConaughey-mind", "Cate Blanchett-et", "Natalie Port-man",
            "Sylvester Stall-own", "Emily Blunt-ed", "Emma Stone-throw",
            "Mel Gibson-soup", "Ryan Reynolds-wrap", "Nicole Kid-man",
            "Amanda Seyf ried-rice", "James Franco-american", "Harrison Ford Focus",
            "Johnny Deep-fryer", "Cameron Diaz-up", "Ryan Gosling-along",
            "Keanu Reeves and Butthead", "Meryl Streep-ing", "Charlize Theron-ament",
            "Kevin Space- jam", "Daniel Radishcliffe", "Amy Wine-house",
            "Steve Carell-ing", "Horrid Henry", "Perfect Peter", "Moody Margaret",
            "Sour Susan", "Rude Ralph", "Aerobic Al", "Greedy Graham", "Lazy Linda",
            "Anxious Andrew", "Weepy William", "Tidy Ted", "Goody-Goody Gordon",
            "Singing Soraya", "Beefy Bert", "Jolly Josh", "Sour-faced Simon",
            "Stuck-up Steve", "Igglepiggle", "Upsy Daisy",
            "Makka Pakka", "Hugh Jack-o-lantern", "Amanda Seyfried-rice",
            "Jennifer Garner-den", "Jennifer Lawrence-of-arabia", "Dwayne 'The Rocking Chair' Johnson",
            "Robert Downey Jr. High", "Jennifer Aniston-bake", "Usain Bolt-of-lightning",
            "Cristiano Ronaldoughnut", "Lionel Messy", "Serena William-son",
            "Michael Jordan-almond", "Tiger Woods-chips", "LeBron Jam",
            "Rafael Nada-lot", "Roger Fed-her", "Novak Jokewitch",
            "Tom Brady-punch", "Kobe Beef Bryant", "Wayne Goofy Rooney",
            "Muhammad Alley-oop", "Miley Virus", "Elvis Parsley",
            "Dolly Part-on", "Madonna-nna", "Frank Sinatra-ture",
            "Britney Spears-it", "Freddie Mercury-fish", "Eminem & M",
            "Mariah Car-rear", "Adele-dazeem", "Ed Sheeran-will",
            "Selena Gómez", "Whitney Houston-we-have-a-problem", "Alicia Keypers",
            "Stevie Wonder-ful", "John Lennon-ade", "Ed Shear-stress",
            "Mick Jagger-not", "Britney Spearmint", "Freddy Mer-curly",
            "Taylor Swift-key", "Miley Cyclone", "John Lemon",
            "Mariah Carrot", "Nickelback-ache", "Madonna-lisa",
            "Jennifer Lopezhole", "Bruno Mars-bar", "Lana Del Spray",
            "Axl Rose-thorn", "Sam Smith-and-Wesson", "Chris Brown-sugar",
            "Bob Marley-ed cheese", "Dua Lipa-stick", "Sting-ray", "Snoop Doggy Bag",
            "Cardi No-B", "Drake-up", "Eminem-and-a-half",
            "Charli Chaplin", "Addison Rae of sunshine", "Bella Porridge",
            "Dixie D'Amelion", "David Dollop", "Zach Kinda-funny",
            "Loren Beer Gray", "Spencer What-does-he-do", "James Cha-rlie's-bro",
            "Noah Smiley", "Amber of the evening", "Josh Kool-Aid",
            "Avani Bandage", "Chase Soap Hudson", "D'Amelion",
            "J.K. Rolling-pin", "Stephen Peanut King", "Agatha Crispie",
            "Dan Brown-bag", "Dr. Seuss and desist", "George R.R. Jar Jar Martin",
            "Jane Awful", "Ernest Hemingweigh", "Charles Dick-in-son",
            "Mark Twain-wreck", "Oscar Wilde-berry", "Virginia Wolf-down",
            "William Shake-a-spear", "Lewis Carrot", "Tolkien-ish",
            "J.D. Salami-nger", "F. Scott Fitchgerald", "Homer's Odyssey",
            "Arthur Conan Doughnut", "Leo Tolstoyed", "Rolling-pin",
            "Peanut King", "Crispie", "Brown-bag", "and desist",
            "Jar Jar Martin", "Awful", "Hemingweigh", "Dick-in-son",
            "Twain-wreck", "Wilde-berry", "Wolf-down", "Shake-a-spear",
            "Bill Gaps", "Tim Cook-e", "Larry Page-turner", "Sergey Brin-ger",
            "Satya Nutella", "Jack Dorsey-doody", "Reed Hastings-hound",
            "Susan YouTube", "Felix Kjellberg-fish", "Mr. Beastie Boys",
            "MKBHTea", "Ninja - the CEO of Fortnite", "Casey Nice-dad",
            "PewDiePie-rce", "T-Series of Unfortunate Events", "Jake Paul-itician",
            "Logan Paul-itics", "Barack O'Banana", "Jennifer Lawn-mower",
            "Jackie Chan-delier", "Nicolas Cage-free", "Julia Rob",
            "LeBron Slam James", "Tom Terrific Brady", "Michael Grand Slam Jordan",
            "Serena Smash Williams", "Lionel Fast Messi", "Roger Slam-derer",
            "Dale Earn-heart Junior", "Usain Streak Bolt", "Steph Curry-flavored",
            "Alex Rodriguez-n-roll", "Shaquille O'Peel", "Michael Phelps-water",
            "Ronda Rousey-n-feathers"
        };

        public List<string> UsedNames = new();

        # endregion

        public static void Main()
        {
            Console.Clear();

            Program program = new Program();

            GameState.CreateNewGameState(program);

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("   _____           _       _        _____    _                                            \n  / ____|         | |     | |      |  __ \\  (_)                                           \n | |  __    ___   | |   __| |      | |  | |  _    __ _    __ _    ___   _ __   ____  ____ \n | | |_ |  / _ \\  | |  / _` |      | |  | | | |  / _` |  / _` |  / _ \\ | '__| |_  / |_  / \n | |__| | | (_) | | | | (_| |      | |__| | | | | (_| | | (_| | |  __/ | |     / /   / /  \n  \\_____|  \\___/  |_|  \\__,_|      |_____/  |_|  \\__, |  \\__, |  \\___| |_|    /___| /___| \n                                                  __/ |   __/ |                           \n                                                 |___/   |___/                            \n");
            Console.ResetColor();

            Thread.Sleep(1000);
            Console.WriteLine("Note that this is a work in progress and I'm constantly updating it");
            Console.WriteLine("Periodically re-download the game file to get the latest version of the game");
            Console.WriteLine("If you have any suggestions or feedback, please let me know");
            Console.WriteLine("\n____________________________________________________________________________________________\n");
            Thread.Sleep(1000);

            Console.WriteLine("Welcome, the aim of the game is to survive for as long as possible before bankruptcy");
            Console.WriteLine("The game is about to start, good luck...\n\n[ENTER]");
            Console.ReadLine();
            Console.Clear();

            program.RunGame();
        }

        public void RunGame()
        {
            RunTutorial();
            Console.Clear();

            DisplayStuff.DisplayResources(this);

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
                        miningOperation.Dig(1, this, dayToDayOperations, achievementsList);
                        break;
                    case 2:
                        _animation = false;
                        Console.WriteLine("\n  __  __           _   _     _           _             _____                       _____    _         \n |  \\/  |         | | | |   (_)         | |           |  __ \\                     |  __ \\  (_)        \n | \\  / |  _   _  | | | |_   _   _ __   | |   ___     | |  | |   __ _   _   _     | |  | |  _    __ _ \n | |\\/| | | | | | | | | __| | | | '_ \\  | |  / _ \\    | |  | |  / _` | | | | |    | |  | | | |  / _` |\n | |  | | | |_| | | | | |_  | | | |_) | | | |  __/    | |__| | | (_| | | |_| |    | |__| | | | | (_| |\n |_|  |_|  \\__,_| |_|  \\__| |_| | .__/  |_|  \\___|    |_____/   \\__,_|  \\__, |    |_____/  |_|  \\__, |\n                                | |                                      __/ |                   __/ |\n                                |_|                                     |___/                   |___/ \n");
                        Console.WriteLine("Enter number of days to dig in one go (upto 30)");
                        int daysToDig = GetValidInt(0, 30);
                        miningOperation.Dig(daysToDig, this, dayToDayOperations, achievementsList);
                        break;
                    case 3:
                        marketOperation.GoToMarket(this);
                        break;
                    case 4:
                        Trade.GoToTrader(this);
                        break;
                    case 5:
                        DisplayStuff.DisplayEmployees(this);
                        break;
                    case 6:
                        Worker.EmployeeHiringScreen(this);
                        break;
                    case 7:
                        Worker.EmployeeFiringScreen(this);
                        break;
                    case 8:
                        Console.WriteLine("\n   _____                      _       ______                      _______                  _           _                 \n  / ____|                    | |     |  ____|                    |__   __|                (_)         (_)                \n | (___     ___   _ __     __| |     | |__      ___    _ __         | |     _ __    __ _   _   _ __    _   _ __     __ _ \n  \\___ \\   / _ \\ | '_ \\   / _` |     |  __|    / _ \\  | '__|        | |    | '__|  / _` | | | | '_ \\  | | | '_ \\   / _` |\n  ____) | |  __/ | | | | | (_| |     | |      | (_) | | |           | |    | |    | (_| | | | | | | | | | | | | | | (_| |\n |_____/   \\___| |_| |_|  \\__,_|     |_|       \\___/  |_|           |_|    |_|     \\__,_| |_| |_| |_| |_| |_| |_|  \\__, |\n                                                                                                                    __/ |\n                                                                                                                   |___/ \n");
                        Console.WriteLine($"Enter number of employees to send on training\nYou have {workersList.Count} employees");
                        int employeesToSend = GetValidInt(0, workersList.Count);

                        // if you can afford it and it'll result in at least 1 employee being left
                        if (dollars.Quantity > trainingCourse.Price * employeesToSend && workersList.Count - employeesToSend >= 1)
                        {
                            Console.WriteLine($"You have chosen to send {employeesToSend} employees on a training course");
                            Console.WriteLine($"You have been charged {trainingCourse.Price} per employee");
                            Console.WriteLine($"Your {employeesToSend} employees will be back in 7 days");
                            EmployeeTrainingCourse(employeesToSend);
                        }

                        // if you can afford it but it'll result in 0 employees being left to dig
                        else if (dollars.Quantity > trainingCourse.Price * employeesToSend && workersList.Count - employeesToSend >= 1)
                        {
                            Console.WriteLine("You can't send that many employees to send on a training course");
                            Console.WriteLine("This could be because of employee illness or because it'll result in you having less than 1 employee left to dig");
                            Console.WriteLine("Hire more employees & try again later");
                        }
                        else
                        {
                            Console.WriteLine($"You don't have enough money to send {employeesToSend} employees on a training course");
                        }

                        break;
                    case 9:
                        Console.WriteLine("This feature is under development - expect it to be a bit bad and not refined");
                        Worker.EmployeeMoraleBoostingScreen(this);
                        break;
                    case 10:
                        DisplayStuff.DisplayGameMechanics(this);
                        break;
                    case 11:
                        DisplayStuff.DisplayStats(this);
                        break;
                    case 12:
                        DisplayStuff.DisplayAchievements(achievementsList, 1);
                        break;
                    case 23:
                        RunTutorial();
                        break;
                    case 14:
                        DisplayStuff.DisplayRealEstate(this);
                        break;
                    case 15:
                        Console.WriteLine("Welcome to the real estate building place!");
                        Console.WriteLine("(Tip: to gain more of a specific resource, convert resources at the trader)\n");
                        Console.WriteLine("What real estate do you want to build?");
                        Console.WriteLine("0. Cancel");
                        Console.WriteLine("1. Apartment - Cost: $10,000, $3,000 weekly rent");
                        Console.WriteLine("2. House - Cost: 100kg coal, 30kg coal weekly rent");
                        Console.WriteLine("3. Office - Cost: 100kg stone, 30kg stone weekly rent");
                        Console.WriteLine("4. Mansion - Cost: 100kg iron, 30kg iron weekly rent");
                        Console.WriteLine("5. Castle - Cost: 100kg gold, 30kg gold weekly rent");
                        Console.WriteLine("6. Palace - Cost: 100kg diamond, 30kg diamond weekly rent");
                        int choice = GetValidInt(0, 6);
                        RealEstate.BuildRealEstate(choice, this);
                        Thread.Sleep(2000);
                        break;
                    case 16:
                        if (ancientArtefact.Quantity <= 0 && timeMachine.Quantity <= 0 &&
                            marketMaster.Quantity <= 0)
                        {
                            Console.WriteLine("\u274c You don't have any powerups to use \u274c");
                            Console.WriteLine("(also idk how but it might say you have negative powerups, ignore that for now... i'll fix it later)");
                            break;
                        }

                        Console.WriteLine("What powerup do you want to use?");
                        Console.WriteLine($"You have {ancientArtefact.Quantity} Ancient Artefacts, {timeMachine.Quantity} Time Machines and {marketMaster.Quantity} Market Masters\n");
                        Console.WriteLine("0 - Cancel & Return");
                        Console.WriteLine("1 - Ancient Artefact --> 2 options inside");
                        Console.WriteLine("2 - Time Machine --> Gain 5 days' worth of rewards without costing you anything");
                        Console.WriteLine("3 - Market Master --> 50% increase in the selling price of all resources for 3 days");
                        int powerUpChoice = GetValidInt(0, 3);

                        Thread.Sleep(1000);
                        Console.Clear();

                        switch (powerUpChoice)
                        {
                            case 1:
                                Console.WriteLine("You have chosen to use an Ancient Artefact. Choose an option:");
                                Console.WriteLine("1 - Increase the probability of finding gold to 50% for 3 days");
                                Console.WriteLine("2 - $200 instantly");
                                int ancientArtefactChoice = GetValidInt(1, 2);
                                UsePowerUp(powerUpChoice, ancientArtefactChoice);
                                break;
                            case 2:
                                UsePowerUp(powerUpChoice, 0);
                                break;
                            case 3:
                                UsePowerUp(powerUpChoice, 0);
                                break;
                        }

                        break;
                    case 17:
                        Console.WriteLine("0 - no im scared of committing crimes");
                        Console.WriteLine($"1 - Pay ${stockMarketCrash.Price} for information on the next stock market crash");
                        Console.WriteLine($"2 - Bribe the government for ${bribe.Price} to not pay wages for the next 3 days");
                        int crimeChoice = GetValidInt(0, 2);
                        switch (crimeChoice)
                        {
                            case 1:
                                if (dollars.Quantity > stockMarketCrash.Price)
                                {
                                    Console.WriteLine($"You have chosen to pay ${stockMarketCrash.Price} for information on the next stock market crash");
                                    dollars.Quantity -= stockMarketCrash.Price;
                                    Console.WriteLine("Giving you the information now...");
                                    Console.WriteLine($"Expect a stock market crash on the {_crashDate}th of every month");
                                    break;
                                }

                                Console.WriteLine("\u274c You can't afford to do this brokie \u274c");
                                break;
                            case 2:
                                if (dollars.Quantity > bribe.Price)
                                {
                                    Console.WriteLine("You have chosen to bribe the government");
                                    Console.WriteLine($"You have been charged {bribe.Price} for the bribe");
                                    dollars.Quantity -= bribe.Price;
                                    Console.WriteLine("You don't have to pay wages for the next three days");
                                    _noWageDaysLeft = 3;
                                    _totalBribes += 1;
                                    break;
                                }

                                Console.WriteLine("\u274c You can't afford to do this brokie \u274c");
                                break;
                        }


                        break;
                    case 18:
                        SaveGameState(1);
                        break;
                    case 19:
                        Console.WriteLine(
                            "This feature does not fully work yet. I'll let it run just cuz, but whenever its done its thing it'll take you back to the main menu screen");
                        SaveGameState(2);
                        break;
                    case 20:
                        Console.WriteLine("Skipping one day");
                        Console.WriteLine(
                            $"You have been charged ${skipDay.Price} for the costs of skipping a day");
                        dollars.Quantity -= skipDay.Price;
                        skipDay.skipDayOrNot = true;
                        miningOperation.Dig(1, this, dayToDayOperations, achievementsList);
                        DisplayStuff.DisplayResources(this);
                        break;
                    case 21:
                        Console.WriteLine("You have chosen to move to a new mine");
                        Console.WriteLine("This feature is under development - expect it to be a bit bad and not refined, or just have basically no effect on the game at all lol");
                        MoveToNewMine();
                        break;
                    case 22:
                        Console.WriteLine("You have chosen to hire a manager");
                        Console.WriteLine("This feature is under development - expect it to be a bit bad and not refined, or just have basically no effect on the game at all lol");
                        Specialist.SpecialistHiringScreen(this);
                        break;
                    case 13:
                        DisplayStuff.DisplayAchievements(achievementsList, 2);
                        break;
                    default:
                        Console.WriteLine("Please enter a valid option");
                        break;
                }
            } while (menuOption != 0 && menuOption != -1);

        }

        public void RunTutorial()
        {
            Thread.Sleep(1250);
            Console.WriteLine("Your father is the manager of a large, successful and respectable gold mining company.");
            Thread.Sleep(1750);
            Console.WriteLine("However, as he ages, he begins to worry about the future of the company.");
            Console.WriteLine("He decides to teach you the ropes of the business - one day, you may have to take over the company.");
            Console.WriteLine("To see if you're worthy, he decides to give you a new company to run.\n\n[ENTER]");
            Console.ReadLine();
            Console.WriteLine("He offers you the chance to learn the business before you begin...");
            Console.WriteLine("Do you want to go through a tutorial? ('y' for 'yes', anything else for 'no'): ");

            string tutorialChoice = Console.ReadLine().ToLower();

            if (tutorialChoice == "y")
            {
                Console.WriteLine("He smiles and says 'Good choice, my child'.");
                Thread.Sleep(2000);
                Console.Clear();
                Console.WriteLine("\n  _______           _                    _           _ \n |__   __|         | |                  (_)         | |\n    | |     _   _  | |_    ___    _ __   _    __ _  | |\n    | |    | | | | | __|  / _ \\  | '__| | |  / _` | | |\n    | |    | |_| | | |_  | (_) | | |    | | | (_| | | |\n    |_|     \\__,_|  \\__|  \\___/  |_|    |_|  \\__,_| |_|\n");
                Console.WriteLine("Welcome to the tutorial!");
                Thread.Sleep(2000);
                Console.WriteLine("You are now the manager of a new gold digging business.");
                Console.WriteLine("Your aim is to survive for as long as possible before bankruptcy, thereby proving your worth to your father.");
                Console.WriteLine("He gives you $100 to start with, along with one of his most unbelievably average employees, 'Bob Smith The OG Worker'.");
                Thread.Sleep(2500);
                Console.WriteLine("\nAs you dig for resources, you can sell them at the market to earn money.");
                Console.WriteLine("As you gain money and look to expand, you can hire more employees.");
                Console.WriteLine("You can also build real estate to earn passive income, but that's for later.");
                Console.WriteLine("\n________________________________________________________________________________________________________________________________________");
                Thread.Sleep(2000);
                Console.WriteLine("To begin, enter '1' in the main menu to dig for resources                                                                              |");
                Console.WriteLine("Whenever you see fit, enter '3' in the main menu to go to the market and sell your resources...and so your snowballing growth begins   |");
                Console.WriteLine("As you begin to get more fluent and learn the ways of the game, try out more and more menu options                                     |");
                Console.WriteLine("________________________________________________________________________________________________________________________________________");
                Thread.Sleep(3500);
                Console.WriteLine("\n(THIS TUTORIAL IS AWFUL, I'M SORRY; I'M WORKING ON IT)\n[ENTER] ");
                Console.ReadLine();
            }

            else
            {
                Console.WriteLine("He frowns upon your arrogance, but decides to give you the company anyway.");
                Console.WriteLine("You'll have to learn on the job and prove you're a worthy successor. Good luck!");
                Thread.Sleep(3000);
            }
        }

        public int UserMenuOption()
        {
            string takeUserInput = CheckIfInDebt();

            if (takeUserInput == "false")
            {
                Console.WriteLine($"Today is {_currentDate:dddd, d MMMM, yyyy}, and you have ${Math.Round(dollars.Quantity, 2)}");
                Console.WriteLine("Choose an option:");
                Console.WriteLine("_________________________________________________________________________________________________________________________________________________________________");
                Console.WriteLine("Main Features:              Employee Options:               Display Features:                 Real Estate Features:          Other Features:                     |");
                Console.WriteLine("                                                                                                                                                                 |");
                Console.WriteLine("0 - Quit game           |   5 - Show employees          |   10 - Display game mechanics   |   14 - Display real estate   |   16 - Use a powerup                  |");
                Console.WriteLine("1 - Dig one day         |   6 - Hire employees          |   11 - Display stats            |   15 - Build real estate     |   17 - Commit a crime (more inside)   |");
                Console.WriteLine("2 - Dig multiple days   |   7 - Fire employees          |   12 - Completed achievements   |                              |   18 - Save current progress          |");
                Console.WriteLine("3 - Go to market        |   8 - Train employees         |   13 - Incomplete achievements  |                              |   19 - Load game state                |");
                Console.WriteLine("4 - Go to Trader        |   9 - Boost employee morale   |   23 - Display tutorial         |   21 - Move to a new mine    |   20 - Skip one day                   |");
                Console.WriteLine("                        |   22 - Hire specialist        |                                 |                              |                                       |");
                Console.WriteLine("\nEnter your choice:");

                int userOption = GetValidInt(0, 22);
                Console.Clear();
                return userOption;
            }

            if (takeUserInput == "bankrupt")
            {
                return -1;
            }

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

                if (noResources && workersList.Count <= 1)
                {
                    Console.WriteLine("\ud83d\udca9\ud83d\udca9 Bro you're literally bankrupt; you have negative money, no resources, and one employee.\nYou have disappointed your father...");
                    Thread.Sleep(2500);
                    Console.WriteLine("..You have failed the game \ud83d\udca9\ud83d\udca9");
                    Thread.Sleep(2000);
                    return "bankrupt";
                }

                if (noResources && workersList.Count >= 2)
                {
                    Console.WriteLine("\ud83d\udc77 \ud83d\udd2bYou don't have resources to sell, so we're selling all your workers for $50 each to pay off the debt \ud83d\udc77 \ud83d\udd2b");
                    dollars.Quantity += workersList.Count * 50;
                    _totalDollarsEarned += workersList.Count * 50;

                    while (workersList.Count > 1)
                    {
                        workersList.RemoveAt(0);
                    }
                }

                else if (!noResources)
                {

                    double changeInDollars = (coal.Quantity * coal.Price + stone.Quantity * stone.Price +
                                              iron.Quantity * iron.Price + gold.Quantity * gold.Price +
                                              diamond.Quantity * diamond.Price) * 0.3;

                    dollars.Quantity += changeInDollars;
                    _totalDollarsEarned += changeInDollars;

                    // if they wont be able to pay wages after gaining the extra dollars, declare bankruptcy to avoid infinite loop

                    if (dollars.Quantity > workersList.Count * _currentWageRate * 0.6)
                    {
                        Console.WriteLine("\ud83d\udca9\ud83d\udca9 Bro you're literally bankrupt; you have no resources, and you can't afford to pay your workers for another day.\nYou have disappointed your father...");
                        Thread.Sleep(2500);
                        Console.WriteLine("..You have failed the game\ud83d\udca9\ud83d\udca9");
                        Thread.Sleep(1500);
                        return "bankrupt";
                    }

                    // else, get the bossman on your case
                    Console.WriteLine("\n\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31");
                    Thread.Sleep(2000);
                    Console.WriteLine("You are in debt, bossman is coming for you \ud83d\ude22");
                    Console.WriteLine("\ud83d\udcc9\ud83d\udcc9 The government will come and sell all your resources for 30% the current market rate... \ud83d\udcc9\ud83d\udcc9");
                    Thread.Sleep(1750);
                    Console.WriteLine($"Right now you have ${Math.Round(dollars.Quantity),2}... it's not looking good for you");
                    Console.WriteLine("Unlucky bro...");
                    Thread.Sleep(1250);

                    coal.Quantity = 0;
                    stone.Quantity = 0;
                    iron.Quantity = 0;
                    gold.Quantity = 0;
                    diamond.Quantity = 0;

                    Console.WriteLine("\u26a1 After bossman stole your resources, you now have:");
                    DisplayStuff.DisplayResources(this);
                }
            }

            return inDebt;
        }

        public void MoveToNewMine()
        {
            Console.WriteLine("You will lose all your current resources and real estate, but keep your employees");
            Console.WriteLine("Enter what mine you want to move to:");
            Console.WriteLine("1. bad mine - $100");
            Console.WriteLine("2. mid mine - $500");
            Console.WriteLine("3. good mine - $1000");
            int mineChoice = GetValidInt(1, 3);
            switch (mineChoice)
            {
                case 1:
                    if (dollars.Quantity >= 100)
                    {

                        Console.WriteLine("You have moved to the bad mine");
                        minePercentageFullness = 100;
                        coal.Probability = coal.OriginalProbability;
                        stone.Probability = stone.OriginalProbability;
                        iron.Probability = iron.OriginalProbability;
                        gold.Probability = gold.OriginalProbability;
                        diamond.Probability = diamond.OriginalProbability;

                        dollars.Quantity -= 100;
                        coal.Quantity = 0;
                        stone.Quantity = 0;
                        iron.Quantity = 0;
                        gold.Quantity = 0;
                        diamond.Quantity = 0;

                    }

                    Console.WriteLine("You don't have enough money to move to the bad mine");
                    break;
                case 2:
                    if (dollars.Quantity >= 500)
                    {

                        Console.WriteLine("You have moved to the mid mine");
                        minePercentageFullness = 100;
                        coal.Probability = coal.OriginalProbability;
                        stone.Probability = stone.OriginalProbability;
                        iron.Probability = iron.OriginalProbability;
                        gold.Probability = gold.OriginalProbability;
                        diamond.Probability = diamond.OriginalProbability;

                        dollars.Quantity -= 500;
                        coal.Quantity = 0;
                        stone.Quantity = 0;
                        iron.Quantity = 0;
                        gold.Quantity = 0;
                        diamond.Quantity = 0;

                    }

                    Console.WriteLine("You don't have enough money to move to the mid mine");
                    break;
                case 3:
                    if (dollars.Quantity >= 1000)
                    {

                        Console.WriteLine("You have moved to the good mine");
                        minePercentageFullness = 100;
                        coal.Probability = coal.OriginalProbability;
                        stone.Probability = stone.OriginalProbability;
                        iron.Probability = iron.OriginalProbability;
                        gold.Probability = gold.OriginalProbability;
                        diamond.Probability = diamond.OriginalProbability;

                        dollars.Quantity -= 1000;
                        coal.Quantity = 0;
                        stone.Quantity = 0;
                        iron.Quantity = 0;
                        gold.Quantity = 0;
                        diamond.Quantity = 0;

                    }

                    Console.WriteLine("You don't have enough money to move to the good mine");
                    break;
            }

        }

        public void UsePowerUp(int powerUpChoice, int subChoice)
        {

            Console.WriteLine("\n  _    _                      _____                                                         \n | |  | |                    |  __ \\                                                        \n | |  | |  ___    ___        | |__) |   ___   __      __   ___   _ __   _   _   _ __    ___ \n | |  | | / __|  / _ \\       |  ___/   / _ \\  \\ \\ /\\ / /  / _ \\ | '__| | | | | | '_ \\  / __|\n | |__| | \\__ \\ |  __/       | |      | (_) |  \\ V  V /  |  __/ | |    | |_| | | |_) | \\__ \\\n  \\____/  |___/  \\___|       |_|       \\___/    \\_/\\_/    \\___| |_|     \\__,_| | .__/  |___/\n                                                                               | |          \n                                                                               |_         ");

            switch (powerUpChoice)
            {
                case 1:

                    if (subChoice == 1)
                    {
                        Console.WriteLine("You have chosen the 50% chance of finding gold for the next five days");
                        _increasedGoldChanceDays = 5;
                    }
                    else if (subChoice == 2)
                    {
                        Console.WriteLine("You have chosen the $200 instantly");
                        dollars.Quantity += 200;
                        _totalDollarsEarned += 200;
                    }

                    if (ancientArtefact.Quantity > 0)
                    {
                        ancientArtefact.Quantity -= 1;
                    }
                    
                    break;

                case 2:

                    Console.WriteLine("You have chosen to use the Time Machine powerup");
                    _noWageDaysLeft = 10;
                    _animation = false;
                    miningOperation.Dig(5, this, dayToDayOperations, achievementsList);
                    if (timeMachine.Quantity > 0)
                    {
                        timeMachine.Quantity -= 1;
                    }
                    
                    break;

                case 3:
                    Console.WriteLine("Applying the Market Master power up...");
                    Console.WriteLine(
                        "The selling price of all resources has increased by 50% for the next 5 days");
                    _marketMasterDaysLeft = 5;

                    coal.Price *= 1.5;
                    stone.Price *= 1.5;
                    iron.Price *= 1.5;
                    gold.Price *= 1.5;
                    diamond.Price *= 1.5;

                    if (marketMaster.Quantity > 0)
                    {
                        marketMaster.Quantity -= 1;
                    }

                    break;
            }

            _totalPowerUpsUsed += 1;
        }

        public void SaveGameState(int saveOrLoad)
        {
            Console.WriteLine("Saving/load game state...");
            // Thread.Sleep(1750);

            GameState gameState = new GameState(this);
            if (saveOrLoad == 1)
            {
                Console.WriteLine("Game state saved to a dictionary");
                gameState.SaveGameState(this);
            }

            else if (saveOrLoad == 2)
            {
                gameState.LoadGameState(this);
            }

        }

        public void UpdateProperties(Dictionary<string, object> properties)
        {

            // eg dollars.Quantity = (double) properties["dollars"];
            Console.WriteLine("THIS DOES NOT WORK YET");
            Console.WriteLine("re-download the .exe file later on to see if it's been added");
            Console.WriteLine("For now, you'll be sent back to the main menu in 5 seconds");
            Thread.Sleep(5000);
        } // applying a loaded game state

        public void QuitGame()
        {
            DisplayStuff.DisplayStats(this);
            Console.WriteLine($"You lasted until {_currentDate.Date:dddd, d MMMM, yyyy}");
            Console.WriteLine("\nGoodbye!");
        }

        public void GameFailed()
        {
            Thread.Sleep(1750);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                         YOU FAILED                         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Thread.Sleep(1750);
            QuitGame();
        }

        public void EmployeeTrainingCourse(int numberOfEmployees)
        {
            // to boost the productivity of employees
            Console.WriteLine($"Training {numberOfEmployees} employees...");
            Console.WriteLine($"This course charged you {trainingCourse.Price * numberOfEmployees} in fees");
            dollars.Quantity -= trainingCourse.Price * numberOfEmployees;
            for (int i = 0; i < numberOfEmployees; i++)
            {
                Console.WriteLine(
                    $"Employee {workersList[i].Name} has begun the training course, they'll be back in a week \ud83d\udcaa");
                workersList[i].ReturnToWorkDate = _currentDate.AddDays(7);
                workersList[i].efficiency *= 1.3;
                toSendToTrainingList.Add(workersList[i]);
            }

            foreach (Worker worker in toSendToTrainingList)
            {
                workersList.Remove(worker);
                trainingWorkersList.Add(worker);
            }

            Thread.Sleep(1250);
        } // move to the appropriate own class

        public int GetValidInt(int min, int max)
        {
            if (int.TryParse(Console.ReadLine(), out int validInt))
            {
                if (validInt >= min && validInt <= max)
                {
                    return validInt;
                }

                Console.WriteLine($"No bro enter a number between {min} and {max}");
                return GetValidInt(min, max);
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
                return GetValidDouble(min, max);
            }

            Console.WriteLine($"Please enter a valid decimal number between {min} and {max}");
            return GetValidDouble(min, max);
        }
    }

    class Worker
    {
        public string Type;
        public string Name;
        public double Wage;
        public double Price;
        public double EmployeeIllProbability;
        public DateTime HireDate;
        public double efficiency;
        public double DaysWorked;
        public int DaysUntilRetirement;
        public DateTime RetirementDate;
        public bool IsIll;
        public DateTime ReturnToWorkDate;
        public double Morale;
        public double BaseMorale;

        public Worker(string type, string name, double wage, double price, double employeeIllProbability, double Efficiency, double baseMorale)
        {
            if (type == "bad")
            {
                Type = "bad";
                Name = name;
                Wage = wage * 0.5;
                Price = price * 0.5;
                EmployeeIllProbability = employeeIllProbability * 2;
                efficiency = Efficiency * 0.5;
                DaysUntilRetirement = 30;
                IsIll = false;
                HireDate = DateTime.Today;
                ReturnToWorkDate = DateTime.Today;
                Morale = baseMorale;
                BaseMorale = baseMorale;
            }

            else if (type == "mid")
            {
                Type = "mid";
                Name = name;
                Wage = wage;
                Price = price;
                EmployeeIllProbability = employeeIllProbability;
                efficiency = Efficiency;
                DaysWorked = 0;
                DaysUntilRetirement = 45;
                IsIll = false;
                HireDate = DateTime.Today;
                ReturnToWorkDate = DateTime.Today;
                Morale = baseMorale;
            }

            else if (type == "good")
            {
                Type = "good";
                Name = name;
                Wage = wage * 2;
                Price = price * 2.5;
                EmployeeIllProbability = employeeIllProbability * 0.5;
                efficiency = Efficiency * 2;
                DaysUntilRetirement = 30;
                IsIll = false;
                HireDate = DateTime.Today;
                ReturnToWorkDate = DateTime.Today;
                Morale = baseMorale;
            }
        }

        public static void EmployeeHiringScreen(Program _program)
        {
            Console.Clear();
            Console.WriteLine(
                "  _    _   _                      ______                       _                                       \n | |  | | (_)                    |  ____|                     | |                                      \n | |__| |  _   _ __    ___       | |__     _ __ ___    _ __   | |   ___    _   _    ___    ___   ___   \n |  __  | | | | '__|  / _ \\      |  __|   | '_ ` _ \\  | '_ \\  | |  / _ \\  | | | |  / _ \\  / _ \\ / __|  \n | |  | | | | | |    |  __/      | |____  | | | | | | | |_) | | | | (_) | | |_| | |  __/ |  __/ \\__ \\  \n |_|  |_| |_| |_|     \\___|      |______| |_| |_| |_| | .__/  |_|  \\___/   \\__, |  \\___|  \\___| |___/  \n                                                      | |                   __/ |                      \n                                                      |_|                  |___/                     ");
            Console.WriteLine($"What type of employee do you want to hire?");
            Console.WriteLine("0 - Cancel and return to the menu");
            Console.WriteLine(
                $"1 - Hire a bad employee: Price = ${_program._currentEmployeePrice * 0.5} Wage = ${_program._currentWageRate * 0.5}, Efficiency = 0.5x, Retires in 30 days");
            Console.WriteLine(
                $"2 - Hire a mid employee: Price = ${_program._currentEmployeePrice}, Wage = ${_program._currentWageRate}, Efficiency = 1x, Retires in 45 days");
            Console.WriteLine(
                $"3 - Hire a good employee: Price = ${_program._currentEmployeePrice * 2}, Wage = ${_program._currentWageRate * 2}, Efficiency = 2x, Retires in 30 days");


            int employeeType = _program.GetValidInt(0, 3);

            if (employeeType != 0)
            {
                Console.WriteLine($"Enter how many employees you want to hire?\nYou have {Math.Round(_program.dollars.Quantity, 2)} dollars");

                int employeesToHire = _program.GetValidInt(0, 350);
                switch (employeeType)
                {
                    case 1:
                        if (employeesToHire * _program._currentEmployeePrice * 0.5 >
                            _program.dollars.Quantity)
                        {
                            Console.WriteLine(
                                "You don't have enough dollars to hire that many bad employees");
                        }
                        else
                        {
                            Console.WriteLine(
                                $"You have hired {employeesToHire} more bad employees.\nSay hello to:");

                            HireNewWorker(employeesToHire, "bad", _program);

                            _program.dollars.Quantity -= employeesToHire * _program._currentEmployeePrice * 0.5;
                            Console.WriteLine($"You now have {_program.workersList.Count} total employees");
                            _program._totalEmployeesHired += employeesToHire;
                        }

                        break;
                    case 2:
                        if (employeesToHire * _program._currentEmployeePrice > _program.dollars.Quantity)
                        {
                            Console.WriteLine(
                                "You don't have enough dollars to hire that many mid employees");
                        }
                        else
                        {
                            Console.WriteLine(
                                $"You have hired {employeesToHire} more mid employees.\nSay hello to:");

                            HireNewWorker(employeesToHire, "mid", _program);

                            _program.dollars.Quantity -= employeesToHire * _program._currentEmployeePrice;
                            Console.WriteLine($"You now have {_program.workersList.Count} total employees");
                            _program._totalEmployeesHired += employeesToHire;
                        }

                        break;
                    case 3:
                        if (employeesToHire * _program._currentEmployeePrice * 2 >
                            _program.dollars.Quantity)
                        {
                            Console.WriteLine(
                                "You don't have enough dollars to hire that many good employees");
                        }
                        else
                        {
                            Console.WriteLine(
                                $"You have hired {employeesToHire} more good employees.\nSay hello to:");

                            HireNewWorker(employeesToHire, "good", _program);

                            _program.dollars.Quantity -= employeesToHire * _program._currentEmployeePrice * 2;
                            Console.WriteLine($"You now have {_program.workersList.Count} total employees");
                            _program._totalEmployeesHired += employeesToHire;
                        }

                        break;
                }
            }
        }

        public static void EmployeeFiringScreen(Program _program)
        {
            Console.WriteLine("This feature doesnt do anything yet because I dont care enough about selling employees");
            Console.WriteLine("It's just here to make the menu look more full lol");
            Console.WriteLine("If you want me to make this work properly, message me and I'll get working on it");
            Console.WriteLine("For now, here's the way it works:");
            Console.WriteLine("You can fire employees, but you don't gain anything from it");
            DisplayStuff.DisplayActiveEmployees(_program);
            Console.WriteLine("Enter the number of the employee you want to fire:");
            int employeeToFire = _program.GetValidInt(1, _program.workersList.Count);
            Console.WriteLine($"You have fired {_program.workersList[employeeToFire - 1].Name}");
            _program.workersList.RemoveAt(employeeToFire - 1);
        }

        public static void HireNewWorker(int numberOfWorkers, string type, Program _program)
        {

            Random _random = new Random();

            for (int i = 0; i < numberOfWorkers; i++)
            {
                double efficiency = 15;

                if (_program._possibleNames.Count > 1)
                {
                    int randomName = _random.Next(0, _program._possibleNames.Count);

                    // making 'levels' of efficiency based on the number of employees
                    // this is to make the game harder over time as the player hires more employees
                    if (_program.workersList.Count > 0)
                    {
                        efficiency = _random.Next(70, 130);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 3)
                    {
                        efficiency = _random.Next(65, 125);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 6)
                    {
                        efficiency = _random.Next(65, 125);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 9)
                    {
                        efficiency = _random.Next(60, 120);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 12)
                    {
                        efficiency = _random.Next(55, 115);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 15)
                    {
                        efficiency = _random.Next(50, 110);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 18)
                    {
                        efficiency = _random.Next(45, 105);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 21)
                    {
                        efficiency = _random.Next(40, 100);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 24)
                    {
                        efficiency = _random.Next(35, 95);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 27)
                    {
                        efficiency = _random.Next(30, 90);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 30)
                    {
                        efficiency = _random.Next(25, 85);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 33)
                    {
                        efficiency = _random.Next(20, 80);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 36)
                    {
                        efficiency = _random.Next(15, 75);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 39)
                    {
                        efficiency = _random.Next(10, 70);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 42)
                    {
                        efficiency = _random.Next(5, 65);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 45)
                    {
                        efficiency = _random.Next(0, 60);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 28)
                    {
                        efficiency = _random.Next(0, 55);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 51)
                    {
                        efficiency = _random.Next(0, 50);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 54)
                    {
                        efficiency = _random.Next(0, 45);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 57)
                    {
                        efficiency = _random.Next(0, 40);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 60)
                    {
                        efficiency = _random.Next(0, 35);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 63)
                    {
                        efficiency = _random.Next(0, 30);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 66)
                    {
                        efficiency = _random.Next(0, 25);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 69)
                    {
                        efficiency = _random.Next(0, 20);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 72)
                    {
                        efficiency = _random.Next(0, 15);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 75)
                    {
                        efficiency = _random.Next(0, 10);
                        efficiency /= 100;
                    }
                    else if (_program.workersList.Count > 78)
                    {
                        efficiency = _random.Next(1, 5);
                        efficiency /= 100;
                    }

                    double employeePrice = 0;
                    double baseMorale = 1;

                    if (type == "bad")
                    {
                        employeePrice = 50;
                    }

                    if (type == "mid")
                    {
                        employeePrice = 100;
                    }

                    if (type == "good")
                    {
                        employeePrice = 200;
                    }

                    Worker newWorker = new Worker(type, _program._possibleNames[randomName], _program._currentWageRate, employeePrice, _program._currentEmployeeIllProbability, efficiency, baseMorale);
                    _program.workersList.Add(newWorker);
                    _program.UsedNames.Add(newWorker.Name);
                    _program._possibleNames.Remove(newWorker.Name);
                    Console.WriteLine($"{newWorker.Name}, Efficiency {Math.Round(newWorker.efficiency, 2)}\ud83e\uddcd\u200d\u2642\ufe0f");

                    // updating bribe price
                    _program.bribe.Price = _program._currentWageRate * _program.workersList.Count * 2;
                }

                else
                {
                    Console.WriteLine("You've hired all 307/307 available employees and so you've run out of names to give to your employees \ud83d\ude2d");
                    Console.WriteLine("Wait for some to retire");
                    break;
                }
            }

        }

        public static void EmployeeMoraleBoostingScreen(Program _program)
        {
            Console.WriteLine("You've called the employee morale boosting screen");
            Console.WriteLine($"Your current average employee morale is {_program._averageEmployeeMorale}");
            Console.WriteLine("Choose an option to improve employee morale:");
            Console.WriteLine("0 - Cancel and return to the menu");
            Console.WriteLine("1 - Increase wage by 20% --> 20% morale increase");
            Console.WriteLine($"2 - Give a bonus of ${_program.bonus.Price} to each employee --> 30% morale increase");
            Console.WriteLine($"3 - Offer a retirement package for {_program.retirementPackage} for all employees when they retire --> 25% morale increase");

            int userInput = _program.GetValidInt(0, 3);
            switch (userInput)
            {
                case 0:
                    Console.WriteLine("Returning to the menu...");
                    Thread.Sleep(500);
                    break;

                case 1:
                    Console.WriteLine("You have chosen to increase the wage of all employees by 20%");
                    Console.WriteLine("Their morales will increase by 50% as a result \ud83d\ude0a");
                    _program._currentWageRate *= 1.2;
                    foreach (Worker worker in _program.workersList)
                    {
                        worker.Wage *= 1.2;
                        worker.Morale += 1.5;
                    }

                    break;

                case 2:
                    if (_program.dollars.Quantity > _program.workersList.Count * _program.bonus.Price)
                    {
                        Console.WriteLine($"You have chosen to give a bonus of {_program.bonus.Price} to each employee");
                        foreach (Worker worker in _program.workersList)
                        {
                            worker.Morale *= _program.retirementPackage.MoraleMultiplier;
                        }

                        _program.dollars.Quantity -= _program.workersList.Count * _program.bonus.Price;
                        break;
                    }

                    Console.WriteLine($"You can't afford to pay for a bonus of ${_program.bonus.Price} per employee \ud83d\ude45\u200d\u2642\ufe0f");
                    break;

                case 3:
                    if (_program.dollars.Quantity > _program.retirementPackage.Price)
                    {
                        Console.WriteLine($"You have chosen to offer a retirement package for {_program.retirementPackage} for all employees when they retire");
                        foreach (Worker worker in _program.workersList)
                        {
                            worker.Morale *= _program.retirementPackage.MoraleMultiplier;
                        }

                        _program.dollars.Quantity -= _program.retirementPackage.Price;
                        break;
                    }

                    Console.WriteLine($"You can't afford to pay for a retirement package of ${_program.retirementPackage.Price} \ud83d\ude45\u200d\u2642\ufe0f");
                    break;
            }
        }
    }

    class Specialist
    {
        public int DaysLeft;
        public double ProbabilityMultiplier;
        public string ResourceName;
        public double Cost;
        public double TotalDaysWorked;

        public Specialist(int daysLeft, double probabilityMultiplier, string resourceName, double cost)

        {
            DaysLeft = daysLeft;
            ProbabilityMultiplier = probabilityMultiplier;
            ResourceName = resourceName;
            Cost = cost;
        }

        public static void SpecialistHiringScreen(Program _program)
        {

            if (_program.dollars.Quantity < 1000)
            {
                Console.WriteLine("You don't have enough dollars to hire a specialist");
            }
            else
            {
                Console.WriteLine("You've called the specialist hiring screen");
                Console.WriteLine("Each specialist doubles the probability of finding a resource for 15 days and costs $1000");
                Console.WriteLine("Choose an option:");
                Console.WriteLine("0 - Cancel and return to the menu");
                Console.WriteLine("1 - Hire a coal specialist");
                Console.WriteLine("2 - Hire a stone specialist");
                Console.WriteLine("3 - Hire an iron specialist");
                Console.WriteLine("4 - Hire a gold specialist");
                Console.WriteLine("5 - Hire a diamond specialist");

                int managerChoice = _program.GetValidInt(0, 5);
                Specialist coalManager = new Specialist(15, 2, "coal", 1000);
                Specialist stoneManager = new Specialist(15, 2, "stone", 1000);
                Specialist ironManager = new Specialist(15, 2, "iron", 1000);
                Specialist goldManager = new Specialist(15, 2, "gold", 1000);
                Specialist diamondManager = new Specialist(15, 2, "diamond", 1000);

                bool hire = true;

                switch (managerChoice)
                {
                    case 0:
                        Console.WriteLine("Returning to main menu screen");
                        break;
                    case 1:
                        foreach (Specialist specialist in _program.managersList)
                        {
                            if (specialist.ResourceName == "coal")
                            {
                                Console.WriteLine("You already have a coal specialist");
                                hire = false;
                            }
                        }

                        if (hire)
                        {
                            _program.dollars.Quantity -= coalManager.Cost;
                            _program.coal.Probability *= coalManager.ProbabilityMultiplier;
                            _program.managersList.Add(coalManager);
                            Console.WriteLine("You have hired a coal specialist");
                        }

                        break;
                    case 2:
                        foreach (Specialist specialist in _program.managersList)
                        {
                            if (specialist.ResourceName == "stone")
                            {
                                Console.WriteLine("You already have a stone specialist");
                                hire = false;
                            }
                        }

                        if (hire)
                        {
                            _program.dollars.Quantity -= stoneManager.Cost;
                            _program.stone.Probability *= stoneManager.ProbabilityMultiplier;
                            _program.managersList.Add(stoneManager);
                            Console.WriteLine("You have hired a stone specialist");
                        }

                        break;
                    case 3:
                        foreach (Specialist specialist in _program.managersList)
                        {
                            if (specialist.ResourceName == "iron")
                            {
                                Console.WriteLine("You already have an iron specialist");
                                hire = false;
                            }
                        }

                        if (hire)
                        {
                            _program.dollars.Quantity -= ironManager.Cost;
                            _program.iron.Probability *= ironManager.ProbabilityMultiplier;
                            _program.managersList.Add(ironManager);
                            Console.WriteLine("You have hired an iron specialist");
                        }

                        break;
                    case 4:
                        foreach (Specialist specialist in _program.managersList)
                        {
                            if (specialist.ResourceName == "gold")
                            {
                                Console.WriteLine("You already have a gold specialist");
                                hire = false;
                            }
                        }

                        if (hire)
                        {
                            _program.dollars.Quantity -= goldManager.Cost;
                            _program.gold.Probability *= goldManager.ProbabilityMultiplier;
                            _program.managersList.Add(goldManager);
                            Console.WriteLine("You have hired a gold specialist");
                        }

                        break;
                    case 5:
                        foreach (Specialist specialist in _program.managersList)
                        {
                            if (specialist.ResourceName == "diamond")
                            {
                                Console.WriteLine("You already have a diamond specialist");
                                hire = false;
                            }
                        }

                        if (hire)
                        {
                            _program.dollars.Quantity -= diamondManager.Cost;
                            _program.diamond.Probability *= diamondManager.ProbabilityMultiplier;
                            _program.managersList.Add(diamondManager);
                            Console.WriteLine("You have hired a diamond specialist");
                        }

                        break;
                }
            }
        }
    }

    class Trade
    {
        public static List<DateTime> datesOfTradesMade = new();
        public double Ratio;
        public Resource FromResource;
        public Resource ToResource;

        public Trade(double ratio, Resource fromResource, Resource toResource)
        {
            Ratio = ratio;
            FromResource = fromResource;
            ToResource = toResource;
        }

        public static void MakeTrade(double ratio, Resource fromResource, Resource toResource,
            DateTime _currentDate)
        {
            if (!datesOfTradesMade.Contains(_currentDate.Date))
            {
                if (fromResource.Quantity < ratio)
                {
                    Console.WriteLine("\u274c You can't afford to make this trade brokie \u274c");
                    return;
                }

                fromResource.Quantity -= ratio;
                toResource.Quantity += 1;

                Console.WriteLine($"\u2705 Trade Complete! You traded {Math.Round(ratio, 2)}kg of {fromResource.ResourceName.ToLower()} for 1kg of {toResource.ResourceName.ToLower()} \u2705");
                datesOfTradesMade.Add(DateTime.Today);
                return;
            }

            Console.WriteLine("You've already made a trade today, try again later \ud83d\udc4b ");
        }

        public static void GoToTrader(Program program)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(
                "             _______                      _               \n            |__   __|                    | |              \n               | |     _ __    __ _    __| |   ___   _ __ \n               | |    | '__|  / _` |  / _` |  / _ \\ | '__|\n               | |    | |    | (_| | | (_| | |  __/ | |   \n               |_|    |_|     \\__,_|  \\__,_|  \\___| |_|");
            Console.ResetColor();

            DisplayStuff.DisplayResources(program);

            Console.WriteLine("This is where you can convert between resources!");
            Console.WriteLine("Here are the options for today:\n");
            Console.WriteLine("0 - Cancel and return");
            Console.WriteLine($"1 - Convert {Math.Round(program.coalToStone.Ratio, 2)} coal --> stone    |  6 - Convert {Math.Round(program.stoneToGold.Ratio, 2)} stone --> gold");
            Console.WriteLine($"2 - Convert {Math.Round(program.coalToIron.Ratio, 2)} coal --> iron       |  7 - Convert {Math.Round(program.stoneToDiamond.Ratio, 2)} stone --> diamond");
            Console.WriteLine($"3 - Convert {Math.Round(program.coalToGold.Ratio, 2)} coal --> gold      |  8 - Convert {Math.Round(program.ironToGold.Ratio, 2)} iron --> gold");
            Console.WriteLine($"4 - Convert {Math.Round(program.coalToDiamond.Ratio, 2)} coal --> diamond   |  9 - Convert {Math.Round(program.ironToDiamond.Ratio, 2)} iron --> diamond");
            Console.WriteLine($"5 - Convert {Math.Round(program.stoneToIron.Ratio, 2)} stone --> iron    |  10 - Convert {Math.Round(program.goldToDiamond.Ratio, 2)} gold --> diamond");
            Console.WriteLine("Remember, you can only make one trade per day. Choose wisely!\n");
            Console.WriteLine("Enter the number of the trade you want to make:");

            int userTrade = program.GetValidInt(0, 10);

            switch (userTrade)
            {
                case 0:
                    break;
                case 1:
                    MakeTrade(program.coalToStone.Ratio, program.coal, program.stone, program._currentDate);
                    break;
                case 2:
                    MakeTrade(program.coalToIron.Ratio, program.coal, program.iron, program._currentDate);
                    break;
                case 3:
                    MakeTrade(program.coalToGold.Ratio, program.coal, program.gold, program._currentDate);
                    break;
                case 4:
                    MakeTrade(program.coalToDiamond.Ratio, program.coal, program.diamond, program._currentDate);
                    break;
                case 5:
                    MakeTrade(program.stoneToIron.Ratio, program.stone, program.iron, program._currentDate);
                    break;
                case 6:
                    MakeTrade(program.stoneToGold.Ratio, program.stone, program.gold, program._currentDate);
                    break;
                case 7:
                    MakeTrade(program.stoneToDiamond.Ratio, program.stone, program.diamond, program._currentDate);
                    break;
                case 8:
                    MakeTrade(program.ironToGold.Ratio, program.iron, program.gold, program._currentDate);
                    break;
                case 9:
                    MakeTrade(program.ironToDiamond.Ratio, program.iron, program.diamond, program._currentDate);
                    break;
                case 10:
                    MakeTrade(program.goldToDiamond.Ratio, program.gold, program.diamond, program._currentDate);
                    break;
            }

            DisplayStuff.DisplayResources(program);
            Console.WriteLine("Thanks for coming to the trader!\nCome later for updated rates!");
        }

    } // you're allowed multiple trades per day

    class Achievements
    {
        
        public int AchievementNumber;
        public bool AchievementUnlocked = false;
        public string AchievementDescription;

        public Achievements(int achievementNumber, string achievementDescription)
        {
            AchievementNumber = achievementNumber;
            AchievementUnlocked = false;
            AchievementDescription = achievementDescription;
        }
    }

    class MiningOperation
    {
        public void Dig(int daysToDig, Program _program, DayToDayOperations _DayToDayOperations, List<Achievements> achievements)
        {

            bool multipleDayDig = daysToDig > 1;

            // for multiple day dig outputs in one go
            double newCoal = 0;
            double newStone = 0;
            double newIron = 0;
            double newGold = 0;
            double newDiamond = 0;

            for (int days = 0; days < daysToDig; days++)
            {
                if (_program.workersList.Count == 0)
                {
                    Console.WriteLine("__________________________________________________________________________");
                    Console.WriteLine("\u2b55 You have no employees to dig for you \u2b55\nHire some employees first");
                    Thread.Sleep(2000);
                    return;
                }

                if (_program.CheckIfInDebt() != "bankrupt")
                {
                    if (_program.CheckIfInDebt() != "true")
                    {
                        if (!_program.skipDay.skipDayOrNot)
                        {
                            if (!multipleDayDig)
                            {
                                // ASCII art animation for digging
                                string[] shovel = new[]
                                {
                                    "    ______",
                                    "   /      \\",
                                    "  /        \\",
                                    "  |        |",
                                    "  \\________/",
                                    "       ||",
                                    "       ||",
                                    "       ||",
                                    "       ||",
                                    "       ||",
                                    "      ----",

                                };

                                for (int i = 0; i < 15; i++)
                                {
                                    Thread.Sleep(80);
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
                                            for (int k = 0; k < i / 2; k++)
                                            {
                                                spaces += " ";
                                            }

                                            Console.WriteLine(spaces + shovel[j]);
                                        }
                                    }

                                    Console.WriteLine("\nProgress:");
                                    for (int j = 0; j < 15; j++)
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
                                Console.WriteLine("__________________________________________________________________________");
                                Console.WriteLine($"Digging done for the day {_program._currentDate.Date:dddd, dd MMMM, yyyy}");
                                Console.WriteLine("Here are the changes to your resources:");
                                Console.Clear();
                            }

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
                            if (_program._increasedGoldChanceDays != 0)
                            {
                                _program.gold.Probability = 50;
                                _program._increasedGoldChanceDays -= 1;

                                if (!multipleDayDig)
                                {
                                    Console.WriteLine($"You have the Ancient Artefact powerup, you have a 50% chance of finding gold for the next {_program._increasedGoldChanceDays} days");
                                }
                            }

                            else
                            {
                                // restore 15% chance of finding gold
                                _program.gold.Probability = 15;
                            }

                            bool coalFound = randomForCoal < _program.coal.Probability;
                            bool stoneFound = randomForStone < _program.stone.Probability;
                            bool ironFound = randomForIron < _program.iron.Probability;
                            bool goldFound = randomForGold < _program.gold.Probability;
                            bool diamondFound = randomForDiamond < _program.diamond.Probability;
                            bool ancientArtefactFound = randomForAncientArtefact < _program.ancientArtefact.Probability;
                            bool marketMasterFound = randomForMarketMaster < _program.marketMaster.Probability;
                            bool timeMachineFound = randomForTimeMachine < _program.timeMachine.Probability;
                            bool magicTokenFound = randomForMagicToken < _program.magicTokens.Probability;

                            if (ancientArtefactFound)
                            {
                                Console.WriteLine("__________________________________________________________");
                                Console.Write("\ud83c\udffa You found the Ancient Artefact power-up \ud83c\udffa");
                                Console.WriteLine("\nChoose an option:");
                                Console.WriteLine("1 - Get a guaranteed 50% chance of finding gold for the next five days");
                                Console.WriteLine("2 - $200 instantly");
                                Console.WriteLine($"3 - Save for later (max {_program.ancientArtefact.MaxQuantity})");
                                int userInput = _program.GetValidInt(1, 2);

                                switch (userInput)
                                {
                                    case 1:
                                        _program.UsePowerUp(1, 1);
                                        break;
                                    case 2:
                                        _program.UsePowerUp(1, 2);
                                        break;
                                    case 3:
                                        if (_program.ancientArtefact.Quantity < _program.ancientArtefact.MaxQuantity)
                                        {
                                            Console.WriteLine("You have chosen to save the Ancient Artefact for later");
                                            _program.ancientArtefact.Quantity += 1;
                                            break;
                                        }

                                        Console.WriteLine("You have reached the maximum quantity of Ancient Artefacts");
                                        Console.WriteLine("Due to your greed, you have lost this Ancient Artefact powerup");
                                        break;
                                }
                            }

                            if (timeMachineFound)
                            {
                                Console.WriteLine("__________________________________________________________");
                                Console.Write("\u23f3 You found the Time Machine power-up \u23f3");
                                Console.WriteLine("\nChoose an option:");
                                Console.WriteLine(
                                    "1 - Use now --> Gain 5 days' worth of rewards without costing you anything");
                                Console.WriteLine($"2 - Save for later (max {_program.timeMachine.MaxQuantity})");
                                int userInput = _program.GetValidInt(1, 2);

                                switch (userInput)
                                {
                                    case 1:
                                        _program.UsePowerUp(2, 1);
                                        break;
                                    case 2:
                                        if (_program.timeMachine.Quantity < _program.timeMachine.MaxQuantity)
                                        {
                                            Console.WriteLine("You have chosen to save the Time Machine for later");
                                            _program.marketMaster.Quantity += 1;
                                            break;
                                        }

                                        Console.WriteLine("You have reached the maximum quantity of Time Machines");
                                        break;
                                }
                            }

                            if (magicTokenFound && _program.magicTokens.Quantity < _program.magicTokens.MaxQuantity)
                            {
                                if (!multipleDayDig)
                                {
                                    Console.WriteLine("__________________________________________________________");
                                    _program.magicTokens.Quantity += 1;
                                    Console.WriteLine($"You've acquired another magic token. You have {_program.magicTokens.Quantity} magic tokens now");
                                    Console.WriteLine($"Selling price increased by a total of {_program.magicTokens.Quantity * 20}%");
                                }

                                _program.coal.Price += _program.coal.InitialPrice * 0.2;
                                _program.stone.Price += _program.stone.InitialPrice * 0.2;
                                _program.iron.Price += _program.iron.InitialPrice * 0.2;
                                _program.gold.Price += _program.gold.InitialPrice * 0.2;
                                _program.diamond.Price += _program.diamond.InitialPrice * 0.2;
                            }

                            if (marketMasterFound)
                            {
                                Console.WriteLine("__________________________________________________________");
                                Console.WriteLine("\ud83e\uddf2 You found the Market Master power up \ud83e\uddf2");
                                Console.WriteLine("Choose an option:");
                                Console.WriteLine("1 - Use now --> Increase the selling price of all resources has increased by 50% for the next 5 days");
                                Console.WriteLine($"2 - Save for later (max {_program.marketMaster.MaxQuantity})");
                                int userInput = _program.GetValidInt(1, 2);

                                switch (userInput)
                                {
                                    case 1:
                                        _program.UsePowerUp(3, 1);
                                        break;
                                    case 2:
                                        if (_program.marketMaster.Quantity < _program.marketMaster.MaxQuantity)
                                        {
                                            Console.WriteLine("You have chosen to save the Market Master for later");
                                            _program.marketMaster.Quantity += 1;
                                            break;
                                        }

                                        Console.WriteLine("You have reached the maximum quantity of Market Master");
                                        break;
                                }
                            }

                            if (coalFound)
                            {
                                double randomResourceQuantityFluctuation = random.Next(80, 120) / 100.0;
                                double newResourcesFound = _program.workersList.Count * _program._averageEmployeeEfficiency * randomResourceQuantityFluctuation;
                                _program.coal.Quantity += newResourcesFound;
                                _program.coal.TotalFound += newResourcesFound;
                                newCoal += newResourcesFound;

                                if (!multipleDayDig)
                                {
                                    Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of coal \ud83e\udea8");
                                }
                            }

                            if (stoneFound)
                            {
                                double randomResourceQuantityFluctuation = random.Next(80, 120) / 100.0;
                                double newResourcesFound = _program.workersList.Count * _program._averageEmployeeEfficiency * randomResourceQuantityFluctuation;
                                _program.stone.Quantity += newResourcesFound;
                                _program.stone.TotalFound += newResourcesFound;
                                newStone += newResourcesFound;

                                if (!multipleDayDig)
                                {
                                    Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of stone \ud83e\udea8");
                                }
                            }

                            if (ironFound)
                            {
                                double randomResourceQuantityFluctuation = random.Next(80, 120) / 100.0;
                                double newResourcesFound = _program.workersList.Count * _program._averageEmployeeEfficiency * randomResourceQuantityFluctuation;
                                _program.iron.Quantity += newResourcesFound;
                                _program.iron.TotalFound += newResourcesFound;
                                newIron += newResourcesFound;

                                if (!multipleDayDig)
                                {
                                    Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of iron \ud83e\uddbe ");
                                }
                            }

                            if (goldFound)
                            {
                                double randomResourceQuantityFluctuation = random.Next(80, 120) / 100.0;
                                double newResourcesFound = _program.workersList.Count * _program._averageEmployeeEfficiency * randomResourceQuantityFluctuation;
                                _program.gold.Quantity += newResourcesFound;
                                _program.gold.TotalFound += newResourcesFound;
                                newGold += newResourcesFound;

                                if (!multipleDayDig)
                                {
                                    Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of gold \ud83d\udc51");
                                }
                            }

                            if (diamondFound)
                            {
                                double randomResourceQuantityFluctuation = random.Next(80, 120) / 100.0;
                                double newResourcesFound = _program.workersList.Count * _program._averageEmployeeEfficiency * randomResourceQuantityFluctuation;
                                _program.diamond.Quantity += newResourcesFound;
                                _program.diamond.TotalFound += newResourcesFound;
                                newDiamond += newResourcesFound;

                                if (!multipleDayDig)
                                {
                                    Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of diamond \ud83d\udc8e");
                                }
                            }

                            if (!coalFound && !stoneFound && !ironFound && !goldFound && !diamondFound &&
                                !ancientArtefactFound && !timeMachineFound && !magicTokenFound && !marketMasterFound && !multipleDayDig)
                            {
                                Console.WriteLine("\ud83d\udeab You found nothing today \ud83d\udeab");
                            }

                        }

                        // calendar/weather etc effects 
                        if (!multipleDayDig)
                        {
                            Console.WriteLine("__________________________________________________________________________");
                            Console.WriteLine("Here are the current active effects affecting your game:");

                            if (_program._noWageDaysLeft != 0)
                            {
                                Console.WriteLine(
                                    $"You don't have to pay wages today, or for the next {_program._noWageDaysLeft} days");
                                _program._noWageDaysLeft -= 1;
                            }

                            else
                            {
                                double totalWages = 0;
                                foreach (Worker worker in _program.workersList)
                                {
                                    totalWages += worker.Wage;
                                }

                                _program.dollars.Quantity -= totalWages;

                                Console.WriteLine($"\ud83d\udcb0 Your {_program.workersList.Count} employees charged a wage of ${Math.Round(totalWages, 2)} today \ud83d\udcb0");
                            }


                            if (_program._marketMasterDaysLeft == 1)
                            {
                                _program._marketMasterDaysLeft = 0;
                                Console.WriteLine("Your Market Master powerup is no longer active");
                            }

                            else if (_program._marketMasterDaysLeft > 1)
                            {
                                Console.WriteLine(
                                    $"{_program._marketMasterDaysLeft} days left of the Market Master powerup");
                                _program._marketMasterDaysLeft -= 1;
                            }

                        }

                        if (multipleDayDig)
                        {
                            if (_program._noWageDaysLeft != 0)
                            {
                                _program._noWageDaysLeft -= 1;
                            }

                            else
                            {
                                double totalWages = 0;
                                foreach (Worker worker in _program.workersList)
                                {
                                    totalWages += worker.Wage;
                                }

                                _program.dollars.Quantity -= totalWages;

                            }

                            if (_program._marketMasterDaysLeft == 1)
                            {
                                _program._marketMasterDaysLeft = 0;
                            }

                            else if (_program._marketMasterDaysLeft > 1)
                            {
                                _program._marketMasterDaysLeft -= 1;
                            }
                        }

                        _program._currentDate = _program._currentDate.AddDays(1);

                        _program._totalDaysDug += 1;

                        // post-digging effects
                        _DayToDayOperations.CalendarEffects(_program, multipleDayDig);
                        _DayToDayOperations.WeatherEffects(_program, multipleDayDig);
                        _DayToDayOperations.DealWithEmployees(_program, multipleDayDig);
                        _DayToDayOperations.DealWithManagers(_program, multipleDayDig);
                        _DayToDayOperations.FluctuatePrices(_program, multipleDayDig);
                        _DayToDayOperations.CheckRealEstate(_program, multipleDayDig);
                        _DayToDayOperations.MineEmptinessUpdate(_program, multipleDayDig);
                        _DayToDayOperations.CheckInventorySize(_program, multipleDayDig);
                    }
                }
                else
                {
                    _program.QuitGame();
                }
            }

            if (multipleDayDig)
            {
                Console.WriteLine("__________________________________________________________________________");
                Console.WriteLine($"Here are the changes to your resources after {daysToDig} days of digging:");
                Console.WriteLine($"You found {Math.Round(newCoal, 2)}kg of coal");
                Console.WriteLine($"You found {Math.Round(newStone, 2)}kg of stone");
                Console.WriteLine($"You found {Math.Round(newIron, 2)}kg of iron");
                Console.WriteLine($"You found {Math.Round(newGold, 2)}kg of gold");
                Console.WriteLine($"You found {Math.Round(newDiamond, 2)}kg of diamond");
            }

            _DayToDayOperations.CheckAchievements(achievements, _program);

            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine($"Here are your updated resources:");
            DisplayStuff.DisplayResources(_program);

            _program.skipDay.skipDayOrNot = false;
        }
    }

    class MarketOperation
    {
        public void GoToMarket(Program _program)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(
                "\n __       __                      __                   __     \n|  \\     /  \\                    |  \\                 |  \\    \n| $$\\   /  $$  ______    ______  | $$   __   ______  _| $$_   \n| $$$\\ /  $$$ |      \\  /      \\ | $$  /  \\ /      \\|   $$ \\  \n| $$$$\\  $$$$  \\$$$$$$\\|  $$$$$$\\| $$_/  $$|  $$$$$$\\\\$$$$$$  \n| $$\\$$ $$ $$ /      $$| $$   \\$$| $$   $$ | $$    $$ | $$ __ \n| $$ \\$$$| $$|  $$$$$$$| $$      | $$$$$$\\ | $$$$$$$$ | $$|  \\\n| $$  \\$ | $$ \\$$    $$| $$      | $$  \\$$\\ \\$$     \\  \\$$  $$\n \\$$      \\$$  \\$$$$$$$ \\$$       \\$$   \\$$  \\$$$$$$$   \\$$$$ \n\n");
            Console.ResetColor();

            DisplayStuff.DisplayResources(_program);

            Console.WriteLine($"Here are the rates for {_program._currentDate:dddd dd MMMM, yyyy}:");

            Console.WriteLine("__________________________________________________________");
            Console.WriteLine($"| Coal: ${Math.Round(_program.coal.Price, 2)} per kg           | Stone: {Math.Round(_program.stone.Price, 2)} per kg");
            Console.WriteLine($"| Iron: ${Math.Round(_program.iron.Price, 2)} per kg          | Gold: ${Math.Round(_program.gold.Price, 2)} per kg");
            Console.WriteLine($"| Diamond: ${Math.Round(_program.diamond.Price, 2)} per kg      | Employees: {Math.Round(_program._currentEmployeePrice, 2)} per employee");
            Console.WriteLine("__________________________________________________________");


            int marketOption;
            do
            {
                Console.WriteLine("\nHere is the menu for the market:");
                Console.WriteLine("0 - Exit market");
                Console.WriteLine("1 - Sell a specific resource");
                Console.WriteLine("2 - Sell all resources for dollars");
                Console.WriteLine("3 - Hire More Employees");
                Console.WriteLine($"4 - Double Inventory Size (Costs {_program._upgradeInventoryCost})");
                Console.WriteLine("\nChoose Option:");
                marketOption = _program.GetValidInt(0, 4);

                switch (marketOption)
                {
                    case 0:
                        Console.WriteLine("Thanks for coming to the market!");
                        break;

                    case 1:
                        Console.WriteLine(
                            "You've chosen to sell a specific resource.\nWhich resource do you want to sell?");
                        // sht here bro add in the emojis
                        Console.WriteLine("1 - Coal\n2 - Stone\n3 - Iron\n4 - Gold\n5 - Diamond");
                        int sellChoice = _program.GetValidInt(1, 5);

                        switch (sellChoice)
                        {
                            case 1:
                                Console.WriteLine("Your have chosen to sell coal for dollars");
                                Console.WriteLine(
                                    $"How much coal do you want to sell?\nYou have {_program.coal.Quantity}kg of coal");
                                double coalToSell = _program.GetValidDouble(0, 100000000000);
                                if (coalToSell > _program.coal.Quantity)
                                {
                                    Console.WriteLine("You don't have enough coal to sell that much");
                                }
                                else
                                {
                                    _program.coal.Quantity -= coalToSell;
                                    _program.dollars.Quantity += coalToSell * _program.coal.Price;
                                    _program._totalDollarsEarned += coalToSell * _program.coal.Price;
                                }

                                Console.WriteLine("Here are your updated resources:");
                                DisplayStuff.DisplayResources(_program);
                                break;

                            case 2:
                                Console.WriteLine("Your have chosen to sell stone for dollars");
                                Console.WriteLine($"How much stone do you want to sell?\nYou have {_program.stone.Quantity}kg of stone");
                                double stoneToSell = _program.GetValidDouble(0, 100000000000);
                                if (stoneToSell > _program.stone.Quantity)
                                {
                                    Console.WriteLine("You don't have enough stone to sell that much");
                                }
                                else
                                {
                                    _program.stone.Quantity -= stoneToSell;
                                    _program.dollars.Quantity += stoneToSell * _program.stone.Price;
                                    _program._totalDollarsEarned += stoneToSell * _program.stone.Price;
                                }

                                Console.WriteLine("Here are your updated resources:");
                                DisplayStuff.DisplayResources(_program);
                                break;
                            case 3:
                                Console.WriteLine("Your have chosen to sell iron for dollars");
                                Console.WriteLine($"How much iron do you want to sell?\nYou have {_program.iron.Quantity}kg of iron");
                                double ironToSell = _program.GetValidDouble(0, 100000000000);
                                if (ironToSell > _program.iron.Quantity)
                                {
                                    Console.WriteLine("You don't have enough iron to sell that much");
                                }
                                else
                                {
                                    _program.iron.Quantity -= ironToSell;
                                    _program.dollars.Quantity += ironToSell * _program.iron.Price;
                                    _program._totalDollarsEarned += ironToSell * _program.iron.Price;
                                }

                                Console.WriteLine("Here are your updated resources:");
                                DisplayStuff.DisplayResources(_program);
                                break;
                            case 4:
                                Console.WriteLine("Your have chosen to sell gold for dollars");
                                Console.WriteLine($"How much gold do you want to sell?\nYou have {_program.gold.Quantity}kg of gold");
                                double goldToSell = _program.GetValidDouble(0, 100000000000);
                                if (goldToSell > _program.gold.Quantity)
                                {
                                    Console.WriteLine("You don't have enough gold to sell that much");
                                }
                                else
                                {
                                    _program.gold.Quantity -= goldToSell;
                                    _program.dollars.Quantity += goldToSell * _program.gold.Price;
                                    _program._totalDollarsEarned += goldToSell * _program.gold.Price;
                                }

                                Console.WriteLine("Here are your updated resources:");
                                DisplayStuff.DisplayResources(_program);
                                break;
                            case 5:
                                Console.WriteLine("Your have chosen to sell diamond for dollars");
                                Console.WriteLine(
                                    $"How much diamond do you want to sell?\nYou have {_program.diamond.Quantity}kg of diamond");
                                double diamondToSell = _program.GetValidDouble(0, 100000000000);
                                if (diamondToSell > _program.diamond.Quantity)
                                {
                                    Console.WriteLine("You don't have enough diamond to sell that much");
                                }
                                else
                                {
                                    _program.diamond.Quantity -= diamondToSell;
                                    _program.dollars.Quantity += diamondToSell * _program.diamond.Price;
                                    _program._totalDollarsEarned += diamondToSell * _program.diamond.Price;
                                }

                                Console.WriteLine("Here are your updated resources:");
                                DisplayStuff.DisplayResources(_program);
                                break;
                        }

                        break;

                    case 2:
                        Console.WriteLine(
                            "\ud83e\udd11 Selling all your resources for dollars \ud83e\udd11 ");

                        _program.dollars.Quantity +=
                            _program.coal.Quantity * _program.coal.Price +
                            _program.stone.Quantity * _program.stone.Price +
                            _program.iron.Quantity * _program.iron.Price +
                            _program.gold.Quantity * _program.gold.Price +
                            _program.diamond.Quantity * _program.diamond.Price;

                        _program._totalDollarsEarned +=
                            _program.coal.Quantity * _program.coal.Price +
                            _program.stone.Quantity * _program.stone.Price +
                            _program.iron.Quantity * _program.iron.Price +
                            _program.gold.Quantity * _program.gold.Price +
                            _program.diamond.Quantity * _program.diamond.Price;

                        _program.coal.Quantity = 0;
                        _program.stone.Quantity = 0;
                        _program.iron.Quantity = 0;
                        _program.gold.Quantity = 0;
                        _program.diamond.Quantity = 0;

                        Console.WriteLine("Here are your updated resources:");
                        DisplayStuff.DisplayResources(_program);
                        break;

                    case 3:
                        Worker.EmployeeHiringScreen(_program);
                        break;

                    case 4:

                        Console.WriteLine($"You have been charged ${_program._upgradeInventoryCost} for doubling the inventory size");
                        Console.WriteLine("You have doubled the inventory size of all resources");

                        _program._upgradeInventoryCost *= 2;

                        foreach (Resource resource in _program.resourcesList)
                        {
                            resource.MaxQuantity *= 2;
                        }

                        break;
                }
            } while (marketOption != 0);
        }
    }

    class DayToDayOperations
    {

        public void WeatherEffects(Program _program, bool multipleDaysOrNot)
        {
            foreach (WeatherEffectsClass weatherEffect in _program.ActiveWeatherEffectsList)
            {
                weatherEffect.DaysLeft -= 1;
                if (!multipleDaysOrNot && weatherEffect.DaysLeft != 0)
                {
                    Console.WriteLine($"{weatherEffect.DaysLeft} days left of {weatherEffect.Name}");
                }
            }

            WeatherEffectsClass Rain = new WeatherEffectsClass("rain", 0, 30, 0.7, 3);
            WeatherEffectsClass Hurricane = new WeatherEffectsClass("hurricane", 0, 6, 0.4, 5);
            WeatherEffectsClass BeautifulSky = new WeatherEffectsClass("beautiful sky", 0, 30, 1.2, 3);
            WeatherEffectsClass Earthquake = new WeatherEffectsClass("earthquake", 0, 5, 1.5, 3);

            Random random = new Random();

            // rain or hurricane reducing efficiency, beautifulSky increasing efficiency

            if (_program.ActiveWeatherEffectsList.Count == 0)
            {
                // 5% chance a hurricane that reduces the probability of finding resources by 50% for the next 5 days
                if (random.Next(0, 100) < Hurricane.Probability)
                {
                    if (!multipleDaysOrNot)
                    {
                        Console.WriteLine("__________________________________________________________________________");
                        Console.WriteLine("\ud83c\udf00 A hurricane is coming, efficiency is now 60% less the next four days \ud83c\udf00");
                        Console.WriteLine("They also lose 20% of their morale because nobody likes working when its a hurricane \ud83d\ude2d");
                    }

                    foreach (Worker worker in _program.workersList)
                    {
                        worker.efficiency *= Hurricane.EfficiencyMultiplier;
                        worker.Morale /= 1.2;
                    }

                    Hurricane.DaysLeft = 4;
                    _program.ActiveWeatherEffectsList.Add(Hurricane);
                }

                // beautiful sky increasing efficiency
                else if (random.Next(0, 100) < BeautifulSky.Probability)
                {
                    if (!multipleDaysOrNot)
                    {
                        Console.WriteLine("__________________________________________________________________________");
                        Console.WriteLine(
                            "\ud83c\udfd6\ufe0f The weather is beautiful today; your employees are 20% more efficient for three days \ud83c\udfd6\ufe0f");
                        Console.WriteLine("Their morale also increased by 10% because everybody likes working when its sunny \ud83d\ude04");
                    }

                    foreach (Worker worker in _program.workersList)
                    {
                        worker.efficiency *= BeautifulSky.EfficiencyMultiplier;
                        worker.Morale *= 1.1;
                    }

                    BeautifulSky.DaysLeft = 3;
                    _program.ActiveWeatherEffectsList.Add(BeautifulSky);
                }

                // rain reducing efficiency
                else if (random.Next(0, 100) < Rain.Probability)
                {
                    if (!multipleDaysOrNot)
                    {
                        Console.WriteLine("__________________________________________________________________________");
                        Console.WriteLine(
                            "\ud83c\udf27\ufe0f Due to torrential rain, your employees are 30% less efficient for the next three days \ud83c\udf27\ufe0f");
                        Console.WriteLine("They also lose 5% of their morale because nobody likes working when its raining \ud83d\ude2d");
                    }

                    foreach (Worker worker in _program.workersList)
                    {
                        worker.efficiency *= Rain.EfficiencyMultiplier;
                        worker.Morale /= 1.05;
                    }

                    Rain.DaysLeft = 3;
                    _program.ActiveWeatherEffectsList.Add(Rain);
                }

                // earthquake increasing efficiency but killing workers and reducing morale
                else if (random.Next(0, 100) < Earthquake.Probability)
                {
                    // earthquake animation
                    string[] earthquake = new[]
                    {
                        "______                  _     _                               _           ",
                        "|  ____|                | |   | |                             | |          ",
                        "| |__      __ _   _ __  | |_  | |__     __ _   _   _    __ _  | | __   ___ ",
                        "|  __|    / _` | | '__| | __| | '_ \\   / _` | | | | |  / _` | | |/ /  / _ \\",
                        "| |____  | (_| | | |    | |_  | | | | | (_| | | |_| | | (_| | |   <  |  __/",
                        "|______|  \\__,_| |_|     \\__| |_| |_|  \\__, |  \\__,_|  \\__,_| |_|\\_\\  \\___|",
                        "                                          | |                              ",
                        "                                          |_|                              "
                    };

                    for (int i = 0; i < 60; i++)
                    {
                        Thread.Sleep(125);
                        Console.Clear();
                        for (int j = 0; j < earthquake.Length; j++)
                        {
                            // Generate a random number between -4 and 4
                            int move = random.Next(-4, 5);

                            // Create a string of spaces based on the random number
                            string spaces = "";

                            for (int k = 0; k < Math.Abs(move); k++)
                            {
                                spaces += " ";
                            }

                            // Depending on the sign of the random number, add the spaces to the left or right of the line
                            string line;

                            if (move < 0)
                            {
                                line = spaces + earthquake[j];
                            }
                            else
                            {
                                line = earthquake[j] + spaces;
                            }

                            Console.WriteLine(line);
                        }
                    }

                    // print the original earthquake array
                    for (int i = 0; i < 9; i++)
                    {
                        Thread.Sleep(125);
                        Console.Clear();
                        for (int j = 0; j < earthquake.Length; j++)
                        {
                            Console.WriteLine(earthquake[j]);
                        }
                    }

                    // earthquake effects
                    if (_program.workersList.Count >= 5)
                    {
                        int employeesDied = (int)Math.Ceiling(_program.workersList.Count * 0.15);
                        Console.WriteLine($"\u26b0\ufe0f The earthquake killed {employeesDied} employees, what a shame! \u26b0\ufe0f");
                        Console.WriteLine("Your other employees got scared, their morale has halved!!");

                        List<Worker> newlyDeadEmployees = new();

                        while (newlyDeadEmployees.Count < employeesDied)
                        {
                            int randomWorkerToDie = random.Next(0, _program.workersList.Count);
                            newlyDeadEmployees.Add(_program.workersList[randomWorkerToDie]);
                        }

                        foreach (Worker worker in newlyDeadEmployees)
                        {
                            _program.workersList.Remove(worker);
                            _program.deadWorkersList.Add(worker);
                        }
                    }

                    else if (_program.workersList.Count < 5)
                    {
                        Console.WriteLine("Because you have less than 5 employees, you got lucky");
                        Console.WriteLine("None of your employees died, but they sure as hell had a fright! their morale has permanently halved");
                    }

                    else
                    {
                        Console.WriteLine("You have no employees, so the earthquake had no effect on your workers");
                        Console.WriteLine("Either that or somehow you've managed to make this bit of code run");
                        Console.WriteLine("This bit of code should only run if you do not have more than 5 workers and if you do not have less than 5 workers");
                        Console.WriteLine("Pls tell me if you see this because this should never occur");
                    }

                    foreach (Worker worker in _program.workersList)
                    {
                        worker.Morale *= 0.5;
                    }

                    Thread.Sleep(1000);
                    Console.WriteLine("\nGlancing over the deaths, the soil has been loosened... This means it is easier to find resources.");
                    Console.WriteLine("The probability of finding each resource has increased by 20%, permanently!");

                    _program.coal.Probability *= 1.2;
                    _program.stone.Probability *= 1.2;
                    _program.iron.Probability *= 1.2;
                    _program.gold.Probability *= 1.2;
                    _program.diamond.Probability *= 1.2;

                    Earthquake.DaysLeft = 3;
                    _program.ActiveWeatherEffectsList.Add(Earthquake);

                    Thread.Sleep(3000);
                }
            }

            // undo weather effects
            List<WeatherEffectsClass> toRemoveWeatherEffectsList = new();

            foreach (WeatherEffectsClass weatherEffect in _program.ActiveWeatherEffectsList)
            {
                if (weatherEffect.DaysLeft == 0)
                {
                    toRemoveWeatherEffectsList.Add(weatherEffect);
                }
            }

            foreach (WeatherEffectsClass finishedWeatherEffect in toRemoveWeatherEffectsList)
            {
                foreach (Worker worker in _program.workersList)
                {
                    worker.efficiency /= finishedWeatherEffect.EfficiencyMultiplier;
                }

                foreach (Worker worker in _program.illWorkersList)
                {
                    worker.efficiency /= finishedWeatherEffect.EfficiencyMultiplier;
                }

                _program.ActiveWeatherEffectsList.Remove(finishedWeatherEffect);

                if (!multipleDaysOrNot)
                {
                    Console.WriteLine($"\ud83c\udf21\ufe0f The {finishedWeatherEffect.Name} has ended \ud83c\udf21\ufe0f - employees are back to their normal efficiency.");
                }
            }
        }

        public void CalendarEffects(Program _program, bool MultipleDaysOrNot)
        {
            // weekend pay, stock market crash, wage increase, employee illness, profit sharing, reduced probability of finding resources

            // every 10 days, probability of finding resources is reduced by 5%
            if (_program._currentDate.Day % 10 == 0)
            {
                Console.WriteLine("__________________________________________________________________________");
                Console.WriteLine(
                    "Congratulations for surviving for another 10 days. The game is now getting even harder...");
                Console.WriteLine(
                    "\ud83d\udc22 The probability of finding resources has reduced by 8% \ud83d\udc22");
                _program.coal.Probability *= 0.92;
                _program.stone.Probability *= 0.92;
                _program.iron.Probability *= 0.92;
                _program.gold.Probability *= 0.92;
                _program.diamond.Probability *= 0.92;
            }

            // +30% pay on weekends - wage is increased on saturday, then reduced again on monday
            if (_program._currentDate.DayOfWeek is DayOfWeek.Saturday)
            {
                if (!MultipleDaysOrNot)
                {
                    Console.WriteLine("__________________________________________________________________________");
                    Console.WriteLine("It's the weekend, your employees want 30% more pay today and tomorrow");
                }

                _program._currentWageRate *= 1.3;
                foreach (Worker workers in _program.workersList)
                {
                    workers.Wage *= 1.3;
                }
            }

            // to undo the effect of weekend pay
            else if (_program._currentDate.DayOfWeek is DayOfWeek.Monday)
            {
                _program._currentWageRate /= 1.3;
                foreach (Worker workers in _program.workersList)
                {
                    workers.Wage /= 1.3;
                }
            }

            // stock market code below
            // to undo the effects of the crash
            if (_program._crashDaysLeft > 1)
            {
                if (!MultipleDaysOrNot)
                {
                    Console.WriteLine("__________________________________________________________________________");
                    Console.WriteLine("\ud83d\udcc8 The stock market has recovered \ud83d\udcc8 ");
                }

                _program.coal.Price *= 2;
                _program.stone.Price *= 2;
                _program.iron.Price *= 2;
                _program.gold.Price *= 2;
                _program.diamond.Price *= 2;
                _program._currentEmployeePrice *= 2;
                _program._crashDaysLeft = 0;
            }

            if (_program._currentDate.Day == _program._crashDate && _program._crashDaysLeft == 0)
            {
                if (!MultipleDaysOrNot)
                {
                    Console.WriteLine("__________________________________________________________________________");
                    Console.WriteLine(
                        "\ud83d\udcc9 The stock market has crashed; prices for everything have plummeted \ud83d\udcc9");
                    Console.WriteLine("(\ud83d\ude09 Now is the best time to hire employees \ud83d\ude09)");
                }

                _program.coal.Price /= 2;
                _program.stone.Price /= 2;
                _program.iron.Price /= 2;
                _program.gold.Price /= 2;
                _program.diamond.Price /= 2;
                _program._currentEmployeePrice /= 2;
                _program._crashDaysLeft = 2;
            }

            // 10% raise on the first of every month (apart from January)
            if (_program._currentDate.Month != 1 && _program._currentDate.Day == 1)
            {
                if (!MultipleDaysOrNot)
                {
                    Console.WriteLine("__________________________________________________________________________");
                    Console.WriteLine(
                        "\ud83e\udd11 It's the first of the month, your employees get a 10% raise for the rest of time \ud83e\udd11");
                }

                _program._currentWageRate *= 1.1;
                foreach (Worker workers in _program.workersList)
                {
                    workers.Wage *= 1.1;
                }
            }

            // 10% profit sharing to each employee on the 15th of every month
            if (_program._currentDate.Day == 15)
            {
                Console.WriteLine("__________________________________________________________________________");
                Console.WriteLine("\ud83d\udcc6 Profit sharing time! \ud83d\udcc6");
                Console.WriteLine("As the manager, you get to choose to share profits or not");
                Console.WriteLine("Choose wisely as keeping profits will decrease morale, and may hurt you in the long run!");
                Console.WriteLine($"Here's how the profit sharing will work for {_program._currentDate:dddd, d MMMM, yyyy}");

                if (_program.workersList.Count < 7)
                {
                    Console.WriteLine("Each employee will 10% of your current $$$ stash, meaning:");
                    Console.WriteLine($"Your {_program.workersList.Count} employees get ${Math.Round(_program.dollars.Quantity * 0.1, 2)} each");
                    Console.WriteLine("1 - Share profits with employees (+50% morale \ud83d\udcc8)\n2 - Keep profits for yourself (-10% morale \ud83d\udcc9)\nEnter your choice:");
                    int profitSharingChoice = _program.GetValidInt(1, 2);

                    double dollarsToLose = _program.dollars.Quantity * 0.1 * _program.workersList.Count;

                    if (profitSharingChoice == 1)
                    {
                        _program.dollars.Quantity -= dollarsToLose;
                        Console.WriteLine($"Your employees have been paid\nYou have lost $ {Math.Round(dollarsToLose, 2)} in the process\nEmployees are happy; morale increased by 50%!");

                        foreach (Worker worker in _program.workersList)
                        {
                            worker.Morale *= 1.5;
                        }
                    }

                    else
                    {
                        Console.WriteLine($"By saving the profits for yourself, you saved {dollarsToLose}, but your employees are not happy; morale decreased by 10%");
                        foreach (Worker worker in _program.workersList)
                        {
                            worker.Morale /= 1.1;
                        }
                    }

                }

                else
                {
                    Console.WriteLine(
                        "Because you have so many employees, 70% of your current $$$ stash is given to them");
                    Console.WriteLine($"This means you'll lose {Math.Round(_program.dollars.Quantity * 0.7, 2)}");
                    _program.dollars.Quantity -= _program.dollars.Quantity * 0.7;
                }
            }
        }

        public void DealWithEmployees(Program _program, bool multipleDaysOrNot)
        {
            // to fix the Nan issue for now, i'll only update the worker properties if you have more than 0 active
            if (_program.workersList.Count > 0)
            {
                // retirements, returning workers, unwell workers, morale, etc.
                Random random = new Random();

                // every 10th day the morale's effect on efficiency is applied
                if (_program._currentDate.DayOfYear % 10 == 0)
                {
                    foreach (Worker worker in _program.workersList)
                    {
                        worker.efficiency *= worker.Morale;
                    }
                }

                // reduce morale by 0.5% for every day worked
                foreach (Worker worker in _program.workersList)
                {
                    worker.Morale *= 0.995;
                }

                // recalculate the average efficiency of the employees
                double totalEfficiency = 0;
                foreach (Worker worker in _program.workersList)
                {
                    totalEfficiency += worker.efficiency;
                }

                _program._averageEmployeeEfficiency = totalEfficiency / _program.workersList.Count;

                // recalculate the average morale of the employees
                double totalMorale = 0;
                foreach (Worker worker in _program.workersList)
                {
                    totalMorale += worker.Morale;
                }

                _program._averageEmployeeMorale = totalMorale / _program.workersList.Count;

                // retiring workers
                for (int i = _program.workersList.Count - 1; i >= 0; i--)
                {
                    Worker worker = _program.workersList[i];
                    if (worker.DaysUntilRetirement != 0)
                    {
                        worker.DaysUntilRetirement -= 1;
                        worker.DaysWorked += 1;
                    }

                    if (worker.DaysUntilRetirement == 0)
                    {
                        _program.UsedNames.Remove(worker.Name);
                        worker.RetirementDate = _program._currentDate.Date;
                        _program.retiredWorkersList.Add(worker);
                        _program.workersList.Remove(worker);
                        if (!multipleDaysOrNot)
                        {
                            Console.WriteLine($"Employee {worker.Name} has retired. Goodbye! \ud83d\udc4b");
                            Console.WriteLine($"You now have {_program.workersList.Count} employees");
                        }
                    }
                }

                // workers that return (due to training course)
                List<Worker> tempList = new();
                foreach (Worker worker in _program.trainingWorkersList)
                {
                    if (worker.ReturnToWorkDate == _program._currentDate.Date)
                    {
                        if (!multipleDaysOrNot)
                        {
                            Console.WriteLine(
                                $"Employee {worker.Name} has returned from their training course \ud83d\udcaa ");
                        }

                        tempList.Add(worker);
                    }
                }

                foreach (Worker worker in tempList)
                {
                    _program.trainingWorkersList.Remove(worker);
                    _program.workersList.Add(worker);
                }

                // unwell workers
                if (_program.workersList.Count > 1)
                {
                    List<Worker> newlyIllWorkers = new List<Worker>();

                    foreach (Worker worker in _program.workersList)
                    {
                        if (random.Next(0, 100) < worker.EmployeeIllProbability)
                        {
                            if (!multipleDaysOrNot)
                            {
                                Console.WriteLine(
                                    $"\ud83e\udd27 Employee {worker.Name} is unwell and doesn't come in today. They'll be back in three days. \ud83e\udd27");
                            }

                            newlyIllWorkers.Add(worker);
                        }
                    }

                    foreach (Worker worker in newlyIllWorkers)
                    {
                        worker.IsIll = true;
                        worker.ReturnToWorkDate = _program._currentDate.AddDays(3);
                        _program.workersList.Remove(worker);
                        _program.illWorkersList.Add(worker);
                    }
                }

                // to undo the effects of unwell workers
                List<Worker> noLongerIllWorkersList = new List<Worker>();

                foreach (Worker worker in _program.illWorkersList)
                {
                    if (worker.IsIll && worker.ReturnToWorkDate.Date == _program._currentDate.Date)
                    {
                        if (!multipleDaysOrNot)
                        {
                            Console.WriteLine(
                                $"Employee {worker.Name} is no longer ill and has returned to work \ud83d\udc4c");
                        }

                        noLongerIllWorkersList.Add(worker);
                    }
                }

                foreach (Worker worker in noLongerIllWorkersList)
                {
                    worker.IsIll = false;
                    _program.illWorkersList.Remove(worker);
                    _program.workersList.Add(worker);
                }

            }
            else
            {
                Console.WriteLine("You have 0 active employees so i didnt do anything in the hopes of solving an error");
            }

        }

        public void DealWithManagers(Program _program, bool multipleDaysOrNot)
        {
            List<Specialist> leftManagersList = new();

            foreach (Specialist manager in _program.managersList)
            {
                if (manager.DaysLeft == 1)
                {
                    manager.DaysLeft = 0;
                    manager.TotalDaysWorked += 1;
                    Console.WriteLine($"Your {manager.ResourceName} specialist has left. You can re-hire them from the menu. Goodbye to them! \ud83d\udc4b");
                    leftManagersList.Add(manager);
                    if (manager.ResourceName == "coal")
                    {
                        _program.coal.Probability /= manager.ProbabilityMultiplier;
                    }
                    else if (manager.ResourceName == "stone")
                    {
                        _program.stone.Probability /= manager.ProbabilityMultiplier;
                    }
                    else if (manager.ResourceName == "iron")
                    {
                        _program.iron.Probability /= manager.ProbabilityMultiplier;
                    }
                    else if (manager.ResourceName == "gold")
                    {
                        _program.gold.Probability /= manager.ProbabilityMultiplier;
                    }
                    else if (manager.ResourceName == "diamond")
                    {
                        _program.diamond.Probability /= manager.ProbabilityMultiplier;
                    }
                }

                else if (manager.DaysLeft != 0)
                {
                    manager.DaysLeft -= 1;
                    manager.TotalDaysWorked += 1;
                }

                if (!multipleDaysOrNot)
                {
                    Console.WriteLine($"Your {manager.ResourceName} manager has {manager.DaysLeft} days left of work");
                }

            }

            foreach (Specialist manager in leftManagersList)
            {
                _program.managersList.Remove(manager);
            }

        }

        public void CheckAchievements(List<Achievements> achievements, Program _program)
        {
            
            if (_program.coal.TotalFound >= 100 && !achievements[0].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 100kg of coal found milestone");
                achievements[0].AchievementUnlocked = true;
            }

            if (_program.coal.TotalFound >= 1000 && !achievements[1].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 1000kg of coal found milestone");
                achievements[1].AchievementUnlocked = true;
            }

            if (_program.coal.TotalFound >= 10000 && !achievements[2].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 10000kg of coal found milestone");
                achievements[2].AchievementUnlocked = true;
            }

            if (_program.stone.TotalFound >= 100 && !achievements[3].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 100kg of stone found milestone");
                achievements[3].AchievementUnlocked = true;
            }

            if (_program.stone.TotalFound >= 1000 && !achievements[4].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 1000kg of stone found milestone");
                achievements[4].AchievementUnlocked = true;
            }

            if (_program.stone.TotalFound >= 10000 && !achievements[5].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 10000kg of stone found milestone");
                achievements[5].AchievementUnlocked = true;
            }

            if (_program.iron.TotalFound >= 75 && !achievements[6].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 75kg of iron found milestone");
                achievements[6].AchievementUnlocked = true;
            }

            if (_program.iron.TotalFound >= 750 && !achievements[7].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 750kg of iron found milestone");
                achievements[7].AchievementUnlocked = true;
            }

            if (_program.iron.TotalFound >= 7500 && !achievements[8].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 7500kg of iron found milestone");
                achievements[8].AchievementUnlocked = true;
            }

            if (_program.gold.TotalFound >= 30 && !achievements[9].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 30kg of gold found milestone");
                achievements[9].AchievementUnlocked = true;
            }

            if (_program.gold.TotalFound >= 300 && !achievements[10].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 300kg of gold found milestone");
                achievements[10].AchievementUnlocked = true;
            }

            if (_program.gold.TotalFound >= 3000 && !achievements[11].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 3000kg of gold found milestone");
                achievements[11].AchievementUnlocked = true;
            }

            if (_program.diamond.TotalFound >= 10 && !achievements[12].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 10kg of diamond found milestone");
                achievements[12].AchievementUnlocked = true;
            }

            if (_program.diamond.TotalFound >= 100 && !achievements[13].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 100kg of diamond found milestone");
                achievements[13].AchievementUnlocked = true;
            }

            if (_program.diamond.TotalFound >= 1000 && !achievements[14].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 1000kg of diamond found milestone");
                achievements[14].AchievementUnlocked = true;
            }

            if (_program._totalDollarsEarned >= 300 && !achievements[15].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: $300 earned milestone");
                achievements[15].AchievementUnlocked = true;
            }

            if (_program._totalDollarsEarned >= 1000 && !achievements[16].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: $1000 earned milestone");
                achievements[16].AchievementUnlocked = true;
            }

            if (_program._totalDollarsEarned >= 10000 && !achievements[17].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: $10000 earned milestone");
                achievements[17].AchievementUnlocked = true;
            }

            if (_program._totalEmployeesHired >= 10 && !achievements[18].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 10 employees hired milestone");
                achievements[18].AchievementUnlocked = true;
            }

            if (_program._totalEmployeesHired >= 100 && !achievements[19].AchievementUnlocked)
            {
                Console.WriteLine("You've unlocked an achievement: 100 employees hired milestone");
                achievements[19].AchievementUnlocked = true;
            }
        }

        public void FluctuatePrices(Program _program, bool multipleDaysOrNot)
        {
            // upto a 20% fluctuation in prices based on random probability
            Random random = new Random();
            double randomChange = random.Next(-10, 10) / 100.0 + 1;

            _program.coal.Price *= randomChange;
            _program.stone.Price *= randomChange;
            _program.iron.Price *= randomChange;
            _program.gold.Price *= randomChange;
            _program.diamond.Price *= randomChange;

            if (!multipleDaysOrNot && randomChange > 1)
            {
                Console.WriteLine($"Prices have risen by {Math.Round(randomChange * 100 - 100, 2)}% from what they were yesterday");
            }

            else if (!multipleDaysOrNot && randomChange < 1)
            {
                Console.WriteLine($"Prices have fallen by {Math.Round(Math.Abs(randomChange * 100 - 100), 2)}% from what they were yesterday");
            }

            // fluctuate trader price ratio
            // one of the factors can be reputation/morale later on tk
            double randomFluctuationOfTraderRatio = random.Next(0, 10) / 100.0 + 1;

            foreach (Trade trader in _program.tradeList)
            {
                trader.Ratio *= randomFluctuationOfTraderRatio;
            }
        }

        public void CheckRealEstate(Program _program, bool multipleDaysOrNot)
        {
            // reduce the days left to build by 1 and move the real estate to the active list if it's been built
            List<RealEstate> toRemoveRealEstateList = new();
            foreach (RealEstate realEstate in _program.buildingRealEstateList)
            {
                if (realEstate.DaysLeftToBuild == 1)
                {
                    _program.activeRealEstate.Add(realEstate);
                    toRemoveRealEstateList.Add(realEstate);
                    if (!multipleDaysOrNot)
                    {
                        Console.WriteLine($"Your {realEstate.Type} has been built");
                    }
                }

                else
                {
                    realEstate.DaysLeftToBuild -= 1;
                    if (!multipleDaysOrNot)
                    {
                        Console.WriteLine($"Your {realEstate.Type} will be ready in {realEstate.DaysLeftToBuild} days");
                    }
                }
            }

            foreach (RealEstate realEstate in toRemoveRealEstateList)
            {
                _program.buildingRealEstateList.Remove(realEstate);
            }

            // give rent to the player
            if (_program._currentDate.DayOfWeek is DayOfWeek.Sunday)
            {
                foreach (RealEstate realEstate in _program.activeRealEstate)
                {
                    _program.dollars.Quantity += realEstate.WeeklyRentQuantity;
                    if (!multipleDaysOrNot)
                    {
                        Console.WriteLine($"You have received {realEstate.WeeklyRentQuantity} {realEstate.WeeklyRentResource} in rent from your {realEstate.Type} \ud83c\udfd8\ufe0f \ud83e\udd11");
                    }
                }
            }
        }

        public void MineEmptinessUpdate(Program _program, bool multipleDaysOrNot)
        {
            // reduce the mine emptiness by 1% for every day dug
            if (_program.minePercentageFullness == 0)
            {
                if (!multipleDaysOrNot)
                {
                    Console.WriteLine("__________________________________________________________________________");
                    Console.WriteLine("Your mine is empty, you need to find a new location to dig");

                    _program.coal.Probability = 0;
                    _program.stone.Probability = 0;
                    _program.iron.Probability = 0;
                    _program.gold.Probability = 0;
                    _program.diamond.Probability = 0;
                }
            }

            if (_program.minePercentageFullness <= 10)
            {
                Console.WriteLine("Your mine is almost empty, you need to find a new location to dig");
            }

            if (!multipleDaysOrNot)
            {
                // progress bar for the mine emptiness

                string emptinessString = "";
                double numberOfHashtags = _program.minePercentageFullness / 3;

                for (int j = 0; j < numberOfHashtags; j++)
                {
                    emptinessString += "##";
                }

                emptinessString += "|\n";

                Console.WriteLine($"Currently this mine is {_program.minePercentageFullness}% full:\n{emptinessString}");


            }

            _program.minePercentageFullness -= 1;
            _program.coal.Probability *= 0.99;
            _program.stone.Probability *= 0.99;
            _program.iron.Probability *= 0.99;
            _program.gold.Probability *= 0.99;
            _program.diamond.Probability *= 0.99;

        }

        public void CheckInventorySize(Program _program, bool multipleDaysOrNot)
        {
            // if the inventory is full, the player can't earn more resources

            List<Resource> fullResources = new();
            List<Resource> noLongerFullResources = new();

            foreach (Resource resource in _program.resourcesList)
            {
                if (resource.Quantity >= resource.MaxQuantity)
                {
                    fullResources.Add(resource);
                }

                else if (resource.Quantity >= 0.8 * resource.MaxQuantity && !multipleDaysOrNot)
                {
                    Console.WriteLine("______________________________________________________________________________________");
                    Console.WriteLine($"Your {resource.ResourceName} inventory is more than 80% full, soon you wont be able to earn more {resource.ResourceName} until you sell some");
                    Console.WriteLine($"Currently you can store a maximum of {resource.MaxQuantity}kg of {resource.ResourceName}. You can upgrade the inventory size within the market");
                }
            }

            if (fullResources.Count > 0)
            {
                Console.WriteLine("______________________________________________________________________________________");
                Console.WriteLine("\ud83d\ude1f Your inventory is full for the following resources:");
                foreach (Resource resource in fullResources)
                {
                    Console.WriteLine($"{resource.ResourceName}");
                    resource.Quantity = resource.MaxQuantity;
                    noLongerFullResources.Add(resource);
                }

                foreach (Resource resource in noLongerFullResources)
                {
                    fullResources.Remove(resource);
                }

                Console.WriteLine("You can't earn more until you sell some \ud83d\ude1f ");
                Console.WriteLine($"Currently you can store a maximum of {_program.coal.MaxQuantity}kg of each resource");
                Console.WriteLine("You can upgrade this within the market");
            }
        }
    }

    class GameState
    {
        public Dictionary<string, object> gameStateDictionary;
        string filename = "gameState.txt";

        public GameState(Program _program)
        {
            gameStateDictionary = new Dictionary<string, object>
            {
                { "increasedGoldChanceDays", _program._increasedGoldChanceDays },
                { "marketMasterDaysLeft", _program._marketMasterDaysLeft },
                { "noWageDaysLeft", _program._noWageDaysLeft },
                { "crashDaysLeft", _program._crashDaysLeft },
                { "totalBribes", _program._totalBribes },
                { "totalPowerUpsUsed", _program._totalPowerUpsUsed },
                { "totalDaysDug", _program._totalDaysDug },
                { "totalEmployeesHired", _program._totalEmployeesHired },
                { "totalDollarsEarned", _program._totalDollarsEarned },
                { "employeesList", _program.workersList },
                { "retiredEmployeesList", _program.retiredWorkersList },
                { "coal", _program.coal },
                { "stone", _program.stone },
                { "iron", _program.iron },
                { "gold", _program.gold },
                { "diamond", _program.diamond },
                { "dollars", _program.dollars },
                { "magicTokens", _program.magicTokens },
                { "timeMachine", _program.timeMachine },
                { "ancientArtefact", _program.ancientArtefact },
                { "marketMaster", _program.marketMaster },
                { "achievementsList", _program.achievementsList },
                { "currentWageRate", _program._currentWageRate },
                { "currentDate", _program._currentDate },
                { "crashDate", _program._crashDate }
            };
        }

        public static void CreateNewGameState(Program program)
        {
            # region initialisation of all resource and other objects

            program.coal = new Resource("Coal", 75, 2, 0, 0, 10);
            program.stone = new Resource("Stone", 65, 5, 0, 0, 10);
            program.iron = new Resource("Iron", 50, 12, 0, 0, 10);
            program.gold = new Resource("Gold", 17, 60, 0, 0, 10);
            program.diamond = new Resource("Diamond", 0, 90, 0, 0, 10);

            program.resourcesList = new List<Resource>
            {
                program.coal, program.stone, program.iron, program.gold, program.diamond
            };

            program.dollars = new Resource("Dollars", 0, 0, 100, 0, 10);
            program.magicTokens = new PowerUp(0, 6, 3);
            program.timeMachine = new PowerUp(0, 3, 3);
            program.ancientArtefact = new PowerUp(0, 5, 3);
            program.marketMaster = new PowerUp(0, 4, 3);
            program.stockMarketCrash = new PayForStuff(100, 1);
            program.skipDay = new PayForStuff(50, 1);
            program.bribe = new PayForStuff(200, 1);
            program.trainingCourse = new PayForStuff(400, 1);
            program.bonus = new PayForStuff(100 + program._totalDaysDug * 2, 1.3);
            program.retirementPackage = new PayForStuff(100 + program._totalDaysDug * 10, 1.25);

            # endregion

            # region initialisation of all trade objects

            program.coalToStone = new Trade(program.stone.Price / program.coal.Price, program.coal, program.stone);
            program.coalToIron = new Trade(program.iron.Price / program.coal.Price, program.coal, program.iron);
            program.coalToGold = new Trade(program.gold.Price / program.coal.Price, program.coal, program.gold);
            program.coalToDiamond = new Trade(program.diamond.Price / program.coal.Price, program.coal, program.diamond);
            program.stoneToIron = new Trade(program.iron.Price / program.stone.Price, program.stone, program.iron);
            program.stoneToGold = new Trade(program.gold.Price / program.stone.Price, program.stone, program.gold);
            program.stoneToDiamond = new Trade(program.diamond.Price / program.stone.Price, program.stone, program.diamond);
            program.ironToGold = new Trade(program.gold.Price / program.iron.Price, program.iron, program.gold);
            program.ironToDiamond = new Trade(program.diamond.Price / program.iron.Price, program.iron, program.diamond);
            program.goldToDiamond = new Trade(program.diamond.Price / program.gold.Price, program.gold, program.diamond);
            program.tradeList = new List<Trade>
            {
                program.coalToStone, program.coalToIron, program.coalToGold, program.coalToDiamond,
                program.stoneToIron, program.stoneToGold, program.stoneToDiamond,
                program.ironToGold, program.ironToDiamond,
                program.goldToDiamond
            };

            # endregion
        }

        public void SaveGameState(Program _program)
        {
            using StreamWriter writer = new StreamWriter(filename);
            {
                foreach (KeyValuePair<string, object> entry in gameStateDictionary)
                {
                    writer.WriteLine($"{entry.Key}:{entry.Value}");
                }

                writer.WriteLine("end");
            }
            Console.WriteLine("Game state saved successfully");
        }

        public void LoadGameState(Program _program)
        {
            Console.WriteLine("Loading game state...");
            using StreamReader reader = new StreamReader(filename);
            {
                Dictionary<string, object> tempDictionary = new Dictionary<string, object>(gameStateDictionary);
                foreach (KeyValuePair<string, object> entry in gameStateDictionary)
                {
                    Console.WriteLine($"Reading the next line: {entry}");
                    string line = reader.ReadLine();
                    if (line != "end")
                    {
                        string[] parts = line.Split(':');
                        string key = parts[0];
                        object value = parts[1];

                        Console.WriteLine(
                            $"working on the current key: {key}, current value: {gameStateDictionary[key]}");
                        tempDictionary[key] = value;
                        Console.WriteLine($"New value: {tempDictionary[key]}");
                    }
                }

                gameStateDictionary = tempDictionary;
                Console.WriteLine("Read until the end and updated the dictionary");
                Thread.Sleep(1750);

                // _program.UpdateProperties(tempDictionary);
            }
        }

        public void UpdateGameState(Program _program, Dictionary<string, object> tempDictionary)
        {
            // updating the properties of the program


            // option 1 - causes enumeration operation error
            foreach (KeyValuePair<string, object> entry in tempDictionary)
            {
                gameStateDictionary[entry.Key] = entry.Value;
                object entryValue = tempDictionary[entry.Key];
                gameStateDictionary[entry.Key] = entryValue;
            }

            // option 2 - causes a casting error
            foreach (KeyValuePair<string, object> entry in tempDictionary)
            {
                Console.WriteLine($"Updating the game state of  the {entry.Key}");
                switch (entry.Key)
                {
                    case "increasedGoldChanceDays":
                        _program._increasedGoldChanceDays = (int)entry.Value;
                        break;
                    case "marketMasterDaysLeft":
                        _program._marketMasterDaysLeft = (int)entry.Value;
                        break;
                    case "noWageDaysLeft":
                        _program._noWageDaysLeft = (int)entry.Value;
                        break;
                    case "crashDaysLeft":
                        _program._crashDaysLeft = (int)entry.Value;
                        break;
                    case "totalBribes":
                        _program._totalBribes = (int)entry.Value;
                        break;
                    case "totalPowerUpsUsed":
                        _program._totalPowerUpsUsed = (int)entry.Value;
                        break;
                    case "totalDaysDug":
                        _program._totalDaysDug = (double)entry.Value;
                        break;
                    case "totalEmployeesHired":
                        _program._totalEmployeesHired = (double)entry.Value;
                        break;
                    case "totalDollarsEarned":
                        _program._totalDollarsEarned = (double)entry.Value;
                        break;
                    case "employeesList":
                        _program.workersList = (List<Worker>)entry.Value;
                        break;
                    case "retiredEmployeesList":
                        _program.retiredWorkersList = (List<Worker>)entry.Value;
                        break;
                    case "coal":
                        _program.coal = (Resource)entry.Value;
                        break;
                    case "stone":
                        _program.stone = (Resource)entry.Value;
                        break;
                    case "iron":
                        _program.iron = (Resource)entry.Value;
                        break;
                    case "gold":
                        _program.gold = (Resource)entry.Value;
                        break;
                    case "diamond":
                        _program.diamond = (Resource)entry.Value;
                        break;
                    case "dollars":
                        _program.dollars = (Resource)entry.Value;
                        break;
                    case "magicTokens":
                        _program.magicTokens = (PowerUp)entry.Value;
                        break;
                    case "timeMachine":
                        _program.timeMachine = (PowerUp)entry.Value;
                        break;
                    case "ancientArtefact":
                        _program.ancientArtefact = (PowerUp)entry.Value;
                        break;
                    case "marketMaster":
                        _program.marketMaster = (PowerUp)entry.Value;
                        break;
                    case "achievementsList":
                        _program.achievementsList = (List<Achievements>)entry.Value;
                        break;
                    case "currentWageRate":
                        _program._currentWageRate = (double)entry.Value;
                        break;
                    case "currentDate":
                        _program._currentDate = (DateTime)entry.Value;
                        break;
                    case "crashDate":
                        _program._crashDate = (int)entry.Value;
                        break;
                }
            }
        }
    } // applying a loaded game state

    class RealEstate
    {
        public string Type;
        public int DaysLeftToBuild;
        public double WeeklyRentQuantity;
        public string WeeklyRentResource;
        public double Cost;

        public RealEstate(string type, int daysLeftToBuild, double weeklyRentQuantity, string weeklyRentResource, double cost)
        {
            Type = type;
            DaysLeftToBuild = daysLeftToBuild;
            WeeklyRentQuantity = weeklyRentQuantity;
            WeeklyRentResource = weeklyRentResource;
            Cost = cost;
        }

        public static void BuildRealEstate(int choice, Program _program)
        {
            switch (choice)
            {
                case 0:
                    Console.WriteLine("Cancelled");
                    break;

                case 1:

                    RealEstate apartment = new RealEstate("apartment", 5, 300, "dollars", 1000);
                    if (_program.dollars.Quantity < apartment.Cost)
                    {
                        Console.WriteLine("You don't have enough money to build an apartment");
                        break;
                    }

                    if (_program.activeRealEstate.Contains(apartment))
                    {
                        Console.WriteLine("You already have an apartment");
                        break;
                    }

                    Console.WriteLine("You have chosen to build a new apartment");
                    Console.WriteLine($"Your apartment will be ready in {apartment.DaysLeftToBuild} days");
                    Console.WriteLine($"You will earn ${apartment.WeeklyRentQuantity} every Monday from this apartment");
                    Console.WriteLine($"You have been charged ${apartment.Cost} for the construction of this apartment");
                    _program.dollars.Quantity -= apartment.Cost;
                    _program.buildingRealEstateList.Add(apartment);
                    break;

                case 2:
                    RealEstate house = new RealEstate("house", 5, 300, "coal", 100);
                    if (_program.coal.Quantity < house.Cost)
                    {
                        Console.WriteLine("You don't have enough coal to build a house");
                        break;
                    }

                    if (_program.activeRealEstate.Contains(house))
                    {
                        Console.WriteLine("You already have an house");
                        break;
                    }

                    Console.WriteLine("You have chosen to build a new house");
                    Console.WriteLine($"Your house will be ready in {house.DaysLeftToBuild} days");
                    Console.WriteLine($"You will earn {house.WeeklyRentQuantity}kg {house.WeeklyRentResource} every Monday from this house");
                    Console.WriteLine($"You have been charged {house.Cost}kg of {house.WeeklyRentResource} for the construction of this house");

                    _program.coal.Quantity -= house.Cost;
                    _program.buildingRealEstateList.Add(house);
                    break;

                case 3:
                    RealEstate office = new RealEstate("office", 5, 300, "stone", 100);
                    if (_program.stone.Quantity < 10000)
                    {
                        Console.WriteLine("You don't have enough money to build an office");
                        break;
                    }

                    if (_program.activeRealEstate.Contains(office))
                    {
                        Console.WriteLine("You already have an office");
                        break;
                    }

                    Console.WriteLine("You have chosen to build a new office");
                    Console.WriteLine($"Your office will be ready in {office.DaysLeftToBuild} days");
                    Console.WriteLine($"You will earn {office.WeeklyRentQuantity}kg {office.WeeklyRentResource} every Monday from this office");
                    Console.WriteLine($"You have been charged {office.Cost}kg of {office.WeeklyRentResource} for the construction of this office");
                    _program.stone.Quantity -= office.Cost;
                    _program.buildingRealEstateList.Add(office);
                    break;

                case 4:
                    RealEstate mansion = new RealEstate("mansion", 5, 300, "iron", 100);
                    if (_program.iron.Quantity < 10000)
                    {
                        Console.WriteLine("You don't have enough iron to build a mansion");
                        break;
                    }

                    if (_program.activeRealEstate.Contains(mansion))
                    {
                        Console.WriteLine("You already have a mansion");
                        break;
                    }

                    Console.WriteLine("You have chosen to build a new mansion");
                    Console.WriteLine($"Your mansion will be ready in {mansion.DaysLeftToBuild} days");
                    Console.WriteLine($"You will earn {mansion.WeeklyRentQuantity}kg {mansion.WeeklyRentResource} every Monday from this mansion");
                    Console.WriteLine($"You have been charged {mansion.Cost}kg of {mansion.WeeklyRentResource} for the construction of this mansion");
                    _program.iron.Quantity -= mansion.Cost;
                    _program.buildingRealEstateList.Add(mansion);
                    break;

                case 5:
                    RealEstate castle = new RealEstate("castle", 5, 300, "gold", 100);
                    if (_program.gold.Quantity < 10000)
                    {
                        Console.WriteLine("You don't have enough money to build a castle");
                        break;
                    }

                    if (_program.activeRealEstate.Contains(castle))
                    {
                        Console.WriteLine("You already have a castle");
                        break;
                    }

                    Console.WriteLine("You have chosen to build a new castle");
                    Console.WriteLine($"Your castle will be ready in {castle.DaysLeftToBuild} days");
                    Console.WriteLine($"You will earn {castle.WeeklyRentQuantity}kg {castle.WeeklyRentResource} every Monday from this castle");
                    Console.WriteLine($"You have been charged {castle.Cost}kg of {castle.WeeklyRentResource} for the construction of this castle");
                    _program.gold.Quantity -= castle.Cost;
                    _program.buildingRealEstateList.Add(castle);
                    break;

                case 6:
                    RealEstate palace = new RealEstate("palace", 5, 300, "diamond", 100);
                    if (_program.diamond.Quantity < 10000)
                    {
                        Console.WriteLine("You don't have enough money to build a palace");
                        break;
                    }

                    if (_program.activeRealEstate.Contains(palace))
                    {
                        Console.WriteLine("You already have a palace");
                        break;
                    }

                    Console.WriteLine("You have chosen to build a new palace");
                    Console.WriteLine($"Your palace will be ready in {palace.DaysLeftToBuild} days");
                    Console.WriteLine($"You will earn {palace.WeeklyRentQuantity}kg {palace.WeeklyRentResource} every Monday from this palace");
                    Console.WriteLine($"You have been charged {palace.Cost}kg of {palace.WeeklyRentResource} for the construction of this palace");
                    _program.diamond.Quantity -= palace.Cost;
                    _program.buildingRealEstateList.Add(palace);
                    break;
            }
        }
    } // balance updates needed

    class DisplayStuff
    {
        public static void DisplayGameMechanics(Program _program)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n   _____                                 __  __                 _                       _              \n  / ____|                               |  \\/  |               | |                     (_)             \n | |  __    __ _   _ __ ___     ___     | \\  / |   ___    ___  | |__     __ _   _ __    _    ___   ___ \n | | |_ |  / _` | | '_ ` _ \\   / _ \\    | |\\/| |  / _ \\  / __| | '_ \\   / _` | | '_ \\  | |  / __| / __|\n | |__| | | (_| | | | | | | | |  __/    | |  | | |  __/ | (__  | | | | | (_| | | | | | | | | (__  \\__ \\\n  \\_____|  \\__,_| |_| |_| |_|  \\___|    |_|  |_|  \\___|  \\___| |_| |_|  \\__,_| |_| |_| |_|  \\___| |___/\n");
            Console.ResetColor();

            Console.WriteLine("__________________________________________________________");
            Thread.Sleep(750);
            Console.WriteLine("Current probabilities of finding resources:");
            Console.WriteLine($"| Coal: {Math.Round(_program.coal.Probability, 2)}%                | Stone: {Math.Round(_program.stone.Probability, 2)}");
            Console.WriteLine($"| Iron: {Math.Round(_program.iron.Probability, 2)}%                | Gold: {Math.Round(_program.gold.Probability, 2)}%");
            Console.WriteLine($"| Diamond: {Math.Round(_program.diamond.Probability, 2)}%              | Ancient Artefact: {Math.Round(_program.ancientArtefact.Probability, 2)}%");
            Console.WriteLine($"| Market Master: {Math.Round(_program.marketMaster.Probability, 2)}%        | Magic Token: {Math.Round(_program.magicTokens.Probability, 2)}%");
            Console.WriteLine($"| Time Machine: {Math.Round(_program.timeMachine.Probability, 2)}%");
            Thread.Sleep(1000);
            Console.WriteLine("__________________________________________________________");
            Console.WriteLine("Current Prices:");
            Console.WriteLine($"| Coal: ${Math.Round(_program.coal.Price, 2)} per kg          | Stone: ${Math.Round(_program.stone.Price, 2)} per kg");
            Console.WriteLine($"| Iron: ${Math.Round(_program.iron.Price, 2)} per kg         | Gold: ${Math.Round(_program.gold.Price, 2)} per kg");
            Console.WriteLine($"| Diamond: ${Math.Round(_program.diamond.Price, 2)} per kg     | Employees: {Math.Round(_program._currentEmployeePrice, 2)} per employee");
            Console.WriteLine("__________________________________________________________");
            Thread.Sleep(1750);

            Console.WriteLine("\nResource prices fluctuate by upto ±10% per day");
            Console.WriteLine("You can find powerups that have different effects");
            Console.WriteLine("The resources you gain are equal to the number of employees you have times their efficiency * some random fluctuation of ±20%");
            Console.WriteLine($"Baseline wage = ${_program._currentWageRate} per employee per day");
            Console.WriteLine("There is a chance an employee is ill and doesn't come in to work");
            Console.WriteLine("30% pay increase on weekends only");
            Console.WriteLine("On the first of every month, employee wage increases by 10% permanently");
            Console.WriteLine("One x date every month, there is a stock market crash where all prices halve (prime time to buy employees)");
            Console.WriteLine("every 10 days, the probabilities of finding resources is reduced by 8%");
            Console.WriteLine($"You can bribe the govt with ${_program.bribe.Price} and not pay any wages for the next 3 days");
            Console.WriteLine("At any time if your $$$ balance goes negative, the govt sells all of your resources for 50% the current market rate");
            Console.WriteLine("If you have no resources to sell, they sell your employees for $100 each until you have 1 employee left");
            Console.WriteLine("If your $$$ balance is negative and you have no resource, you fail the game");
            Console.WriteLine("__________________________________________________________________________");
        }

        public static void DisplayStats(Program program)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                        YOUR STATS                          ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            Console.WriteLine($"Here are your stats as of {program._currentDate.Date: dddd, dd MMMM yyyy}:");
            Console.WriteLine("__________________________________________________________");
            Console.WriteLine($"Total coal found: {Math.Round(program.coal.TotalFound, 2)}kg");
            Console.WriteLine($"Total stone found: {Math.Round(program.stone.TotalFound, 2)}kg");
            Console.WriteLine($"Total iron found: {Math.Round(program.iron.TotalFound, 2)}kg");
            Console.WriteLine($"Total gold found: {Math.Round(program.gold.TotalFound, 2)}kg");
            Console.WriteLine($"Total diamond found: {Math.Round(program.diamond.TotalFound, 2)}kg");
            Console.WriteLine($"Total powerups used: {program._totalPowerUpsUsed}");
            Console.WriteLine($"Total employees hired: {program._totalEmployeesHired}");
            Console.WriteLine($"Total bribes paid: {program._totalBribes}");
            Console.WriteLine("__________________________________________________________");
            Console.WriteLine($"\nTotal dollars earned: ${Math.Round(program._totalDollarsEarned, 2)}");
            Console.WriteLine($"Total days dug: {program._totalDaysDug}");
        }

        public static void DisplayResources(Program program)
        {
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine($"                     You have ${Math.Round(program.dollars.Quantity, 2)}\n");
            Console.WriteLine($"| You have {Math.Round(program.coal.Quantity, 2)}kg of coal         | You have {Math.Round(program.stone.Quantity, 2)}kg of stone");
            Console.WriteLine($"| You have {Math.Round(program.iron.Quantity, 2)}kg of iron         | You have {Math.Round(program.gold.Quantity, 2)}kg of gold");
            Console.WriteLine($"| You have {Math.Round(program.diamond.Quantity, 2)}kg of diamond      | You have {Math.Round(program.magicTokens.Quantity, 2)} magic tokens");
            Console.WriteLine($"| You have {program.workersList.Count} employees         | Your employees' average efficiency is {Math.Round(program._averageEmployeeEfficiency, 2)}");
            Console.WriteLine("__________________________________________________________________________");
        }

        public static void DisplayEmployees(Program program)
        {
            Console.WriteLine("\n  ______                       _                                     \n |  ____|                     | |                                    \n | |__     _ __ ___    _ __   | |   ___    _   _    ___    ___   ___ \n |  __|   | '_ ` _ \\  | '_ \\  | |  / _ \\  | | | |  / _ \\  / _ \\ / __|\n | |____  | | | | | | | |_) | | | | (_) | | |_| | |  __/ |  __/ \\__ \\\n |______| |_| |_| |_| | .__/  |_|  \\___/   \\__, |  \\___|  \\___| |___/\n                      | |                   __/ |                    \n                      |_|                  |___/                     \n");
            Console.WriteLine("_____________________________________________________________________");
            int i = 0;

            if (program.deadWorkersList.Count != 0)
            {
                Console.WriteLine("__________________________________________________________________________");
                Thread.Sleep(2500);
            }

            foreach (Worker worker in program.deadWorkersList)
            {
                i++;
                Console.WriteLine($"Dead Employee Number {i} - {worker.Name}, Efficiency {Math.Round(worker.efficiency, 2)}, Died on {worker.RetirementDate.Date}, Worked for {worker.DaysWorked} days \ud83e\uddcd\u200d\u2642\ufe0f");
            }

            if (program.retiredWorkersList.Count != 0)
            {
                Console.WriteLine("__________________________________________________________________________");
                Thread.Sleep(2500);
            }

            foreach (Worker worker in program.retiredWorkersList)
            {
                i++;
                Console.WriteLine($"Retiree Number {i} - {worker.Name}, Efficiency {Math.Round(worker.efficiency, 2)}, Retired on {worker.RetirementDate:dddd, d MMMM, yyyy}, Worked for {worker.DaysWorked} days \ud83e\uddcd\u200d\u2642\ufe0f");
            }

            if (program.retiredWorkersList.Count != 0)
            {
                Console.WriteLine("__________________________________________________________________________");
                Thread.Sleep(2500);
            }

            DisplayActiveEmployees(program);
        }

        public static void DisplayActiveEmployees(Program program)
        {
            Console.WriteLine("Here are your current working employees:");
            int j = 0;
            double totalWages = 0;
            foreach (Worker worker in program.workersList)
            {
                j++;
                totalWages += worker.Wage;
                Console.WriteLine($"Employee Number {j} - {worker.Name}, Efficiency {Math.Round(worker.efficiency, 2)}, Morale {Math.Round(worker.Morale, 2)} Current wage {Math.Round(worker.Wage, 2)}, Retiring in {worker.DaysUntilRetirement} days \ud83e\uddcd\u200d\u2642\ufe0f");
            }

            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine($"Total wages: ${Math.Round(totalWages, 2)}");
            Console.WriteLine($"Average employee morale: {Math.Round(program._averageEmployeeMorale, 2)}");
            Console.WriteLine($"Average employee efficiency: {Math.Round(program._averageEmployeeEfficiency, 2)}");
            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine("\n\n[ENTER]");
            Console.ReadLine();
        }

        public static void DisplayRealEstate(Program program)
        {
            if (program.activeRealEstate.Count == 0)
            {
                Console.WriteLine("__________________________________________________________________________");
                Console.WriteLine("You have no real estate properties active right now, level up brokie \ud83d\ude45\u200d\u2642\ufe0f ");
                Console.WriteLine("__________________________________________________________________________");
                return;
            }

            Console.WriteLine("__________________________________________________________________________");
            Console.WriteLine($"You have {program.activeRealEstate.Count} real estate properties active right now:");
            foreach (RealEstate realEstate in program.activeRealEstate)
            {
                if (realEstate.WeeklyRentResource == "dollars")
                {
                    Console.WriteLine($"Your {realEstate.Type} is giving you ${realEstate.WeeklyRentQuantity} per week");
                }

                else
                {
                    Console.WriteLine($"Your {realEstate.Type} is giving you {realEstate.WeeklyRentQuantity}kg of {realEstate.WeeklyRentResource} per week");
                }
            }

            Console.WriteLine("__________________________________________________________________________");
        }

        public static void DisplayAchievements(List<Achievements> achievements, int subChoice)
        {
            Console.WriteLine("\n                    _       _                                                    _         \n     /\\            | |     (_)                                                  | |        \n    /  \\      ___  | |__    _    ___  __   __   ___   _ __ ___     ___   _ __   | |_   ___ \n   / /\\ \\    / __| | '_ \\  | |  / _ \\ \\ \\ / /  / _ \\ | '_ ` _ \\   / _ \\ | '_ \\  | __| / __|\n  / ____ \\  | (__  | | | | | | |  __/  \\ V /  |  __/ | | | | | | |  __/ | | | | | |_  \\__ \\\n /_/    \\_\\  \\___| |_| |_| |_|  \\___|   \\_/    \\___| |_| |_| |_|  \\___| |_| |_|  \\__| |___/\n                                                                                           \n");
            if (subChoice == 1)
            {
                Console.WriteLine("Here are your completed achievements:\n");

                foreach (Achievements achievement in achievements)
                {
                    if (achievement.AchievementUnlocked)
                    {
                        Console.WriteLine($"Achievement {achievement.AchievementNumber} - {achievement.AchievementDescription}");
                    }
                }
            }
            
            else if (subChoice == 2)
            {
                Console.WriteLine("Here are your not-yet-completed achievements:\n");

                foreach (Achievements achievement in achievements)
                {
                    if (!achievement.AchievementUnlocked)
                    {
                        Console.WriteLine($"Locked achievement {achievement.AchievementNumber} - {achievement.AchievementDescription}");
                    }
                }
            }
            
        }
    }

    // classes that can be in their own files:
    class WeatherEffectsClass
    {
        public string Name;
        public int DaysLeft;
        public double Probability;
        public double EfficiencyMultiplier;
        public int Duration;

        public WeatherEffectsClass(string name, int daysLeft, double probability, double efficiencyMultiplier, int duration)
        {
            Name = name;
            DaysLeft = daysLeft;
            Probability = probability;
            EfficiencyMultiplier = efficiencyMultiplier;
            Duration = duration;
        }
    }

    class PayForStuff
    {
        public double Price;
        public bool skipDayOrNot;
        public double MoraleMultiplier;

        public PayForStuff(double price, double moraleMultiplier)
        {
            Price = price;
            skipDayOrNot = false;
            MoraleMultiplier = moraleMultiplier;
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
            Probability = initialProbability;
            MaxQuantity = maxQuantity;
        }
    }

    class Resource
    {
        public string ResourceName;
        public double InitialPrice;
        public double Probability;
        public double Price;
        public double Quantity;
        public double TotalFound;
        public double OriginalProbability;
        public double MaxQuantity;

        public Resource(string resourceName, double initialProbability, double initialPrice, double initialQuantity,
            double totalFound, double maxQuantity)
        {
            ResourceName = resourceName;
            Probability = initialProbability;
            InitialPrice = initialPrice;
            Price = initialPrice;
            Quantity = initialQuantity;
            TotalFound = totalFound;
            OriginalProbability = initialProbability;
            MaxQuantity = maxQuantity;
        }
    }
}

