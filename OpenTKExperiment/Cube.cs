using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace OpenTKExperiment
{
    public struct Vertex {
        Vector3 position;
        Vector3 color;

        public Vertex(Vector3 pos, Vector3 col)
        {
            position = pos;
            color = col;
            FunnyNormalize(ref col);
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

/*            Console.WriteLine($"p000 {p000}");
            Console.WriteLine($"p001 {p001}");
            Console.WriteLine($"p010 {p010}");
            Console.WriteLine($"p011 {p011}");
            Console.WriteLine($"p100 {p100}");
            Console.WriteLine($"p101 {p101}");
            Console.WriteLine($"p110 {p110}");
            Console.WriteLine($"p111 {p111}");*/

            Vector3 c000 = new Vector3(0, 0, 0);
            Vector3 c001 = new Vector3(0, 0, 1);
            Vector3 c010 = new Vector3(0, 1, 0);
            Vector3 c011 = new Vector3(0, 1, 1);
            Vector3 c100 = new Vector3(1, 0, 0);
            Vector3 c101 = new Vector3(1, 0, 1);
            Vector3 c110 = new Vector3(1, 1, 0);
            Vector3 c111 = new Vector3(1, 1, 1);

            v000 = new Vertex(p000, c000);
            v001 = new Vertex(p001, c001);
            v010 = new Vertex(p010, c010);
            v011 = new Vertex(p011, c011);
            v100 = new Vertex(p100, c100);
            v101 = new Vertex(p101, c101);
            v110 = new Vertex(p110, c110);
            v111 = new Vertex(p111, c111);
        }
    }
}
