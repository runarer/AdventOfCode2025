namespace Day10
{
    public class IndicatorLights(int targetLights, int[] buttons, int[] targetJoltage, int[][] buttonsArray)
    {
        public int[] Buttons { get; private set; } = buttons;
        public int TargetLights { get; private set; } = targetLights;
        public int[] TargetJoltage { get; private set; } = targetJoltage;
        public int[][] ButtonsArray { get; init; } = buttonsArray;
    }
}
