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
            "Ok, anything else?",
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
            "sorry, could you ask another simpler question",
            "sorry, I am not sure I can understand what you meant"
        };


        private static int minQueryLength = 7;
        private static int maxRecogTime = 10;

        public static void Init()
        {
            var subKey = GlobalData.SpeechKey;
            var region = GlobalData.SpeechRegion;
            Recognizer.Setup(subKey, region);

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
            foreach (var item in endConversationKeyword)
            {
                if (words.Contains(item)) return true;
            }
            return false;
        }

        public static async Task ProcessResponse(string question)
        {
            
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

            Synthesizer.Speak(reply);
        }

        public static void Start()
        {
            GlobalFlowControl.ChatBot.ResetBeforeNewConversation();
            Synthesizer.Speak(PickOne(startNewConversationKeyword));

            Task.Factory.StartNew(async () =>
            {
                do
                {
                    var ques = await Recognizer.RecognizeQueryWithTimeOut(minQueryLength, maxRecogTime)
                                    .ConfigureAwait(false);

                    if (ques.Length < minQueryLength)
                    {
                        GlobalFlowControl.ChatBot.ConversationEnable = false;
                    }
                    else
                    if (IsEndConversationKeyword(ques))
                    {
                        Synthesizer.Speak("Ok, Bye");

                        GlobalFlowControl.ChatBot.ConversationEnable = false;
                    }
                    else
                    {
                        await ProcessResponse(ques);
                        Wait(1000);

                        Synthesizer.Speak(PickOne(confirmToContinueKeyword));
                    }
                                        
                } while (GlobalFlowControl.ChatBot.ConversationEnable);
                
            });

        }
    }
}
