using System;
using System.Collections.Generic;

namespace Gold_Diggerzz
{
    public class Trade
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
    }// you're allowed multiple trades per day ++ trade prices don't fluctuate
}
