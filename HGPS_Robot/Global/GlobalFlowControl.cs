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
        // just testing
        
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
            // Results of each group : Result of taken steps
            // String format: result(0/1)-subCnt-subTime-groupAns
            public static List<List<string>> GroupResults { get; set; }

            private static List<string> ServingHintsQueue { get; set; } = new List<string>();
            private static Queue<string> ServeHintsQueue { get; set; } = new Queue<string>();
            public static bool IsOfferingHint { get; set; } = false;
            public static bool IsHappening { get; set; } = false;
            
            public static void AddToServingQueue(int groupNumber, string hint)
            {
                ServingHintsQueue.Add(groupNumber + "-" + hint);

                // The group who have passed less steps will be served first
                ServingHintsQueue = 
                    ServingHintsQueue.OrderBy(x => x.Split('-')[1]).ToList();
                //ServeHintsQueue.Enqueue(groupNumber + "-" + hint);
            }

            /// <summary>
            /// GET the top of queue without removing it
            /// </summary>
            /// <returns></returns>
            public static string GetNextOffer()
            {
                return ServingHintsQueue[0];
            }
            
            public static void RemoveCurrentOffer()
            {
                ServingHintsQueue.RemoveAt(0);
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
                        Synthesizer.SpeakAsync("Attention please, you" +
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

            public static bool HaveGroupChallenge { get; set; }
            
            public static bool QuizIsStarting { get; set; }

            // StudentID who be chosen to ask robot go there for asking after a quiz finished
            // Chosen by teacher from teacher panel control
            public static string ChosenStudent { get; set; }

            public static List<string> ChosenStudentList { get; set; }

            public static List<GroupChallengeRecord> GroupRecords { get; set; } = new List<GroupChallengeRecord>();

            public static bool IsStudentChosenBefore(string std)
            {
                return ChosenStudentList.Contains(std);
            }

            public static bool StudentFeedbackReceived { get; set; }

            // Devide class into 2 groups (left and right)
            // Student for each group submit and accumulate results

            public static bool GroupCompetitionIsHappening { get; set; } = false;

            public static void ResetAll()
            {
                Starting = true;
                Synthesizer.SetSpeed(-3);
                TablePositionHelper.LoadTablesInfo();

                if (HaveGroupChallenge)
                {
                    GroupChallengeHelper.ResetAll();
                }

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
