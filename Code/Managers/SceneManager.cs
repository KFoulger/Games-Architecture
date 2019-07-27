using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using OpenGL_Game.Scenes;

namespace OpenGL_Game.Managers
{
    class SceneManager : GameWindow
    {
        Scene scene;
        public static int width = 800, height = 450;

        public delegate void SceneDelegate(FrameEventArgs e);
        public SceneDelegate renderer;
        public SceneDelegate updater;

        AudioContext audioContext;

         public SceneManager() : base(width, height, new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(8, 8, 8, 8), 16))
         {
             
         }
         

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.Enable(EnableCap.DepthTest);

            //Load the GUI
            GUI.SetUpGUI(width, height);

            audioContext = new AudioContext();

            StartMenu();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            updater(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            renderer(e);

            GL.Flush();
            SwapBuffers();
        }

        /*private void Mouse_ButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButton.Left:
                    scene.Close();
                    StartNewGame();
                    break;
                case MouseButton.Right:
                    scene.Close();
                    StartMenu();
                    break;
            }
        }
        */
        public void SceneChange(SceneTypes SceneType, int score)
        {
            scene.Close();
            switch (SceneType)
            {
                case SceneTypes.MAIN_MENU:
                    StartMenu();
                    break;
                case SceneTypes.GAME:
                    StartNewGame();
                    break;
                case SceneTypes.GAME_OVER:
                    StartGameOver(score);
                    break;
                case SceneTypes.GAME_WIN:
                    StartGameWin(score);
                    break;
            }
        }
        public void StartNewGame()
        {
            scene = new GameScene(this);
        }

        public void StartMenu()
        {
            scene = new MainMenuScene(this);
        }
        public void StartGameOver(int score)
        {
            scene = new GameOverScene(this, score);
        }

        public void StartGameWin(int score)
        {
            scene = new GameWinScene(this, score);
        }

        public static int WindowWidth
        {
            get { return width; }
        }

        public static int WindowHeight
        {
            get { return height; }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            SceneManager.width = Width;
            SceneManager.height = Height;

            //Load the GUI
            GUI.SetUpGUI(Width, Height);
        }
    }

}

