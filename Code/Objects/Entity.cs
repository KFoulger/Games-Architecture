using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using OpenGL_Game.Components;
using OpenTK;

namespace OpenGL_Game.Objects
{
    class Entity
    {
        string name;
        List<IComponent> componentList = new List<IComponent>();
        ComponentTypes mask;
 
        public Entity(string name)
        {
            this.name = name;
        }

        /// <summary>Adds a single component</summary>
        public void AddComponent(IComponent component)
        {
            Debug.Assert(component != null, "Component cannot be null");
            
            componentList.Add(component);
            mask |= component.ComponentType;
        }

        /// <summary>
        /// Returns the name of this entity
        /// </summary>
        public String Name
        {
            get { return name; }
        }

        /// <summary>
        /// Returns this entity's mask
        /// </summary>
        public ComponentTypes Mask
        {
            get { return mask; }
        }

        /// <summary>
        /// Returns list of components in this entity
        /// </summary>
        public List<IComponent> Components
        {
            get { return componentList; }
        }

        /// <summary>
        /// Changes the position of this entity
        /// </summary>
        /// <param name="newPos">Updated position of entity</param>
        public void ChangePosition(Vector3 newPos)
        {
            List<IComponent> compList = this.Components;
            compList.RemoveAt(0);
            compList.Insert(0, new ComponentPosition(newPos));
        }

        /// <summary>
        /// Returns the Vector3 position of this entity
        /// </summary>
        /// <returns></returns>
        public Vector3 GetPosition()
        {
            List<IComponent> compList = this.Components;
            ComponentPosition compPos = (ComponentPosition)compList[0];
            return compPos.Position;
        }

        /// <summary>
        /// Changes the velocity of this entity
        /// </summary>
        /// <param name="newVel">Updated velocity of entity</param>
        public void ChangeVelocity(Vector3 newVel)
        {
            List<IComponent> compList = this.Components;
            compList.RemoveAt(1);
            compList.Insert(1, new ComponentVelocity(newVel));
        }

        /// <summary>
        /// Returns the Vector3 velocity of this entity
        /// </summary>
        /// <returns></returns>
        public Vector3 GetVelocity()
        {
            List<IComponent> compList = this.Components;
            ComponentVelocity compVel = (ComponentVelocity)compList[1];
            return compVel.Velocity;
        }

        /// <summary>
        /// Returns the ComponentAudio of this entity
        /// </summary>
        /// <returns></returns>
        public ComponentAudio GetAudio(int number)
        {
            List<IComponent> compList = this.Components;
            List<IComponent> audioList = compList.FindAll(x => x.ComponentType.ToString() == "COMPONENT_AUDIO");
            return (ComponentAudio)audioList[number];
        }

        /// <summary>
        /// Renders a stationary entity
        /// </summary>
        /// <param name="position">Position of the entity created</param>
        /// <param name="geo">Entity's geometry file</param>
        /// <param name="texture">Entity's texture file</param>
        public void CreateEntity(Vector3 position, string geo, string texture)
        {
            this.AddComponent(new ComponentPosition(position));
            this.AddComponent(new ComponentGeometry(geo));
            this.AddComponent(new ComponentTexture(texture));
        }

        /// <summary>
        /// Renders a stationary entity
        /// </summary>
        /// <param name="position">Position of the entity created</param>
        /// <param name="geo">Entity's geometry file</param>
        /// <param name="texture">Entity's texture file</param>
        /// <param name="audio">Entity's audio file</param>
        /// <param name="loop">Sets audio to loop if true</param>
        public void CreateEntity(Vector3 position, string geo, string texture, string audio, bool loop)
        {
            this.AddComponent(new ComponentPosition(position));
            this.AddComponent(new ComponentGeometry(geo));
            this.AddComponent(new ComponentTexture(texture));
            this.AddComponent(new ComponentAudio(audio, loop));
            if (loop)
            {
                ComponentAudio sound = this.GetAudio(0);
                sound.Start();
            }
        }

        /// <summary>
        /// Renders an entity with velocity
        /// </summary>
        /// <param name="position">Position of the entity rendered</param>
        /// <param name="entitySpeed">Movement speed of the entity</param>
        /// <param name="geo">Entity's geometry file</param>
        /// <param name="texture">Entity's texture file</param>
        public void CreateMovingEntity(Vector3 position, float entitySpeed, string geo, string texture)
        {

            this.AddComponent(new ComponentPosition(position));
            this.AddComponent(new ComponentVelocity(0, 0, entitySpeed));
            this.AddComponent(new ComponentGeometry(geo));
            this.AddComponent(new ComponentTexture(texture));
        }

        /// <summary>
        /// Renders an entity with velocity
        /// </summary>
        /// <param name="position">Position of the entity rendered</param>
        /// <param name="entitySpeed">Movement speed of the entity</param>
        /// <param name="geo">Entity's geometry file</param>
        /// <param name="texture">Entity's texture file</param>
        /// <param name="audio">Entity's audio file</param>
        /// <param name="loop">Sets audio to loop if true</param>
        public void CreateMovingEntity(Vector3 position, float entitySpeed, string geo, string texture, string audio, bool loop)
        {

            this.AddComponent(new ComponentPosition(position));
            this.AddComponent(new ComponentVelocity(0, 0, entitySpeed));
            this.AddComponent(new ComponentGeometry(geo));
            this.AddComponent(new ComponentTexture(texture));
            this.AddComponent(new ComponentAudio(audio, loop));
            if (loop)
            {
                ComponentAudio sound = this.GetAudio(0);
                sound.Start();
            }
        }
    }
}
