using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeechLibrary;

namespace HGPS_Robot
{
    class Conversation
    {
        private static string[] activateKeywords = { "coddie", "cody", "coding" };
        private static string[] startNewConversation =
            { "Hi! I am Cody. How can I help you with?", 
            "Cody here, an teaching assistant robot", 
            "Hi, my name is Cody. Nice to be your buddy!" };


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

        public static void Start()
        {
            GlobalFlowControl.ChatBot.ResetBeforeNewConversation();
            Synthesizer.Speak(PickOne(startNewConversation));
            
            Task.Factory.StartNew(async () =>
            {
                while (GlobalFlowControl.ChatBot.ConversationEnable)
                {                    
                    var ques = await Recognizer.RecognizeQueryWithTimeOut(10, 10).ConfigureAwait(false);                    
            
                    var res = await ChatBot.GetResponse(ques);

                    Synthesizer.Speak(res);
                }
            });
                        
        }
    }
}
