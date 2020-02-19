using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SpeechLibrary;

namespace HGPS_Robot
{
    class GroupChallengeHelper
    {
        private static List<GroupChallengeRecord> record;

        public static void AssessGroupChallenge()
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
            var p5 = new GroupChallengeRecord
            {
                ChallengeNumber = 1,
                GroupNumber = 5,
                Submission = new List<string> { "4-0", "6-0", "10-1" }
            };

            records.Add(p1);
            records.Add(p2);
            records.Add(p3);
            records.Add(p4);
            records.Add(p5);

            var currentChallengeRecords
                    = records.Where(x => x.ChallengeNumber
                            == 1).ToList();

            currentChallengeRecords.Sort(); // Sort by Comparable in GroupChallengeRecord class

            record = currentChallengeRecords;

            foreach (var record in currentChallengeRecords)
            {
                Debug.WriteLine(record.ToStringFormat());
            }

            AnnouceGroupResults();
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
                            $"You guys actually submitted a correct answer. " +
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

                speech += "You guys are also very good. " +
                    "Just be careful one thing. That is check" +
                    " your answer before submitting. ok?. Try your best. Don't give up. ";

                Synthesizer.Speak(speech);
            }
            else
            {
                Synthesizer.Speak("Well. Unfortunately for " +
                    "another groups, you guys did not " +
                    "have any correct submission ever. " +
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
                case 3:
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
                        $"are the fastest group, got the correct answer! ";

                    speech += "Everyone, please give a round of applause for" +
                        $"group {groupNum}.";

                    Synthesizer.Speak(speech);

                    AudioHelper.PlayApplauseSound();
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
                                    $"the second of {subTime}. And  " +
                            $"you now, are the top {rank} of this challenge. ";
                                break;
                        }

                    }

                    speech += "Everyone, please, give another round of applause, " +
                        "to group " + groupNum;
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

            Wait(1000);

            Synthesizer.Speak("Great. Now I will try to check the remaining " +
                "groups. Is there any time you submitted the correct answer. ");
            Wait(1500);
            FindAnyCorrectAnswer(record.Count - i + 1);
        }
    }
}
