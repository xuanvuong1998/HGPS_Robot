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

        }
    }
}
