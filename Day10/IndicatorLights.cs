using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Day10
{
    public class IndicatorLights(ushort targetLights, ushort[] buttons)
    {
        public ushort Lights { get; private set; } = 0;
        public ushort[] Buttons { get; private set; } = buttons;
        public ushort TargetLights { get; private set; } = targetLights;
    }
}
