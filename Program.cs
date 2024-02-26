using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;

namespace Gold_Diggerzz
{
    // as of sunday 18feb 1pm, 27hours 45 minutes spent on digging sim as of google calendar
    // initial inspiration: https://replit.com/@AadityaKalwani/Digging-Simulator#main.py

    /* current issues
     
     * you are allowed to make multiple trades per day
       employees that are ill don't have the weather effect multiplier applied to them
     * LOAD GAME STATE
        * you can not load a game state because of either casting issues or enumeration operation errors
     * inconsistent between weather effect displaying and actual time left
        * eg "6 days left of bad weather" but then it's only 5 days
     */

    /* to-do ideas
     *Probability of finding stone still says 70%}”
       
       You have negative power ups 
       
       Sell employees
       
       All employees retired so you had infinite / NaN resources
       
       “We’re selling all your resources” rather than specify iron and gold
       
       Make it more obvious when you sell employees because of bankruptcy
       
       Apply !multiple day did to “you found nothing today”
       
       
       Move hire more employees out of the market option
       Maybe another section for “employee stuff”
     * 
     * move UsePowerUp to PowerUp class? and other such offloading of tasks from the main class - this causes major static non-static etc issues
     * remove printed line at the start that says:
        * /Library/Frameworks/Mono.framework/Versions/6. 12. O/bin/mono-sgen64 /Users/aadityakalwani/Documents/Coding/Gold_Diggerzz/bin/Debug/Gold_Diggerzz. exe 
     * update ascii art for menu options: achievements, tutorial, use powerups, game state saved, game mechanics
     * Get a certain number of resource to build something eg. a stone castle which gives income or a house or a flat or a mansion
         * eg. a wedding happened or dracula came into the castle and scared away they guests, castle can collapse etc
         * lol turn it into a real estate game
     * Fix trader logic because sister had -16iron after going to the trader.... long
     * If weather became fine, no more effects that day
     * a proper tutorial with images and arrows? (or just a better tutorial)
        * when the tutorial is better, an option before the game starts to play the tutorial
     * adding more incentive to keep playing
        * option to print all achievements and show if unlocked or not yet 
        * goals to reach
        * if you reach _______ income you can find ______
     * a list of all possible trades, for each trade, if the player has enough of the fromResource, display the trade option?
     * create morale and reputation
     *  employee morale --> if morale is low, the employee could be less efficient.
        * morale-boosting powerup
     * player can offer employees a retirement package in exchange for a morale boost for the remaining workers
     * Allow employees to specialize in certain areas, making them more efficient at gathering certain resources. This could add another layer of strategy to the game as players decide how to best allocate their workforce.
     * Resource Discovery: Add a feature where players can discover new resources as they dig deeper. These new resources could be more valuable but also more difficult to extract. also based on achievements unlocked
     * a 'mine emptiness', where the player has to move to a new mine and start again (acting as prestige)
        * new mine also means new resources??? like a dinosaur mine that has dinosaur bones as well as stone, gold, etc. a space mine that has space rocks, etc
        * as the mine gets emptier, chance of finding resources decreases
     * Exploration: Allow the player to explore new areas or mines. This could involve a risk/reward system where exploring new areas could lead to finding more valuable resources but also has the potential for more dangerous events.
     * Trader's prices fluctuate (one of the factors can be reputation)
     * achievements are OOP-ed? idk about this one - give it some thought
     * otherwise option to print all achievements as an incentive to work towards them/keep playing
     * earthquakes that loosen soil and make shit easier to find (+ cool animations possible) ++ kill some employees ++ morale lost
     * a "mine collapse" event could temporarily reduce the player's digging efficiency ++ kill some employees ++ morale lost
     * loans - you can take a loan from the bank and pay it back with interest
     * stock market feature (kinda done?)
         * ++ idea that every 5 gold sold, increase gold price and for every 5 gold mined/gained, decrease price? Incentivising selling fast and not holding resources for ages
     * managers that do shit
        * eg a temporary 'gold' manager that improves chance of finding gold but is hired for a week
        * or a 'diamond' manager to double chance of finding gold for 10 days
     * competition / fake in some other mining companies and you're also trying to beat them (give them a quadratic rate of growth?)
     */

    class Resource
        {
            public string ResourceName;
            public double InitialPrice;
            public double Probability;
            public double Price;
            public double Quantity;
            public double TotalFound;

            public Resource(string resourceName, double initialProbability, double initialPrice, double initialQuantity,
                double totalFound)
            {
                ResourceName = resourceName;
                Probability = initialProbability;
                InitialPrice = initialPrice;
                Price = initialPrice;
                Quantity = initialQuantity;
                TotalFound = totalFound;
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

        public Worker(string type, string name, double wage, double price, double employeeIllProbability, double Efficiency)
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
            }
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
        public double Price;
        public bool skipDayOrNot;

        public PayForStuff(double price)
        {
            Price = price;
            skipDayOrNot = false;
        }
    }

    class DisplayStuff
    {
        public static void DisplayGameMechanics(Program _program)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("╔════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    GAME MECHANICS                      ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════╝");
            Console.ResetColor();
            
            Console.WriteLine("__________________________________________________________");
            Thread.Sleep(750);
            Console.WriteLine("Current probabilities of finding resources:");
            Console.WriteLine($"| Coal: {Math.Round(_program.coal.Probability, 2)}%                | Stone: {Math.Round(_program.stone.Probability, 2)}}}%\"");
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
            Console.WriteLine("On the 15th of each month, each employee gets 10% of your current $$$ stash (profit sharing)");
            Console.WriteLine("One x date every month, there is a stock market crash where all prices halve (prime time to buy employees)");
            Console.WriteLine("every 10 days, the probabilities of finding resources is reduced by 8%");
            Console.WriteLine($"You can bribe the govt with ${_program.bribe.Price} and not pay any wages for the next 3 days");
            Console.WriteLine("At any time if your $$$ balance goes negative, the govt sells all of your resources for 50% the current market rate");
            Console.WriteLine("If you have no resources to sell, they sell your employees for $100 each until you have 1 employee left");
            Console.WriteLine("If your $$$ balance is negative and you have no resource, you fail the game");
            Console.WriteLine("__________________________________________________________");
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
            foreach (Worker worker in program.retiredWorkersList)
            {
                i++;
                Console.WriteLine($"Retiree Number {i} - {worker.Name}, Efficiency {Math.Round(worker.efficiency, 2)}, Retired on {worker.RetirementDate.Date}, Worked for {worker.DaysWorked} days \ud83e\uddcd\u200d\u2642\ufe0f");
            }

