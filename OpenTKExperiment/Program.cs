using OpenTK.Mathematics;
using System;           // Import basic system functionalities like Console, Math, etc.
using WindowEngine;     // Import the WindowEngine namespace, which contains Game class and other related classes

namespace WindowEngine
{
    // Entry point of the application
    class Program
    {
        // Brings the vector back into the range of 0 - 1
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

        // Main method: the starting point of every C# console application
        static void Main(string[] args)
        {
            // 'using' ensures proper disposal of resources when the Game object is no longer needed
            using (Game game = new Game(new Vector3(0, 0, 0), new Vector3(0.5f, 0, 0.5f), new Vector3(0, 0.5f, 0), new Vector3(1, 1, 1)))
            {
                // Vector stuff
                Console.WriteLine("=== VECTOR OPERATIONS ===");

                ref Vector3 bottomLeftCol = ref game.bottomLeftCol;
                ref Vector3 bottomRightCol = ref game.bottomRightCol;
                ref Vector3 topRightCol = ref game.topRightCol;
                ref Vector3 topLeftCol = ref game.topLeftCol;

                
                Console.WriteLine("BEFORE");
                Console.WriteLine($"Bottom Left: {bottomLeftCol}");
                Console.WriteLine($"Bottom Right: {bottomRightCol}");
                Console.WriteLine($"Top Right: {topRightCol}");
                Console.WriteLine($"Top Left: {topLeftCol}");

                Random rand = new Random();
                // between 0 and 1
                Vector3 randVec = new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());

                Console.WriteLine($"\nrandom vector -> {randVec}\n");

                bottomLeftCol += randVec;
                bottomRightCol -= randVec;
                topRightCol.X = Vector3.Dot(topRightCol, randVec);
                topRightCol.Y = Vector3.Dot(topRightCol, randVec); // topRightCol modified by the dot above
                topRightCol.Z = Vector3.Dot(topRightCol, randVec); // topRightCol modified by the dot above

                topLeftCol = Vector3.Cross(topLeftCol, randVec);

                FunnyNormalize(ref bottomLeftCol);
                FunnyNormalize(ref bottomRightCol);
                FunnyNormalize(ref topRightCol);
                FunnyNormalize(ref topLeftCol);

                Console.WriteLine("AFTER");
                Console.WriteLine($"Bottom Left: {bottomLeftCol}");
                Console.WriteLine($"Bottom Right: {bottomRightCol}");
                Console.WriteLine($"Top Right: {topRightCol}");
                Console.WriteLine($"Top Left: {topLeftCol}");

                // Matrix stuff
                Console.WriteLine("\n=== MATRIX OPERATIONS ===");

                Matrix3 identity = Matrix3.Identity;
                Vector3 newVec = randVec * identity;
                Console.WriteLine($"Identity matrix:\n{identity}\nmultiplied by Vector{randVec} = {newVec}\nSame value");

                Matrix3 scaling = new Matrix3(new Vector3(2, 0, 0), new Vector3(0, 0.5f, 0), new Vector3(0, 0, 4));
                Vector3 scaledVec = randVec * scaling;
                Console.WriteLine($"Scaling matrix:\n{scaling}\nmultiplied by Vector{randVec} = {scaledVec}\nDifferent values");

                const float angleRadians = 45.0f * (float)(Math.PI / 180.0); // 45 degrees
                Matrix3 xAxisRotation = new Matrix3(
                    new Vector3(1, 0, 0),
                    new Vector3(0, (float)Math.Cos(angleRadians), -1.0f * (float)Math.Sin(angleRadians)),
                    new Vector3(0, (float)Math.Sin(angleRadians), (float)Math.Cos(angleRadians)));

                Vector3 rotatedVec = randVec * xAxisRotation;
                Console.WriteLine($"Rotation matrix:\n{xAxisRotation}\nmultiplied by Vector{randVec} = {rotatedVec}\nRotated on X axis, which component stayed the same, while the others changed");

                Matrix3 multiplied = scaling * xAxisRotation;
                Console.WriteLine($"Multiplying matricies:\n{scaling}\nMultiplied\n{xAxisRotation}\nEquals\n{multiplied}");

                game.Run();
            }
                
            {
                // Start the game loop
                // The Run() method usually contains the main update-render loop
                
            } // At this point, the 'game' object is automatically disposed, freeing any resources it used
        }
    }
}