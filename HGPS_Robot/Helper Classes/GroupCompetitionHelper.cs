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

        private static Random rand = new Random();

        public static void AnnouceCurrentResult()
        {
            string speech = "";

            int rdmNum = rand.Next(3);
            switch (rdmNum)
            {
                case 0:
                    speech = "Everyone, please look at the projector screen to see " +
                        "the result. Left, or right, will win this challenge?";
                    break;
                case 1:
                    speech = "Now. Let see, left, or right. is the winner of this question?";
                    break;
                case 2:
                    speech = "Boys and girls, do you think the left, or the right will be " +
                        "the winner of this question?";
                    break;
            }

            LessonHelper.InsertNextSlideCommand("speak", speech);

            speech = "here it is. ";

            if (LeftResult > RightResult)
            {
                switch (rdmNum)
                {
                    case 0: speech += "Group 1 is the winner. Well done!"; break;
                    case 1: speech += "Group 1 is the winner. Good job!"; break;
                    case 2: speech += "Group 1 is the winner. Very good!"; break;
                }

            }
            else if (LeftResult == RightResult)
            {
                switch (rdmNum)
                {
                    case 0: speech += "Unbelievable boys and girls. It is a draw"; break;
                    case 1: speech += "Amazing boys and girls. It is a draw"; break;
                    case 2: speech += "Fantastic boys and girls. It is a draw"; break;
                }
            }
            else
            {
                switch (rdmNum)
                {
                    case 0: speech += "Group 2 is the winner. Wonderful!"; break;
                    case 1: speech += "Group 2 is the winner. Congratualtion!"; break;
                    case 2: speech += "Group 2 is the winner. Awesome"; break;
                }
            }

            LessonHelper.InsertNextSlideCommand("myhub", "RequestOpenResultURL,"
                        + "group-competition");

            LessonHelper.InsertPraise(speech);

            // Make sure praise added to next slide before toggle signal to continue the slide
            ResultReceived = true;
        }

        public static void AnnouceOverallResult()
        {
            int rdmNum = rand.Next(3);
            string speech;
            speech = "Now. It is the time to summary, which group, left, or right is the " +
                    "winner in overall. ";

            LessonHelper.InsertNextSlideCommand("speak", speech);

            speech = "";
            if (LeftResult > RightResult)
            {
                switch (rdmNum)
                {
                    case 0: speech += "Incredible! The winner is group 1. "; break;
                    case 1: speech += "Marvelous! Group 1 is the winner today. "; break;
                    case 2: speech += "Terrific! Group 1 became the winner today"; break;
                }
            }
            else if (LeftResult == RightResult)
            {
                speech += "Unbelievable boys and girls. It is a draw. ";
            }
            else
            {
                switch (rdmNum)
                {
                    case 0: speech += "Brilliant! The winner is group 2"; break;
                    case 1: speech += "Marvelous! Group 2 is the winner today. "; break;
                    case 2: speech += "Terrific! Group 2 became the winner today"; break;
                }
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

            AnnouceCurrentResult();

        }
    }
}
