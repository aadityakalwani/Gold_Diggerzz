namespace Gold_Diggerzz
{
    public class PowerUp
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
}
