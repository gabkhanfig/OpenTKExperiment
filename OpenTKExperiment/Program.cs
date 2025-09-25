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
            using (Game game = new Game())
            { 
                game.Run();
            }
                
            {
                // Start the game loop
                // The Run() method usually contains the main update-render loop
                
            } // At this point, the 'game' object is automatically disposed, freeing any resources it used
        }
    }
}