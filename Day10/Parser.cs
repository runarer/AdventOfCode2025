using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static ushort LightsToUshort(string lightsAsText) 
        {
            int lights = 0;

            for (int i = 0; i < lightsAsText.Length; i++)
            {                
                if (lightsAsText[lightsAsText.Length - 1 - i] == '#')
                {
                    lights = ( lights | 1 << i);
                } 
            }

            return (ushort)lights;
        }

        public static ushort ButtonToUshort(int[] buttonBits)
        {
            int button = 0;

            foreach (int bit in buttonBits)
            {
                button |= (1 << bit);
            }


            return (ushort)button;
        }

        public static ushort[] ButtonsToUshort(string buttonsAsText)
        {
            var splitOnSpace = buttonsAsText.Split(' ');
            var trimOfEnds = splitOnSpace.Select(x => x.Trim(['(',')']));
            var splitEachOnKommaAndParse = trimOfEnds.Select(x => x.Split(',').Select(int.Parse).ToArray());
            return [..splitEachOnKommaAndParse.Select(ButtonToUshort)];
        }

        public static IndicatorLights CreateIndicatorLights(string line)
        {
            (string lightsText, string buttonsText, string joltageText) = Parser.SplitLine(line);
            ushort targetLights = Parser.LightsToUshort(lightsText);
            ushort[] buttons = Parser.ButtonsToUshort(buttonsText);

            return new IndicatorLights(targetLights, buttons);
        }
    }
}
