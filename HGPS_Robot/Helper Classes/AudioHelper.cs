using System;
using System.Media;
using System.Windows.Forms;

namespace HGPS_Robot
{
    class AudioHelper
    {

        public static void PlayApplauseSound()
        {
            int rdNum = RandomHelper.RandomInt(5);
            PlayAudio("cheering" + (rdNum + 1) + ".wav");
        }

        public static void PlaySadSound()
        {
            int rdNum = RandomHelper.RandomInt(4);
            PlayAudio("sad" + (rdNum + 1) + ".wav");
        }

        public static void PlayChampionSound()
        {
            int rdNum = RandomHelper.RandomInt(2);
            PlayAudio("champion" + (rdNum + 1) + ".wav");
        }

        public static void PlayAudio(string fileLocation)
        {
            string startupPath = Application.StartupPath + @"\media\";
            SoundPlayer player = new SoundPlayer(startupPath + fileLocation);

            player.PlaySync();

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
