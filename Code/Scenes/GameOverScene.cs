using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;
using OpenGL_Game.Managers;

namespace OpenGL_Game.Scenes
{
    class GameOverScene : Scene
    {
        int scoreTotal;
        public GameOverScene(SceneManager sceneManager, int score) : base(sceneManager)
        {
            // Set the title of the window
            sceneManager.Title = "Game Over";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;
            scoreTotal = score;
            sceneManager.Mouse.ButtonDown += Mouse_ButtonDown;
        }

        public override void Update(FrameEventArgs e)
        {
        }

        public void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                Environment.Exit(0);
            }
        }

        private void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.IsPressed)
            {
                sceneManager.Mouse.ButtonDown -= Mouse_ButtonDown;
                sceneManager.SceneChange(SceneTypes.MAIN_MENU,0);
            }
        }

        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, sceneManager.Width, 0, sceneManager.Height, -1, 1);

            GUI.clearColour = Color.BurlyWood;

            //Display the Title
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, (height) / 10f);
            GUI.Label(new Rectangle(0, (int)(fontSize / 2f), (int)width, (int)(fontSize * 3f)), "Get Good", (int)fontSize, StringAlignment.Center);
            GUI.Label(new Rectangle(0, 200, (int)width, (int)(fontSize * 3f)), "Score: " + scoreTotal.ToString(), 30, StringAlignment.Center);
            GUI.Render();
        }

        public override void Close()
        {
        }
    }
}
