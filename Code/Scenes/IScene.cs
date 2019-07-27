using OpenTK;

namespace OpenGL_Game.Scenes
{
    enum SceneTypes
    {
        MAIN_MENU,
        GAME,
        GAME_OVER,
        GAME_WIN
    }
    interface IScene
    {
        void Render(FrameEventArgs e);
        void Update(FrameEventArgs e);
        void Close();
    }
}
