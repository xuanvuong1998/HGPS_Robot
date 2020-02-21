﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpeechLibrary;

namespace HGPS_Robot
{
    /// <summary>
    /// This class to process command from teacher panel (using signalR) 
    /// </summary>
    public class RobotCmd
    {
        #region properties

        public string Navigation { get; set; }
        public string Movement { get; set; }
        public string SpecialAction { get; set; }
        public string LessonStatus { get; set; }
        public string GroupChallenge { get; set; }
        public string Gesture { get; set; }

        public List<StudentHistoryDTO> AssessPerformance { get; set; }
        #endregion

        public void AskRandomStudent(List<StudentHistoryDTO> list)
        {
            var rdm = new Random();
            int rdmNum = rdm.Next(1, 11);
            if (GlobalFlowControl.Lesson.ChosenStudent != null
                || rdmNum <= 8)
            {
                //ask
                Debug.WriteLine("ASKING STUDENT");

                StudentHistoryDTO student;

                if (GlobalFlowControl.Lesson.ChosenStudent != null)
                {
                    student = list.FirstOrDefault(x =>
                            x.Student_id == GlobalFlowControl.Lesson.ChosenStudent);

                    if (GlobalFlowControl.Lesson.IsStudentChosenBefore(student.Student_id) == false)
                    {
                        GlobalFlowControl.Lesson.ChosenStudentList.Add(student.Student_id);
                    }
                }
                else
                {
                    int randNum;

                    // All students have their all turns to be asked (approached) by robot
                    if (GlobalFlowControl.Lesson.ChosenStudentList.Count
                         == list.Count)
                    {
                        GlobalFlowControl.Lesson.ChosenStudentList.Clear();
                    }
                    do
                    {
                        randNum = rdm.Next(0, list.Count);
                        Debug.WriteLine("Random Student: " + randNum);
                    } while (GlobalFlowControl.Lesson.IsStudentChosenBefore(list[randNum].Student_id));


                    student = list[randNum];

                    GlobalFlowControl.Lesson.ChosenStudentList.Add(student.Student_id);
                }

                string speech = student.UserAccountFullName + "! ";
                int num = rdm.Next(0, 5);
                switch (num)
                {
                    case 0:
                        speech += "Can you share what is your answer?";
                        break;
                    case 1:
                        speech += "Did you have your own answer? Is this question easy or difficult?";
                        break;
                    case 2:
                        speech += "What did you choose as your answer for this question?";
                        break;
                    case 3:
                        speech += "What do you think about this question?";
                        break;
                    case 4:
                        speech += "Can you share your answer to the class and explain why did you choose it?";
                        break;
                }
                //Get position of student
                var position = TablePositionHelper.FindTablePosByStdId(student.Student_id);

                if (position != null)
                {
                    GlobalFlowControl.Lesson.ApproachStudent = position;

                    //Go to student
                    LessonHelper.InsertCommand("gountil", position);
                    LessonHelper.InsertCommand("speak", speech);
                    LessonHelper.InsertCommand("asking", "1");

                    bool studentResult = student.ResultInBinaryString.Last() == '1';
                    string resultSpeech = "";

                    if (studentResult)
                    {
                        switch (num)
                        {
                            case 0:
                            case 1:
                                resultSpeech = "Excellent ";
                                break;
                            case 2:
                            case 3:
                                resultSpeech = "Not bad ";
                                break;
                            case 4:
                                resultSpeech = "Very good ";
                                break;

                        }
                        resultSpeech += student.Student_id + "! Your answer is correct!";
                    }
                    else
                    {
                        resultSpeech = "Sorry " + student.Student_id + ". Your answer is not correct!";
                    }

                    LessonHelper.InsertCommand("speak", resultSpeech);

                }
            }


            AnalyzeStudentPerformance(AssessPerformance);

        }

