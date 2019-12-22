using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public class RobotCommand
    {
        public string Type { get; private set; }
        public string Value { get; private set; }

        public RobotCommand(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
