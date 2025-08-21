using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_launcher
{
    public class MathfExtra
    {
        public static float Clamp01(float value)
        {
            if (value > 1) return 1f;
            if (value < 0) return 0f;
            return value;
        }
        
    }
}
