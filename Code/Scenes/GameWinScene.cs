using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;
using OpenGL_Game.Managers;

namespace OpenGL_Game.Scenes
{
    class GameWinScene : Scene
    {
        int scoreTotal;
        public GameWinScene(SceneManager sceneManager, int score) : base(sceneManager)
        {
            // Set the title of the window
            sceneManager.Title = "Good job";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;
            scoreTotal = score;
            sceneManager.Mouse.ButtonDown += Mouse_ButtonDown;
        }

        public override void Update(FrameEventArgs e)
        {
        }

        private void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.IsPressed)
            {
                sceneManager.Mouse.ButtonDown -= Mouse_ButtonDown;
                sceneManager.SceneChange(SceneTypes.MAIN_MENU, 0);
            }
        }

        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, sceneManager.Width, 0, sceneManager.Height, -1, 1);

            GUI.clearColour = Color.DarkGoldenrod;

            //Display the Title
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, (height) / 10f);
            GUI.Label(new Rectangle(0, (int)(fontSize / 2f), (int)width, (int)(fontSize * 3f)), "Good job", (int)fontSize, StringAlignment.Center);
            GUI.Label(new Rectangle(0, 200, (int)width, (int)(fontSize * 3f)), "Score: " + scoreTotal.ToString(), (int)fontSize, StringAlignment.Center);

            GUI.Render();
        }

        public override void Close()
        {
        }
    }
}