        #region Student Performance
        private void AnalyzeStudentPerformance(List<StudentHistoryDTO> list)
        {
            var rdm = new Random();
            var rdmNum = rdm.Next(1, 4); // generate random number 1-3

            int incorrectCnt = StudentsPerformanceHelper.GetNumberOfInCorrectStudent(list);
            int correctCnt = StudentsPerformanceHelper.GetNumberOfCorrectStudent(list);

            var speech = "";
            if (correctCnt == 0)
            {
                var x = rdm.Next(0, 3);
                switch (x)
                {
                    case 0:
                        speech = "I am very sad now, because no one got any correct answer for this question. ";
                        break;
                    case 1:
                        speech = "No one is correct. Is it because this question too difficult? ";
                        break;
                    case 2:
                        speech = "Sorry to say this, but none of you had a correct answer. ";
                        break;
                }
                speech += "Come on guys, don't give up. ";

                LessonHelper.InsertPraise(speech);


                return;
            }
            else
            {
                var x = rdm.Next(0, 3);
                if (incorrectCnt == 0)
                {
                    switch (x)
                    {
                        case 0:
                            speech = "Brilliant! No one did any mistake for this question. ";
                            break;
                        case 1:
                            speech = "Amazing! Everybody answers are correct. ";
                            break;
                        case 2:
                            speech = "Incredible! All of you got the correct answer. ";
                            break;
                    }

                    speech += "Everyone, please clap your hands";
                    LessonHelper.InsertPraise(speech);
                    return;
                }
                else if (correctCnt >= incorrectCnt)
                {
                    switch (x)
                    {
                        case 0:
                            speech = "Well, I am every happy to see many of you got correct answer. ";
                            break;
                        case 1:
                            speech = "Great! Most of you did a good job! ";
                            break;
                        case 2:
                            speech = "Good job everybody, most of you are correct. ";
                            break;
                    }
                }
            }

            var numOfFullScore = StudentsPerformanceHelper.GetNumOfFullScore(list);
            if (numOfFullScore != 0)
            {
                speech += $"Awesome, we have {numOfFullScore.ToString()} students with full score! ";
            }

            switch (rdmNum)
            {
                case 1:
                    speech += GetTopStudentsPraise(list);
                    break;

                case 2:

                    speech += GetTopStudentsPraise(list);

                    break;

                case 3:
                    speech += GetTopStudentsPraise(list);
                    break;
            }

            LessonHelper.InsertPraise(speech);
        }

        private string GetTopStudentsPraise(List<StudentHistoryDTO> list)
        {
            if (list != null)
            {
                var speech = new StringBuilder();

                var topStudents = StudentsPerformanceHelper.GetTopStudents(list);
                var x = new Random().Next(0, 3);

                if (topStudents.Count == 1)
                {
                    switch (x)
                    {
                        case 0:
                            speech.Append($"Currently the top students for this class is {topStudents.FirstOrDefault().Key}" +
                             $" with a score of {topStudents.FirstOrDefault().Value.ToString()}. ");
                            break;
                        case 1:
                            speech.Append($"{topStudents.FirstOrDefault().Key} is having the" +
                                $" highest score in the class, congratulation! ");
                            break;
                        case 2:
                            speech.Append($"With the score of {topStudents.FirstOrDefault().Value.ToString()}. " +
                                $"I wanna say that, now, {topStudents.FirstOrDefault().Key} is the top student" +
                                $", congratulation {topStudents.FirstOrDefault().Key}. ");
                            break;
                    }

                    speech.Append("The rest of you please try your best!");
                }
                else if (topStudents.Count <= 5)
                {
                    foreach (var stud in topStudents)
                    {
                        speech.Append(stud.Key);
                        speech.Append(", ");
                    }
                    speech.Append(" are currently the top students in this class, ");
                    speech.Append($"with a score of {topStudents.FirstOrDefault().Value.ToString()}. ");
                    speech.Append("Great job everyone!");
                }
                else
                {
                    speech.Append($"Many of you have done well with a score of {topStudents.FirstOrDefault().Value.ToString()}. ");
                    speech.Append("Keep up the good work! ");
                }
                return speech.ToString();
            }
            return null;
        }



        #endregion

        #region Emotion Survey
        private void AnalyzeEmotion(double happyPc, double sadPc, double neutralPc)
        {
            if (sadPc >= 0.3) // roughly 1/3 unhappy, give teacher time to explain
                              // for unhappy students again or engage some activities
            {
                //LessonHelper.SendPausedStatusToServer("paused");
                LessonHelper.PauseLesson();
                LessonHelper.ResumeSpeak();
                Synthesizer.Speak("Well, since some of you are not sure of this topic, let Mr Nizam explain again. ");
            } // Ok, happy or neutral
            else
            {
                Debug.WriteLine("Before Pause Lesson");
                LessonHelper.PauseLesson();
                Debug.WriteLine("After pause lesson");
                LessonHelper.ResumeSpeak();

                Synthesizer.Speak("Wow, most of you understand the topic! Let us continue with the lesson. ");

                LessonHelper.ResumeLesson();

                Debug.WriteLine("After resume");
            }
        }
        #endregion

        #region GroupChallenge

        public GroupChallengeRecord UpdateGroupSubmission()
        {
            var groupNumber = int.Parse(GroupChallenge.Split('-')[0]);
            var result = GroupChallenge.Split('-')[1];
            var submittedTime = int.Parse(GroupChallenge.Split('-')[2]);
            var records = GlobalFlowControl.Lesson.GroupRecords;

            var groupRecord = records.SingleOrDefault
                (x => x.ChallengeNumber == LessonHelper.ChallengeNumber
                   && x.GroupNumber == groupNumber);

            if (groupRecord == null)
            {
                groupRecord = new GroupChallengeRecord
                {
                    ChallengeNumber = LessonHelper.ChallengeNumber,
                    GroupNumber = groupNumber,
                };
                groupRecord.Submission.Add(submittedTime + "-" + result);
                records.Add(groupRecord);
            }
            else
            {
                groupRecord.Submission.Add(submittedTime + "-" + result);
            }

            Debug.WriteLine("Submission Updated!");
            foreach (var item in records)
            {
                Debug.WriteLine(item.ToStringFormat());
            }

            return groupRecord;
        }

