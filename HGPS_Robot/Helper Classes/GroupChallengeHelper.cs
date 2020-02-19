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
                Submission = new List<string> { "5-1", "7-1", "12-0" }
            };
            var p2 = new GroupChallengeRecord
            {
                ChallengeNumber = 1,
                GroupNumber = 2,
                Submission = new List<string> { "2-0", "10-0", "20-1" }
            };
            var p3 = new GroupChallengeRecord
            {
                ChallengeNumber = 1,
                GroupNumber = 3,
                Submission = new List<string> { "4-0", "10-0", "30-1" }
            };
            var p4 = new GroupChallengeRecord
            {
                ChallengeNumber = 1,
                GroupNumber = 4,
                Submission = new List<string> { "4-0", "20-1", "27-1" }
            };

            records.Add(p1);
            records.Add(p2);
            records.Add(p3);
            records.Add(p4);

            var xxx = p4.GetFinalResult();

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
        private static void FindAnyCorrectAnswer()
        {
            bool existCorrectAns = false;

            List<int> mistakeGroups = new List<int>();
            
            foreach (var group in record)
            {
                bool finalRes = group.GetFinalSubResultInBool();

                if (finalRes == true) continue;
                int correctNum = group.GetNumberOfCorrectSub();

                if (correctNum > 0)
                {
                    mistakeGroups.Add(group.GroupNumber);
                    existCorrectAns = true;
                    int rdmNum = new Random().Next(2);
                    if (rdmNum == 0)
                    {
                        Synthesizer.Speak($"Wow. Group {group.GroupNumber}. " +
                            $"Unfortunately, your final answer is incorrect, " +
                            $"But. your group also had {correctNum} times correct.");
                    }
                    else
                    {
                        Synthesizer.Speak($"Group {group.GroupNumber}. " +
                            $"I am very regretful... to " +
                            $"You guys actually submitted correct answer " +
                            $"{correctNum} times before. ");
                    }

                    Wait(1500);

                }

            }

            if (existCorrectAns)
            {
                string speech = "So. ";

                foreach (var g in mistakeGroups)
                {
                    speech += "Group " + g + ". ";
                }

                speech += "You guys are also very good. " +
                    "Just be careful, and do not make mistake " +
                    "in your last submission. Try your best. Don't give up. ";

                Synthesizer.Speak(speech);
            }
            else
            {
                Synthesizer.Speak("Well. Unfortunately, you guys did not " +
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
            FindAnyCorrectAnswer();

            return true;
        }

        private static void AnnouceGroupResults()
        {
            Synthesizer.Speak("Well done every body, the time is over. Now, " +
                "I am very excited to reveal your group results. ");

            // All group answer are wrong
            bool isWorst = CheckFirstTopGroup();

            if (isWorst) return;
            
            
            int rdmNum = new Random().Next(3);
            switch (rdmNum)
            {
                case 0: Synthesizer.Speak("Are you confident to become the top 1? "); break;
                case 1: Synthesizer.Speak("Are you ready? "); break;
                case 2: Synthesizer.Speak("Where do you think your ranking are? "); break;
            }

            Thread.Sleep(2000);

            Synthesizer.Speak("Here it is! ");
            
            int rank = 1;
            int i = 0;
            
            while(i < record.Count)
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
                    
                    speech += $" group {groupNum}. "
                        + $"You are the top 1! Within {subTime} seconds, you " +
                        $"are the fastest group, got the correct answer! ";

                    speech += "Everyone, please give a round of applause for" +
                        $"group {groupNum}.";

                    Synthesizer.Speak(speech);

                    AudioHelper.PlayApplauseSound();
                }
                else if (isCorrect)
                {
                    if (subTime == record[i - 1].GetFinalSubTime())
                    {
                        speech = $"Unbelievable! Group {groupNum} has " +
                            $"the same submisson time with group " +
                            $"{record[i - 1].GroupNumber}. " +
                            $"Therefore, Group {groupNum} is also the top " +
                            $"{rank} of this challenge. ";
                    }
                    else
                    {
                        rank++;
                        rdmNum = new Random().Next(3);
                        switch (rdmNum)
                        {
                            case 0:
                                speech = $"Group {groupNum}. Congratulation, " +
                         $"you are the top {rank} of this challenge. "; break;
                            case 1:
                                speech = $"Group {groupNum}. Well done, " +
                            $"you are the top {rank} of this challenge. ";
                                break;
                                
                            case 2:
                                speech = $"Group {groupNum}. Good job, " +
                            $"you are the top {rank} of this challenge. ";
                                break;
                        }
                        
                        
                    }

                    speech += "Everyone, please, give another round of applause " +
                        "to group " + groupNum;
                    Synthesizer.Speak(speech);
                }
                else
                {
                    break;
                }
                i++;
            }

            Wait(1000);

            Synthesizer.Speak("Great. Now I will try to check the remaining " +
                "groups. Is there any time, you got any correct submission, or not? ");
            Wait(1500);
            FindAnyCorrectAnswer();
        }
    }
}
