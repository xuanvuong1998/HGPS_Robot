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
        // records of current challenge sorted by performance (correct then time order)
        private static List<GroupChallengeRecord> currentChallengeRecord;
        private static List<int> responsibleGroups;
        public static List<string> hints = new List<string>(); // List of hints image path
        private static int CHECKING_INTERVAL = 60; // 30 seconds
        private const int OFFER_HINT_INTERVAL = 5; // 30 seconds
        private static int checkingTimerTick = 0;
        private static Timer checkingTimer = new Timer
        { AutoReset = true, Interval = 1000 };

        static GroupChallengeHelper()
        {
            checkingTimer.Elapsed += CheckingTimer_Elapsed;
        }

        private static void Wait(int miliSec)
        {
            Thread.Sleep(miliSec);
        }

        public static List<int> receivedHintStudentList = new List<int>();

        public static void SuggestHint(int groupNum)
        {
            int receivedBefore = receivedHintStudentList.Count(
                x => x == groupNum);

            if (receivedBefore == 0)
            {
                var hint = GetChallengeHint(1);
                Debug.WriteLine(hint);
                if (hint != null)
                {
                    receivedHintStudentList.Add(groupNum);
                    GlobalFlowControl.GroupChallenge.AddToServingQueue(groupNum,
                       hint);
                }

            }
            else if (receivedBefore == 1)
            {
                var hint = GetChallengeHint(2);
                Debug.WriteLine(hint);
                if (hint != null)
                {
                    receivedHintStudentList.Add(groupNum);
                    GlobalFlowControl.GroupChallenge.AddToServingQueue(groupNum,
                        GetChallengeHint(2));
                }
            }
            else
            {
                // Too enough, have to do yourself
                // No hint any more
            }
        }

        public static string GetChallengeHint(int hintNum)
        {
            var avlHints = hints.Where(x =>
                            x.Contains("C" + LessonHelper.ChallengeNumber)).ToList();

            if (hintNum > avlHints.Count) return null;

            var hintName = "C" + LessonHelper.ChallengeNumber
                      + "H" + hintNum + ".png";

            return hintName;
        }

        public static void LoadHints()
        {
            var hintFolder = FileHelper.GetHintFolderPath();

            if (Directory.Exists(hintFolder))
            {
                string[] files = Directory.GetFiles(hintFolder);

                foreach (var file in files)
                {
                    hints.Add(file);
                }
            }

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
            LessonHelper.StartGroupChallenge();
            receivedHintStudentList.Clear();
            InitRobotResponsibility();
            InitChallengeRecordList();

            GlobalFlowControl.GroupChallenge.IsHappening = true;
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

        public static void EndChallenge()
        {
            BaseHelper.Go("HOME");
            GlobalFlowControl.GroupChallenge.IsHappening = false;
            //hints.Clear();
        }

        public static void StartServing()
        {
            Debug.WriteLine("START SERVING");
            checkingTimerTick = 0;
            checkingTimer.Start();
        }

        private static bool IsRobotResponsible(int groupNum)
        {
            return responsibleGroups.Contains(groupNum);
        }

        // Check if some groups haven't submitted any answer
        private static void FindGroupNeedHelp()
        {
            var currentChallengeRecord =
                GlobalFlowControl.Lesson.GroupRecords
                .Where(x => x.ChallengeNumber == LessonHelper.ChallengeNumber);

            foreach (var groupRecord in currentChallengeRecord)
            {
                if (groupRecord.GetSubmissionCount() == 0
                    && IsRobotResponsible(groupRecord.GroupNumber)) // Haven't submitted
                                                                    // and is robot responsibility
                {
                    Debug.WriteLine("Suggest group " + groupRecord.GroupNumber);
                    SuggestHint(groupRecord.GroupNumber);
                }
            }
        }

        private static void CheckingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            checkingTimerTick++;

            if (GlobalFlowControl.GroupChallenge.IsHappening == false)
            {
                checkingTimer.Stop();
            }

            if (checkingTimerTick % CHECKING_INTERVAL == 0) // Every each 20 seconds
            {
                Debug.WriteLine("Find group who haven't submitted");
                FindGroupNeedHelp();
            }

            if (checkingTimerTick % OFFER_HINT_INTERVAL == 0 // Offer hint every each 4 seconds
                && GlobalFlowControl.GroupChallenge.IsOfferingHint == false
                && GlobalFlowControl.GroupChallenge.IsServingQueueEmpty() == false
               )
            {
                int timeLeft = LessonHelper.CurrentQuizTimeout
                                - GlobalFlowControl.Lesson.QuizElapsedTime;
                if (timeLeft >= 30)
                {
                    bool stt = CheckGroupStatusBeforeOfferHint();

                    // true: can offer, false: no need offer, that group did well already

                    if (stt)
                    {
                        Debug.WriteLine("Checked! Some group need help now. ");
                        GlobalFlowControl.GroupChallenge.IsOfferingHint = true;
                        LessonHelper.OfferHint();
                    }
                    else
                    {
                        Debug.WriteLine("No need hint any more");
                    }
                }
            }

        }

        /// <summary>
        /// At the time we added to the queue, haven't submitted or wrong ans
        /// But at the time when robot try to offer, that group has submitted 
        /// Correct answer => No need
        /// </summary>
        private static bool CheckGroupStatusBeforeOfferHint()
        {
            string nextOffer = GlobalFlowControl.GroupChallenge.GetNextOffer();

            int groupNumber = int.Parse(nextOffer.Split('-')[0]);

            var currentChallengeRecords
                    = GlobalFlowControl.Lesson.GroupRecords
                        .Where(x => x.ChallengeNumber
                            == LessonHelper.ChallengeNumber).ToList();

            var groupRecord = currentChallengeRecords.SingleOrDefault(x =>
                            x.GroupNumber == groupNumber);

            // Good job, this group has already had a correct submission
            if ((groupRecord.GetSubmissionCount() > 0
                && groupRecord.GetFinalSubResultInBool() == true)
                || groupRecord.GetLeftChancesNumber() == 0) // No more chance => No need hint
            {
                GlobalFlowControl.GroupChallenge.RemoveCurrentOffer();
                return false;
            }

            return true;
        }

        public static void MockData()
        {
            var records = GlobalFlowControl.Lesson.GroupRecords;

            var p1 = new GroupChallengeRecord
            {
                ChallengeNumber = 1,
                GroupNumber = 1,
                Submission = new List<string> { "5-0", "8-0", "12-0" }
            };
            var p2 = new GroupChallengeRecord
            {
                ChallengeNumber = 1,
                GroupNumber = 2,
                Submission = new List<string> { "2-1", "10-0", "50-0" }
            };
            var p3 = new GroupChallengeRecord
            {
                ChallengeNumber = 1,
                GroupNumber = 3,
                Submission = new List<string> { "4-1", "10-0", "30-1" }
            };
            var p4 = new GroupChallengeRecord
            {
                ChallengeNumber = 1,
                GroupNumber = 4,
                Submission = new List<string> { "5-1", "7-1", "50-1" }
            };


            //////////// c2
            var p12 = new GroupChallengeRecord
            {
                ChallengeNumber = 2,
                GroupNumber = 1,
                Submission = new List<string> { "5-0", "8-0", "12-0", "20-1" }
            };
            var p22 = new GroupChallengeRecord
            {
                ChallengeNumber = 2,
                GroupNumber = 2,
                Submission = new List<string> { "1-1", "2-1", "3-1" }
            };
            var p32 = new GroupChallengeRecord
            {
                ChallengeNumber = 2,
                GroupNumber = 3,
                Submission = new List<string> { "4-1", "10-1", "30-0" }
            };
            var p42 = new GroupChallengeRecord
            {
                ChallengeNumber = 2,
                GroupNumber = 4,
                Submission = new List<string> { "5-0", "9-0", "20-1" }
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

        public static void AssessGroupChallenge()
        {
            var records = GlobalFlowControl.Lesson.GroupRecords;

            var currentChallengeRecords
                    = records.Where(x => x.ChallengeNumber
                            == LessonHelper.ChallengeNumber).ToList();

            currentChallengeRecords.Sort(); // Sort by Comparable in GroupChallengeRecord class

            currentChallengeRecord = currentChallengeRecords;

            foreach (var rec in currentChallengeRecords)
            {
                Debug.WriteLine(rec.ToStringFormat());
            }

            AnnouceGroupResults();

            if (LessonHelper.ChallengeNumberTotal > 1
                && LessonHelper.ChallengeNumber
                == LessonHelper.ChallengeNumberTotal) // last challenge ended
            {
                // Summary all challenges

                //Synthesizer.Speak("So... We already finished " +
                //    LessonHelper.ChallengeNumberTotal + " challenges. " +
                //    "Lets summary the results of all challenges. ");

                //Wait(2000);
                //ConsolidateGroupChallenges();
            }
        }

        public static void ConsolidateGroupChallenges()
        {
            var records = GlobalFlowControl.Lesson.GroupRecords;

            // Key: groupnumber, value: passedCount-TotalSubmittedTime
            Dictionary<int, string> totalRecords
                = new Dictionary<int, string>();

            // Set values for list by iterating the records list
            foreach (var currentChallengeRecord in records)
            {
                var gnum = currentChallengeRecord.GroupNumber;
                var subTime = currentChallengeRecord.GetFinalSubTime();
                var result = currentChallengeRecord.GetFinalSubResultInBool();

                if (totalRecords.ContainsKey(gnum) == false)
                {
                    totalRecords.Add(gnum, "0-0");
                }

                var cur = totalRecords[gnum];

                // Current Number of passed challenges
                var curTotalPoints = int.Parse(cur.Split('-')[0]);
                // Current total submitted time
                var curTotalTime = int.Parse(cur.Split('-')[1]);

                if (result) // if it is a correct pass
                {
                    curTotalPoints += 1; // 1 more passed challenge
                    curTotalTime += subTime;

                    //Update again the dictionary
                    totalRecords[gnum] = curTotalPoints + "-" + curTotalTime;
                }


            }
            // Greedy rank groups by number of passed challenge and
            // Total submitted time (only correct ones)

            var sortedList = totalRecords.OrderByDescending(x =>
                        int.Parse(x.Value.Split('-')[0]))
                         .ThenBy(x => int.Parse(x.Value.Split('-')[1]))
                         .ToList();

            Synthesizer.Speak($"After {LessonHelper.ChallengeNumberTotal}" +
                $" challenges, we have the result like this...");

            int rank = 0;
            foreach (var group in sortedList)
            {
                rank++;
                int gnum = group.Key;

                int correctQty = int.Parse(group.Value.Split('-')[0]);
                int subTime = int.Parse(group.Value.Split('-')[1]);

                var members = TablePositionHelper.GetMembersByGroupNumber(gnum);

                string speech = "Group " + gnum + ". ";

                foreach (var m in members)
                {
                    speech += m + ". ";
                }

                if (rank <= sortedList.Count / 2)
                {
                    int rdmNum = new Random().Next(2);

                    if (rdmNum == 0)
                    {
                        speech += "you guys are the number " + rank + " in overall. " +
                                                "Very good job. Congratulation!";
                    }
                    else
                    {
                        speech += "Your overall standing is number " + rank
                            + ". Congratulation!";
                    }

                    Synthesizer.Speak(speech);

                    if (rank == 1)
                    {
                        AudioHelper.PlayChampionSound();
                    }
                    else
                    {
                        AudioHelper.PlayApplauseSound();
                    }
                }
                else
                {
                    speech += "you are the top " + rank + ". ";
                    int rdmNum = new Random().Next(2);
                    if (rdmNum == 0)
                    {
                        speech += "you guys are also very good. Good luck for the " +
                            "next time. Be more careful and faster";
                    }
                    else
                    {
                        speech += "you all also did a good job. Do your best " +
                            "in the next challenge to get the higher position. ";
                    }
                    Synthesizer.Speak(speech);
                    AudioHelper.PlayApplauseSound();
                }

            }

        }

        /// <summary>
        /// Check those groups did not have correct last submission
        /// But had correct submission before???
        /// </summary>
        private static void FindAnyCorrectAnswer(int remainingCnt)
        {
            int regretCnt = 0;

            List<int> mistakeGroups = new List<int>();

            foreach (var group in currentChallengeRecord)
            {
                bool finalRes = group.GetFinalSubResultInBool();

                if (finalRes == true) continue;
                int correctNum = group.GetNumberOfCorrectSub();

                var firstCorrect = group.GetFirstCorrectSubmission();

                if (correctNum > 0)
                {
                    mistakeGroups.Add(group.GroupNumber);
                    regretCnt++;
                    int rdmNum = new Random().Next(2);
                    if (rdmNum == 0)
                    {
                        Synthesizer.Speak($"Oh...dear..." +
                            $"Group {group.GroupNumber}. you guys had {correctNum} times correct. " +
                            $"But. Unluckily, your last submission is not correct. ");

                        AudioHelper.PlaySadSound();
                    }
                    else
                    {
                        Synthesizer.Speak($"Oh...No...Group {group.GroupNumber}. " +
                            $"It is so regretful... " +
                            $"You guys actually submitted correct answer at the " +
                            $"second of {firstCorrect.Split('-')[0]}. " +
                            $"But sadly, your final answer is not correct. ");

                        AudioHelper.PlaySadSound();

                    }
                    Wait(1500);

                }
            }

            if (regretCnt > 0 && regretCnt < remainingCnt)
            {
                string speech = "So. ";

                foreach (var g in mistakeGroups)
                {
                    speech += "Group " + g + ". ";
                }

                speech += "All of you are also very good. " +
                    "Just be careful one thing. Remember to check" +
                    " your answer before submitting the last one. ok?." +
                    " Please Try your best. Don't give up. ";

                Synthesizer.Speak(speech);
            }
            else
            {
                Synthesizer.Speak(
                    "another groups, all your answers are not correct. " +
                    "Don't worry. " +
                    "Let's try your best in the next challenge. " +
                    "All of you, please don't give up. ");
            }

            AudioHelper.PlayApplauseSound();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if all group have no correct answer</returns>
        private static bool CheckFirstTopGroup()
        {
            bool firstG = currentChallengeRecord[0].GetFinalSubResultInBool();

            if (firstG == true) return false;

            string speech = "Well... I am very sorry to say that, " +
                "with this challenge, no group got any correct answer. ";
            Synthesizer.Speak(speech);

            AudioHelper.PlaySadSound();

            speech = "Ok. Don't worry about it. " +
                "Let me check again, is there any time that you " +
                "have submitted any correct answer. ";

            Synthesizer.Speak(speech);

            Wait(2000);

            FindAnyCorrectAnswer(currentChallengeRecord.Count);

            return true;
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

            Thread.Sleep(1500);
            // All group answer are wrong
            bool isWorst = CheckFirstTopGroup();

            if (isWorst) return;

            switch (rdmNum)
            {
                case 0: Synthesizer.Speak("Are you confident to become the number 1? "); break;
                case 1: Synthesizer.Speak("Are you ready? "); break;
                case 2: Synthesizer.Speak("Where do you think your ranking is? "); break;
            }

            Thread.Sleep(2000);

            Synthesizer.Speak("Here it is! ");

            Wait(1500);

            int rank = 1;
            int i = 0;

            while (i < currentChallengeRecord.Count)
            {
                var curGroup = currentChallengeRecord[i];

                int groupNum = curGroup.GroupNumber;
                int subTime = curGroup.GetFinalSubTime();
                bool isCorrect = curGroup.GetFinalSubResultInBool();

                string speech = "";
                if (i == 0)
                {
                    rdmNum = new Random().Next(3);
                    switch (rdmNum)
                    {
                        case 0: speech = "Very good job, "; break;
                        case 1: speech = "Excellent, "; break;
                        case 2: speech = "Amazing, "; break;
                    }

                    speech += $" group {groupNum}...... "
                        + $"You are the number 1... Within only {subTime} seconds, you " +
                        $"become the fastest group, who got the correct answer! ";

                    speech += "Everyone, please give a round of applause for" +
                        $"group {groupNum}.";

                    Synthesizer.Speak(speech);

                    AudioHelper.PlayChampionSound();
                }
                else if (isCorrect)
                {
                    // Same rank with the higher one
                    if (subTime == currentChallengeRecord[i - 1].GetFinalSubTime())
                    {
                        speech = $"Unbelievable! Group {groupNum} has " +
                            $"the same submisson time with group " +
                            $"{currentChallengeRecord[i - 1].GroupNumber}. " +
                            $"Therefore, Group {groupNum} is also the top " +
                            $"{rank} of this challenge. ";

                        AudioHelper.PlayApplauseSound();
                    }
                    else
                    {
                        rank++;

                        rdmNum = new Random().Next(3);
                        switch (rdmNum)
                        {
                            case 0:
                                speech = "With the time of " + subTime + ", I wanna " +
                                 "say that... ";
                                speech += $"Group {groupNum}... Congratulation, " +
                         $"you are the top {rank} of this challenge. ";
                                break;
                            case 1:
                                speech = "Next group is... ";
                                speech += $"Group {groupNum}... Well done, " +
                            $"you are the top {rank} of this challenge. " +
                            $"Your submitted time is {subTime}. ";
                                break;

                            case 2:
                                speech = "The following group is... ";
                                speech += $"Group {groupNum}. Good job, " +
                                    $"your first correct answer, was recorded at " +
                                    $"the second of {subTime}. And now, " +
                            $"you are the top {rank} of this challenge. ";
                                break;
                        }

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

            Synthesizer.Speak("What about the remaining groups? ");
            //Synthesizer.Speak("I am so sorry to say that your final " +
            //    "answer is not correct. But. Don't worry. " +
            //    "Now, I will check again, to see is there any time you had " +
            //    "a correct answer. ");

            Wait(1500);
            FindAnyCorrectAnswer(currentChallengeRecord.Count - i + 1);
        }
    }
}
