using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenGL_Game.Components;
using OpenGL_Game.Systems;
using OpenGL_Game.Managers;
using OpenGL_Game.Objects;
using System.Collections.Generic;
using System;
using System.IO;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

namespace OpenGL_Game.Scenes
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class GameScene : Scene
    {
        bool upPressed, downPressed, wallCollide, ghostKill, collisionOn = true, ghostsOn = true ,left, right, up, down, aDead, bDead, cDead;
        int pelletsCollected = 0, lifeTotal = 3, pelletIndex = 0;
        public int score = 0;
        float timer = 7.0f, a = 1.5f, ghostASpeed = 2.5f, ghostBSpeed = 2.0f, ghostCSpeed = 1.8f, ghostTimer = 0, aTimer = 0, bTimer = 0, cTimer = 0;
        Vector3 listenerPosition, listenerDirection, listenerUp, eyePos, targetPos, spawnPoint, ghostASpawn, ghostBSpawn, ghostCSpawn;
        public Matrix4 view, projection;
        public static float dt = 0;
        EntityManager entityManager;
        SystemManager systemManager;
        ComponentAudio ding1, ding2;
        public static GameScene gameInstance;

        public GameScene(SceneManager sceneManager) : base(sceneManager)
        {
            gameInstance = this;
            entityManager = new EntityManager();
            systemManager = new SystemManager();

            // Set the title of the window
            sceneManager.Title = "Pac-Man's Poundland Cousin";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;
            // Set Keyboard events to go to a method in this class
            sceneManager.Keyboard.KeyDown += Keyboard_KeyDown;
            sceneManager.Keyboard.KeyUp += keyboard_KeyUp;

            GL.ClearColor(0.4f, 0.0f, 0.75f, 1.0f);

            MapReader("Maps/map.txt");
            CreateSystems();
            SetupAudio();
        }

        private void SetupAudio()
        {
            // Setup OpenAL Listener
            listenerPosition = eyePos;
            listenerDirection = targetPos;
            listenerUp = Vector3.UnitY;

            ding1 = new ComponentAudio("Audio/ding1.wav", false);
            Vector3 emitterPosition = eyePos;
            ding1.SetPosition(emitterPosition);

            ding2 = new ComponentAudio("Audio/ding2.wav", false);
            ding2.SetPosition(emitterPosition);

        }

        private void CreateSystems()
        {
            ISystem newSystem;

            newSystem = new SystemRender();
            systemManager.AddSystem(newSystem);

            newSystem = new SystemPhysics();
            systemManager.AddSystem(newSystem);

            newSystem = new SystemAudio();
            systemManager.AddSystem(newSystem);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>
        public override void Update(FrameEventArgs e)
        {
            if (GamePad.GetState(1).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Key.Escape))
                sceneManager.Exit();
            List<Entity> entities = entityManager.Entities();
            dt = (float)e.Time;

            if (upPressed)
            {
                if (targetPos.X == -5000)
                {
                    CheckXCollision(entities, -0.1f);
                    if (!wallCollide)
                    {
                        MoveOnX(false);
                    }
                }
                else if (targetPos.X == 5000)
                {
                    CheckXCollision(entities, 0.1f);
                    if (!wallCollide)
                    {
                        MoveOnX(true);
                    }
                }
                else if (targetPos.Z < eyePos.Z)
                {
                    CheckZCollision(entities, -0.1f);
                    if (!wallCollide)
                    {
                        MoveOnZ(false);
                    }
                }
                else if (targetPos.Z > eyePos.Z)
                {
                    CheckZCollision(entities, 0.1f);
                    if (!wallCollide)
                    {
                        MoveOnZ(true);
                    }
                }
            }
            if (downPressed)
            {
                if (targetPos.X == -5000)
                {
                    CheckXCollision(entities, 0.1f);
                    if (!wallCollide)
                    {
                        MoveOnX(true);
                    }
                }
                else if (targetPos.X == 5000)
                {
                    CheckXCollision(entities, -0.1f);
                    if (!wallCollide)
                    {
                        MoveOnX(false);
                    }
                }
                else if (targetPos.Z < eyePos.Z)
                {
                    CheckZCollision(entities, 0.1f);
                    if (!wallCollide)
                    {
                        MoveOnZ(true);
                    }
                }
                else if (targetPos.Z > eyePos.Z)
                {
                    CheckZCollision(entities, -0.1f);
                    if (!wallCollide)
                    {
                        MoveOnZ(false);
                    }
                }
            }

            listenerPosition = eyePos;
            listenerDirection = targetPos;

            if (pelletsCollected == pelletIndex)
            {
                sceneManager.SceneChange(SceneTypes.GAME_WIN, score);
            }

            if (lifeTotal == 0)
            {
                sceneManager.SceneChange(SceneTypes.GAME_OVER, score);
            }

            if (ghostKill)
            {
                timer -= dt;
            }
            if (timer <= 0)
            {
                ghostKill = false;
            }

            Entity ghost = entities.Find(x => x.Name == "ghostA");
            if (aDead)
            {
                aTimer += dt;
                if(aTimer >= 5 && aTimer <= 5.1)
                {
                    aDead = false;
                    aTimer = 0;
                    ghost.ChangeVelocity(new Vector3(0, 0, -ghostASpeed));
                }
            }
            Vector3 ghostVel = ghost.GetVelocity();
            Vector3 ghostPos = ghost.GetPosition();
            ghostCollision(entities, ghost, ghostVel, ghostPos, ghostASpeed, 5);

            ghost = entities.Find(x => x.Name == "ghostB");
            ghostTimer += dt;

            if (ghostTimer >= 10 && ghostTimer <= 10.1)
            {
                ghost.ChangeVelocity(new Vector3(0, 0, -ghostBSpeed));
            }
            if (bDead)
            {
                bTimer += dt;
                if (bTimer >= 5 && bTimer <= 5.1)
                {
                    bDead = false;
                    bTimer = 0;
                    ghost.ChangeVelocity(new Vector3(0, 0, -ghostBSpeed));
                }
            }
            ghostVel = ghost.GetVelocity();
            ghostPos = ghost.GetPosition();
            ghostCollision(entities, ghost, ghostVel, ghostPos, ghostBSpeed, 4);



            ghost = entities.Find(x => x.Name == "ghostC");
            if (ghostTimer >= 20 && ghostTimer <= 20.1)
            {
                ghost.ChangeVelocity(new Vector3(0, 0, -ghostCSpeed));
            }
            if (cDead)
            {
                cTimer += dt;
                if (cTimer >= 5 && cTimer <= 5.1)
                {
                    cDead = false;
                    cTimer = 0;
                    ghost.ChangeVelocity(new Vector3(0, 0, -ghostCSpeed));
                }
            }
            ghostVel = ghost.GetVelocity();
            ghostPos = ghost.GetPosition();
            ghostCollision(entities, ghost, ghostVel, ghostPos, ghostCSpeed, 6);

            // TODO: Add your update logic here

            // Move sounds source from right to left at 2.5 meters per second

            //emitterPosition = componentAudio.SetVelocity(emitterPosition, (float)(2.5 * e.Time));
            //AL.Source(mySource, ALSource3f.Position, ref emitterPosition);

            // update OpenAL
            AL.Listener(ALListener3f.Position, ref listenerPosition);
            AL.Listener(ALListenerfv.Orientation, ref listenerDirection, ref listenerUp);

        }

        private void ghostCollision(List<Entity> entities, Entity ghost, Vector3 ghostVel, Vector3 ghostPos, float ghostSpeed, int turning)
        {
            foreach (Entity entity in entities)
            {
                Vector3 entityPos = entity.GetPosition();
                if (entity.Name.Contains("wall"))
                {
                    if (ghostVel.Z != 0)
                    {
                        float zCollide = (ghostPos.Z + (ghostVel.Z / (ghostSpeed / 0.5f))) - entityPos.Z;
                        float xCollide = ghostPos.X - entityPos.X;
                        if (zCollide < a && zCollide > -a && xCollide < a && xCollide > -a)
                        {
                            xCollide = (ghostPos.X + (ghostSpeed / (ghostSpeed / 0.5f))) - entityPos.X;
                            zCollide = ghostPos.Z - entityPos.Z;
                            if (!(zCollide < a && zCollide > -a && xCollide < a && xCollide > -a))
                            {
                                right = true;
                            }
                            xCollide = (ghostPos.X - (ghostSpeed / (ghostSpeed / 0.5f))) - entityPos.X;
                            zCollide = ghostPos.Z - entityPos.Z;
                            if (!(zCollide < a && zCollide > -a && xCollide < a && xCollide > -a))
                            {
                                left = true;
                            }
                            if (right && left)
                            {
                                Random rand = new Random();
                                int dir = rand.Next(turning);
                                if (dir >= 0 && dir <= 2)
                                {
                                    ghost.ChangeVelocity(new Vector3(ghostSpeed, 0, 0));
                                }
                                else
                                {
                                    ghost.ChangeVelocity(new Vector3(-ghostSpeed, 0, 0));
                                }
                            }
                            else if (right)
                            {
                                ghost.ChangeVelocity(new Vector3(ghostSpeed, 0, 0));
                            }
                            else
                            {
                                ghost.ChangeVelocity(new Vector3(-ghostSpeed, 0, 0));
                            }
                        }
                    }
                    else
                    {
                        float zCollide = ghostPos.Z - entityPos.Z;
                        float xCollide = (ghostPos.X + (ghostVel.X / (ghostSpeed / 0.5f))) - entityPos.X;
                        if (zCollide < a && zCollide > -a && xCollide < a && xCollide > -a)
                        {
                            zCollide = (ghostPos.Z + (ghostSpeed / (ghostSpeed / 0.5f))) - entityPos.Z;
                            xCollide = ghostPos.X - entityPos.X;
                            if (!(zCollide < a && zCollide > -a && xCollide < a && xCollide > -a))
                            {
                                down = true;
                            }
                            zCollide = (ghostPos.Z - (ghostSpeed / (ghostSpeed / 0.5f))) - entityPos.Z;
                            xCollide = ghostPos.X - entityPos.X;
                            if (!(zCollide < a && zCollide > -a && xCollide < a && xCollide > -a))
                            {
                                up = true;
                            }
                            if (up && down)
                            {
                                Random rand = new Random();
                                int dir = rand.Next(turning);
                                if (dir >= 0 && dir <= 2)
                                {
                                    ghost.ChangeVelocity(new Vector3(0, 0, ghostSpeed));
                                }
                                else
                                {
                                    ghost.ChangeVelocity(new Vector3(0, 0, -ghostSpeed));
                                }
                            }
                            else if (down)
                            {
                                ghost.ChangeVelocity(new Vector3(0, 0, ghostSpeed));
                            }
                            else
                            {
                                ghost.ChangeVelocity(new Vector3(0, 0, -ghostSpeed));
                            }
                        }
                    }
                }
            }
        }

        private void CheckZCollision(List<Entity> entities, float dir)
        {
            foreach (Entity entity in entities)
            {
                Vector3 entityPos = entity.GetPosition();
                if (entity.Name.Contains("wall") && collisionOn)
                {
                    float zCollide = (eyePos.Z + dir) - entityPos.Z;
                    float xCollide = eyePos.X - entityPos.X;
                    if (zCollide < 1.5 && zCollide > -1.5 && xCollide < 1.5 && xCollide > -1.5)
                    {
                        wallCollide = true;
                        break;
                    }
                }
                if (entity.Name.Contains("pellet"))
                {
                    float zCollide = eyePos.Z - entityPos.Z;
                    float xCollide = eyePos.X - entityPos.X;
                    if (zCollide < 1.0 && zCollide > -1.0 && xCollide < 1.0 && xCollide > -1.0)
                    {
                        entity.ChangePosition(new Vector3(0, -10, 0));
                        Vector3 emitterPosition = eyePos;
                        ding2.SetPosition(emitterPosition);
                        ding2.Start();
                        pelletsCollected += 1;
                        score += 100;
                        break;
                    }
                }

                if (entity.Name.Contains("ghost"))
                {
                    float zCollide = (eyePos.Z + dir) - entityPos.Z;
                    float xCollide = eyePos.X - entityPos.X;
                    if (zCollide < 1.5 && zCollide > -1.5 && xCollide < 1.5 && xCollide > -1.5)
                    {
                        if (ghostKill)
                        {
                            ComponentAudio killGhost = entity.GetAudio(0);
                            killGhost.Start();
                            switch (entity.Name)
                            {
                                
                                case "ghostA":
                                    entity.ChangePosition(ghostASpawn);
                                    entity.ChangeVelocity(new Vector3(0, 0, 0));
                                    aDead = true;
                                    break;
                                case "ghostB":
                                    entity.ChangePosition(ghostBSpawn);
                                    entity.ChangeVelocity(new Vector3(0, 0, 0));
                                    bDead = true;
                                    break;
                                case "ghostC":
                                    entity.ChangePosition(ghostCSpawn);
                                    entity.ChangeVelocity(new Vector3(0, 0, 0));
                                    cDead = true;
                                    break;
                            }
                            score += 500;
                            break;
                        }
                        else
                        {
                            ComponentAudio loseLife = entity.GetAudio(1);
                            loseLife.Start();
                            eyePos = spawnPoint;
                            targetPos = new Vector3(eyePos.X, eyePos.Y, eyePos.Z - 2);
                            lifeTotal -= 1;
                            
                            break;
                        }
                    }
                }

                if (entity.Name.Contains("pUp"))
                {
                    float zCollide = eyePos.Z - entityPos.Z;
                    float xCollide = eyePos.X - entityPos.X;
                    if (zCollide < 1.0 && zCollide > -1.0 && xCollide < 1.0 && xCollide > -1.0)
                    {
                        entity.ChangePosition(new Vector3(0, -10, 0));
                        Vector3 emitterPosition = eyePos;
                        ding1.SetPosition(emitterPosition);
                        ding1.Start();
                        ComponentAudio buzz = entity.GetAudio(0);
                        buzz.Stop();
                        ghostKill = true;
                        timer = 7.0f;
                        score += 250;
                        break;
                    }
                }
            }
        }

        private void CheckXCollision(List<Entity> entities, float dir)
        {
            foreach (Entity entity in entities)
            {
                Vector3 entityPos = entity.GetPosition();
                if (entity.Name.Contains("wall") && collisionOn)
                {
                    float xCollide = (eyePos.X + dir) - entityPos.X;
                    float zCollide = eyePos.Z - entityPos.Z;
                    if (zCollide < 1.5 && zCollide > -1.5 && xCollide < 1.5 && xCollide > -1.5)
                    {
                        wallCollide = true;
                        break;
                    }
                }
                if (entity.Name.Contains("pellet"))
                {
                    float zCollide = eyePos.Z - entityPos.Z;
                    float xCollide = eyePos.X - entityPos.X;
                    if (zCollide < 1.0 && zCollide > -1.0 && xCollide < 1.0 && xCollide > -1.0)
                    {
                        entity.ChangePosition(new Vector3(0, -10, 0));
                        Vector3 emitterPosition = eyePos;
                        ding2.SetPosition(emitterPosition);
                        ding2.Start();
                        pelletsCollected += 1;
                        score += 100;
                        break;
                    }
                }
                if (entity.Name.Contains("ghost"))
                {
                    float xCollide = (eyePos.X + dir) - entityPos.X;
                    float zCollide = eyePos.Z - entityPos.Z;
                    if (zCollide < 1.5 && zCollide > -1.5 && xCollide < 1.5 && xCollide > -1.5)
                    {
                        if (ghostKill)
                        {
                            ComponentAudio killGhost = entity.GetAudio(0);
                            killGhost.Start();
                            if (entity.Name == "ghostA")
                            {
                                entity.ChangePosition(ghostASpawn);
                                entity.ChangeVelocity(new Vector3(0, 0, 0));
                                aDead = true;
                            }
                            else if (entity.Name == "ghostB")
                            {
                                entity.ChangePosition(ghostBSpawn);
                                entity.ChangeVelocity(new Vector3(0, 0, 0));
                                bDead = true;
                            }
                            else
                            {
                                entity.ChangePosition(ghostCSpawn);
                                entity.ChangeVelocity(new Vector3(0, 0, 0));
                                cDead = true;
                            }
                            score += 500;
                            break;
                        }
                        else
                        {
                            ComponentAudio loseLife = entity.GetAudio(1);
                            loseLife.Start();
                            eyePos = spawnPoint;
                            targetPos = new Vector3(eyePos.X, eyePos.Y, eyePos.Z - 2);
                            lifeTotal -= 1;
                            break;
                        }
                    }
                }
                if (entity.Name.Contains("pUp"))
                {
                    float zCollide = eyePos.Z - entityPos.Z;
                    float xCollide = eyePos.X - entityPos.X;
                    if (zCollide < 1.0 && zCollide > -1.0 && xCollide < 1.0 && xCollide > -1.0)
                    {
                        Vector3 emitterPosition = eyePos;
                        ding1.SetPosition(emitterPosition);
                        ding1.Start();
                        ComponentAudio buzz = entity.GetAudio(0);
                        buzz.Stop();
                        entity.ChangePosition(new Vector3(0, -10, 0));
                        ghostKill = true;
                        timer = 7.0f;
                        score += 250;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>
        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 10f;
            GUI.clearColour = Color.Transparent;
            GUI.Label(new Rectangle(0, 40, (int)width, (int)(fontSize * 2f)), "Pellets: " + pelletsCollected.ToString() + "/" + pelletIndex.ToString(), 20, StringAlignment.Near, Color.White);
            GUI.Label(new Rectangle(0, 0, (int)width, (int)(fontSize * 2f)), "Score: " + score.ToString(), 18, StringAlignment.Near, Color.White);
            GUI.Label(new Rectangle(0, -40, (int)width, (int)(fontSize * 2f)), "Lives: " + lifeTotal.ToString(), 20, StringAlignment.Far, Color.White);
            if (ghostKill)
            {
                GUI.Label(new Rectangle(0, -20, (int)width, (int)(fontSize * 2f)), "Power-up:", 18, StringAlignment.Center, Color.White);
                GUI.Label(new Rectangle(0, 0, (int)width, (int)(fontSize * 2f)), timer.ToString("0.00"), 18, StringAlignment.Center, Color.White);
            }
            if (!collisionOn)
            {
                GUI.Label(new Rectangle(0, 0, (int)width, (int)(fontSize * 2f)), "Collision off ", 25, StringAlignment.Far, Color.White);
            }
            if (!ghostsOn)
            {
                GUI.Label(new Rectangle(0, 40, (int)width, (int)(fontSize * 2f)), "Ghosts off ", 25, StringAlignment.Far, Color.White);
            }
            systemManager.ActionSystems(entityManager);

            GUI.Render();
        }

        /// <summary>
        /// This is called when the game exits.
        /// </summary>
        public override void Close()
        {
            List<Entity> entities = entityManager.Entities();
            foreach(Entity entity in entities)
            {
                if (entity.Components.Exists(x => x.ComponentType.ToString() == "COMPONENT_AUDIO"))
                {
                    ComponentAudio audio = entity.GetAudio(0);
                    audio.Close();
                    if (entity.Name.Contains("ghost"))
                    {
                        audio = entity.GetAudio(1);
                        audio.Close();
                    }
                }
            }
            ding1.Close();
            ding2.Close();
        }

        /// <summary>
        /// Moves the player along the Z-axis
        /// </summary>
        /// <param name="dir">Moves back if true, foward if false</param>
        public void MoveOnZ(bool dir)
        {
            float x = -0.1f;
            if (dir)
            {
                x = 0.1f;
            }
            eyePos.Z += x;
            targetPos.Z += x;
            CameraUpdate(eyePos, targetPos);
            Entity skybox = entityManager.Entities().Find(a => a.Name == "skybox");
            skybox.ChangePosition(eyePos);
        }

        /// <summary>
        /// Moves the player along the X-axis
        /// </summary>
        /// <param name="dir">Moves back if true, foward if false</param>
        public void MoveOnX(bool dir)
        {
            float x = -0.1f;
            if (dir)
            {
                x = 0.1f;
            }
            eyePos.X += x;
            CameraUpdate(eyePos, targetPos);
            Entity skybox = entityManager.Entities().Find(a => a.Name == "skybox");
            skybox.ChangePosition(eyePos);
        }

        /// <summary>
        /// Turns the player to the boundaries of the X-axis
        /// </summary>
        /// <param name="dir">Looks to left boundary if false, right boundary if true</param>
        public void LookToX(bool dir)
        {
            int x = -5000;
            if (dir)
            {
                x = 5000;
            }
            targetPos.X = x;
            CameraUpdate(eyePos, targetPos);
        }

        /// <summary>
        /// Turns the player to the boundaries of the Z-axis
        /// </summary>
        /// <param name="dir">Looks to the top if false, bottom if true</param>
        public void LookToZ(bool dir)
        {
            int x = -2;
            if (dir)
            {
                x = 2;
            }
            targetPos.X = eyePos.X;
            targetPos.Z = eyePos.Z + x;
            CameraUpdate(eyePos, targetPos);
        }

        /// <summary>
        /// Handles keyboard input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            List<Entity> entities = entityManager.Entities();
            switch (e.Key)
            {
                case Key.Up:
                    wallCollide = false;
                    upPressed = true;
                    break;

                case Key.Down:
                    wallCollide = false;
                    downPressed = true;
                    break;

                case Key.Left:
                    if (targetPos.X != -5000 && targetPos.X != 5000)
                    {
                        if (targetPos.Z < eyePos.Z)
                        {
                            LookToX(false);
                        }
                        else
                        {
                            LookToX(true);
                        }
                        break;
                    }
                    if (targetPos.X == -5000)
                    {
                        LookToZ(true);
                    }
                    else if (targetPos.X == 5000)
                    {
                        LookToZ(false);
                    }
                    break;

                case Key.Right:
                    if (targetPos.X != -5000 && targetPos.X != 5000)
                    {
                        if (targetPos.Z < eyePos.Z)
                        {
                            LookToX(true);
                        }
                        else
                        {
                            LookToX(false);
                        }
                        break;
                    }
                    if (targetPos.X == -5000)
                    {
                        LookToZ(false);
                    }
                    else if (targetPos.X == 5000)
                    {
                        LookToZ(true);
                    }
                    break;
                case Key.C:
                    if (!collisionOn)
                    {
                        collisionOn = true;
                    }
                    else
                    {
                        collisionOn = false;
                    }
                    break;
                case Key.G:
                    if (!ghostsOn)
                    {
                        ghostsOn = true;
                        Entity ghost = entities.Find(x => x.Name == "ghostA");
                        ghost.ChangeVelocity(new Vector3(0, 0, -ghostASpeed));
                        ghostTimer = 0;
                    }
                    else
                    {
                        ghostsOn = false;
                        Entity ghost = entities.Find(x => x.Name == "ghostA");
                        ghost.ChangePosition(ghostASpawn);
                        ghost.ChangeVelocity(new Vector3(0, 0, 0));
                        ghost = entities.Find(x => x.Name == "ghostB");
                        ghost.ChangePosition(ghostBSpawn);
                        ghost.ChangeVelocity(new Vector3(0, 0, 0));
                        ghost = entities.Find(x => x.Name == "ghostC");
                        ghost.ChangePosition(ghostCSpawn);
                        ghost.ChangeVelocity(new Vector3(0, 0, 0));
                        ghostTimer = 50;
                    }
                    break;
                case Key.Escape:
                    sceneManager.Keyboard.KeyDown -= Keyboard_KeyDown;
                    sceneManager.SceneChange(SceneTypes.GAME_OVER, 0);
                    break;
            }
        }

        public void keyboard_KeyUp(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    upPressed = false;
                    wallCollide = false;
                    break;
                case Key.Down:
                    downPressed = false;
                    wallCollide = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Reads the given map and loads all entities marked
        /// </summary>
        /// <param name="filename">Map file name</param>
        public void MapReader(string filename)
        {
            StreamReader map = new StreamReader(filename);
            int size = int.Parse(map.ReadLine());
            int wallIndex = 0, pUpIndex = 0, floorIndex = 0;
            string mapPiece, mapLine;
            string[] mapArray = new string[size];
            Entity entity;
            for (int i = 0; i < size; i++)
            {
                mapLine = map.ReadLine();
                mapArray = mapLine.Split(' ');
                for (int j = 0; j < size; j++)
                {
                    mapPiece = mapArray[j];
                    switch (mapPiece)
                    {
                        case "W":
                            entity = new Entity("wall" + wallIndex);
                            entity.CreateEntity(new Vector3(j * 2, 0.0f, i * 2), "Geometry/WallGeometry.txt", "Textures/hedge.png");
                            entityManager.AddEntity(entity);
                            wallIndex++;
                            break;

                        case ".": //-1.0f, 0.25f
                            entity = new Entity("floor" + floorIndex);
                            entity.CreateEntity(new Vector3(j * 2, -0.5f, i * 2), "Geometry/FloorGeometry.txt", "Textures/floor.png");
                            entityManager.AddEntity(entity);
                            entity = new Entity("pellet" + pelletIndex); 
                            entity.CreateEntity(new Vector3(j * 2, 0.75f, i * 2), "Geometry/PelletGeometry.txt", "Textures/random1.png");
                            entityManager.AddEntity(entity);
                            pelletIndex++;
                            floorIndex++;
                            break;

                        case ",": //-1.0f
                            entity = new Entity("floor" + floorIndex);
                            entity.CreateEntity(new Vector3(j * 2, -0.5f, i * 2), "Geometry/FloorGeometry.txt", "Textures/floor.png");
                            entityManager.AddEntity(entity);
                            floorIndex++;
                            break;

                        case "p": //0.0f, -1.0f
                            entity = new Entity("pUp" + pUpIndex);
                            entity.CreateEntity(new Vector3(j * 2, 0.5f, i * 2), "Geometry/pUpGeometry.txt", "Textures/random2.png", "Audio/buzz.wav", true);
                            entityManager.AddEntity(entity);
                            entity = new Entity("floor" + floorIndex);
                            entity.CreateEntity(new Vector3(j * 2, -0.5f, i * 2), "Geometry/FloorGeometry.txt", "Textures/floor.png");
                            entityManager.AddEntity(entity);
                            floorIndex++;
                            pUpIndex++;
                            break;

                        case "G":
                            entity = new Entity("ghostA");
                            ghostASpawn = new Vector3(j * 2, 0.5f, i * 2);
                            entity.CreateMovingEntity(ghostASpawn, -ghostASpeed, "Geometry/wallGeometry.txt", "Textures/random3.png", "Audio/killGhost.wav", false);
                            entity.AddComponent(new ComponentAudio("Audio/loseLife.wav", false));
                            entityManager.AddEntity(entity);
                            entity = new Entity("floor" + floorIndex);
                            entity.CreateEntity(new Vector3(j * 2, -0.5f, i * 2), "Geometry/FloorGeometry.txt", "Textures/floor.png");
                            entityManager.AddEntity(entity);
                            floorIndex++;
                            break;

                        case "g":
                            entity = new Entity("ghostB");
                            ghostBSpawn = new Vector3(j * 2, 0.5f, i * 2);
                            entity.CreateMovingEntity(ghostBSpawn, 0, "Geometry/wallGeometry.txt", "Textures/random3.png", "Audio/killGhost.wav", false);
                            entity.AddComponent(new ComponentAudio("Audio/loseLife.wav", false));
                            entityManager.AddEntity(entity);
                            entity = new Entity("floor" + floorIndex);
                            entity.CreateEntity(new Vector3(j * 2, -0.5f, i * 2), "Geometry/FloorGeometry.txt", "Textures/floor.png");
                            entityManager.AddEntity(entity);
                            floorIndex++;
                            break;

                        case "P":
                            entity = new Entity("ghostC");
                            ghostCSpawn = new Vector3(j * 2, 0.5f, i * 2);
                            entity.CreateMovingEntity(ghostCSpawn, 0, "Geometry/wallGeometry.txt", "Textures/random3.png", "Audio/killGhost.wav", false);
                            entity.AddComponent(new ComponentAudio("Audio/loseLife.wav", false));
                            entityManager.AddEntity(entity);
                            entity = new Entity("floor" + floorIndex);
                            entity.CreateEntity(new Vector3(j * 2, -0.5f, i * 2), "Geometry/FloorGeometry.txt", "Textures/floor.png");
                            entityManager.AddEntity(entity);
                            floorIndex++;
                            break;

                        case "S": //-1.0f
                            entity = new Entity("floor" + floorIndex);
                            entity.CreateEntity(new Vector3(j * 2, -0.5f, i * 2), "Geometry/FloorGeometry.txt", "Textures/ddevito.png");
                            entityManager.AddEntity(entity);
                            floorIndex++;
                            spawnPoint = new Vector3(j * 2, 1.5f, i * 2);
                            eyePos = new Vector3(j * 2, 1.5f, i * 2);
                            targetPos = new Vector3(j * 2, 1.5f, (i * 2) - 2);
                            CameraUpdate(eyePos, targetPos);
                            entity = new Entity("skybox");
                            entity.CreateEntity(eyePos, "Geometry/SkyboxGeometry.txt", "Textures/sky.png");
                            entityManager.AddEntity(entity);
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Changes the camera's position and it's direction
        /// </summary>
        /// <param name="eyePos">Where the camera is placed</param>
        /// <param name="targetPos">Where the camera is looking</param>
        public void CameraUpdate(Vector3 eyePos, Vector3 targetPos)
        {
            view = Matrix4.LookAt(new Vector3(eyePos), new Vector3(targetPos), new Vector3(0, 1, 0));
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), 800f / 480f, 0.01f, 100f);
        }
    }
}
