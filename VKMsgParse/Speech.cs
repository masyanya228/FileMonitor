using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech;
using System.Speech.Synthesis;

namespace VKMsgParse
{
    public class Speech
    {
        SpeechSynthesizer speech = new SpeechSynthesizer();
        public void Speak(string txt)
        {
            speech.Volume = 100;
            speech.Rate = 2;
            speech.SpeakAsync(txt);
        }
    }
}