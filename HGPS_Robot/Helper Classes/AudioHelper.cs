﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HGPS_Robot
{
    class AudioHelper
    {
        public static void PlayApplauseSound()
        {
            PlayAudio("applause.wav");
        }

        public static void PlayInCorrectSound()
        {
            PlayAudio("incorrect.mp3");
        }

        public static void PlayAudio(string fileLocation)
        {            
            string startupPath = Application.StartupPath + @"\media\";
            SoundPlayer player = new SoundPlayer(startupPath + fileLocation);
            player.PlaySync();
        }
    }
}
