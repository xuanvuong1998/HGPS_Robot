using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public class RobotLesson
    {
        public string Name { get; set; }
        public List<RobotSlide> Slides { get; set; }

        public RobotLesson() { }
        public RobotLesson(string name, List<RobotSlide> slides)
        {
            Name = name;
            Slides = slides;
        }
    }
}
