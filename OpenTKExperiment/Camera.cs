using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKExperiment
{
    public class Camera
    {
        public Vector3 position;
        public Vector3 front;
        public float yaw;
        public float pitch;
        public float moveSpeed;
        public float sensitivity;

        public Camera(Vector3 pos)
        {
            position = pos;
            front = -Vector3.UnitZ;
            yaw = -90;
            pitch = 0;
            moveSpeed = 10;
            sensitivity = 0.05f;
        }

        public Matrix4 viewMatrix()
        {
            return Matrix4.LookAt(position, position + front, new Vector3(0, 1, 0));
        }

        public void update(float deltaTime, KeyboardState keyboardState)
        {
            if(keyboardState.IsKeyDown(Keys.W))
            {
                front.Y += deltaTime * moveSpeed;
            } 
            if(keyboardState.IsKeyDown(Keys.S))
            {
                front.Y -= deltaTime * moveSpeed;
            }
            if(keyboardState.IsKeyDown(Keys.A))
            {
                front.X += deltaTime * moveSpeed;
            } 
            if(keyboardState.IsKeyDown(Keys.D))
            {
                front.X -= deltaTime * moveSpeed;
            }
        }
    }
}
