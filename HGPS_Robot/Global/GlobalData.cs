using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    class GlobalData
    {
        public static readonly string Voice1 = ConfigurationManager.AppSettings["Voice 1"];
        public static readonly string Voice2 = ConfigurationManager.AppSettings["Voice 2"];
        public static readonly string Voice3 = ConfigurationManager.AppSettings["Voice 3"];

        public static readonly string SpeechKey = ConfigurationManager.AppSettings["SpeechRecognitionSubscriptionKey"];
        public static readonly string SpeechRegion = ConfigurationManager.AppSettings["SubscriptionRegion"];
        public static readonly string DirectLine = ConfigurationManager.AppSettings["DirectLineSecret"];
        public static readonly string BotId = ConfigurationManager.AppSettings["BotId"];
                
        
    }
}
