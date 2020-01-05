using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot
{
    class AudioHelper
    {
        public static void PlayPraiseSound()
        {
            PlayAudio("applause.wav");
        }

        public static void PlayAudio(string fileLocation)
        {            
            string startupPath = Directory.GetCurrentDirectory() + @"\media\";
            SoundPlayer player = new SoundPlayer(startupPath + fileLocation);
            player.PlaySync();
        }
    }
}
