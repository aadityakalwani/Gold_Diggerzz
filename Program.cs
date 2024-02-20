using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Gold_Diggerzz
{
    // as of sunday 18feb 1pm, 27hours 45 minutes spent on digging sim as of google calendar
    // initial inspiration: https://replit.com/@AadityaKalwani/Digging-Simulator#main.py

    /*
    * current issues
    * inconsistent between weather effect Displaying and actual
       * eg "6 days left of bad weather" but then it's only 5 days
    */

    /* to-do ideas
     * a list of all possible trades, foreach trade, if the player has enough of the fromResource, display the trade option?
     * OOP the weather effects
     * 3 types of workers, bad, mid and good with different probabilities of being ill, wages, efficiencies, retiring times etc.
     *  employee morale, if morale is low, the employee could be less efficient.
        * morale-boosting powerup
     * create morale and reputation
     * Resource Quality: Different quality levels for resources, which affect their price and the chance of finding them. Higher quality resources could be found less frequently but sold for a higher price.
     * the player could choose to offer them a retirement package in exchange for a morale boost for the remaining workers.
     * Allow employees to specialize in certain areas, making them more efficient at gathering certain resources. This could add another layer of strategy to the game as players decide how to best allocate their workforce.
     * Resource Discovery: Add a feature where players can discover new resources as they dig deeper. These new resources could be more valuable but also more difficult to extract.
     * a 'mine emptiness', where the player has to move to a new mine and start again (acting as prestige)
        * as the mine gets emptier, chance of finding resources decreases 
     * Achievements and Rewards: Implement more achievements and provide rewards for achieving them. This could be in the form of in-game currency, special power-ups, or even new gameplay features.
     * Exploration: Allow the player to explore new areas or mines. This could involve a risk/reward system where exploring new areas could lead to finding more valuable resources but also has the potential for more dangerous events.
     * Trader's prices fluctuate (one of the factors can be reputation)
     * Introduce Difficulty Levels: You can introduce difficulty levels that the player can choose at the start of the game.
     * Environmental Impact: Implement an environmental impact system where the player's mining operations could have negative effects on the environment, which could lead to penalties or restrictions.
     * Corporate Espionage: Introduce a system where the player can engage in corporate espionage to gain an advantage over their competitors. This could involve risks of getting caught and facing penalties.
        * Higher difficulty levels can have more frequent negative events, higher costs, and lower probabilities of finding resources.
     * achievements are OOP-ed? idk about this one
     * reorder the menu options to be more flowy and logical
     * earthquakes that loosen soil and make shit easier to find (+ cool animations possible)
     * a "mine collapse" event could temporarily reduce the player's digging efficiency and kill some employees
     * a heatwave could decrease efficiency but increase the chance of finding certain resources.
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
     * send individual number of employees for training course that then boosts their productivity
     */
    
    class Resource
    {
        public string ResourceName;
        public double InitialPrice;
        public double Probability;
        public double Price;
        public double Quantity;
        public double TotalFound;

        public Resource(string resourceName, double initialProbability, double initialPrice, double initialQuantity, double totalFound)
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
        public double DefaultEfficiency;
        public int DaysUntilRetirement;
        public DateTime RetirementDate;
        public bool IsIll;
        public DateTime ReturnToWorkDate;
        
        public Worker(string type,string name, double wage, double price, double employeeIllProbability, double defaultEfficiency)
        {
            if (type == "mid")
            {
                Type = "mid";
                Name = name;
                Wage = wage;
                Price = price;
                EmployeeIllProbability = employeeIllProbability;
                DefaultEfficiency = defaultEfficiency;
                DaysUntilRetirement = 30;
                IsIll = false;
                HireDate = DateTime.Today;
                ReturnToWorkDate = DateTime.Today;
            }
            
            else if (type == "bad")
            {
                Type = "bad";
                Name = name;
                Wage = wage * 0.5;
                Price = price * 0.5;
                EmployeeIllProbability = employeeIllProbability * 2;
                DefaultEfficiency = defaultEfficiency * 0.5;
                DaysUntilRetirement = 30;
                IsIll = false;
                HireDate = DateTime.Today;
                ReturnToWorkDate = DateTime.Today;
            }
            
            else if (type == "good")
            {
                Type = "good";
                Name = name;
                Wage = wage * 2;
                Price = price * 2;
                EmployeeIllProbability = employeeIllProbability * 0.5;
                DefaultEfficiency = defaultEfficiency * 2;
                DaysUntilRetirement = 20;
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
            Console.WriteLine($"| You have {program.workersList.Count} employees         | Your employees' average efficiency is {Math.Round(program._averageEmployeeEfficiency, 2)}");
            Console.WriteLine("_____________________________________________________________________");
        }
        
        public static void DisplayEmployees(Program program)
        {
            Console.WriteLine("_____________________________________________________________________");
            int i = 0;
            foreach (Worker worker in program.retiredWorkersList)
            {
                i++;
                Console.WriteLine($"Retiree Number {i} - {worker.Name}, Efficiency {Math.Round(worker.DefaultEfficiency, 2)}, Retired on {worker.RetirementDate} days \ud83e\uddcd\u200d\u2642\ufe0f");
            }
            Console.WriteLine("Here are your current working employees:");
            int j = 0;
            foreach (Worker worker in program.workersList)
            {
                j++;
                Console.WriteLine($"Employee Number {j} - {worker.Name}, Efficiency {Math.Round(worker.DefaultEfficiency, 2)}, Retiring in {worker.DaysUntilRetirement} days \ud83e\uddcd\u200d\u2642\ufe0f");
            }
            Console.WriteLine();
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
        
        public static void MakeTrade(double ratio, Resource fromResource, Resource toResource, DateTime _currentDate)
        {
            if (!datesOfTradesMade.Contains(_currentDate))
            {
                if (ratio * fromResource.Quantity > toResource.Quantity)
                {
                    fromResource.Quantity -= ratio;
                    toResource.Quantity += 1;
                
                    Console.WriteLine($"Trade Complete! You traded {ratio}kg of {fromResource.ResourceName} for 1kg of {toResource.ResourceName}");
                    datesOfTradesMade.Add(DateTime.Today);
                }
                Console.WriteLine("You can't afford to make this trade brokie");
                return;
            }
            Console.WriteLine("You've already made a trade today, try again later \ud83d\udc4b ");
        }
    }

    class MiningOperation
    {
        public void Dig(int daysToDig, Program _program, DayToDayOperations _DayToDayOperations, List<string> achievements)

        {
            for (int days = 0; days < daysToDig; days++)
            {
                if (_program.CheckIfInDebt() != "true")
                {
                    if (!_program.skipDay.skipDayOrNot)
                    {
                        if (_program._animation)
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

                        Console.WriteLine($"Digging done for the day {_program._currentDate.Date:dddd, dd MMMM, yyyy}");
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
                        if (_program._increasedGoldChanceDays != 0)
                        {
                            Console.WriteLine(
                                $"You have the Ancient Artefact powerup, you have a 50% chance of finding gold for the next {_program._increasedGoldChanceDays} days");
                            _program.gold.Probability = 50;
                            _program._increasedGoldChanceDays -= 1;
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

                        // update values within the resources dictionary

                        double newResourcesFound = _program.workersList.Count * _program._averageEmployeeEfficiency;

                        if (coalFound)
                        {
                            Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of coal \ud83e\udea8");
                            _program.coal.Quantity += newResourcesFound;
                            _program.coal.TotalFound += newResourcesFound;
                        }

                        if (stoneFound)
                        {
                            Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of stone \ud83e\udea8");
                            _program.stone.Quantity += newResourcesFound;
                            _program.stone.TotalFound += newResourcesFound;
                        }

                        if (ironFound)
                        {
                            Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of iron \ud83e\uddbe ");
                            _program.iron.Quantity += newResourcesFound;
                            _program.iron.TotalFound += newResourcesFound;
                        }

                        if (goldFound)
                        {
                            Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of gold \ud83d\udc51");
                            _program.gold.Quantity += newResourcesFound;
                            _program.gold.TotalFound += newResourcesFound;
                        }

                        if (diamondFound)
                        {
                            Console.WriteLine($"You found {Math.Round(newResourcesFound, 2)}kg of diamond \ud83d\udc8e");
                            _program.diamond.Quantity += newResourcesFound;
                            _program.diamond.TotalFound += newResourcesFound;
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
                            Console.WriteLine($"2 - Save for later (max {_program.ancientArtefact.MaxQuantity})");
                            int userInput = _program.GetValidInt(1, 2);

                            switch (userInput)
                            {
                                case 1:
                                    _program.UsePowerUp(1);
                                    break;
                                case 2:
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
                            Console.Write("\u23f3 You found the Time Machine power-up \u23f3");
                            Console.WriteLine("Choose an option:");
                            Console.WriteLine("1 - Use now");
                            Console.WriteLine($"2 - Save for later (max {_program.timeMachine.MaxQuantity})");
                            int userInput = _program.GetValidInt(1, 2);

                            switch (userInput)
                            {
                                case 1:
                                    _program.UsePowerUp(2);
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
                            _program.magicTokens.Quantity += 1;
                            Console.WriteLine($"You've acquired another magic token. You have {_program.magicTokens.Quantity} magic tokens now");
                            Console.WriteLine($"Selling price increased by a total of {_program.magicTokens.Quantity * 20}%");
                            _program.coal.Price += _program.coal.InitialPrice * 0.2;
                            _program.stone.Price += _program.stone.InitialPrice * 0.2;
                            _program.iron.Price += _program.iron.InitialPrice * 0.2;
                            _program.gold.Price += _program.gold.InitialPrice * 0.2;
                            _program.diamond.Price += _program.diamond.InitialPrice * 0.2;
                        }

                        if (marketMasterFound)
                        {
                            Console.WriteLine("You found the Market Master power up");
                            Console.WriteLine("Choose an option:");
                            Console.WriteLine("1 - Use now");
                            Console.WriteLine($"2 - Save for later (max {_program.marketMaster.MaxQuantity})");
                            int userInput = _program.GetValidInt(1, 2);

                            switch (userInput)
                            {
                                case 1:
                                    _program.UsePowerUp(3);
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
                    Console.WriteLine("Here are the current active effects affecting your game:");

                    if (_program._noWageDaysLeft != 0)
                    {
                        Console.WriteLine($"You don't have to pay wages today, or for the next {_program._noWageDaysLeft} days");
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

                        Console.WriteLine($"Your {_program.workersList.Count} employees charged a wage of ${Math.Round(totalWages, 2)} today.");
                    }

                    if (_program._badWeatherDaysLeft > 0)
                    {
                        _program._badWeatherDaysLeft -= 1;
                        Console.WriteLine($"{_program._badWeatherDaysLeft} days left of torrential rain");
                    }

                    if (_program._hurricaneDaysLeft > 0)
                    {
                        _program._hurricaneDaysLeft -= 1;
                        Console.WriteLine($"{_program._hurricaneDaysLeft} days left of hurricane");
                    }

                    if (_program._beautifulSkyDaysLeft > 0)
                    {
                        _program._beautifulSkyDaysLeft -= 1;
                        Console.WriteLine($"{_program._beautifulSkyDaysLeft} days left of beautiful weather");
                    }

                    if (_program._marketMasterDaysLeft == 1)
                    {
                        _program._marketMasterDaysLeft = 0;
                        Console.WriteLine("Your Market Master powerup is no longer active");
                    }

                    else if (_program._marketMasterDaysLeft > 1)
                    {
                        Console.WriteLine($"{_program._marketMasterDaysLeft} days left of the Market Master powerup");
                        _program._marketMasterDaysLeft -= 1;
                    }

                    _program._currentDate = _program._currentDate.AddDays(1);
                }
                
                _program._totalDaysDug += 1;

                if (daysToDig >= 2)
                {
                    Console.WriteLine($"Current balance = {_program.dollars.Quantity}");
                    Console.WriteLine($"There are {daysToDig - days - 1} days left to dig");
                }

                // post-digging effects
                _DayToDayOperations.CalendarEffects(_program);
                _DayToDayOperations.WeatherEffects(_program);
                _DayToDayOperations.DealWithEmployees(_program);
                _DayToDayOperations.FluctuatePrices(_program);

                Console.WriteLine("___________________________________");
            }

            _DayToDayOperations.CheckAchievements(achievements, _program);

            Console.WriteLine($"After {daysToDig} days of digging, here are your updated resources:");
            DisplayStuff.DisplayResources(_program);
            
            _program.skipDay.skipDayOrNot = false;
        }
    }

    class MarketOperation
    {
        public void GoToMarket(Program _program)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("\n __       __                      __                   __     \n|  \\     /  \\                    |  \\                 |  \\    \n| $$\\   /  $$  ______    ______  | $$   __   ______  _| $$_   \n| $$$\\ /  $$$ |      \\  /      \\ | $$  /  \\ /      \\|   $$ \\  \n| $$$$\\  $$$$  \\$$$$$$\\|  $$$$$$\\| $$_/  $$|  $$$$$$\\\\$$$$$$  \n| $$\\$$ $$ $$ /      $$| $$   \\$$| $$   $$ | $$    $$ | $$ __ \n| $$ \\$$$| $$|  $$$$$$$| $$      | $$$$$$\\ | $$$$$$$$ | $$|  \\\n| $$  \\$ | $$ \\$$    $$| $$      | $$  \\$$\\ \\$$     \\  \\$$  $$\n \\$$      \\$$  \\$$$$$$$ \\$$       \\$$   \\$$  \\$$$$$$$   \\$$$$ \n\n");
            Console.ResetColor();

            Console.WriteLine($"Here are the rates for {_program._currentDate:dddd dd MMMM, yyyy}:");

            Console.WriteLine("______________________________");
            Console.WriteLine($"| Coal: ${Math.Round(_program.coal.Price, 2)} per kg");
            Console.WriteLine($"| Stone: ${Math.Round(_program.stone.Price, 2)} per kg");
            Console.WriteLine($"| Iron: ${Math.Round(_program.iron.Price, 2)} per kg");
            Console.WriteLine($"| Gold: ${Math.Round(_program.gold.Price, 2)} per kg");
            Console.WriteLine($"| Diamond: ${Math.Round(_program.diamond.Price, 2)} per kg");
            Console.WriteLine($"| Employees: ${Math.Round(_program._currentEmployeePrice, 2)} per employee");
            Console.WriteLine($"| Wages: ${Math.Round(_program._currentWageRate, 2)} per employee per day");
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
                marketOption = _program.GetValidInt(1, 4);

                switch (marketOption)
                {
                    case 1:
                        Console.WriteLine("You've chosen to sell a specific resource.\nWhich resource do you want to sell?");
                        // sht here bro add in the emojis
                        Console.WriteLine("1 - Coal\n2 - Stone\n3 - Iron\n4 - Gold\n5 - Diamond");
                        int sellChoice = _program.GetValidInt(1, 5);

                        switch (sellChoice)
                        {
                            case 1:
                                Console.WriteLine("Your have chosen to sell coal for dollars");
                                Console.WriteLine($"How much coal do you want to sell?\nYou have {_program.coal.Quantity}kg of coal");
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
                                Console.WriteLine($"How much stone do you want to sell?\nYou have {_program.stone.Quantity}kg of stone");
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
                                Console.WriteLine($"How much iron do you want to sell?\nYou have {_program.iron.Quantity}kg of iron");
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
                                Console.WriteLine($"How much gold do you want to sell?\nYou have {_program.gold.Quantity}kg of gold");
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
                                Console.WriteLine($"How much diamond do you want to sell?\nYou have {_program.diamond.Quantity}kg of diamond");
                                double diamondToSell = _program.GetValidDouble(0, 100000000000);
                                if (diamondToSell > _program.diamond.Quantity)
                                {
                                    Console.WriteLine("You don't have enough diamond to sell that much");
                                }
                                else
                                {
                                    _program.diamond.Quantity -= diamondToSell;
                                    _program.dollars.Quantity += _program.diamond.Quantity * _program.diamond.Price;
                                    _program._totalDollarsEarned += _program.diamond.Quantity * _program.diamond.Price;
                                }

                                Console.WriteLine("Here are your updated resources:");
                                DisplayStuff.DisplayResources(_program);
                                break;
                        }

                        break;

                    case 2:
                        Console.WriteLine("We're selling all your coal and iron and gold and stone and diamond for dollars");
                        
                        _program.dollars.Quantity += _program.coal.Quantity * _program.coal.Price + _program.stone.Quantity * _program.stone.Price +
                                                     _program.iron.Quantity * _program.iron.Price + _program.gold.Quantity * _program.gold.Price +
                                                     _program.diamond.Quantity * _program.diamond.Price;
                        
                        _program._totalDollarsEarned += _program.coal.Quantity * _program.coal.Price + _program.stone.Quantity * _program.stone.Price +
                                                        _program.iron.Quantity * _program.iron.Price + _program.gold.Quantity * _program.gold.Price +
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
                        Console.WriteLine("  _    _   _                      ______                       _                                       \n | |  | | (_)                    |  ____|                     | |                                      \n | |__| |  _   _ __    ___       | |__     _ __ ___    _ __   | |   ___    _   _    ___    ___   ___   \n |  __  | | | | '__|  / _ \\      |  __|   | '_ ` _ \\  | '_ \\  | |  / _ \\  | | | |  / _ \\  / _ \\ / __|  \n | |  | | | | | |    |  __/      | |____  | | | | | | | |_) | | | | (_) | | |_| | |  __/ |  __/ \\__ \\  \n |_|  |_| |_| |_|     \\___|      |______| |_| |_| |_| | .__/  |_|  \\___/   \\__, |  \\___|  \\___| |___/  \n                                                      | |                   __/ |                      \n                                                      |_|                  |___/                     ");
                        Console.WriteLine($"What type of employee do you want to hire?");
                        Console.WriteLine($"1- Hire a bad employee: Price = ${_program._currentEmployeePrice * 0.5},Wage = ${_program._currentWageRate * 0.5}, Efficiency = 0.5x");
                        Console.WriteLine($"2- Hire a mid employee: Price = ${_program._currentEmployeePrice},Wage = ${_program._currentWageRate}, Efficiency = 1x");
                        Console.WriteLine($"3 - Hire a good employee: Price = ${_program._currentEmployeePrice * 2},Wage = ${_program._currentWageRate * 2}, Efficiency = 2x");
                        Console.WriteLine("4 - Cancel and return to market menu");
                        int employeeType = int.Parse(Console.ReadLine());
                        Console.WriteLine($"Enter how many employees you want to hire?\nYou have {_program.dollars.Quantity} dollars");
                        int employeesToHire = _program.GetValidInt(0, 100000);
                        switch (employeeType)
                        {
                            case 1:
                                if (employeesToHire * _program._currentEmployeePrice * 0.5 > _program.dollars.Quantity)
                                {
                                    Console.WriteLine("You don't have enough dollars to hire that many bad employees");
                                }
                                else
                                {
                                    Console.WriteLine($"You have hired {employeesToHire} more bad employees.\nSay hello to:");
                            
                                    _program.HireNewWorker(employeesToHire, "bad");
                            
                                    _program.dollars.Quantity -= employeesToHire * _program._currentEmployeePrice * 0.5;
                                    Console.WriteLine($"You now have {_program.workersList.Count} total employees");
                                    _program._totalEmployeesHired += employeesToHire;
                                }
                                break;
                            case 2:
                                if (employeesToHire * _program._currentEmployeePrice > _program.dollars.Quantity)
                                {
                                    Console.WriteLine("You don't have enough dollars to hire that many mid employees");
                                }
                                else
                                {
                                    Console.WriteLine($"You have hired {employeesToHire} more mid employees.\nSay hello to:");
                            
                                    _program.HireNewWorker(employeesToHire, "mid");
                            
                                    _program.dollars.Quantity -= employeesToHire * _program._currentEmployeePrice;
                                    Console.WriteLine($"You now have {_program.workersList.Count} total employees");
                                    _program._totalEmployeesHired += employeesToHire;
                                }
                                break;
                            case 3:
                                if (employeesToHire * _program._currentEmployeePrice * 2 > _program.dollars.Quantity)
                                {
                                    Console.WriteLine("You don't have enough dollars to hire that many good employees");
                                }
                                else
                                {
                                    Console.WriteLine($"You have hired {employeesToHire} more good employees.\nSay hello to:");
                            
                                    _program.HireNewWorker(employeesToHire, "good");
                            
                                    _program.dollars.Quantity -= employeesToHire * _program._currentEmployeePrice * 2;
                                    Console.WriteLine($"You now have {_program.workersList.Count} total employees");
                                    _program._totalEmployeesHired += employeesToHire;
                                }
                                break;
                            case 4:
                                Console.WriteLine("Returning to the market menu...");
                                break;
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
        public void WeatherEffects(Program _program)
        {
            Random random = new Random();

            // rain or hurricane reducing efficiency, beautifulSky increasing efficiency

            // undoing weather effects 
            if (_program._badWeatherDaysLeft == 1)
            {
                Console.WriteLine("\ud83c\udf21\ufe0f The weather has cleared up, your employees are back to normal efficiency \ud83c\udf21\ufe0f");
                foreach (Worker worker in _program.workersList)
                {
                    worker.DefaultEfficiency *= 1.3;
                }
            }

            if (_program._beautifulSkyDaysLeft == 1)
            {
                Console.WriteLine("\ud83c\udf21\ufe0f The weather is mid, your employees are back to normal efficiency \ud83c\udf21\ufe0f");
                foreach (Worker worker in _program.workersList)
                {
                    worker.DefaultEfficiency /= 1.2;
                }
                _program._beautifulSkyDaysLeft = 0;
            }

            if (_program._hurricaneDaysLeft == 1)
            {
                Console.WriteLine("\ud83c\udf21\ufe0f The hurricane has passed, your employees are back to normal efficiency \ud83c\udf21\ufe0f");
                foreach (Worker worker in _program.workersList)
                {
                    worker.DefaultEfficiency *= 1.4;
                }
            }

            bool noActiveWeatherEffects = _program._badWeatherDaysLeft == 0 && _program._hurricaneDaysLeft == 0 && _program._beautifulSkyDaysLeft == 0;

            // 5% chance a hurricane that reduces the probability of finding resources by 50% for the next 5 days
            if (random.Next(0, 100) < 5 && noActiveWeatherEffects)
            {
                Console.WriteLine(" \ud83c\udf00 A hurricane is coming, efficiency is now 40% less the next five days \ud83c\udf00");
                foreach (Worker worker in _program.workersList)
                {
                    worker.DefaultEfficiency /= 1.4;
                }
                _program._hurricaneDaysLeft = 6;
            }

            // rain reducing efficiency
            else if (random.Next(0, 100) < 30 && noActiveWeatherEffects)
            {
                Console.WriteLine("\ud83c\udf27\ufe0f Due to torrential rain, your employees are 30% less efficient for the next two days \ud83c\udf27\ufe0f");
                foreach (Worker worker in _program.workersList)
                {
                    worker.DefaultEfficiency /= 1.3;
                }
                _program._badWeatherDaysLeft = 3;
            }

            // 30% chance beautiful sky increasing efficiency
            else if (random.Next(0, 100) < 30 && noActiveWeatherEffects)
            {
                Console.WriteLine("\ud83c\udfd6\ufe0f The weather is beautiful today, your employees are 20% more efficient for two days \ud83c\udfd6\ufe0f");
                foreach (Worker worker in _program.workersList)
                {
                    worker.DefaultEfficiency *= 1.2;
                }
                _program._beautifulSkyDaysLeft = 3;
            }

        }

        public void CalendarEffects(Program _program)
        {
            // weekend pay, stock market crash, wage increase, employee illness, profit sharing, reduced probability of finding resources

            // every 10 days, probability of finding resources is reduced by 5%
            if (_program._currentDate.Day % 10 == 0)
            {
                Console.WriteLine("Congratulations for surviving for another 10 days. The game is now getting even harder...");
                Console.WriteLine("\ud83d\udc22 The probability of finding resources has reduced by 8% \ud83d\udc22");
                _program.coal.Probability *= 0.92;
                _program.stone.Probability *= 0.92;
                _program.iron.Probability *= 0.92;
                _program.gold.Probability *= 0.92;
                _program.diamond.Probability *= 0.92;
            }

            // +30% pay on weekends - wage is increased on saturday, then reduced again on monday
            if (_program._currentDate.DayOfWeek is DayOfWeek.Saturday)
            {
                Console.WriteLine("It's the weekend, your employees want 30% more pay");
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
                Console.WriteLine("\ud83d\udcc8 The stock market has recovered \ud83d\udcc8 ");
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
                Console.WriteLine("\ud83d\udcc9 The stock market has crashed, your iron and gold prices have plummeted but you can hire employees for cheaper \ud83d\udcc9");

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
                Console.WriteLine("\ud83e\udd11 It's the first of the month, your employees get a 10% raise for the rest of time \ud83e\udd11");
                _program._currentWageRate *= 1.1;
                foreach (Worker workers in _program.workersList)
                {
                    workers.Wage *= 1.1;
                }
            }

            // 10% profit sharing to each employee on the 15th of every month
            if (_program._currentDate.Day == 15)
            {
                Console.WriteLine("\ud83d\udcc6 Profit sharing time! \ud83d\udcc6");

                if (_program.workersList.Count < 7)
                {
                    Console.WriteLine("Each employee gets 10% of your current $$$ stash");
                    Console.WriteLine($"Your {_program.workersList.Count} employees get ${_program.dollars.Quantity * 0.1} each");
                    double dollarsToLose = _program.dollars.Quantity * 0.1 *_program. workersList.Count;
                    _program.dollars.Quantity -= dollarsToLose;
                    Console.WriteLine($"Your employees have been paid, you have lost $ {dollarsToLose} in the process");
                }

                else
                {
                    Console.WriteLine("Because you have so many employees, 70% of your current $$$ stash is given to them");
                    Console.WriteLine($"This means you'll lose {_program.dollars.Quantity * 0.7}");
                    _program.dollars.Quantity -= _program.dollars.Quantity * 0.7;
                }
            }
        }
        
        public void DealWithEmployees(Program _program)
        {
            
            Random random = new Random();
            
            // recalculate the average efficiency of the employees
            double totalEfficiency = 0;
            foreach (Worker worker in _program.workersList)
            {
                totalEfficiency += worker.DefaultEfficiency;
            }
            _program._averageEmployeeEfficiency = totalEfficiency / _program.workersList.Count;
            
            // retiring workers
            for (int i = _program.workersList.Count - 1; i >= 0; i--)
            {
                Worker worker = _program.workersList[i];
                if (worker.DaysUntilRetirement != 0)
                {
                    worker.DaysUntilRetirement -= 1;
                }

                if (worker.DaysUntilRetirement == 0)
                {
                    Console.WriteLine($"Employee {worker.Name} has retired. Goodbye!");
                    worker.RetirementDate = _program._currentDate.Date;
                    _program.retiredWorkersList.Add(worker);
                    _program.workersList.Remove(worker);
                    Console.WriteLine($"You now have {_program.workersList.Count} employees");
                }
            }
            
            // workers that return (due to training course)
            foreach (Worker worker in _program.trainingWorkersList)
            {
                if (worker.ReturnToWorkDate == _program._currentDate.Date)
                {
                    Console.WriteLine($"Employee {worker.Name} has returned from their training course \ud83d\udcaa ");
                    _program.trainingWorkersList.Remove(worker);
                    _program.workersList.Add(worker);
                }
            }

            // unwell workers
            if (_program.workersList.Count > 1)
            {
                List<Worker> newlyIllWorkers = new List<Worker>();
                
                foreach (Worker worker in _program.workersList)
                {
                    if (random.Next(0, 100) < worker.EmployeeIllProbability)
                    {
                        Console.WriteLine($"\ud83e\udd27 Employee {worker.Name} is unwell and doesn't come in today. They'll be back in three days. \ud83e\udd27");
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
                    Console.WriteLine($"Employee {worker.Name} is no longer ill and has returned to work \ud83d\udc4c");
                    noLongerIllWorkersList.Add(worker);
                }
            }
            
            foreach (Worker worker in noLongerIllWorkersList)
            {
                worker.IsIll = false;
                _program.illWorkersList.Remove(worker);
                _program. workersList.Add(worker);
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

        public void FluctuatePrices(Program _program)
        {
            // upto a 20% fluctuation in prices based on random probability
            Random random = new Random();
            double randomChange = random.Next(-10, 10) / 100.0 + 1;

            _program.coal.Price *= randomChange;
            _program.stone.Price *= randomChange;
            _program.iron.Price *= randomChange;
            _program.gold.Price *= randomChange;
            _program.diamond.Price *= randomChange;
        }
    }

    class Program
    {
        # region global variables
        
        public  bool _animation = true;
        public int _increasedGoldChanceDays;
        public int _marketMasterDaysLeft;
        public int _noWageDaysLeft;
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
        public double _currentEmployeeIllProbability = 5;
        public double _currentEmployeePrice = 100;
        public DateTime _currentDate = new DateTime(2024, 1, 1);
        public static Random _random = new();
        public int _crashDate = _random.Next(0, 28);
        
        public List<Worker> illWorkersList = new();
        public List<Worker> retiredWorkersList = new();
        public List<Worker> trainingWorkersList = new();
        public List<Worker> toSendToTrainingList = new();
        
        public List<Worker> workersList = new()
        { new Worker("mid", "Your First Worker", 10, 100, 10, 1) };
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
        
        // 200 possible names for the workers
        // to stop screaming at me for names it doesn't recognise/think are typos
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        private static List<string> _possibleNames = new List<string>()
        {
            "Elon Tusk", "Taylor Shift", "Jeff Bezosaurus", "Barack O, Banana", "Lady GooGoo", "Michael Jackhammer",
            "Tom Crouton", "Beyoncé Know-all", "Albert Eggstein", "Dwayne 'The Rocking Chair, Johnson",
            "Marilyn Mon-roll", "Mark Zuckerburger", "Oprah Win-free", "Steve Jobsite", "Hillary Cling-on", "Emma Stoned",
            "Donald Duck Trump", "Bruce Spring-chicken", "Bill Gateskeeper", "Justin Beaver", "Kim Car-dashing", "Shakira-law",
            "Charlie Sheen-bean", "Ellen DeGenerous", "Jennif er Lawn-mower", "Will Smithereens", "Cristiano Armani", "Serena Williams-son",
            "Vladimir Poutine", "Ariana Grand-piano", "Jackie Chan-deliep", "Scarlett Johandsome", "Vin Diesel-truck", "Harrison Ford F-150",
            "Gordon Rams-lamb", "Ryan Gooseling", "Nicolas Cage-f ree", "Johnny Depp-fryer", "Arnold Schwarzenegger-schnitzel", "Jessica Al-bacon",
            "Keanu Reefs", "Denzel Washing-done", "Samuel L. Vodka", "Matt Damon-salad", "Angelina Jolie-ox", "Tom Cruise-control",
            "Kate Wins-lit", "Julia Robberies", "Robert Downey Jp. High", "Chris Prattfall", "Jennif er Aniston-bake", "George Clooney-tunes",
            "Leonardo DiCapriSun", "Kanye East", "Benedict Cucumberbatch", "Taylor Swiftboat", "Morgan Free-wheeling", "Jimmy Fallon-over",
            "Nicole Kidneybean", "Hugh Jackman-go", "John Lemonade", "Jennif er Lawrence-of-arabia", "Jennif er Garner-den", "Daniel Radish-cliff e",
            "Ryan Goose-down", "Emma Watson-burgep", "Justin Timberlake-up", "Tom Hanks-giving", "Leonardo DiCaprio-daVinci", "Jack Black-hole",
            "Miley Cyrus-virus", "Katy Perry-corn", "Hugh Grant-ed", "Anne Hathaway with Words", "Sandra Bullockbuster", "Jim Carrey-on",
            "Eddie Murphy-bed", "Bruce Willis-tower", "Johnny Cash-flow", "Celine Dion-osaur", "Jennif er Lopez-ing", "Ellen DeGeneres-erator",
            "Chris Hemsworth-the-clock", "Halle Berry-good", "Julia Roberts-rule", "Zach Galif ianakis-bar", "Brad Pitt-stop", "Eva Longoria-lunch",
            "Julianne Moore-or-less", "Chris Evans-sive", "Reese Witherspoonful", "Charlize Thereon", "Amy Wine-handy", "Tommy Lee Bones",
            "Kurt Russell Sprouts", "Alicia Keys-to-the-city", "Adam Sand-dollar", "Bruce Spring-clean", "Jennif er Aniston-the-pants", "Hugh Jacked-man",
            "Johnny Deep-fry", "Rihanna-na-na", "Bruce Lee-sure", "Chris Pineapple", "Leonardo DiCapri-pants", "Jackie Chain-reaction",
            "Morgan Freeman-dle", "Robert Downey Jr. Low", "Chris Rocking Chair", "Helen Mirren-aged", "Jamie Foxx-in-the-box", "Dwayne 'The Flocking Chair'  Johnson",
            "Arnold Schwarzenegger-salad", "Will Ferrell-y-good", "Gwyneth Paltrow-lint", "Bradley Cooper-ate", "Liam Neeson-light", "Tom Hardy-har-har",
            "Daniel Day-Lewis and Clark", "Johnny Depp-o", "Ben Affleck-tion", "Julia Roberts-ized", "Russell Crow-bar", "Reese Witherspoon-fed",
            "Jennif er Garner-ner", "Ben Stiller-ware", "Halle Berry-licious", "John Travolted", "Amy Adams-apples", "Kevin Bacon-ator",
            "Will Smithen", "Owen Wilson-you-over", "Jake Gyllen-hall", "Matthew McConaughey-mind", "Cate Blanchett-et", "Natalie Port-man",
            "Sylvester Stall-own", "Emily Blunt-ed", "Emma Stone-throw", "Mel Gibson-soup", "Ryan Reynolds-wrap", "Nicole Kid-man",
            "Amanda Seyf ried-rice", "James Franco-american", "Harrison Ford Focus", "Johnny Deep-fryer", "Cameron Diaz-up", "Ryan Gosling-along",
            "Keanu Reeves and Butthead", "Meryl Streep-ing", "Charlize Theron-ament", "Kevin Space- jam", "Daniel Radishcliffe", "Amy Wine-house",
            "Steve Carell-ing",
        };
        
        private static List<string>_usedNames = new List<string>();
        
        # endregion
        
        public static void Main()
        {
            Program program = new Program();
            
            # region initialisation of all objects
            
            program.coal = new Resource("Coal",80, 3, 0, 0);
            program.stone = new Resource("Stone",70, 6, 0, 0);
            program.iron = new Resource("Iron",60, 13, 0, 0);
            program.gold = new Resource("Gold",15, 65, 0, 0);
            program.diamond = new Resource("Diamond",3, 200, 0, 0);
            program.dollars = new Resource("Dollars",0, 0, 100, 0);
            program.magicTokens = new PowerUp(0, 6, 3);
            program.timeMachine = new PowerUp(0, 3, 3);
            program.ancientArtefact = new PowerUp(0, 7, 3);
            program.marketMaster = new PowerUp(0, 4, 3);
            program.stockMarketCrash = new PayForStuff(100);
            program.skipDay = new PayForStuff(50);
            program.bribe = new PayForStuff(200);
            program.trainingCourse = new PayForStuff(400);
            
            program.coalToStone = new Trade(2, program.coal, program.stone);
            program.coalToIron = new Trade(5, program.coal, program.iron);
            program.coalToGold = new Trade(12, program.coal, program.gold);
            program.coalToDiamond = new Trade(70, program.coal, program.diamond);
            program.stoneToIron = new Trade(3, program.stone, program.iron);
            program.stoneToGold = new Trade(13, program.stone, program.gold);
            program.stoneToDiamond = new Trade(40, program.stone, program.diamond);
            program.ironToGold = new Trade(5, program.iron, program.gold);
            program.ironToDiamond = new Trade(16, program.iron, program.diamond);
            program.goldToDiamond = new Trade(3, program.gold, program.diamond);
            
            # endregion 
            
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n   _____           _       _        _____    _                                            \n  / ____|         | |     | |      |  __ \\  (_)                                           \n | |  __    ___   | |   __| |      | |  | |  _    __ _    __ _    ___   _ __   ____  ____ \n | | |_ |  / _ \\  | |  / _` |      | |  | | | |  / _` |  / _` |  / _ \\ | '__| |_  / |_  / \n | |__| | | (_) | | | | (_| |      | |__| | | | | (_| | | (_| | |  __/ | |     / /   / /  \n  \\_____|  \\___/  |_|  \\__,_|      |_____/  |_|  \\__, |  \\__, |  \\___| |_|    /___| /___| \n                                                  __/ |   __/ |                           \n                                                 |___/   |___/                            \n");
            Console.ResetColor();

            Console.WriteLine("The aim of the game is to survive for as long as possible before bankruptcy");
            Console.WriteLine("These are your initial resources...");

            DisplayStuff.DisplayResources(program);
            
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
                        miningOperation.Dig(1, this, dayToDayOperations, achievementsList);
                        break;
                    case 2:
                        _animation = false;
                        Console.WriteLine("\n___  ___        _  _    _         _        ______               ______  _        \n|  \\/  |       | || |  (_)       | |       |  _  \\              |  _  \\(_)       \n| .  . | _   _ | || |_  _  _ __  | |  ___  | | | | __ _  _   _  | | | | _   __ _ \n| |\\/| || | | || || __|| || '_ \\ | | / _ \\ | | | |/ _` || | | | | | | || | / _` |\n| |  | || |_| || || |_ | || |_) || ||  __/ | |/ /| (_| || |_| | | |/ / | || (_| |\n\\_|  |_/ \\__,_||_| \\__||_|| .__/ |_| \\___| |___/  \\__,_| \\__, | |___/  |_| \\__, |\n                          | |                             __/ |             __/ |\n                          |_|                            |___/             |___/ \n");
                        Console.WriteLine("Enter number of days to dig in one go (upto 30)");
                        int daysToDig = GetValidInt(1, 30);
                        miningOperation.Dig(daysToDig, this, dayToDayOperations, achievementsList);
                        break;
                    case 3:
                        marketOperation.GoToMarket(this);
                        break;
                    case 4:
                        Console.WriteLine("Skipping one day");
                        Console.WriteLine($"You have been charged ${skipDay.Price} for the costs of skipping a day");
                        dollars.Quantity -= skipDay.Price;
                        skipDay.skipDayOrNot = true;
                        miningOperation.Dig(1, this, dayToDayOperations, achievementsList);
                        DisplayStuff.DisplayResources(this);
                        break;
                    case 5:
                        
                        if (ancientArtefact.Quantity == 0 && timeMachine.Quantity == 0 && marketMaster.Quantity == 0)
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
                        Console.WriteLine($"Enter number of employees to send on training\nEnter -1 to send all employees\nYou have {workersList.Count} employees");
                        int employeesToSend = GetValidInt(-1, workersList.Count);
                        if (employeesToSend == -1)
                        {
                            employeesToSend = workersList.Count;
                        }
                        if (dollars.Quantity > trainingCourse.Price * employeesToSend && workersList.Count != 0)
                        {
                            Console.WriteLine($"You have chosen to send {employeesToSend} employees on a training course");
                            Console.WriteLine($"You have been charged {trainingCourse.Price} per employee");
                            Console.WriteLine($"Your {employeesToSend} employees will be back in 7 days");
                            EmployeeTrainingCourse(employeesToSend);
                        }
                        else if (dollars.Quantity > trainingCourse.Price * employeesToSend && workersList.Count == 0)
                        {
                            Console.WriteLine("You don't have any employees to send on a training course");
                            Console.WriteLine("This could be because of employee illness - try again later");
                        }
                        else
                        {
                            Console.WriteLine($"You don't have enough money to send {employeesToSend} employees on a training course");
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
                    case 13:
                        GoToTrader();
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
                Console.WriteLine("13 - Go To Trader           12 - Display employees\n");
                Console.WriteLine("Your choice:");

                int userOption = GetValidInt(0, 13);
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
                    miningOperation.Dig(5, this, dayToDayOperations, achievementsList);
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

        public void EmployeeTrainingCourse(int numberOfEmployees)
        {
            // to boost the productivity of employees
            Console.WriteLine($"Training {numberOfEmployees} employees...");
            Console.WriteLine($"This course charged you {trainingCourse.Price * numberOfEmployees} in fees");
            dollars.Quantity -= trainingCourse.Price * numberOfEmployees;
            for (int i = 0; i < numberOfEmployees; i++)
            {
                Console.WriteLine($"Employee {workersList[i].Name} has begun the training course, they'll be back in a week \ud83d\udcaa");
                workersList[i].ReturnToWorkDate = _currentDate.AddDays(7);
                workersList[i].DefaultEfficiency *= 1.3;
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
            if (_possibleNames.Count > 0)
            {
                for (int i = 0; i < numberOfWorkers; i++)
                {
                    int randomName = _random.Next(0, _possibleNames.Count);
                    
                    // making 'levels' of efficiency based on the number of employees
                    // this is to make the game harder over time as the player hires more employees
                    double efficiency = 1;
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
                        efficiency = _random.Next(0, 5);
                        efficiency /= 100;
                    }
                    
                    Worker newWorker = new Worker(type,_possibleNames[randomName], _currentWageRate, _currentEmployeePrice, _currentEmployeeIllProbability, efficiency);
                    workersList.Add(newWorker);
                    _usedNames.Add(newWorker.Name);
                    _possibleNames.Remove(newWorker.Name);
                    Console.WriteLine($"{newWorker.Name}, Efficiency {Math.Round(newWorker.DefaultEfficiency, 2)}\ud83e\uddcd\u200d\u2642\ufe0f");
                    
                    // updating bribe price
                    bribe.Price = _currentWageRate * workersList.Count * 2;
                }
            }
            else
            {
                Console.WriteLine("You've hired all 163/163 available employees and so you've run out of names to give to your employees \ud83d\ude2d");
            }
        }

        public void GoToTrader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("             _______                      _               \n            |__   __|                    | |              \n               | |     _ __    __ _    __| |   ___   _ __ \n               | |    | '__|  / _` |  / _` |  / _ \\ | '__|\n               | |    | |    | (_| | | (_| | |  __/ | |   \n               |_|    |_|     \\__,_|  \\__,_|  \\___| |_|");
            Console.ResetColor();
            DisplayStuff.DisplayResources(this);
            Console.WriteLine("Here are the options for today:");
            Console.WriteLine("\n0 - Cancel and return");
            Console.WriteLine($"1 - Convert {coalToStone.Ratio}kg of coal for stone");
            Console.WriteLine($"2 - Convert {coalToIron.Ratio}kg of coal for iron");
            Console.WriteLine($"3 - Convert {coalToGold.Ratio}kg of coal for gold");
            Console.WriteLine($"4 - Convert {coalToDiamond.Ratio}kg of coal for diamond");
            Console.WriteLine($"5 - Convert {stoneToIron.Ratio}kg of stone for ron");
            Console.WriteLine($"6 - Convert {stoneToGold.Ratio}kg of stone for gold");
            Console.WriteLine($"7 - Convert {stoneToDiamond.Ratio}kg of stone for diamond");
            Console.WriteLine($"8 - Convert {ironToGold.Ratio}kg of iron for gold");
            Console.WriteLine($"9 - Convert {ironToDiamond.Ratio}kg of iron for diamond");
            Console.WriteLine($"10 - Convert {goldToDiamond.Ratio}kg of gold for diamond");
            Console.WriteLine("Remember, you can only make one trader per day. Choose wisely!");
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
