using SpeechLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{

    class GlobalFlowControl
    {
        public class Navigation
        {
            public static bool Moving { get; set; }

            private static bool reachedGoal;
            public static bool ReachedGoal
            {
                get { return reachedGoal; }
                set
                {
                    reachedGoal = value;

                    if (value == true)
                    {
                        Moving = false;
                    }
                }
            }

            private static bool stucked;
            public static bool Stucked
            {
                get { return stucked; }
                set
                {
                    stucked = value;
                    if (value == true)
                    {
                        Moving = false;
                    }
                }
            }

            private static bool canceled;

            public static bool Canceled
            {
                get { return canceled; }
                set
                {
                    canceled = value;
                    if (value == true)
                    {
                        Moving = false;
                    }
                }
            }

            /// <summary>
            /// not moving any more
            /// </summary>            
            public static void Reset()
            {
                Moving = false;
            }

            public static void ResetBeforeNavigation()
            {
                Moving = true;
                Canceled = false;
                Stucked = false;
                ReachedGoal = false;
            }
        }

        public class GroupChallenge
        {
            private static Queue<string> ServeHintsQueue { get; set; } = new Queue<string>();
            public static bool IsOfferingHint { get; set; } = false;
            public static bool IsHappening { get; set; } = false;

            public static void AddToServingQueue(int groupNumber, string hint)
            {
                ServeHintsQueue.Enqueue(groupNumber + "-" + hint);
            }

            /// <summary>
            /// GET the top of queue without removing it
            /// </summary>
            /// <returns></returns>
            public static string GetNextOffer()
            {
                return ServeHintsQueue.Peek();
            }

            public static string RemoveCurrentOffer()
            {
                return ServeHintsQueue.Dequeue();
            }

            public static void ResetQueue()
            {
                ServeHintsQueue.Clear();
            }

            public static bool IsServingQueueEmpty()
            {
                return ServeHintsQueue.Count == 0;
            }
           
        }

        public class Lesson
        {
            private static int quizElapsedValue;

            public static int QuizElapsedTime
            {
                get { return quizElapsedValue; }
                set
                {
                    quizElapsedValue = value;
                    var secondsLeft = LessonHelper.CurrentQuizTimeout
                        - value;

                    if (secondsLeft <= 0) return;
                    
                    if (secondsLeft <= 10)
                    {
                        Synthesizer.SpeakAsync(secondsLeft + "");
                    }
                    
                    if (secondsLeft == 30 || secondsLeft == 20)
                    {
                        Synthesizer.SpeakAsync(secondsLeft + " more seconds " +
                            "every one. ");
                    }

                    if (secondsLeft == 40)
                    {
                        Synthesizer.SpeakAsync("Pay attention please, you" +
                        " guys have " + secondsLeft + " seconds left");

                    }
                }
            }

            public static string Name { get; set; }

            public static bool Starting { get; set; }

            //Student positon when robot approach for asking 
            public static string ApproachStudent
            {
                get; set;
            }

            public static bool StartingQuiz { get; set; }

            // StudentID who be chosen to ask robot go there for asking after a quiz finished
            public static string ChosenStudent { get; set; }

            public static List<string> ChosenStudentList { get; set; }

            public static List<GroupChallengeRecord> GroupRecords { get; set; } = new List<GroupChallengeRecord>();

            public static bool IsStudentChosenBefore(string std)
            {
                return ChosenStudentList.Contains(std);
            }

            public static bool StudentFeedbackReceived { get; set; }

            public static void ResetAll()
            {
                Starting = true;
                Synthesizer.SetSpeed(-1);
                GroupRecords.Clear();
                GroupChallenge.ResetQueue();
                GroupChallengeHelper.LoadHints();// load list of hints image names from dropbox hint folder
                TablePositionHelper.LoadTablesInfo();
                TablePositionHelper.DeleteChosenStudentList();
                if (ChosenStudentList != null) ChosenStudentList.Clear();
                else ChosenStudentList = new List<string>();
                StudentFeedbackReceived = false;
            }

        }

        public class UpperBody
        {
            public static bool MovingRandomly { get; set; }
        }

        public class ChatBot
        {
            public static bool Talking { get; set; }

            public static bool Waiting { get; set; }

            public static string RecognizedQuestion { get; set; }

            public static bool StopAskingQuestion { get; set; }

            public static bool ConversationEnable { get; set; }

            public static void ResetBeforeNewConversation()
            {
                ConversationEnable = true;
                RecognizedQuestion = "";
            }
        }
    }
}
