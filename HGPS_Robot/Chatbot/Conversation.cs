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
            "Yes, I am here, please ask me a question",
            "Hi, Cody here, what is your question?" };

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
            "bye. bye",
            "no more question",
            "don't have question",
            "do not have question"

        };

        private static string[] dontUnderstandWords = {
            "Sorry, can you speak louder and clearly?",
            "Can you ask again? Please speak louder",
            "Can you try to ask this question 1 more time. Please speak louder"
        };


        private static int minQueryLength = 5;
        private static int maxRecogTime = 12;
        private static int unableToReplyCount;
        public static void Init()
        {
            var subKey = GlobalData.SpeechKey;
            var region = GlobalData.SpeechRegion;
            Recognizer.Setup(subKey, region);
            Recognizer.SilenceTimeOut = 2000;

            var botId = GlobalData.BotId;
            var directLi = GlobalData.DirectLine;

            ChatBot.Setup(directLi, botId);
        }

        private static string PickOne(string[] list)
        {
            var speech = list[new Random().Next(list.Length)];
            return speech;
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

            while (!char.IsLetterOrDigit(question[question.Length - 1]))
            {
                question = question.Remove(question.Length - 1);
            }

            return question;
        }

        public static string ProcessResponse(string question)
        {
            question = Filter(question);
            string reply = "";
            if (MathIntepreter.IsContainMathOperation(question))
            {
                var res = MathIntepreter.ProcessOperation(question);
                if (res == "invalid")
                {
                    reply = "don't understand";
                }
                else
                {
                    reply = "The result is " + res;
                }
            }
            else
            {
                reply = "don't understand";

            }


            return reply;
        }

        public static void Start()
        {
            GlobalFlowControl.ChatBot.ResetBeforeNewConversation();

            Synthesizer.SelectVoiceByName(GlobalData.Voice1);

            int tryCnt = 0;

            Task.Factory.StartNew(async () =>
            {
                RobotActionHelper.MoveDuringChatbot();
                Synthesizer.Speak(PickOne(startNewConversationKeyword));
                do
                {
                    unableToReplyCount = 0;
                    var ques = await Recognizer.RecognizeQueryWithTimeOut(minQueryLength, maxRecogTime)
                                    .ConfigureAwait(false);

                    string reply = ProcessResponse(ques);

                    if (reply.Contains("don't understand"))
                    {
                        tryCnt++;
                        if (tryCnt == 2) break;
                        reply = PickOne(dontUnderstandWords);
                        Synthesizer.Speak(reply);

                    }
                    else
                    {
                        Synthesizer.Speak(reply);
                        break;
                    }
                } while (true);

                GlobalFlowControl.ChatBot.ConversationEnable = false;

            });

        }
    }
}