        /// <summary>
        /// Receive new submission
        /// </summary>
        public void ProcessGroupChallenge()
        {
            var groupRecord = UpdateGroupSubmission();
            var subCnt = groupRecord.GetSubmissionCount();
            var groupNum = groupRecord.GroupNumber;

            var members = TablePositionHelper.GetMembersByGroupNumber(groupNum);

            int leftChances = members.Count - subCnt;

            var leftSeconds = LessonHelper.CurrentQuizTimeout -
                GlobalFlowControl.Lesson.QuizElapsedTime;

            if (leftSeconds <= 10) return;
            
            Synthesizer.Speak($"Group {groupNum} submitted. ");

            if (leftSeconds <= 20) return;
            
            int rdmNum = new Random().Next(3);

            if (leftChances == 0)
            {
                switch (rdmNum)
                {
                    case 0:
                        Synthesizer.Speak("No more chance for group " + groupNum); break;
                    case 1:
                        Synthesizer.Speak("This is the final submission of group " +
                            groupNum + " as well.  "); break;
                    case 2:
                        Synthesizer.Speak("Group " + groupNum + " is done, please " +
                            "wait for the result. "); break;
                }
               
            }
            else 
            {
                switch (rdmNum)
                {
                    case 0:
                        Synthesizer.Speak("Group " + groupNum + " has " +
                leftChances + " more chances. "); break;
                    case 1:
                        Synthesizer.Speak("Group " + groupNum + " can " +
                            "submit " + leftChances + " more times. "); break;
                    case 2:
                        Synthesizer.Speak(leftChances + " more tries for " +
                            "group " + groupNum + ". "); break;
                }

                if (leftChances == 1) // last try left
                {
                    switch (rdmNum)
                    {
                        case 0:
                            Synthesizer.Speak("Remember to check carefully before " +
                                "submitting your last submisson"); break;
                        case 1:
                            Synthesizer.Speak("Be careful with the final answer"); break;
                        case 2:
                            Synthesizer.Speak("Please think carefully for the " +
                                "last chance. "); break;
                    }
                }

                var subResult = GroupChallenge.Split('-')[1];

                
                if (subResult == "0") 
                    // Suggest hint for group just Submit incorrect answer
                    // (But at most 2 hints)
                {
                    var gnum = int.Parse(GroupChallenge.Split('-')[0]);
                    GroupChallengeHelper.SuggestHint(groupNum);
                }
            }
            
        }

        #endregion
        // From Teacher Panel
        public void ProcessCommand()
        {
            if (Navigation != null)
            {
                GlobalFlowControl.Lesson.ChosenStudent = Navigation;
            }

            if (Movement != null)
            {
                BaseHelper.DoBaseMovements(Movement);
            }

            if (Gesture != null)
            {
                UpperBodyHelper.DoGestures(Gesture);
            }

            if (GroupChallenge != null)
            {
                ProcessGroupChallenge();
            }

            if (LessonStatus != null)
            {
                Debug.WriteLine(LessonStatus + " Lesson Status");
                if (LessonStatus == "StopQuestion")
                {
                    var secondsLeft = LessonHelper.CurrentQuizTimeout
                        - GlobalFlowControl.Lesson.QuizElapsedTime;
                    
                    if (secondsLeft > 11)
                    {
                        Synthesizer.SpeakAsync("Wow. Since all groups have done " +
                            "already. I will terminate the quiz early, and " +
                            "reveal the result now. Are you ready???");
                        GlobalFlowControl.Lesson.StartingQuiz = false;
                    }
                    
                }
            }

            if (SpecialAction != null)
            {
                Debug.WriteLine("Special Action : " + SpecialAction);

                string thing = SpecialAction.Split('-')[0];
                string info = SpecialAction.Split('-')[1];

                if (thing == "Emotion")
                {
                    GlobalFlowControl.Lesson.StudentFeedbackReceived = true;
                    double sadPc = double.Parse(info.Split(';')[0]);
                    double neutralPc = double.Parse(info.Split(';')[1]);
                    double happyPc = double.Parse(info.Split(';')[2]);

                    AnalyzeEmotion(happyPc, sadPc, neutralPc);

                }

            }


            if (AssessPerformance != null)
            {
                AskRandomStudent(AssessPerformance);

            }
        }
    }
}
