using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace HGPS_Robot
{
    class AudioHelper
    {

        public static void PlayApplauseSound()
        {
            int rdNum = new Random().Next(5);
        }

        public static void PlayCheeringSound()
        {
            int rdNum = new Random().Next(5);
            PlayAudio("cheering" + (rdNum + 1) + ".wav");
        }

        public static void PlaySadSound()
        {
            int rdNum = new Random().Next(4);
            PlayAudio("sad" + (rdNum + 1) + ".wav");
        }

        public static void PlayChampionSound()
        {
            int rdNum = new Random().Next(2);
            PlayAudio("champion" + (rdNum + 1) + ".wav");
        }

        public static void PlayAudio(string fileLocation)
        {
            string startupPath = Application.StartupPath + @"\media\";
            SoundPlayer player = new SoundPlayer(startupPath + fileLocation);

            player.Play();

            //CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
            //Debug.WriteLine("Current Volume:" + defaultPlaybackDevice.Volume);
            //defaultPlaybackDevice.Volume = 60;

            //MediaPlayer player = new MediaPlayer();
            //player.Open(new Uri(startupPath + fileLocation));

            //player.Volume = 100 / 100.0f;
            //player.Play();

        }
    }
}
