using System;

namespace Gold_Diggerzz
{
    internal class Program
        # region idea dumping ground
            
        /*
         * This is my digging simulator guys
         * initial ideas:
         * (initial inspiration: https://replit.com/@AadityaKalwani/Digging-Simulator#main.py)
         * basically you dig and you get x amount of gold per day with a possibility of getting diamonds too
         * you can use gold to go to the market and upgrade stuff like your shovel and employees
         * it follows the gregorian calendar
         * so for example, on weekends the employees want extra pay and once per few months the stock market crashes and your gold rates plummet
         * you can trade gold and diamonds for $$$ depending on the rates
         * you use your $$$ to buy upgrades for things like your shovel or your employees
         * or you can 'restart' and sacrifice all your $$$ for a better location with better gold payments per day
         * (like prestige in all the idle miner games i played)
         */
            
        /*
         * basic functionality first:
         * menu option:
         * 1 - dig one day
         * simulates 1 day's worth of digging
         * 2 - go to market
         * later, the ability to trade in the market
         */
            
        # endregion

    {
        public static void Main(string[] args)
        {
            RunGame();
        }

        public static void RunGame()
        {
            UserMenuOption();
        }

        public static int UserMenuOption()
        {
            Console.WriteLine("Welcome to Gold Diggerzz!");
            Console.WriteLine("Please select an option:");
            Console.WriteLine("1 - Dig one day");
            Console.WriteLine("2 - Go to market");
            Console.WriteLine("3 - Quit game");
            
            int UserOption = int.Parse(Console.ReadLine());
            
            switch (UserOption)
            {
                case 1:
                    Console.WriteLine("You have chosen to dig one day");
                    break;
                case 2:
                    Console.WriteLine("You have chosen to go to the market");
                    break;
                case 3:
                    Console.WriteLine("You have chosen to quit the game");
                    break;
                default:
                    Console.WriteLine("Please enter a valid option");
                    break;
            }

            return UserOption;

        }

        public static void DigOneDay()
        {
            
        }
        
        public static void GoToMarket()
        {
            
        }

        public static void UpdateResources()
        {
            /* i plan to use a 2d array to store the resources
             the resources are:
             gold
             diamonds
             $$$
             employees
            */
            
        }

    }
}
