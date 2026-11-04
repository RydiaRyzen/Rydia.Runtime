using System;
using System.Collections.Generic;
using System.Text;
using Rydia.Engine.Audio;
using Rydia.Engine.Serialization;

namespace Rydia.Engine.Resources
{
    public class Sound : Resource
    {

        public ContentRef<AudioData> AudioData
        {
            get;
            set;
        }

        [DontSerialize]
        public SoundInstance SoundInstance
        {
            get;
            private set;
        }

        public void CreateBufferAndSource()
        {
            var instance = new SoundInstance();
            instance.CreateBufferAndSource(this);
            SoundInstance = instance;
        }

    }
}
