namespace Day10
{
    public static class Parser
    {
        public static (string, string, string) SplitLine(string line)
        {
            int firstSpace = line.IndexOf(' ');
            string lights = line.Substring(0, firstSpace).Trim([' ','[',']']);
            int lastSpace = line.LastIndexOf(" ");
            string buttons = line.Substring(firstSpace, lastSpace-firstSpace).Trim();
            string joltage = line.Substring(lastSpace).Trim([' ', '{', '}']);

            return (lights, buttons, joltage);
        }

        public static int LightsToUshort(string lightsAsText) 
        {
            // we want to reverse the string, because the first char is the least significant bit
            char[] chars = lightsAsText.Replace('.', '0').Replace('#', '1').ToCharArray();
            int lights = Convert.ToInt32(new string([.. chars.Reverse()]), 2);
            
            return lights;
        }

        public static int ButtonToInt(int[] buttonBits)
        {
            int button = 0;

            foreach (int bit in buttonBits)
            {
                button |= (1 << bit);
            }

            return button;
        }

        public static int[] ButtonsToInt(int[][] splitEachOnKommaAndParse)
        {
            return [..splitEachOnKommaAndParse.Select(ButtonToInt)];
        }

        public static int[][] ButtonsToIntArray(string buttonsAsText)
        {
            var splitOnSpace = buttonsAsText.Split(' ');
            var trimOfEnds = splitOnSpace.Select(x => x.Trim(['(', ')']));
            return [.. trimOfEnds.Select(x => x.Split(',').Select(int.Parse).ToArray())];
        }

        public static IndicatorLights CreateIndicatorLights(string line)
        {
            (string lightsText, string buttonsText, string joltageText) = Parser.SplitLine(line);
            int targetLights = LightsToUshort(lightsText);
            int[][] buttonsAsInt = ButtonsToIntArray(buttonsText);
            int[] buttons = ButtonsToInt(buttonsAsInt);
            int[] targetJoltage = [.. joltageText.Split(',').Select(int.Parse)];

            return new IndicatorLights(targetLights, buttons, targetJoltage, buttonsAsInt);
        }
    }
}
