
namespace Gold_Diggerzz
{
    public class Resource
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
}
