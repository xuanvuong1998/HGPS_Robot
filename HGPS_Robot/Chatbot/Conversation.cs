using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SpeechLibrary;

namespace HGPS_Robot
{
    class Conversation
    {
        private static string[] startNewConversationKeyword =
            { "Hi! I am Cody. How can I help you with?",
            "Yes, I am here, nice to talking with you",
            "Hi, my name is Cody. Nice to become your buddy!" };

        private static string[] confirmToContinueKeyword =
        {
            "anything else?",
            "What's else I can help you?",
            "Do you have another question?"
        };

        private static string[] endConversationKeyword =
        {
            "thank you",
            "no thanks",
            "bye. bye"
        };

        private static string[] dontUnderstandWords = {
            "sorry, this question is out of my knowledge",
            "I am still thinking about it",
            "sorry, I am not sure I can understand what you meant"
        };


        private static int minQueryLength = 7;
        private static int maxRecogTime = 12;
        private static int unableToReplyCount;
        public static void Init()
        {
            var subKey = GlobalData.SpeechKey;
            var region = GlobalData.SpeechRegion;
            Recognizer.Setup(subKey, region);
            Recognizer.SilenceTimeOut = 1500;

            var botId = GlobalData.BotId;
            var directLi = GlobalData.DirectLine;

            ChatBot.Setup(directLi, botId);
        }

        private static string PickOne(string[] list)
        {
            return list[new Random().Next(list.Length)];
        }


        public static void Stop()
        {
            GlobalFlowControl.ChatBot.ConversationEnable = false;
            Recognizer.StopAndDeleteRecognizedWords();
        }

        private static void Wait(int miliSec)
        {
            Thread.Sleep(miliSec);
        }


        private static bool IsEndConversationKeyword(string words)
        {
            words = words.ToLower();
            foreach (var item in endConversationKeyword)
            {
                if (words.Contains(item)) return true;
            }
            return false;
        }

        private static string Filter(string question)
        {
            if (!char.IsLetterOrDigit(question[question.Length - 1]))
            {
                question = question.Remove(question.Length - 1);
            }            

            return question;
        }

        public static async Task<string> ProcessResponse(string question)
        {
            question = Filter(question);
            string reply = "";
            if (MathIntepreter.IsContainMathOperation(question))
            {
                var res = MathIntepreter.ProcessOperation(question);
                if (res == "invalid")
                {
                    reply = await ChatBot.GetResponse(question);
                }
                else
                {
                    reply = "The result is " + res;
                }
            }
            else
            {
                reply = await ChatBot.GetResponse(question);
            }

            if (reply.ToLower().Contains("don't understand"))
            {
                reply = PickOne(dontUnderstandWords);
            }

            return reply;
        }

        public static void Start()
        {
            GlobalFlowControl.ChatBot.ResetBeforeNewConversation();            

            Task.Factory.StartNew(async () =>
            {                
                Synthesizer.Speak(PickOne(startNewConversationKeyword));
                do
                {
                    unableToReplyCount = 0;
                    var ques = await Recognizer.RecognizeQueryWithTimeOut(minQueryLength, maxRecogTime)
                                    .ConfigureAwait(false);

                    if (ques.Length < minQueryLength)
                    {
                        GlobalFlowControl.ChatBot.ConversationEnable = false;
                    }
                    else
                    if (ques.ToLower() == "no." || ques.ToLower() == "no" ||
                        IsEndConversationKeyword(ques))
                    {
                        Synthesizer.Speak("Ok, Bye");

                        GlobalFlowControl.ChatBot.ConversationEnable = false;
                    }
                    else
                    {
                        string reply = await ProcessResponse(ques);
                        Synthesizer.Speak(reply);
                        
                        if (reply.Contains("sorry,"))
                        {
                            unableToReplyCount++;
                        }
                                                
                        if (unableToReplyCount >= 2) break;
                        Wait(1000);
                        
                        Synthesizer.Speak(PickOne(confirmToContinueKeyword));
                    }

                } while (GlobalFlowControl.ChatBot.ConversationEnable);

            });

        }
    }
}
