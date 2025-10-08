using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace OpenTKExperiment
{
    public struct Vertex {
        public Vector3 position;
        public Vector3 normal;
        public Vector3 color;
        public Vector2 texCoord;

        public Vertex(Vector3 pos, Vector3 norm, Vector3 col, Vector2 tex)
        {
            position = pos;
            normal = norm;
            color = col;
            FunnyNormalize(ref col);
            texCoord = tex;
        }

        static void FunnyNormalize(ref Vector3 v)
        {
            while (v.X < 0)
            {
                v.X += 1;
            }
            while (v.X > 1)
            {
                v.X -= 1;
            }
            while (v.Y < 0)
            {
                v.Y += 1;
            }
            while (v.Y > 1)
            {
                v.Y -= 1;
            }
            while (v.Z < 0)
            {
                v.Z += 1;
            }
            while (v.Z > 1)
            {
                v.Z -= 1;
            }
        }
    }

    public struct Square {
        public Vertex v00;
        public Vertex v01;
        public Vertex v10;
        public Vertex v11;

        public Square(Vector3 baseVector, float length, Vector3 normal)
        {
            Vector3 inverseNorm = flipNormal(normal);
            Vector3 p00 = baseVector;
            Vector3 p11 = baseVector + (inverseNorm * new Vector3(length));
            Vector3 p01 = Vector3.Zero;
            Vector3 p10 = Vector3.Zero;
            if (normal.X == 1)
            {
                p01 = baseVector + new Vector3(0, length, 0);
                p10 = baseVector + new Vector3(0, 0, length);
            }
            else if(normal.Y == 1)
            {
                p01 = baseVector + new Vector3(length, 0, 0);
                p10 = baseVector + new Vector3(0, 0, length);
            }
            else if(normal.Z == 1)
            {
                p01 = baseVector + new Vector3(length, 0, 0);
                p10 = baseVector + new Vector3(0, length, 0);
            }
            if (normal.X == -1)
            {
                p01 = baseVector + new Vector3(0, -length, 0);
                p10 = baseVector + new Vector3(0, 0, -length);
            }
            else if(normal.Y == -1)
            {
                p01 = baseVector + new Vector3(-length, 0, 0);
                p10 = baseVector + new Vector3(0, 0, -length);
            }
            else if(normal.Z == -1)
            {
                p01 = baseVector + new Vector3(-length, 0, 0);
                p10 = baseVector + new Vector3(0, -length, 0);
            }

            v00 = new Vertex(p00, normal, new Vector3(1, 1, 1), new Vector2(0, 0));
            v01 = new Vertex(p01, normal, new Vector3(1, 1, 1), new Vector2(0, 1));
            v10 = new Vertex(p10, normal, new Vector3(1, 1, 1), new Vector2(1, 0));
            v11 = new Vertex(p11, normal, new Vector3(1, 1, 1), new Vector2(1, 1));
        }

        static Vector3 flipNormal(Vector3 v)
        {
            Vector3 inverse = new Vector3();
            inverse.X = (v.X == 0) ? 1 : 0;
            inverse.Y = (v.Y == 0) ? 1 : 0;
            inverse.Z = (v.Z == 0) ? 1 : 0;
            return inverse;
        }
    }

    public struct Cube
    {
        public Vertex v000;
        public Vertex v001;
        public Vertex v010;
        public Vertex v011;
        public Vertex v100;
        public Vertex v101;
        public Vertex v110;
        public Vertex v111;

        public Cube(Vector3 baseVector, float length)
        {
            Vector3 p000 = baseVector;
            Vector3 p001 = new Vector3(baseVector.X + length, baseVector.Y, baseVector.Z);
            Vector3 p010 = new Vector3(baseVector.X, baseVector.Y + length, baseVector.Z);
            Vector3 p011 = new Vector3(baseVector.X + length, baseVector.Y + length, baseVector.Z);
            Vector3 p100 = new Vector3(baseVector.X, baseVector.Y, baseVector.Z + length);
            Vector3 p101 = new Vector3(baseVector.X + length, baseVector.Y, baseVector.Z + length);
            Vector3 p110 = new Vector3(baseVector.X, baseVector.Y + length, baseVector.Z + length);
            Vector3 p111 = new Vector3(baseVector.X + length, baseVector.Y + length, baseVector.Z + length);

            Vector3 c000 = new Vector3(0, 0, 0);
            Vector3 c001 = new Vector3(0, 0, 1);
            Vector3 c010 = new Vector3(0, 1, 0);
            Vector3 c011 = new Vector3(0, 1, 1);
            Vector3 c100 = new Vector3(1, 0, 0);
            Vector3 c101 = new Vector3(1, 0, 1);
            Vector3 c110 = new Vector3(1, 1, 0);
            Vector3 c111 = new Vector3(1, 1, 1);

            v000 = new Vertex(p000, Vector3.Zero, c000, new Vector2(0, 0)); // good
            v001 = new Vertex(p001, Vector3.Zero, c001, new Vector2(1, 0)); // good
            v010 = new Vertex(p010, Vector3.Zero, c010, new Vector2(0, 1)); // good
            v011 = new Vertex(p011, Vector3.Zero, c011, new Vector2(1, 1)); // good
            v100 = new Vertex(p100, Vector3.Zero, c100, new Vector2(1, 0)); // good
            v101 = new Vertex(p101, Vector3.Zero, c101, new Vector2(0, 0)); // maybe
            v110 = new Vertex(p110, Vector3.Zero, c110, new Vector2(1, 1)); // good
            v111 = new Vertex(p111, Vector3.Zero, c111, new Vector2(0, 1)); // maybe
        }
    }
}
