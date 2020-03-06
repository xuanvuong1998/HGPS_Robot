using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{

    // Class divided into 2 groups (left and right)
    class GroupCompetitionHelper
    {
        // Indicate whether robot received group performance results from web server ot not yet
        public static bool ResultReceived { get; set; }

        public static double LeftResult { get; set; }

        public static double RightResult { get; set; }

        public static void AnnouceCurrentResult()
        {
            string speech;

            if (LeftResult > RightResult)
            {
                speech = "Group 1 is the winner of this question";  
            }else if (LeftResult == RightResult)
            {
                speech = "It is draw. Unbelievable";
            }
            else
            {
                speech = "Group 2 is the winner of this question";
            }

            LessonHelper.InsertNextSlideCommand("myhub", "RequestOpenResultURL,"
                        + "group-competition");

            LessonHelper.InsertPraise(speech);
            
            // Make sure praise added to next slide before toggle signal to continue the slide
            ResultReceived = true;
        }

        public static void AnnouceOverallResult()
        {
            string speech;
            speech = "Now. It is the time to summary, which group is the " +
                    "winner in overall. ";

            if (LeftResult > RightResult)
            {
                speech += "Congratulation. Group 1 is the winner in overall. ";
            }
            else if (LeftResult == RightResult)
            {
                speech += "Its a draw. Unbelievable";
            }
            else
            {
                speech += "Excellent. Group 2 is the winner in overall. ";
            }

            LessonHelper.InsertNextSlideCommand("myhub", "RequestOpenResultURL,"
                        + "group-competition-final");
            LessonHelper.InsertNextSlideCommand("speak", speech);
            LessonHelper.InsertNextSlideCommand("playaudio", "champion");
            
        }

        public static void ProcessResult(string message)
        {
            string type = message.Split(';')[0];
            LeftResult = double.Parse(message.Split(';')[1]);
            RightResult = double.Parse(message.Split(';')[2]);

            if (type == "group-competition")
            {
                AnnouceCurrentResult();
            }
            else
            {
                // This case never happens for the time being
                AnnouceOverallResult();
            }
        }
    }
}
