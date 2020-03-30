using SpeechLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace HGPS_Robot
{
    class GroupChallengeHelper
    {
        #region Properties

        // records of current challenge sorted by performance (correct then time order)
        private static List<GroupChallengeRecord> currentChallengeRecord;
        private static List<int> responsibleGroups;
        public static List<string> hints = new List<string>(); // List of hints image path
        private const int CHECKING_INTERVAL = 60; // 30 seconds
        private const int OFFER_HINT_INTERVAL = 3; //
        private const int MIN_REMAINING_OFFER_TIME = 25; //
        private static int checkingTimerTick = 0;

        public static Dictionary<int, Quiz> GroupChallengeSteps { get; set; }
                        = new Dictionary<int, Quiz>();

        private static Timer checkingTimer = new Timer
        { AutoReset = true, Interval = 1000 };

        static GroupChallengeHelper()
        {
            checkingTimer.Elapsed += CheckingTimer_Elapsed;
        }

        #endregion

        #region Group Challenge Steps

        /// <summary>
        /// Check if the command is one of  group challenge property 
        /// And add to the *steps* list
        /// </summary>
        /// <param name="cmd"></param>
        public static void CheckForGroupChallenge(RobotCommand cmd)
        {
            var type = cmd.Type.ToLower();
            var val = cmd.Value;

            if (type.Contains("step"))
            {
                int step = type[4] - '0';

                if (GroupChallengeSteps.ContainsKey(step) == false)
                {
                    GroupChallengeSteps.Add(step,
                        new Quiz { QuizFormat = "text" });
                }

                var curStepQuiz = GroupChallengeSteps[step];

                if (type.Contains("question"))
                {
                    curStepQuiz.QuestionContent = val;
                }
                else if (type.Contains("answer"))
                {
                    curStepQuiz.Answer = val;
                }
                else if (type.Contains("points"))
                {
                    curStepQuiz.Points = int.Parse(val);
                }
                else if (type.Contains("timeout"))
                {
                    curStepQuiz.TimeOut = int.Parse(val);
                }
                else if (type.Contains("choice"))
                {
                    curStepQuiz.QuizFormat = "mcq";
                    curStepQuiz.Choices += val + ";";
                }
            }
        }



        /// <summary>
        /// Total time out of all steps
        /// </summary>
        /// <returns></returns>
        public static int GetTotalTimeOut()
        {
            int cnt = 0;
            foreach (var step in GroupChallengeSteps)
            {
                cnt += step.Value.TimeOut;
            }

            return cnt;
        }

        #endregion

        #region Init
        private static bool IsRobotResponsible(int groupNum)
        {
            return responsibleGroups.Contains(groupNum);
        }
        public static void ResetAll()
        {
            InitRobotResponsibility();
            hints.Clear();
            LoadHints();
            GroupChallengeSteps.Clear();
            GlobalFlowControl.GroupChallenge.ResetQueue();
            GlobalFlowControl.Lesson.GroupRecords.Clear();
        }
        private static void InitChallengeRecordList()
        {
            var groupQty = TablePositionHelper.GetGroupQuantity();

            var globalRecord = GlobalFlowControl.Lesson.GroupRecords;

            for (int i = 1; i <= groupQty; i++)
            {
                globalRecord.Add(new GroupChallengeRecord
                {
                    ChallengeNumber = LessonHelper.ChallengeNumber,
                    GroupNumber = i
                });
            }
        }

        public static void InitNewChallenge()
        {
            BaseHelper.Go("HOME");

            LessonHelper.InitGroupChallenge();

            receivedHintStudentList.Clear();

            InitChallengeRecordList();

            GlobalFlowControl.GroupChallenge.IsHappening = true;

            RobotActionHelper.StopAll();

            // periodically check struggling groups and offer hints
            StartServing();
        }

        /// <summary>
        /// Assign middle-class groups for robot take care(offer hints during group challenge)
        /// </summary>
        public static void InitRobotResponsibility()
        {
            var ranks = StudentsPerformanceHelper.GetStudentsRanking();

            var tables = TablePositionHelper.TablePositions;

            Dictionary<int, int> groupLevel = new Dictionary<int, int>();

            foreach (var table in tables)
            {
                if (table.GroupNumber != null)
                {
                    int gNum = (int)table.GroupNumber;

                    string std = table.Student_Id;

                    if (ranks.ContainsKey(std))
                    {
                        if (groupLevel.ContainsKey(gNum) == false)
                        {
                            groupLevel.Add(gNum, ranks[std]);
                        }
                        else
                        {
                            groupLevel[gNum] = (groupLevel[gNum] + ranks[std]) / 2;
                        }
                    }
                }
            }

            var list = groupLevel.OrderBy(x => x.Value).ToList();

            int parts = (list.Count + 2) / 3; // 3 divided level (smart - normal - weak)

            List<int> chosenGroups = new List<int>();
            // Choose 'middle-class'
            for (int i = parts; i < parts * 2; i++)
            {
                chosenGroups.Add(list[i].Key);
            }

            if (parts == 0)
            {
                for (int i = 1; i <= TablePositionHelper.GetGroupQuantity(); i++)
                {
                    chosenGroups.Add(i);
                }
            }

            Debug.WriteLine("Group responsibility");

            foreach (var item in chosenGroups)
            {
                Debug.WriteLine("Group " + item);
            }

            responsibleGroups = chosenGroups;
        }

        #endregion 

        #region Hints
        public static List<int> receivedHintStudentList = new List<int>();
        private static void FindGroupNeedHelp()
        {

        }

        /// <summary>
        /// Make sure the remaining time is not less than a specified period
        /// </summary>
        /// <returns></returns>
        private static bool CheckTimeElapsedBeforeOfferHint()
        {
            string nextOffer = GlobalFlowControl.GroupChallenge.GetNextOffer();

            int offerStep = nextOffer.Split('-')[1][3] - '0'; // C1H2 -> 2: is step number

            int preStepTimeout = 0;

            for (int i = 1; i < offerStep; i++)
            {
                preStepTimeout = GroupChallengeSteps[i].TimeOut;
            }

            int curStepTimeElapsed = GlobalFlowControl.Lesson.QuizElapsedTime
                                - preStepTimeout;
            int curStepTimeOut = GroupChallengeSteps[offerStep].TimeOut;

            if (curStepTimeOut - curStepTimeElapsed < MIN_REMAINING_OFFER_TIME)
            {
                GlobalFlowControl.GroupChallenge.RemoveCurrentOffer();
                return false;
            }

            return true;
        }

        private static bool CheckGroupStatusBeforeOfferHint()
        {
            Debug.WriteLine("Checking group status before offering hint");

            if (CheckTimeElapsedBeforeOfferHint() == false)
            {
                Debug.WriteLine("Time of the step is almost over. no offer");
                return false;
            }

            string nextOffer = GlobalFlowControl.GroupChallenge.GetNextOffer();
            int groupNumber = int.Parse(nextOffer.Split('-')[0]);

            var records = GlobalFlowControl.GroupChallenge.GroupResults;


            var offerStep = nextOffer.Split('-')[1][3] - '0';
            var stepSub = records[groupNumber - 1][offerStep - 1];
            var subCnt = stepSub.Split('-')[1];

            // Good job, this group has already had a correct submission
            if (stepSub[0] == '1' || (stepSub[0] == '0' && subCnt == "2"))
            {
                Debug.WriteLine("Oh. group " + groupNumber
                    + " has submitted a right answer. Or no more chance");
                GlobalFlowControl.GroupChallenge.RemoveCurrentOffer();
                return false;
            }

            return true;
        }

        public static void SuggestHint(int groupNum, int stepNum)
        {
            var hint = GetChallengeHint(stepNum);

            if (hint != null)
            {
                GlobalFlowControl.GroupChallenge.AddToServingQueue(groupNum, hint);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hintNum"></param>
        /// <example >C1H1.png</example>
        /// <returns>file name with extension of a  hint</returns>
        public static string GetChallengeHint(int hintNum)
        {
            string hintConvention = "C" + LessonHelper.ChallengeNumber
                                + "H" + hintNum;

            Debug.WriteLine("Hint Convention");

            string foundHint = hints.Where(x =>
                            x.Contains(hintConvention)).FirstOrDefault();

            Debug.WriteLine("Found hint : " + foundHint);
            if (foundHint != null)
            {
                return hintConvention;
            }

            return null;
        }

        public static string GetFullHintImagePath(string hintConvention)
        {
            string foundHint = hints.Where(x =>
                            x.Contains(hintConvention)).FirstOrDefault();

            return foundHint;
        }

        public static void LoadHints()
        {
            var hintFolder = FileHelper.GetHintFolderPath();
            //var hintFolder = @"C:\Users\Surface\Dropbox\Lessons\Week 08 - Group Challenge Breaking Down\Hints";

            if (Directory.Exists(hintFolder))
            {
                string[] files = Directory.GetFiles(hintFolder);

                foreach (var file in files)
                {
                    hints.Add(file);
                }
            }

        }

        #endregion

        #region Flow
        private static void Wait(int miliSec)
        {
            Thread.Sleep(miliSec);
        }

        // Start Serving hints to groups need help (periodically checking)
        public static void StartServing()
        {
            Debug.WriteLine("START SERVING");
            checkingTimerTick = 0;
            checkingTimer.Start();
        }

        #endregion

        #region Checking Hints Timer

        private static void CheckingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            checkingTimerTick++;

            if (GlobalFlowControl.GroupChallenge.IsHappening == false)
            {
                checkingTimer.Stop();
            }

            if (checkingTimerTick % CHECKING_INTERVAL == 0) // Every each 20 seconds
            {
                //Debug.WriteLine("Find group who haven't submitted");
                //FindGroupNeedHelp();
            }

            if (checkingTimerTick % OFFER_HINT_INTERVAL == 0 // Offer hints after a short period
                && GlobalFlowControl.GroupChallenge.IsOfferingHint == false
                && GlobalFlowControl.GroupChallenge.IsServingQueueEmpty() == false)
            {

                bool stt = CheckGroupStatusBeforeOfferHint();

                // true: can offer, false: no need offer, this group had a 
                // correct sub (after first wrong sub)
                // Or the remaining time is less than a specified time

                if (stt)
                {
                    Debug.WriteLine("Checked! Group will be served");
                    GlobalFlowControl.GroupChallenge.IsOfferingHint = true;
                    LessonHelper.OfferHint();
                }
                else
                {
                    Debug.WriteLine("Checked! Don't offer hint");
                }
            }
        }

        #endregion

        #region Mock DATA
        public static void MockData()
        {
            var records = GlobalFlowControl.Lesson.GroupRecords;

            var p1 = new GroupChallengeRecord
            {
                ChallengeNumber = 1,
                GroupNumber = 1,
                StepSubmissions = new List<string> { "5-0", "8-0", "12-0" }
            };
            var p2 = new GroupChallengeRecord
            {
                ChallengeNumber = 1,
                GroupNumber = 2,
                StepSubmissions = new List<string> { "2-1", "10-0", "50-0" }
            };
            var p3 = new GroupChallengeRecord
            {
                ChallengeNumber = 1,
                GroupNumber = 3,
                StepSubmissions = new List<string> { "4-1", "10-0", "30-1" }
            };
            var p4 = new GroupChallengeRecord
            {
                ChallengeNumber = 1,
                GroupNumber = 4,
                StepSubmissions = new List<string> { "5-1", "7-1", "50-1" }
            };


            //////////// c2
            var p12 = new GroupChallengeRecord
            {
                ChallengeNumber = 2,
                GroupNumber = 1,
                StepSubmissions = new List<string> { "5-0", "8-0", "12-0", "20-1" }
            };
            var p22 = new GroupChallengeRecord
            {
                ChallengeNumber = 2,
                GroupNumber = 2,
                StepSubmissions = new List<string> { "1-1", "2-1", "3-1" }
            };
            var p32 = new GroupChallengeRecord
            {
                ChallengeNumber = 2,
                GroupNumber = 3,
                StepSubmissions = new List<string> { "4-1", "10-1", "30-0" }
            };
            var p42 = new GroupChallengeRecord
            {
                ChallengeNumber = 2,
                GroupNumber = 4,
                StepSubmissions = new List<string> { "5-0", "9-0", "20-1" }
            };

            records.Add(p1);
            records.Add(p2);
            records.Add(p3);
            records.Add(p4);

            records.Add(p12);
            records.Add(p22);
            records.Add(p32);
            records.Add(p42);

        }
        #endregion

        #region Results

        private static int GetNumberOfCorrectSteps(List<string> stepSubs)
        {
            int cnt = 0;
            foreach (var step in stepSubs)
            {
                cnt += step[1] - '0';
            }
            return cnt;
        }

        private static int GetNumberOfSubmissions(List<string> stepSubs)
        {
            int cnt = 0;
            foreach (var step in stepSubs)
            {
                cnt += step[2] - '0';
            }
            return cnt;
        }

        private static int GetTotalSubTime(List<string> stepSubs)
        {
            int cnt = 0;
            foreach (var step in stepSubs)
            {
                cnt += int.Parse(step.Split('-')[2]);
            }
            return cnt;
        }

        private static int CompareStepsSubmission(List<string> g1, List<string> g2)
        {
            int x = GetNumberOfCorrectSteps(g1);
            int y = GetNumberOfCorrectSteps(g2);

            if (x != y) return y - x; // who has more passed steps will have smaller order index

            x = GetNumberOfSubmissions(g1);
            y = GetNumberOfSubmissions(g2);

            if (x != y) return x - y; // prioritize who has less submissions

            x = GetTotalSubTime(g1);
            y = GetTotalSubTime(g2);

            return x - y; // Last factor 

        }

        /// <summary>
        /// Convert List of List(string) to List of GroupChallengeRecord
        /// </summary>
        private static void ConvertToGroupRecordModel()
        {
            var records = GlobalFlowControl.GroupChallenge.GroupResults;
            if (currentChallengeRecord == null)
            {
                currentChallengeRecord = new List<GroupChallengeRecord>();
            }
            else
            {
                currentChallengeRecord.Clear();
            }
            for (int i = 0; i < records.Count; i++)
            {
                var newRecord = new GroupChallengeRecord
                {
                    ChallengeNumber = LessonHelper.ChallengeNumber,
                    GroupNumber = i + 1,
                    StepSubmissions = records[i]
                };

                currentChallengeRecord.Add(newRecord);
            }

            currentChallengeRecord.Sort();
        }

        public static List<int> GetTop3()
        {
            List<int> top3 = new List<int>();
            for(int i = 0; i < Math.Min(3, currentChallengeRecord.Count); i++)
            {
                top3.Add(currentChallengeRecord[i].GroupNumber);
            }

            return top3;
        }
        public static void AssessGroupChallenge()
        {
            ConvertToGroupRecordModel();

            AnnouceGroupResults();

        }

        private static string GetMembersSpeech(int group)
        {
            List<string> members = TablePositionHelper.GetMembersByGroupNumber(group);

            string speech = "";

            foreach (var std in members)
            {
                speech += std + ", ";
            }

            return speech;
        }

        private static void AnnouceGroupResults()
        {
            int rdmNum = new Random().Next(3);

            var secondsLeft = LessonHelper.CurrentQuizTimeout
                        - GlobalFlowControl.Lesson.QuizElapsedTime;

            switch (rdmNum)
            {
                case 0:
                    Synthesizer.Speak("Well done every body, the time is over. Now, " +
             "I am very excited to reveal your group results. "); break;
                case 1:
                    Synthesizer.Speak("Time is over. Now let's go to the " +
                        "most interesting part, to see which group, " +
                        "is the " +
                        "champion of this challenge. ");
                    break;
                case 2:
                    Synthesizer.Speak("Time's up everybody. Let's " +
                        "see how good is your result. ");
                    break;
            }

            Thread.Sleep(1000);

            switch (rdmNum)
            {
                case 0: Synthesizer.Speak("Are you confident to become the number 1? "); break;
                case 1: Synthesizer.Speak("Are you ready? "); break;
                case 2: Synthesizer.Speak("Where do you think your ranking is? "); break;
            }

            Thread.Sleep(2000);

            Synthesizer.Speak("Here it is! ");

            Wait(1000);

            SyncHelper.RequestOpeningURL("group-challenge");

            int rank = 1;
            int i = 0;


            // Sorted by performance

            while (i < currentChallengeRecord.Count)
            {
                var curGroup = currentChallengeRecord[i];

                int groupNum = curGroup.GroupNumber;
                int passedCnt = curGroup.GetNumberOfCorrectSteps();
                int totalSubTime = curGroup.GetTotalSubTime();

                string speech = "";

                if (i == 0)
                {
                    rdmNum = new Random().Next(3);
                    switch (rdmNum)
                    {
                        case 0: speech += "Very good job, "; break;
                        case 1: speech += "Excellent, "; break;
                        case 2: speech += "Amazing, "; break;
                    }
                    
                    speech += $" group {groupNum}. ";
                    if (i < 3)
                    {
                        speech += GetMembersSpeech(groupNum);
                    }

                    speech += $"You are the number 1... Within only {totalSubTime} seconds, you " +
                        $"has passed {passedCnt} steps of the group challenge. ";

                    speech += "Everyone, please give a big applause for" +
                        $"group {groupNum}.";

                    Synthesizer.Speak(speech);

                    AudioHelper.PlayChampionSound();
                }
                else if (passedCnt > 0)
                {
                    rank++;

                    rdmNum = new Random().Next(3);
                    switch (rdmNum)
                    {
                        case 0:
                            speech += $"Group {groupNum}... Congratulation, " +
                     $"you are the top {rank} of this challenge. ";
                            break;
                        case 1:
                            speech = "Next group is... ";
                            speech += $"Group {groupNum}... Well done, " +
                        $"you are the top {rank} of this challenge. " +
                        $"Your total submitted time is {totalSubTime} seconds. ";
                            break;

                        case 2:
                            speech = "The following group is... ";
                            speech += $"Group {groupNum}. Good job, " +
                                $"you got {passedCnt} correct steps. " +
                        $"you are the top {rank} of this challenge. ";
                            break;
                    }


                    rdmNum = new Random().Next(2);
                    if (rdmNum == 0)
                    {
                        speech += "Everyone, please, give another round of applause " +
                                                "to group " + groupNum;
                    }
                    else
                    {
                        speech += "Everybody, why don't you give another applause for" +
                            " group " + groupNum;
                    }

                    Synthesizer.Speak(speech);
                    AudioHelper.PlayApplauseSound();
                    Wait(1000);
                }
                else
                {
                    break;
                }
                i++;
            }

            Wait(2000);

            if (i == currentChallengeRecord.Count) return;

            Synthesizer.Speak("Other groups. You guys have no any correct " +
                "submission. Please try your best in the next challenge ");

        }
        #endregion

        #region Submissions

        // Receive from robot
        internal static void ReceiveNewSubmission(string groupSub)
        {
            string[] info = groupSub.Split('-');

            string groupNum = info[0];
            string step = info[1];
            string result = info[2];
            string subCnt = info[3];
            string subTime = info[4];

            if (result == "1")
            {
                Synthesizer.SpeakAsync("Group " + groupNum + " has passed step " +
                    "number " + step);
            }
            else
            {
                if (subCnt == "2")
                {
                    //Synthesizer.SpeakAsync("NO MORE CHANCE FOR group " + groupNum);
                    // second time also wrong
                }
                else if (subCnt == "1")
                {
                    // First time wrong, try to offer a hint (if any)
                    //Synthesizer.SpeakAsync("1 MORE CHANCE FOR group " + groupNum);

                    Debug.WriteLine("Group " + groupNum + " submitted wrong " +
                        "in the first time");

                    SuggestHint(int.Parse(groupNum), int.Parse(step));
                }
            }

        }
        #endregion
    }
}
