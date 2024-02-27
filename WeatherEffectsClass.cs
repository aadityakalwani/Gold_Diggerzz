namespace Gold_Diggerzz
{
    public class WeatherEffectsClass
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
}