            if (program.retiredWorkersList.Count != 0)
            {
                Console.WriteLine("_____________________________________________________________________");
            }
            
            Console.WriteLine("Here are your current working employees:");
            int j = 0;
            double totalWages = 0;
            foreach (Worker worker in program.workersList)
            {
                totalWages += worker.Wage;
                j++;
                Console.WriteLine($"Employee Number {j} - {worker.Name}, Efficiency {Math.Round(worker.efficiency, 2)}, Current wage {Math.Round(worker.Wage, 2)}, Retiring in {worker.DaysUntilRetirement} days \ud83e\uddcd\u200d\u2642\ufe0f");
            }

            Console.WriteLine("___________________________________________________________");
            Console.WriteLine($"Total wages right now: ${Math.Round(totalWages, 2)}");
            Console.WriteLine($"Average employee efficiency right now: {Math.Round(program._averageEmployeeEfficiency, 2)}");
            Console.WriteLine("___________________________________________________________");
            Console.WriteLine("\n\n[ENTER]");
            Console.ReadLine();
        }
    }

    class Trade
    {
        public static List<DateTime> datesOfTradesMade = new List<DateTime>();
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
            if (!datesOfTradesMade.Contains(_currentDate))
            {
                if (ratio * fromResource.Quantity < toResource.Quantity)
                {
                    Console.WriteLine("\u274c You can't afford to make this trade brokie \u274c");
                    return;
                }

                fromResource.Quantity -= ratio;
                toResource.Quantity += 1;

                Console.WriteLine($"\u2705 Trade Complete! You traded {ratio}kg of {fromResource.ResourceName} for 1kg of {toResource.ResourceName} \u2705");
                datesOfTradesMade.Add(DateTime.Today);
                return;
            }

            Console.WriteLine("You've already made a trade today, try again later \ud83d\udc4b ");
        }
    }
    
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

    class MiningOperation
    {
        public void Dig(int daysToDig, Program _program, DayToDayOperations _DayToDayOperations, List<string> achievements)

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
                if (_program.CheckIfInDebt() != "true")
                {
                    if (!_program.skipDay.skipDayOrNot)
                    {
                        if (!multipleDayDig)
                        {
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
                            
                            
                            for (int i = 0; i < 10; i++)
                            {
                                Thread.Sleep(125);
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
                            Console.WriteLine("___________________________________________________________________________");
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
                            !ancientArtefactFound && !timeMachineFound && !magicTokenFound && !marketMasterFound)
                        {
                            Console.WriteLine("\ud83d\udeab You found nothing today \ud83d\udeab");
                        }

                        if (ancientArtefactFound)
                        {
                            Console.WriteLine("__________________________________________________________");
                            Console.Write("\ud83c\udffa You found the Ancient Artefact power-up \ud83c\udffa");
                            Console.WriteLine("\nChoose an option:");
                            Console.WriteLine(
                                "1 - Get a guaranteed 50% chance of finding gold for the next five days");
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
                    }

                    // calendar/weather etc effects 
                    if (!multipleDayDig)
                    {
                        Console.WriteLine("__________________________________________________________");
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

                            Console.WriteLine(
                                $"Your {_program.workersList.Count} employees charged a wage of ${Math.Round(totalWages, 2)} today.");
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
                }

                _program._totalDaysDug += 1;
                
                // post-digging effects
                _DayToDayOperations.CalendarEffects(_program, multipleDayDig);
                _DayToDayOperations.WeatherEffects(_program, multipleDayDig);
                _DayToDayOperations.DealWithEmployees(_program, multipleDayDig);
                _DayToDayOperations.FluctuatePrices(_program, multipleDayDig);
            }
            
            if (multipleDayDig)
            {
                Console.WriteLine("__________________________________________________________");
                Console.WriteLine($"Here are the changes to your resources after {daysToDig} days of digging:");
                Console.WriteLine($"You found {Math.Round(newCoal, 2)}kg of coal");
                Console.WriteLine($"You found {Math.Round(newStone, 2)}kg of stone");
                Console.WriteLine($"You found {Math.Round(newIron, 2)}kg of iron");
                Console.WriteLine($"You found {Math.Round(newGold, 2)}kg of gold");
                Console.WriteLine($"You found {Math.Round(newDiamond, 2)}kg of diamond");
            }

            _DayToDayOperations.CheckAchievements(achievements, _program);

            Console.WriteLine("__________________________________________________________");
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

            if (_program._currentDate.Month == 1 && _program._currentDate.Day == 1)
            {
                Console.WriteLine("Happy New Year! ");
            }

            else
            {
                Console.WriteLine("Prices have fluctuated ± 10% from what they were yesterday");
            }
            
            DisplayStuff.DisplayResources(_program);

            Console.WriteLine($"Here are the rates for {_program._currentDate:dddd dd MMMM, yyyy}:");

            Console.WriteLine("__________________________________________________________");
            Console.WriteLine(
                $"| Coal: ${Math.Round(_program.coal.Price, 2)} per kg           | Stone: {Math.Round(_program.stone.Price, 2)} per kg");
            Console.WriteLine(
                $"| Iron: ${Math.Round(_program.iron.Price, 2)} per kg          | Gold: ${Math.Round(_program.gold.Price, 2)} per kg");
            Console.WriteLine(
                $"| Diamond: ${Math.Round(_program.diamond.Price, 2)} per kg      | Employees: {Math.Round(_program._currentEmployeePrice, 2)} per employee");
            Console.WriteLine("__________________________________________________________");


            int marketOption;
            do
            {
                Console.WriteLine("\nHere is the menu for the market:");
                Console.WriteLine("1 - Sell a specific resource");
                Console.WriteLine("2 - Sell all resources for dollars");
                Console.WriteLine("3 - Hire More Employees");
                Console.WriteLine("4 - Exit market");
                Console.WriteLine("\nChoose Option:");
                marketOption = _program.GetValidInt(1, 4);

                switch (marketOption)
                {
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
                                    _program.dollars.Quantity += _program.coal.Quantity * _program.coal.Price;
                                    _program._totalDollarsEarned += _program.coal.Quantity * _program.coal.Price;
                                }

                                Console.WriteLine("Here are your updated resources:");
                                DisplayStuff.DisplayResources(_program);
                                break;

                            case 2:
                                Console.WriteLine("Your have chosen to sell stone for dollars");
                                Console.WriteLine(
                                    $"How much stone do you want to sell?\nYou have {_program.stone.Quantity}kg of stone");
                                double stoneToSell = _program.GetValidDouble(0, 100000000000);
                                if (stoneToSell > _program.stone.Quantity)
                                {
                                    Console.WriteLine("You don't have enough stone to sell that much");
                                }
                                else
                                {
                                    _program.stone.Quantity -= stoneToSell;
                                    _program.dollars.Quantity += _program.stone.Quantity * _program.stone.Price;
                                    _program._totalDollarsEarned += _program.stone.Quantity * _program.stone.Price;
                                }

                                Console.WriteLine("Here are your updated resources:");
                                DisplayStuff.DisplayResources(_program);
                                break;
                            case 3:
                                Console.WriteLine("Your have chosen to sell iron for dollars");
                                Console.WriteLine(
                                    $"How much iron do you want to sell?\nYou have {_program.iron.Quantity}kg of iron");
                                double ironToSell = _program.GetValidDouble(0, 100000000000);
                                if (ironToSell > _program.iron.Quantity)
                                {
                                    Console.WriteLine("You don't have enough iron to sell that much");
                                }
                                else
                                {
                                    _program.iron.Quantity -= ironToSell;
                                    _program.dollars.Quantity += _program.iron.Quantity * _program.iron.Price;
                                    _program._totalDollarsEarned += _program.iron.Quantity * _program.iron.Price;
                                }

                                Console.WriteLine("Here are your updated resources:");
                                DisplayStuff.DisplayResources(_program);
                                break;
                            case 4:
                                Console.WriteLine("Your have chosen to sell gold for dollars");
                                Console.WriteLine(
                                    $"How much gold do you want to sell?\nYou have {_program.gold.Quantity}kg of gold");
                                double goldToSell = _program.GetValidDouble(0, 100000000000);
                                if (goldToSell > _program.gold.Quantity)
                                {
                                    Console.WriteLine("You don't have enough gold to sell that much");
                                }
                                else
                                {
                                    _program.gold.Quantity -= goldToSell;
                                    _program.dollars.Quantity += _program.gold.Quantity * _program.gold.Price;
                                    _program._totalDollarsEarned += _program.gold.Quantity * _program.gold.Price;
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
                                    _program.dollars.Quantity += _program.diamond.Quantity * _program.diamond.Price;
                                    _program._totalDollarsEarned +=
                                        _program.diamond.Quantity * _program.diamond.Price;
                                }

                                Console.WriteLine("Here are your updated resources:");
                                DisplayStuff.DisplayResources(_program);
                                break;
                        }

                        break;

                    case 2:
                        Console.WriteLine(
                            "We're selling all your coal and iron and gold and stone and diamond for dollars");

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
                        Console.Clear();
                        Console.WriteLine(
                            "  _    _   _                      ______                       _                                       \n | |  | | (_)                    |  ____|                     | |                                      \n | |__| |  _   _ __    ___       | |__     _ __ ___    _ __   | |   ___    _   _    ___    ___   ___   \n |  __  | | | | '__|  / _ \\      |  __|   | '_ ` _ \\  | '_ \\  | |  / _ \\  | | | |  / _ \\  / _ \\ / __|  \n | |  | | | | | |    |  __/      | |____  | | | | | | | |_) | | | | (_) | | |_| | |  __/ |  __/ \\__ \\  \n |_|  |_| |_| |_|     \\___|      |______| |_| |_| |_| | .__/  |_|  \\___/   \\__, |  \\___|  \\___| |___/  \n                                                      | |                   __/ |                      \n                                                      |_|                  |___/                     ");
                        Console.WriteLine($"What type of employee do you want to hire?");
                        Console.WriteLine(
                            $"1 - Hire a bad employee: Price = ${_program._currentEmployeePrice * 0.5} Wage = ${_program._currentWageRate * 0.5}, Efficiency = 0.5x, Retires in 30 days");
                        Console.WriteLine(
                            $"2 - Hire a mid employee: Price = ${_program._currentEmployeePrice}, Wage = ${_program._currentWageRate}, Efficiency = 1x, Retires in 45 days");
                        Console.WriteLine(
                            $"3 - Hire a good employee: Price = ${_program._currentEmployeePrice * 2}, Wage = ${_program._currentWageRate * 2}, Efficiency = 2x, Retires in 30 days");
                        Console.WriteLine("4 - Cancel and return to market menu");
                        
                        int employeeType = _program.GetValidInt(1, 4);
                        
                        if (employeeType != 4)
                        {
                            Console.WriteLine($"Enter how many employees you want to hire?\nYou have {Math.Round(_program.dollars.Quantity, 2)} dollars");
                            
                            int employeesToHire = _program.GetValidInt(0, 163);
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

                                    _program.HireNewWorker(employeesToHire, "bad");

                                    _program.dollars.Quantity -=
                                        employeesToHire * _program._currentEmployeePrice * 0.5;
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

                                    _program.HireNewWorker(employeesToHire, "mid");

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

                                    _program.HireNewWorker(employeesToHire, "good");

                                    _program.dollars.Quantity -=
                                        employeesToHire * _program._currentEmployeePrice * 2;
                                    Console.WriteLine($"You now have {_program.workersList.Count} total employees");
                                    _program._totalEmployeesHired += employeesToHire;
                                }
                                break;
                            }
                        }

                        break;

                    case 4:
                        Console.WriteLine("Thanks for coming to the market!");
                        break;

                }
            } while (marketOption != 4);
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
            
            Random random = new Random();

            // rain or hurricane reducing efficiency, beautifulSky increasing efficiency

            if (_program.ActiveWeatherEffectsList.Count == 0)
            { 
                // 5% chance a hurricane that reduces the probability of finding resources by 50% for the next 5 days
                if (random.Next(0, 100) < Hurricane.Probability)
                {
                    if (!multipleDaysOrNot)
                    {
                        Console.WriteLine("__________________________________________________________");
                        Console.WriteLine("\ud83c\udf00 A hurricane is coming, efficiency is now 60% less the next four days \ud83c\udf00");
                    }
                    
                    foreach (Worker worker in _program.workersList)
                    {
                        worker.efficiency *= Hurricane.EfficiencyMultiplier;
                    }
                    Hurricane.DaysLeft = 4;
                    _program.ActiveWeatherEffectsList.Add(Hurricane);
                }

                // rain reducing efficiency
                else if (random.Next(0, 100) < Rain.Probability)
                {
                    if (!multipleDaysOrNot)
                    {
                        Console.WriteLine("__________________________________________________________");
                        Console.WriteLine(
                            "\ud83c\udf27\ufe0f Due to torrential rain, your employees are 30% less efficient for the next three days \ud83c\udf27\ufe0f");
                    }
                    
                    foreach (Worker worker in _program.workersList)
                    {
                        worker.efficiency *= Rain.EfficiencyMultiplier;
                    }

                    Rain.DaysLeft = 3;
                    _program.ActiveWeatherEffectsList.Add(Rain);
                }

                // beautiful sky increasing efficiency
                else if (random.Next(0, 100) < BeautifulSky.Probability)
                {
                    if (!multipleDaysOrNot)
                    {
                        Console.WriteLine("__________________________________________________________");
                        Console.WriteLine(
                            "\ud83c\udfd6\ufe0f The weather is beautiful today; your employees are 20% more efficient for three days \ud83c\udfd6\ufe0f");
                    }
                    
                    foreach (Worker worker in _program.workersList)
                    {
                        worker.efficiency *= BeautifulSky.EfficiencyMultiplier;
                    }

                    BeautifulSky.DaysLeft = 3;
                    _program.ActiveWeatherEffectsList.Add(BeautifulSky);
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
                Console.WriteLine("__________________________________________________________");
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
                    Console.WriteLine("__________________________________________________________");
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
                    Console.WriteLine("__________________________________________________________");
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
                    Console.WriteLine("__________________________________________________________");
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
                    Console.WriteLine("__________________________________________________________");
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
                Console.WriteLine("__________________________________________________________");
                Console.WriteLine("\ud83d\udcc6 Profit sharing time! \ud83d\udcc6");

                if (_program.workersList.Count < 7)
                {
                    Console.WriteLine("Each employee gets 10% of your current $$$ stash");
                    Console.WriteLine(
                        $"Your {_program.workersList.Count} employees get ${Math.Round(_program.dollars.Quantity * 0.1, 2)} each");
                    double dollarsToLose = _program.dollars.Quantity * 0.1 * _program.workersList.Count;
                    _program.dollars.Quantity -= dollarsToLose;
                    Console.WriteLine(
                        $"Your employees have been paid, you have lost $ {Math.Round(dollarsToLose, 2)} in the process");
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

            Random random = new Random();

            // recalculate the average efficiency of the employees
            double totalEfficiency = 0;
            foreach (Worker worker in _program.workersList)
            {
                totalEfficiency += worker.efficiency;
            }

            _program._averageEmployeeEfficiency = totalEfficiency / _program.workersList.Count;

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
                    Program._usedNames.Remove(worker.Name);
                    worker.RetirementDate = _program._currentDate.Date;
                    _program.retiredWorkersList.Add(worker);
                    _program.workersList.Remove(worker);
                    if (!multipleDaysOrNot)
                    {
                        Console.WriteLine($"Employee {worker.Name} has retired. Goodbye!");
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
                    
                    _program.trainingWorkersList.Remove(worker);
                    tempList.Add(worker);
                }
            }

            foreach (Worker worker in tempList)
            {
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
                if (worker.IsIll && worker.ReturnToWorkDate == _program._currentDate.Date)
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

        public void CheckAchievements(List<string> achievements, Program _program)
        {

            if (_program.coal.TotalFound >= 100 && !_program._achievement1)
            {
                Console.WriteLine("You've unlocked an achievement: 100kg of coal found milestone");
                _program._achievement1 = true;
                achievements.Add("100kg of coal found");

            }

            if (_program.coal.TotalFound >= 1000 && !_program._achievement2)
            {
                Console.WriteLine("You've unlocked an achievement: 1000kg of coal found milestone");
                _program._achievement2 = true;
                achievements.Add("1000kg of coal found");
            }

            if (_program.coal.TotalFound >= 10000 && !_program._achievement3)
            {
                Console.WriteLine("You've unlocked an achievement: 10000kg of coal found milestone");
                _program._achievement3 = true;
                achievements.Add("10000kg of coal found");
            }

            if (_program.stone.TotalFound >= 100 && !_program._achievement4)
            {
                Console.WriteLine("You've unlocked an achievement: 100kg of stone found milestone");
                _program._achievement4 = true;
                achievements.Add("100kg of stone found");
            }

            if (_program.stone.TotalFound >= 1000 && !_program._achievement5)
            {
                Console.WriteLine("You've unlocked an achievement: 1000kg of stone found milestone");
                _program._achievement5 = true;
                achievements.Add("1000kg of stone found");
            }

            if (_program.stone.TotalFound >= 10000 && !_program._achievement6)
            {
                Console.WriteLine("You've unlocked an achievement: 10000kg of stone found milestone");
                _program._achievement6 = true;
                achievements.Add("10000kg of stone found");
            }

            if (_program.iron.TotalFound >= 75 && !_program._achievement7)
            {
                Console.WriteLine("You've unlocked an achievement: 75kg of iron found milestone");
                _program._achievement7 = true;
                achievements.Add("75kg of iron found");
            }

            if (_program.iron.TotalFound >= 750 && !_program._achievement8)
            {
                Console.WriteLine("You've unlocked an achievement: 750kg of iron found milestone");
                _program._achievement8 = true;
                achievements.Add("750kg of iron found");
            }

            if (_program.iron.TotalFound >= 7500 && !_program._achievement9)
            {
                Console.WriteLine("You've unlocked an achievement: 7500kg of iron found milestone");
                _program._achievement9 = true;
                achievements.Add("7500kg of iron found");
            }

            if (_program.gold.TotalFound >= 30 && !_program._achievement10)
            {
                Console.WriteLine("You've unlocked an achievement: 30kg of gold found milestone");
                _program._achievement10 = true;
                achievements.Add("30kg of gold found");
            }

            if (_program.gold.TotalFound >= 300 && !_program._achievement11)
            {
                Console.WriteLine("You've unlocked an achievement: 300kg of gold found milestone");
                _program._achievement11 = true;
                achievements.Add("300kg of gold found");
            }

            if (_program.gold.TotalFound >= 3000 && !_program._achievement12)
            {
                Console.WriteLine("You've unlocked an achievement: 3000kg of gold found milestone");
                _program._achievement12 = true;
                achievements.Add("3000kg of gold found");
            }

            if (_program.diamond.TotalFound >= 10 && !_program._achievement13)
            {
                Console.WriteLine("You've unlocked an achievement: 10kg of diamond found milestone");
                _program._achievement13 = true;
                achievements.Add("10kg of diamond found");
            }

            if (_program.diamond.TotalFound >= 100 && !_program._achievement14)
            {
                Console.WriteLine("You've unlocked an achievement: 100kg of diamond found milestone");
                _program._achievement14 = true;
                achievements.Add("100kg of diamond found");
            }

            if (_program.diamond.TotalFound >= 1000 && !_program._achievement15)
            {
                Console.WriteLine("You've unlocked an achievement: 1000kg of diamond found milestone");
                _program._achievement15 = true;
                achievements.Add("1000kg of diamond found");
            }

            if (_program._totalDollarsEarned >= 300 && !_program._achievement16)
            {
                Console.WriteLine("You've unlocked an achievement: $300 earned milestone");
                _program._achievement16 = true;
                achievements.Add("$300 earned");
            }

            if (_program._totalDollarsEarned >= 1000 && !_program._achievement17)
            {
                Console.WriteLine("You've unlocked an achievement: $1000 earned milestone");
                _program._achievement17 = true;
                achievements.Add("$1000 earned");
            }

            if (_program._totalDollarsEarned >= 10000 && !_program._achievement18)
            {
                Console.WriteLine("You've unlocked an achievement: $10000 earned milestone");
                _program._achievement18 = true;
                achievements.Add("$10000 earned");
            }

            if (_program._totalEmployeesHired >= 10 && !_program._achievement19)
            {
                Console.WriteLine("You've unlocked an achievement: 10 employees hired milestone");
                _program._achievement19 = true;
                achievements.Add("10 employees hired");
            }

            if (_program._totalEmployeesHired >= 100 && !_program._achievement20)
            {
                Console.WriteLine("You've unlocked an achievement: 100 employees hired milestone");
                _program._achievement20 = true;
                achievements.Add("100 employees hired");
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
                        // ReSharper disable once PossibleNullReferenceException
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
                        _program.achievementsList = (List<string>)entry.Value;
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

    }

    class Program
    {
        # region global variables

        public bool _animation = true;
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
        public double _currentEmployeeIllProbability = 5;
        public double _currentEmployeePrice = 100;
        public DateTime _currentDate = new DateTime(2024, 1, 1);
        public static Random _random = new();
        public int _crashDate = _random.Next(0, 28);

        public List<Worker> illWorkersList = new();
        public List<Worker> retiredWorkersList = new();
        public List<Worker> trainingWorkersList = new();
        public List<Worker> toSendToTrainingList = new();
        
        public List<WeatherEffectsClass> ActiveWeatherEffectsList = new();

        public List<Worker> workersList = new()
            { new Worker("mid", "Bob Smith The OG Worker", 10, 100, 10, 1) };

        MiningOperation miningOperation = new MiningOperation();
        MarketOperation marketOperation = new MarketOperation();
        DayToDayOperations dayToDayOperations = new DayToDayOperations();

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

        public List<string> achievementsList = new List<string>();

        // 307 possible names for the workers
        // to stop screaming at me for names it doesn't recognise/think are typos
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private static List<string> _possibleNames = new List<string>()
        {
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

        public static List<string> _usedNames = new ();

        # endregion

        public static void Main()
        {
            Program program = new Program();

            # region initialisation of all objects

            program.coal = new Resource("Coal", 80, 3, 0, 0);
            program.stone = new Resource("Stone", 70, 6, 0, 0);
            program.iron = new Resource("Iron", 60, 13, 0, 0);
            program.gold = new Resource("Gold", 15, 65, 0, 0);
            program.diamond = new Resource("Diamond", 3, 200, 0, 0);
            program.dollars = new Resource("Dollars", 0, 0, 100, 0);
            program.magicTokens = new PowerUp(0, 6, 3);
            program.timeMachine = new PowerUp(0, 3, 3);
            program.ancientArtefact = new PowerUp(0, 7, 3);
            program.marketMaster = new PowerUp(0, 4, 3);
            program.stockMarketCrash = new PayForStuff(100);
            program.skipDay = new PayForStuff(50);
            program.bribe = new PayForStuff(200);
            program.trainingCourse = new PayForStuff(400);

            program.coalToStone = new Trade(2 * (_random.Next(80,100)/100), program.coal, program.stone);
            program.coalToIron = new Trade(5 * (_random.Next(80,100)/100), program.coal, program.iron);
            program.coalToGold = new Trade(12 * (_random.Next(80,100)/100), program.coal, program.gold);
            program.coalToDiamond = new Trade(70 * (_random.Next(80,100)/100), program.coal, program.diamond);
            program.stoneToIron = new Trade(3 * (_random.Next(80,100)/100), program.stone, program.iron);
            program.stoneToGold = new Trade(13 * (_random.Next(80,100)/100), program.stone, program.gold);
            program.stoneToDiamond = new Trade(40 * (_random.Next(80,100)/100), program.stone, program.diamond);
            program.ironToGold = new Trade(5 * (_random.Next(80,100)/100), program.iron, program.gold);
            program.ironToDiamond = new Trade(16 * (_random.Next(80,100)/100), program.iron, program.diamond);
            program.goldToDiamond = new Trade(3 * (_random.Next(80,100)/100), program.gold, program.diamond);

            # endregion

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("   _____           _       _        _____    _                                            \n  / ____|         | |     | |      |  __ \\  (_)                                           \n | |  __    ___   | |   __| |      | |  | |  _    __ _    __ _    ___   _ __   ____  ____ \n | | |_ |  / _ \\  | |  / _` |      | |  | | | |  / _` |  / _` |  / _ \\ | '__| |_  / |_  / \n | |__| | | (_) | | | | (_| |      | |__| | | | | (_| | | (_| | |  __/ | |     / /   / /  \n  \\_____|  \\___/  |_|  \\__,_|      |_____/  |_|  \\__, |  \\__, |  \\___| |_|    /___| /___| \n                                                  __/ |   __/ |                           \n                                                 |___/   |___/                            \n");
            Console.ResetColor();

            Console.WriteLine("The aim of the game is to survive for as long as possible before bankruptcy");
            Console.WriteLine("These are your initial resources...");
            Thread.Sleep(1750);

            DisplayStuff.DisplayResources(program);

            Thread.Sleep(1750);
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
                        miningOperation.Dig(1, this, dayToDayOperations, achievementsList);
                        break;
                    case 2:
                        _animation = false;
                        Console.WriteLine("\n  __  __           _   _     _           _             _____                       _____    _         \n |  \\/  |         | | | |   (_)         | |           |  __ \\                     |  __ \\  (_)        \n | \\  / |  _   _  | | | |_   _   _ __   | |   ___     | |  | |   __ _   _   _     | |  | |  _    __ _ \n | |\\/| | | | | | | | | __| | | | '_ \\  | |  / _ \\    | |  | |  / _` | | | | |    | |  | | | |  / _` |\n | |  | | | |_| | | | | |_  | | | |_) | | | |  __/    | |__| | | (_| | | |_| |    | |__| | | | | (_| |\n |_|  |_|  \\__,_| |_|  \\__| |_| | .__/  |_|  \\___|    |_____/   \\__,_|  \\__, |    |_____/  |_|  \\__, |\n                                | |                                      __/ |                   __/ |\n                                |_|                                     |___/                   |___/ \n");
                        Console.WriteLine("Enter number of days to dig in one go (upto 30)");
                        int daysToDig = GetValidInt(1, 30);
                        miningOperation.Dig(daysToDig, this, dayToDayOperations, achievementsList);
                        break;
                    case 3:
                        marketOperation.GoToMarket(this);
                        break;
                    case 4:
                        GoToTrader();
                        break;
                    case 5:
                        DisplayStuff.DisplayGameMechanics(this);
                        break;
                    case 6:
                        DisplayStuff.DisplayStats(this);
                        break;
                    case 7:
                        for (int achievementNumber = 0;
                             achievementNumber < achievementsList.Count;
                             achievementNumber++)
                        {
                            Console.WriteLine(
                                $"Achievement {achievementNumber}: {achievementsList[achievementNumber]}");
                        }

                        break;
                    case 8:
                        RunTutorial();
                        break;
                    case 9:
                        DisplayStuff.DisplayEmployees(this);
                        break;
                    case 10:
                        Console.WriteLine("Skipping one day");
                        Console.WriteLine(
                            $"You have been charged ${skipDay.Price} for the costs of skipping a day");
                        dollars.Quantity -= skipDay.Price;
                        skipDay.skipDayOrNot = true;
                        miningOperation.Dig(1, this, dayToDayOperations, achievementsList);
                        DisplayStuff.DisplayResources(this);
                        break;
                    case 11:

                        if (ancientArtefact.Quantity == 0 && timeMachine.Quantity == 0 &&
                            marketMaster.Quantity == 0)
                        {
                            Console.WriteLine("\u274c You don't have any powerups to use \u274c");
                            break;
                        }

                        Console.WriteLine("What powerup do you want to use?");
                        Console.WriteLine(
                            $"You have {ancientArtefact.Quantity} Ancient Artefacts, {timeMachine.Quantity} Time Machines and {marketMaster.Quantity} Market Masters\n");
                        Console.WriteLine("0 - Cancel & Return");
                        Console.WriteLine("1 - Ancient Artefact --> 2 options inside");
                        Console.WriteLine(
                            "2 - Time Machine --> Gain 5 days' worth of rewards without costing you anything");
                        Console.WriteLine(
                            "3 - Market Master --> 50% increase in the selling price of all resources for 3 days");
                        int powerUpChoice = GetValidInt(0, 3);

                        switch (powerUpChoice)
                        {
                            case 1:
                                if (ancientArtefact.Quantity >= 0)
                                {
                                    Console.WriteLine(
                                        "You have chosen to use an Ancient Artefact. Choose an option:");
                                    Console.WriteLine(
                                        "1 - Increase the probability of finding gold to 50% for 3 days");
                                    Console.WriteLine("2 - $200 instantly");
                                    int ancientArtefactChoice = GetValidInt(1, 2);
                                    UsePowerUp(powerUpChoice, ancientArtefactChoice);
                                }
                                else
                                {
                                    Console.WriteLine("You don't have any Ancient Artefacts to use");
                                }

                                break;

                            case 2:

                                if (timeMachine.Quantity >= 0)
                                {
                                    UsePowerUp(powerUpChoice, 0);
                                }
                                else
                                {
                                    Console.WriteLine("You don't have any Time Machines to use");
                                }

                                break;

                            case 3:
                                if (marketMaster.Quantity >= 0)
                                {
                                    UsePowerUp(powerUpChoice, 0);
                                }
                                else
                                {
                                    Console.WriteLine("You don't have any Market Masters to use");
                                }

                                break;
                        }

                        break;
                    case 12:
                        Console.WriteLine("\n   _____                      _       ______                      _______                  _           _                 \n  / ____|                    | |     |  ____|                    |__   __|                (_)         (_)                \n | (___     ___   _ __     __| |     | |__      ___    _ __         | |     _ __    __ _   _   _ __    _   _ __     __ _ \n  \\___ \\   / _ \\ | '_ \\   / _` |     |  __|    / _ \\  | '__|        | |    | '__|  / _` | | | | '_ \\  | | | '_ \\   / _` |\n  ____) | |  __/ | | | | | (_| |     | |      | (_) | | |           | |    | |    | (_| | | | | | | | | | | | | | | (_| |\n |_____/   \\___| |_| |_|  \\__,_|     |_|       \\___/  |_|           |_|    |_|     \\__,_| |_| |_| |_| |_| |_| |_|  \\__, |\n                                                                                                                    __/ |\n                                                                                                                   |___/ \n");
                        Console.WriteLine(
                            $"Enter number of employees to send on training\nEnter -1 to send all employees\nYou have {workersList.Count} employees");
                        int employeesToSend = GetValidInt(-1, workersList.Count);
                        if (employeesToSend == -1)
                        {
                            employeesToSend = workersList.Count;
                        }

                        if (dollars.Quantity > trainingCourse.Price * employeesToSend && workersList.Count != 0)
                        {
                            Console.WriteLine(
                                $"You have chosen to send {employeesToSend} employees on a training course");
                            Console.WriteLine($"You have been charged {trainingCourse.Price} per employee");
                            Console.WriteLine($"Your {employeesToSend} employees will be back in 7 days");
                            EmployeeTrainingCourse(employeesToSend);
                        }
                        else if (dollars.Quantity > trainingCourse.Price * employeesToSend &&
                                 workersList.Count == 0)
                        {
                            Console.WriteLine("You don't have any employees to send on a training course");
                            Console.WriteLine("This could be because of employee illness - try again later");
                        }
                        else
                        {
                            Console.WriteLine(
                                $"You don't have enough money to send {employeesToSend} employees on a training course");
                        }

                        break;
                    case 13:
                        Console.WriteLine("\n   _____                                   _   _                           _____          _                    \n  / ____|                                 (_) | |            /\\           / ____|        (_)                   \n | |        ___    _ __ ___    _ __ ___    _  | |_          /  \\         | |       _ __   _   _ __ ___     ___ \n | |       / _ \\  | '_ ` _ \\  | '_ ` _ \\  | | | __|        / /\\ \\        | |      | '__| | | | '_ ` _ \\   / _ \\\n | |____  | (_) | | | | | | | | | | | | | | | | |_        / ____ \\       | |____  | |    | | | | | | | | |  __/\n  \\_____|  \\___/  |_| |_| |_| |_| |_| |_| |_|  \\__|      /_/    \\_\\       \\_____| |_|    |_| |_| |_| |_|  \\___|\n");
                        Console.WriteLine("You've chosen to commit a crime. Choose an option:");
                        Console.WriteLine("0 - NO IM SCARED OF GOING TO JAIL... CANCEL!!!");
                        Console.WriteLine($"1 - Pay ${stockMarketCrash.Price} for information on the next stock market crash");
                        Console.WriteLine($"2 - Bribe the government for ${bribe.Price} to not pay wages for the next 3 days");
                        int crimeChoice = GetValidInt(0, 2);

                        switch (crimeChoice)
                        {
                            case 1:
                                Console.WriteLine($"You have chosen to pay ${stockMarketCrash.Price} for information on the next stock market crash");
                                dollars.Quantity -= stockMarketCrash.Price;
                                Console.WriteLine("Giving you the information now...");
                                Console.WriteLine($"Expect a stock market crash on the {_crashDate}th of every month");
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
                                }
                                else
                                {
                                    Console.WriteLine("\u274c You can't afford to do this broke \u274c");
                                }

                                break;

                                

                                
                                
                        }

                        break;
                    case 14:
                        SaveGameState(1);
                        break;
                    case 15:
                        Console.WriteLine(
                            "This feature does not fully work yet. I'll let it run just cuz, but whenever its done its thing it'll take you back to the main menu screen");
                        SaveGameState(2);
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
            Console.WriteLine(
                "You are a gold digger, and you have to survive for as long as possible before bankruptcy");
            Console.WriteLine("You have a few resources to start with:");
            Console.WriteLine(
                "You have $100, 0kg of coal, 0kg of iron, 0kg of gold, 0kg stone, 0kg diamond and 1 employee");
            Console.WriteLine("You can hire more employees, dig for resources, and sell resources at the market");
            Console.WriteLine("You can also bribe the government to not pay wages for the next three days");
            Console.WriteLine(
                $"You can also pay ${stockMarketCrash.Price} for information on the next stock market crash");
            Console.WriteLine(
                "You can also send all employees for a training course for $400 per employee (+30% efficiency) (7 days)");
            Console.WriteLine("You can also sell all your iron and gold for dollars");
            Console.WriteLine($"You can also skip one day for ${skipDay.Price}");
            Console.WriteLine("You can also quit the game");
            Console.WriteLine("You can also dig for multiple days");
            Console.WriteLine("Here are the game mechanics:");
            Console.WriteLine("THIS TUTORIAL IS AWFUL, I'M SORRY; I'M WORKING ON IT\n");
        }

        public int UserMenuOption()
        {
            string takeUserInput = CheckIfInDebt();

            if (takeUserInput == "false")
            {
                Console.WriteLine($"Today is {_currentDate:dddd, d MMMM, yyyy}");
                Console.WriteLine("_____________________________________________________________________________________________________________");
                Console.WriteLine("Main Features:              Display Options:                  Other Features:                                |");
                Console.WriteLine("                                                                                                             |");
                Console.WriteLine("0 - Quit game               5 - Display game mechanics        10 - Skip one day                              |");
                Console.WriteLine("1 - Dig one day             6 - Display stats                 11 - Use a powerup                             |");
                Console.WriteLine("2 - Dig multiple days       7 - Display achievements          12 - Send employees for training               |");
                Console.WriteLine("3 - Go to market            8 - Display tutorial              13 - Commit a crime (further options inside)   |");
                Console.WriteLine("4 - Go To Trader            9 - Display employees             14 - Save current progress                (15) |\n");
                Console.WriteLine("Enter your choice:");

                int userOption = GetValidInt(0, 15);
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


                if (inDebt == "true")
                {
                    Console.WriteLine("\n\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31\ud83d\ude31");
                    Thread.Sleep(2000);
                    Console.WriteLine("You are in debt, bossman is coming for you");
                    Console.WriteLine(
                        "The government will come and sell all your resources for 40% the current market rate...");
                    Thread.Sleep(1500);
                    Console.WriteLine(
                        $"Right now you have ${Math.Round(dollars.Quantity), 2}, {Math.Round(coal.Quantity), 2} coal, {Math.Round(stone.Quantity), 2}stone, {Math.Round(iron.Quantity), 2} iron, {Math.Round(gold.Quantity), 2} gold, and {Math.Round(diamond.Quantity), 2} diamond");
                    Console.WriteLine("Unlucky bro...");
                    Thread.Sleep(750);
                    Console.WriteLine("After bossman stole your resources, you now have:");

                    double changeInDollars = (coal.Quantity * coal.Price + stone.Quantity * stone.Price +
                                              iron.Quantity * iron.Price + gold.Quantity * gold.Price +
                                              diamond.Quantity * diamond.Price) * 0.4;
                    dollars.Quantity += changeInDollars;

                    _totalDollarsEarned += changeInDollars;

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
                    Console.WriteLine(
                        "You don't have resources to sell, so we're selling workers for $50 per guy.");
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

        public void UsePowerUp(int powerUpChoice, int subChoice)
        {
            switch (powerUpChoice)
            {
                case 1:
                {
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

                    ancientArtefact.Quantity -= 1;
                    break;
                }

                case 2:
                {
                    Console.WriteLine("You have chosen to use the Time Machine powerup");
                    _noWageDaysLeft = 10;
                    _animation = false;
                    miningOperation.Dig(5, this, dayToDayOperations, achievementsList);
                    timeMachine.Quantity -= 1;
                    break;
                }

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

                    marketMaster.Quantity -= 1;
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
        }

        public void HireNewWorker(int numberOfWorkers, string type)
        {
            
            for (int i = 0; i < numberOfWorkers; i++)
            {
                double efficiency = 15;

                if (_possibleNames.Count > 1)
                {
                    int randomName = _random.Next(0, _possibleNames.Count);

                    // making 'levels' of efficiency based on the number of employees
                    // this is to make the game harder over time as the player hires more employees
                    if (workersList.Count > 0)
                    {
                        efficiency = _random.Next(70, 130);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 3)
                    {
                        efficiency = _random.Next(65, 125);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 6)
                    {
                        efficiency = _random.Next(65, 125);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 9)
                    {
                        efficiency = _random.Next(60, 120);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 12)
                    {
                        efficiency = _random.Next(55, 115);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 15)
                    {
                        efficiency = _random.Next(50, 110);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 18)
                    {
                        efficiency = _random.Next(45, 105);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 21)
                    {
                        efficiency = _random.Next(40, 100);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 24)
                    {
                        efficiency = _random.Next(35, 95);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 27)
                    {
                        efficiency = _random.Next(30, 90);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 30)
                    {
                        efficiency = _random.Next(25, 85);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 33)
                    {
                        efficiency = _random.Next(20, 80);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 36)
                    {
                        efficiency = _random.Next(15, 75);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 39)
                    {
                        efficiency = _random.Next(10, 70);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 42)
                    {
                        efficiency = _random.Next(5, 65);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 45)
                    {
                        efficiency = _random.Next(0, 60);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 28)
                    {
                        efficiency = _random.Next(0, 55);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 51)
                    {
                        efficiency = _random.Next(0, 50);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 54)
                    {
                        efficiency = _random.Next(0, 45);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 57)
                    {
                        efficiency = _random.Next(0, 40);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 60)
                    {
                        efficiency = _random.Next(0, 35);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 63)
                    {
                        efficiency = _random.Next(0, 30);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 66)
                    {
                        efficiency = _random.Next(0, 25);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 69)
                    {
                        efficiency = _random.Next(0, 20);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 72)
                    {
                        efficiency = _random.Next(0, 15);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 75)
                    {
                        efficiency = _random.Next(0, 10);
                        efficiency /= 100;
                    }
                    else if (workersList.Count > 78)
                    {
                        efficiency = _random.Next(1, 5);
                        efficiency /= 100;
                    }

                    double employeePrice = 0;

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
                    
                    Worker newWorker = new Worker(type, _possibleNames[randomName], _currentWageRate, employeePrice, _currentEmployeeIllProbability, efficiency);
                    workersList.Add(newWorker);
                    _usedNames.Add(newWorker.Name);
                    _possibleNames.Remove(newWorker.Name);
                    Console.WriteLine($"{newWorker.Name}, Efficiency {Math.Round(newWorker.efficiency, 2)}\ud83e\uddcd\u200d\u2642\ufe0f");

                    // updating bribe price
                    bribe.Price = _currentWageRate * workersList.Count * 2;
                }
                
                else
                {
                    Console.WriteLine("You've hired all 307/307 available employees and so you've run out of names to give to your employees \ud83d\ude2d");
                    Console.WriteLine("Wait for some to retire");
                    break;
                }
            }
            
        }

        public void GoToTrader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(
                "             _______                      _               \n            |__   __|                    | |              \n               | |     _ __    __ _    __| |   ___   _ __ \n               | |    | '__|  / _` |  / _` |  / _ \\ | '__|\n               | |    | |    | (_| | | (_| | |  __/ | |   \n               |_|    |_|     \\__,_|  \\__,_|  \\___| |_|");
            Console.ResetColor();
            
            DisplayStuff.DisplayResources(this);
             
            Console.WriteLine("Here are the options for today:\n");
            Console.WriteLine("0 - Cancel and return");
            Console.WriteLine($"1 - Convert {Math.Round(coalToStone.Ratio,2)} coal --> stone");
            Console.WriteLine($"2 - Convert {Math.Round(coalToIron.Ratio, 2)} coal --> iron");
            Console.WriteLine($"3 - Convert {Math.Round(coalToGold.Ratio, 2)} coal --> gold");
            Console.WriteLine($"4 - Convert {Math.Round(coalToDiamond.Ratio, 2)} coal --> diamond");
            Console.WriteLine($"5 - Convert {Math.Round(stoneToIron.Ratio, 2)} stone --> iron");
            Console.WriteLine($"6 - Convert {Math.Round(stoneToGold.Ratio, 2)} stone --> gold");
            Console.WriteLine($"7 - Convert {Math.Round(stoneToDiamond.Ratio, 2)} stone --> diamond");
            Console.WriteLine($"8 - Convert {Math.Round(ironToGold.Ratio, 2)} iron --> gold");
            Console.WriteLine($"9 - Convert {Math.Round(ironToDiamond.Ratio, 2)} iron --> diamond");
            Console.WriteLine($"10 - Convert {Math.Round(goldToDiamond.Ratio, 2)} gold --> diamond");
            Console.WriteLine("Remember, you can only make one trade per day. Choose wisely!");
            Console.WriteLine("_________________________________________");

            int userTrade = GetValidInt(0, 10);

            switch (userTrade)
            {
                case 0:
                    break;
                case 1:
                    Console.WriteLine($"You've chosen to convert {coalToStone.Ratio}kg coal for stone");
                    Trade.MakeTrade(coalToStone.Ratio, coal, stone, _currentDate);
                    break;
                case 2:
                    Console.WriteLine($"You've chosen to convert {coalToIron.Ratio}kg coal for iron");
                    Trade.MakeTrade(coalToIron.Ratio, coal, iron, _currentDate);
                    break;
                case 3:
                    Console.WriteLine($"You've chosen to convert {coalToGold.Ratio}kg coal for gold");
                    Trade.MakeTrade(coalToGold.Ratio, coal, gold, _currentDate);
                    break;
                case 4:
                    Console.WriteLine($"You've chosen to convert {coalToDiamond.Ratio}kg coal for diamond");
                    Trade.MakeTrade(coalToDiamond.Ratio, coal, diamond, _currentDate);
                    break;
                case 5:
                    Console.WriteLine($"You've chosen to convert {stoneToIron.Ratio}kg stone for iron");
                    Trade.MakeTrade(stoneToIron.Ratio, stone, iron, _currentDate);
                    break;
                case 6:
                    Console.WriteLine($"You've chosen to convert {stoneToGold.Ratio}kg stone for gold");
                    Trade.MakeTrade(stoneToGold.Ratio, stone, gold, _currentDate);
                    break;
                case 7:
                    Console.WriteLine($"You've chosen to convert {stoneToDiamond.Ratio}kg stone for diamond");
                    Trade.MakeTrade(stoneToDiamond.Ratio, stone, diamond, _currentDate);
                    break;
                case 8:
                    Console.WriteLine($"You've chosen to convert {ironToGold.Ratio}kg iron for gold");
                    Trade.MakeTrade(ironToGold.Ratio, iron, gold, _currentDate);
                    break;
                case 9:
                    Console.WriteLine($"You've chosen to convert {ironToDiamond.Ratio}kg gold for diamond");
                    Trade.MakeTrade(ironToDiamond.Ratio, iron, diamond, _currentDate);
                    break;
                case 10:
                    Console.WriteLine($"You've chosen to convert {goldToDiamond.Ratio}kg gold for diamond");
                    Trade.MakeTrade(goldToDiamond.Ratio, gold, diamond, _currentDate);
                    break;
            }

            DisplayStuff.DisplayResources(this);
            Console.WriteLine("Thanks for coming to the trader!\nCome later for updated rates!");
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
}
