using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    public class RobotCmd
    {
        public string Navigation { get; set; }
        public string Movement { get; set; }
        public string Gesture { get; set; }
        public string Chatbot { get; set; }
        public string Video { get; set; }
        public string Audio { get; set; }
        public string SpecialAction { get; set; }
        public string Praise { get; set; }

        public string LessonStatus { get; set; }

        public List<StudentHistoryDTO> AssessPerformance { get; set; }


        // From Teacher Panel
        public void ProcessCommand()
        {
            if (Navigation != null)
            {
                
            }

            if (Movement != null)
            {

            }

            if (Gesture != null)
            {

            }

            if (Chatbot != null)
            {

            }

            if (AssessPerformance != null)
            {

            }
        }
    }
}
