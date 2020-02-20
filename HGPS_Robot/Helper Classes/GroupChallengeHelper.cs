using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpeechLibrary;

namespace HGPS_Robot
{
    class GroupChallengeHelper
    {
        private static List<GroupChallengeRecord> record;
        public static List<int> receivedHintStudentList = new List<int>();

        public static void SuggestHint(int groupNum)
        {
            int receivedBefore = receivedHintStudentList.Count(
                x => x == groupNum);

            string hint = "";
            if (receivedBefore == 0)
            {
                hint = GetChallengeHint(2);
                receivedHintStudentList.Add(groupNum);
                MessageBox.Show("GO TO Group " + groupNum);
                Synthesizer.Speak("This is " + hint);
            }
            else if (receivedBefore == 1){
                hint = GetChallengeHint(1);
                receivedHintStudentList.Add(groupNum);
                MessageBox.Show("GO TO Group " + groupNum);
                Synthesizer.Speak("This is " + hint);
            }
            else
            {
                // Too enough, have to do yourself
                // No hint
            }
                        
        }

        public static string GetChallengeHint(int hintNum)
        {
            return "Hint " + hintNum;
        }

        public static void InitNewChallenge()
        {
            LessonHelper.ChallengeNumber++;
            receivedHintStudentList.Clear();
        }

        public static void InitMockData()
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

            record = currentChallengeRecords;

            foreach (var record in currentChallengeRecords)
            {
                Debug.WriteLine(record.ToStringFormat());
            }

            AnnouceGroupResults();

            if (LessonHelper.ChallengeNumberTotal > 1
                && LessonHelper.ChallengeNumber
                == LessonHelper.ChallengeNumberTotal) // last challenge ended
            {
                // Summary all challenges

                Synthesizer.Speak("So... We already finished " +
                    LessonHelper.ChallengeNumberTotal + " challenges. " +
                    "Lets summary the results of all challenges. ");

                Wait(2000);
                ConsolidateGroupChallenges();


            }
        }

        public static void ConsolidateGroupChallenges()
        {
            var records = GlobalFlowControl.Lesson.GroupRecords;

            // Key: groupnumber, value: passedCount-TotalSubmittedTime
            Dictionary<int, string> totalRecords
                = new Dictionary<int, string>();

            // Set values for list by iterating the records list
            foreach (var record in records)
            {
                var gnum = record.GroupNumber;
                var subTime = record.GetFinalSubTime();
                var result = record.GetFinalSubResultInBool();

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
                        speech += "you guys are the top " + rank + " in overall. " +
                                                "Very good job. Congratulation!";
                    }
                    else
                    {
                        speech += "Your overall standing is number " + rank
                            + ". Congratulation!";
                    }

                    Synthesizer.Speak(speech);
                    AudioHelper.PlayChampionSound();
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

        private static void Wait(int miliSec)
        {
            Thread.Sleep(miliSec);
        }

        /// <summary>
        /// Check those groups did not have correct last submission
        /// But had correct submission before???
        /// </summary>
        private static void FindAnyCorrectAnswer(int remainingCnt)
        {
            int regretCnt = 0;

            List<int> mistakeGroups = new List<int>();

            foreach (var group in record)
            {
                bool finalRes = group.GetFinalSubResultInBool();

                if (finalRes == true) continue;
                int correctNum = group.GetNumberOfCorrectSub();

                if (correctNum > 0)
                {
                    mistakeGroups.Add(group.GroupNumber);
                    regretCnt++;
                    int rdmNum = new Random().Next(2);
                    if (rdmNum == 0)
                    {
                        Synthesizer.Speak($"Wow. Believe it or not. " +
                            $"Group {group.GroupNumber}. you guys had {correctNum} times correct. " +
                            $"But. Unluckily, your last submission is not correct. ");

                        AudioHelper.PlayInCorrectSound();
                    }
                    else
                    {
                        Synthesizer.Speak($"Group {group.GroupNumber}. " +
                            $"It is so regretful... " +
                            $"You guys actually submitted correct answer before. " +
                            $"But your final answer is not correct. ");
                        AudioHelper.PlayInCorrectSound();

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
                Synthesizer.Speak("Well. Unfortunately for " +
                    "another groups, your answer is very near to the " +
                    "correct answer, but not fully correct. Don't worry. " +
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
            bool firstG = record[0].GetFinalSubResultInBool();

            if (firstG == true) return false;

            string speech = "Well... I am very sorry to say that, " +
                "with this challenge, no group got any correct answer. ";
            Synthesizer.Speak(speech);

            AudioHelper.PlayInCorrectSound();

            speech = "Ok. Don't too worry about it. " +
                "Let's me check again, is there any time that, you " +
                "have submitted any correct answer. ";

            Synthesizer.Speak(speech);

            Wait(2000);

            FindAnyCorrectAnswer(record.Count);

            return true;
        }

        private static void AnnouceGroupResults()
        {

            int rdmNum = new Random().Next(3);

            switch (rdmNum)
            {
                case 0:
                    Synthesizer.Speak("Well done every body, the time is over. Now, " +
             "I am very excited to reveal your group results. "); break;
                case 1:
                    Synthesizer.Speak("Time is over. Now let's go to the " +
                        "most interesting part. Where we can see which group " +
                        "is the " +
                        "champion of this challenge. ");
                    break;
                case 2:
                    Synthesizer.Speak("Time's up everybody. Let's " +
                        "see how good is your result. ");
                    break;
            }

            // All group answer are wrong
            bool isWorst = CheckFirstTopGroup();

            if (isWorst) return;


            switch (rdmNum)
            {
                case 0: Synthesizer.Speak("Are you confident to become the top 1? "); break;
                case 1: Synthesizer.Speak("Are you ready? "); break;
                case 2: Synthesizer.Speak("Where do you think your ranking is? "); break;
            }

            Thread.Sleep(2000);

            Synthesizer.Speak("Here it is! ");

            Wait(1500);

            int rank = 1;
            int i = 0;

            while (i < record.Count)
            {
                var curGroup = record[i];

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
                        + $"You are the top 1... Within only {subTime} seconds, you " +
                        $"become the fastest group, who got the correct answer! ";

                    speech += "Everyone, please give a round of applause for" +
                        $"group {groupNum}.";

                    Synthesizer.Speak(speech);

                    AudioHelper.PlayChampionSound();
                }
                else if (isCorrect)
                {
                    // Same rank with the higher one
                    if (subTime == record[i - 1].GetFinalSubTime())
                    {
                        speech = $"Unbelievable! Group {groupNum} has " +
                            $"the same submisson time with group " +
                            $"{record[i - 1].GroupNumber}. " +
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
                            $"you are the top {rank} of this challenge. ";
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
                        speech += "Everyone, please, give another round of applause, " +
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

            if (i == record.Count) return;

            Synthesizer.Speak("What's about the remaining groups? ");
            Wait(1500);
            Synthesizer.Speak("I am so sorry to say that your final " +
                "answer is not correct. But. Don't too worry. " +
                "Now, I will check again, to see is there any time you had " +
                "a correct answer. ");

            Wait(1500);
            FindAnyCorrectAnswer(record.Count - i + 1);
        }
    }
}
