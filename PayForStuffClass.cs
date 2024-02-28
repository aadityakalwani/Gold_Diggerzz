namespace Gold_Diggerzz
{
    public class PayForStuff
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
}
