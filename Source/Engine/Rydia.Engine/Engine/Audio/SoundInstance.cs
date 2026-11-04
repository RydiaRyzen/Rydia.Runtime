using System;
using System.Collections.Generic;
using System.Text;
using Rydia.Audio.Api.OpenAL;
using Rydia.Engine.Resources;
using Rydia.Graphics;
using Rydia.Serialization;

namespace Rydia.Engine.Audio
{

    public class SoundInstance : DisposableBase
    {

        public SoundInstance()
        {
            AudioBuffer = AL.GenBuffer();
            AudioSource = AL.GenSource();
        }

        public int AudioBuffer
        {
            get;
            protected set;
        }

        public int AudioSource
        {
            get;
            protected set;
        }

        protected IAL AL
        {
            get
            {
                return AudioHost.AL;
            }
        }

        public void Play()
        {
            AL.SourcePlay(AudioSource);
        }

        public void Pause()
        {
            AL.SourcePause(AudioSource);
        }

        public void Rewind()
        {
            AL.SourceRewind(AudioSource);
        }

        public void Stop()
        {
            AL.SourceStop(AudioSource);
        }

        public void CreateBufferAndSource(Sound sound)
        {
            var alBuffer = new NativeBuffer<short>(sound.AudioData.Res.PCMData);
            AL.BufferData(AudioBuffer, sound.AudioData.Res.Channels == 1 ? ALFormat.Mono16 : ALFormat.Stereo16,
            alBuffer,
            sound.AudioData.Res.PCMData.Length * sizeof(short),
            sound.AudioData.Res.SampleRate);
            AL.Source(AudioSource, ALSourcei.Buffer, AudioBuffer);
        }

        protected override void Disposing(bool disposing)
        {
            AL.SourceStop(AudioSource);
            AL.DeleteSource(AudioSource);
            AL.DeleteBuffer(AudioBuffer);
        }
    }

}
