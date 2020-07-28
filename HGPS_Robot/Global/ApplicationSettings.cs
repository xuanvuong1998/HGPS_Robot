using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    class ApplicationSettings
    {
        //public static string BASE_ADDRESS = "https://robo-ta.com/";
        public static string BASE_ADDRESS = "https://coddie.io/";

        //public static string BASE_ADDRESS { get; set; }
        //            = @"https://localhost:44353/";

        public static bool RobotGestureEnable { get; set; } = true;

        public static bool RobotMovementEnable { get; set; } = true;
    }
}
