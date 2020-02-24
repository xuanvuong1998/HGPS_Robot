using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HGPS_Robot.Utils
{
    class SynthesizerFilter
    {
       
        /// <summary>
        /// Change some word for the Synthesizer pronounces properly
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public static string ChangeWords(string sentence)
        {
            sentence =  sentence.Replace("Coddie", "Cody");
            sentence = sentence.Replace("Nizam", "Neezam");
            return sentence;
        }
    }
}
