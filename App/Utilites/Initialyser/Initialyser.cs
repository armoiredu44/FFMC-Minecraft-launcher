using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_launcher
{
    public static class Initialyser
    {
        public static void InitialyseApp()
        {
            Debugger.CreateLogFileAtStartup();
        }
    }
}
