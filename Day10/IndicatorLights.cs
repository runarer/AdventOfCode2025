namespace Day10
{
    public class IndicatorLights(int targetLights, int[] buttons, int[] targetJoltage)
    {
        public int[] Buttons { get; private set; } = buttons;
        public int TargetLights { get; private set; } = targetLights;
        public int[] TargetJoltage { get; private set; } = targetJoltage;
    }
}
