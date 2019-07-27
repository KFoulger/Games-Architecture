using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;
using OpenGL_Game.Managers;

namespace OpenGL_Game.Scenes
{
    class MainMenuScene : Scene
    {
        public MainMenuScene(SceneManager sceneManager) : base(sceneManager)
        {
            // Set the title of the window
            sceneManager.Title = "Main Menu";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;

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
                sceneManager.SceneChange(SceneTypes.GAME, 0);
            }
        }

        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, sceneManager.Width, 0, sceneManager.Height, -1, 1);

            GUI.clearColour = Color.IndianRed;

            //Display the Title
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 10f;
            GUI.Label(new Rectangle(0, 70, (int)width, (int)(fontSize * 3.5f)), "Pac-Mans Poundland Cousin", (int)fontSize, StringAlignment.Center);
            GUI.Label(new Rectangle(0, 400, (int)width, (int)(fontSize * 3.5f)), "Click to start", 20, StringAlignment.Center);

            GUI.Render();
        }

        public override void Close()
        {
        }
    }
}