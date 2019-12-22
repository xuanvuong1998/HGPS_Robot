using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public class RobotSlide
    {
        public string SlideNumber { get; set; }
        public string SlideImage { get; set; }

        public RobotSlide(string slideNum, string slideImage)
        {
            SlideNumber = slideNum;
            SlideImage = slideImage;
        }
    }
}
