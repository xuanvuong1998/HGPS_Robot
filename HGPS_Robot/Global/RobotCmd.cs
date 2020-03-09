using System;
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

        private Random rand = new Random();
        #endregion

        #region Student Asking
        public void AfterQuizFinish(List<StudentHistoryDTO> list)
        {
            //var rand = new Random();
            int rdmNum = rand.Next(1, 11);
            if (LessonHelper.LessonSubject.ToLower() != "story"
                && GlobalFlowControl.Lesson.ChosenStudent != null)
                //|| rdmNum <= 5, robot will approach when teacher choose it
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
                        randNum = rand.Next(0, list.Count);
                        Debug.WriteLine("Random Student: " + randNum);
                    } while (GlobalFlowControl.Lesson.IsStudentChosenBefore(list[randNum].Student_id));


                    student = list[randNum];

                    GlobalFlowControl.Lesson.ChosenStudentList.Add(student.Student_id);
                }

                string speech = student.UserAccountFullName + "! ";
                int num = rand.Next(0, 5);
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
                    LessonHelper.InsertCommand("asking", "1"); // pause a lesson and ask a student

                    bool studentResult = student.ResultInBinaryString.Last() == '1';
                    string resultSpeech = "";

                    if (studentResult)
                    {
                        switch (num)
                        {
                            case 0:
                                resultSpeech = "I can't agree more ";
                                break;
                            case 1:
                                resultSpeech = "Excellent ";
                                break;
                            case 2:
                                resultSpeech = "Marvelous ";
                                break;
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
                        switch (num)
                        {
                            case 0: 
                                resultSpeech = "Sorry " + student.Student_id + ". Your answer is not correct!";
                                break;
                            case 1:
                                resultSpeech = "It is wrong " + student.Student_id;
                                break;
                            case 2:
                            case 3:
                            case 4:
                                resultSpeech = "It is not correct " + student.Student_id;
                                break;
                           

                        }
                        
                    }

                    LessonHelper.InsertCommand("speak", resultSpeech);

                }
            }

            if (LessonHelper.LessonSubject.ToLower() == "story")
            {
                
            }
            else
            {
                AnalyzeStudentPerformance(AssessPerformance);

                // Make sure all praise speech added to the next slide before
                // marking the received signal to RobotCommands

                StudentsPerformanceHelper.ResultReceived = true;

                
                if (GlobalFlowControl.Lesson.GroupCompetitionIsHappening)
                {
                    GroupCompetitionHelper.ResultReceived = false;
                    
                    // Invoke method from myhub to retrieve left and right percentage
                    SyncHelper.InvokeRankingsResult("group-competition");
                }
            }
            
        }
        #endregion

        #region Student Performance
        private void AnalyzeStudentPerformance(List<StudentHistoryDTO> list)
        {
            StudentsPerformanceHelper.studentPerformanceOverall = list;


            int incorrectCnt = StudentsPerformanceHelper.GetNumberOfInCorrectStudent(list);
            int correctCnt = StudentsPerformanceHelper.GetNumberOfCorrectStudent(list);

            var speech = "";
            var x = rand.Next(0, 5);
            if (correctCnt == 0)
            {
                switch (x)
                {
                    case 0:
                        speech = "I am very sad now, because no one got any correct answer for this question. ";
                        break;
                    case 1:
                        speech = "No one is correct. Is it because this question too difficult? ";
                        break;
                    case 2:
                    case 3:
                    case 4:
                        speech = "Sorry to say this, but none of you had a correct answer. ";
                        break;
                    
                }
                speech += "Come on guys, don't give up. ";

                LessonHelper.InsertPraise(speech);

                return;
            }
            else
            {
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
                        case 3:
                        case 4:
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
                        case 3:
                            speech = "Oh, very good, a lot of students made it correct. ";
                            break;
                        case 4:
                            speech = "Wow, I received a lot of correct answers for this question.";
                            break;
                    }
                }
            }

            var numOfFullScore = StudentsPerformanceHelper.GetNumOfFullScore(list);
            if (numOfFullScore != 0)
            {
                switch (x)
                {
                    case 0:
                        speech += "Incredible! We have " + numOfFullScore + " students with full" +
                            "score. ";
                        break;
                    case 1:
                        speech += "Excellent! " + numOfFullScore + " students in the class " +
                            "have not made any mistake so far. ";
                        break;
                    case 2:
                    case 3:
                    case 4:
                        speech += "Amazing! " + numOfFullScore + " students have the maximum " +
                            "scores so far. ";
                        break;
                }
            }

            speech += GetTopStudentsPraise(list);

            LessonHelper.InsertPraise(speech);
        }

        private string GetTopStudentsPraise(List<StudentHistoryDTO> list)
        {
            if (list != null)
            {
                var speech = new StringBuilder();

                var topStudents = StudentsPerformanceHelper.GetTopStudents(list);
                var x = rand.Next(0, 3);

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
                LessonHelper.PauseLesson();
                LessonHelper.ResumeSpeak();
                Synthesizer.Speak("Well, since some of you are not sure of this topic, let Mr Neezam explain again. ");
            } // Ok, happy or neutral
            else
            {
                LessonHelper.PauseLesson();
                LessonHelper.ResumeSpeak();

                Synthesizer.Speak("Wow, most of you understand the topic! Let us continue with the lesson. ");

                LessonHelper.ResumeLesson();
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

            int leftChances = groupRecord.GetLeftChancesNumber();

            var leftSeconds = LessonHelper.CurrentQuizTimeout -
                GlobalFlowControl.Lesson.QuizElapsedTime;

            if (leftSeconds <= 10) return;

            if (leftChances == 0)
            {
                Synthesizer.SpeakAsync("Group " + groupNum + " is done");
            }
            else
            {
                Synthesizer.SpeakAsync($"Group {groupNum} submitted. ");
            }

            var subResult = GroupChallenge.Split('-')[1];

            if (subResult == "0" && leftSeconds > 20)
            // Suggest hint for group just Submit incorrect answer
            // (But at most 2 hints)
            {
                var gnum = int.Parse(GroupChallenge.Split('-')[0]);
                GroupChallengeHelper.SuggestHint(groupNum);
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
                            
                    //Thread.Sleep(2000);
                    if (secondsLeft > 13) // If just have only few seconds
                        // let the robot count down from 10 to 1
                    {
                        //if (GlobalFlowControl.GroupChallenge.IsHappening == true)
                        //{
                        //    Synthesizer.Speak("Wow. Since all of you have done " +
                        //                            "already. I will terminate the quiz early, and " +
                        //                            "reveal the result now. Are you ready???");
                        //}
                        
                        GlobalFlowControl.Lesson.StartingQuiz = false;
                    }
                }
                else if (LessonStatus == "Next")
                {
                    if (LessonHelper.CurrentDisplaySlideNumber < LessonHelper.LastSlideNumber)
                    {
                        LessonHelper.CurrentDisplaySlideNumber++;
                        LessonStatusHelper.Update(LessonHelper.LessonName, LessonHelper.CurrentDisplaySlideNumber, "started", null, null, null);
                    }
                }
                else if (LessonStatus == "Previous")
                {
                    if (LessonHelper.CurrentDisplaySlideNumber > 1)
                    {
                        LessonHelper.CurrentDisplaySlideNumber--;
                        LessonStatusHelper.Update(LessonHelper.LessonName, LessonHelper.CurrentDisplaySlideNumber, "started", null, null, null);
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
                else
                {
                    string who = thing;
                    string message = info;

                    Synthesizer.Resume();
                    if (who == "class")
                    {
                        Synthesizer.SpeakAsync(message);
                    }
                    else
                    {
                        Synthesizer.SpeakAsync(who + ". " + message);
                    }
                }
            }

            if (AssessPerformance != null)
            {
                AfterQuizFinish(AssessPerformance);
            }
        }
    }
}
