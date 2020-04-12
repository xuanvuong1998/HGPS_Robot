using SpeechLibrary;
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

        // Group 1
        public static double LeftResult { get; set; }

        // Group 2
        public static double RightResult { get; set; }

        private static Random rand = new Random();

        public static void AnnouceGroupCompetition()
        {

            string speech = "Before starting the quiz, I would like to " +
                "divide our class into 2 groups. My right side is group 1. " +
                "And my left side is group 2. ";

            Synthesizer.Speak(speech);


            //// A1 A3 A5 A7 A9
            //var left = TablePositionHelper.GetLeftGroupMembers();
            //// A2 A4 A6 A8

            //var right = TablePositionHelper.GetRightGroupMembers();
            //speech = "Now I will read " +
            //    "all group member names. Group 1 are ";

            //foreach (var std in left)
            //{
            //    speech += std + ", ";
            //}

            //Synthesizer.Speak(speech);

            //speech = "Group 2 are ";
            //foreach (var std in right)
            //{
            //    speech += std + ", ";
            //}

            //Synthesizer.Speak(speech);

        }

        public static void AnnouceCurrentResult()
        {
            string speech = "";

            int rdmNum = rand.Next(3);
            switch (rdmNum)
            {
                case 0:
                    speech = "Everyone, please look at the projector screen to see " +
                        "the competition result, Who will be the winner of " +
                        "this question? Group 1 or Group 2?";
                    break;
                case 1:
                    speech = "Ok, now let see which group is the winner of the question, " +
                        "Group 1 or Group 2. ";
                    break;
                case 2:
                    speech = "Boys and girls, will group 1 be the winner, or " +
                        "group 2?";
                    break;
            }

            LessonHelper.InsertNextSlideCommand("speak", speech);

            speech = "The result is...";

            if (LeftResult > RightResult)
            {
                switch (rdmNum)
                {
                    case 0: speech += "Oh yeah, Group 1 defeated Group 2, and become " +
                            "the winner"; break;
                    case 1: speech += $"Oh yes, With {(int)LeftResult} percent of correct answers. " +
                            $"Group 1 has defeated group 2. "; break;
                    case 2: speech += $"Wow, Group 1 is the winner. You get {(int)LeftResult} " +
                            $"percent correct answers. " +
                            "Very good!"; break;
                }
            }
            else if (LeftResult == RightResult)
            {
                switch (rdmNum)
                {
                    case 0: speech += "Absolutely unbelievable boys and girls. It is a draw"; break;
                    case 1: speech += $"Group 1 got {LeftResult} percent, " +
                            $"Group 2 also got {RightResult} percent. It is a draw. " +
                            $"Good job all of you. "; break;
                    case 2: speech += "Fantastic boys and girls. It is a draw. " +
                            "Group 1 and Group 2 have the same correct percentage. " +
                            "Congratulation to all! "; break;
                }
            }
            else
            {
                switch (rdmNum)
                {
                    case 0: speech += "Awesome. Group 1 was defeated by group 2. Congratulation " +
                            "group 2"; break;
                    case 1:
                        speech += $"With {(int)RightResult} percent of correct answers. " +
                        $"Group 2 is the winner of this question. Good job group 2"; break;
                    case 2:
                        speech += $"Group 2 is the winner. {(int)RightResult} is the " +
                        $"percentage of your correct answers. " +
                        "Very good!"; break;
                }
            }

            // Open result in slide viewer (display on projector)
            LessonHelper.InsertNextSlideCommand("myhub", "RequestOpenResultURL,"
                        + "group-competition");

            LessonHelper.InsertNextSlideCommand("wait", "2000");
            LessonHelper.InsertPraise(speech);

            LessonHelper.InsertNextSlideCommand("myhub", "RequestOpenResultURL,"
                        + "hide-results");
            
            // Make sure praise added to next slide before toggle signal to continue the slide
            ResultReceived = true;
        }

        public static void AnnouceOverallResult()
        {
            int rdmNum = rand.Next(3);
            string speech;
            speech = "Now. It is the time to summarize the result. Who " +
                "will be the winner after all? Group 1 or Group 2? ";

            LessonHelper.InsertNextSlideCommand("speak", speech);

            List<string> winnerMembers = new List<string>();

            speech = "";
            if (LeftResult > RightResult)
            {
                switch (rdmNum)
                {
                    case 0: speech += "Incredible! The winner today is group 1. "; break;
                    case 1: speech += "Marvelous! Group 1 is the winner today. "; break;
                    case 2: speech += "Terrific! Group 1 become the winner today. "; break;
                }

                winnerMembers = TablePositionHelper.GetLeftGroupMembers();
                
            }
            else if (LeftResult == RightResult)
            {
                speech += "It is absolutely unbelievable boys and girls. It is a draw. " +
                    "Congratulation to the whole class. You guys are very awesome!";
            }
            else
            {
                switch (rdmNum)
                {
                    case 0: speech += "Brilliant! The winner is group 2. "; break;
                    case 1: speech += "Marvelous! Group 2 is the winner today. "; break;
                    case 2: speech += "Terrific! Group 2 became the winner today. "; break;
                }
                winnerMembers = TablePositionHelper.GetRightGroupMembers();
            }

            if (winnerMembers.Count > 0)
            {
                speech += "Congratulation ";
                foreach (var member in winnerMembers)
                {
                    speech += member + ", ";
                }
            }
            
            LessonHelper.InsertNextSlideCommand("myhub", "RequestOpenResultURL,"
                        + "group-competition-final");
            LessonHelper.InsertNextSlideCommand("speak", speech);
            LessonHelper.InsertNextSlideCommand("playaudio", "champion");
            LessonHelper.InsertNextSlideCommand("myhub", "RequestOpenResultURL,"
                        + "hide-results");
        }

        public static void ProcessResult(string message)
        {
            string type = message.Split(';')[0];
            LeftResult = double.Parse(message.Split(';')[1]);
            RightResult = double.Parse(message.Split(';')[2]);

            LeftResult *= 100;
            RightResult *= 100; 

            AnnouceCurrentResult();

        }
    }
}
