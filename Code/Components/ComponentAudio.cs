using OpenTK;
using OpenTK.Audio.OpenAL;
using OpenGL_Game.Managers;

namespace OpenGL_Game.Components
{
    class ComponentAudio : IComponent
    {
        int mySource;

        public ComponentAudio(string audioName, bool loop)
        {
            int myBuffer = ResourceManager.LoadAudio(audioName);

            // Create a sounds source using the audio clip
            mySource = AL.GenSource(); // gen a Source Handle
            AL.Source(mySource, ALSourcei.Buffer, myBuffer); // attach the buffer to a source
            AL.Source(mySource, ALSourceb.Looping, loop); // source loops infinitely
        }

        public void SetPosition(Vector3 emitterPosition)
        {
            AL.Source(mySource, ALSource3f.Position, ref emitterPosition);
        }

        public Vector3 SetVelocity(Vector3 emitterPosition, float change)
        {
            emitterPosition = new Vector3(emitterPosition.X - change, emitterPosition.Y, emitterPosition.Z);
            AL.Source(mySource, ALSource3f.Position, ref emitterPosition);
            return emitterPosition;
        }

        public void Start()
        {
            AL.SourcePlay(mySource);
        }

        public void Stop()
        {
            AL.SourceStop(mySource);
        }

        public int Audio
        {
            get { return mySource; }
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_AUDIO; }
        }

        public void Close()
        {
            Stop();
            AL.DeleteSource(mySource);
        }
    }
}