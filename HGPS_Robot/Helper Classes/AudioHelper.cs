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
            string startupPath = Directory.GetCurrentDirectory();
            SoundPlayer player = new SoundPlayer(startupPath + @"\media\applause.wav");
            player.Play();
        }

        public static void PlayAudio(string fileLocation)
        {
            SoundPlayer player = new SoundPlayer(fileLocation);            
        }
    }
}
