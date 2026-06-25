namespace Day10
{
    // Use fields instead of properties so I can change the arrays
    public class IndicatorLights(int targetLights, int[] buttons, int[] targetJoltage, int[][] buttonsArray)
    {
        public int[] Buttons = buttons;
        public int TargetLights { get; private set; } = targetLights;
        public int[] TargetJoltage = targetJoltage;
        public int[][] ButtonsArray = buttonsArray;
    }
}
