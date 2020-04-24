using System;

namespace Processing_Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //You can pick here which test you want to run by simply uncommenting the desired one and commenting the rest.

            // Mandelbrot Animation Generator*
            //new MandelbrotV1();

            // Creates Image Transition Animations using particles from pixels*
            //new PixelParticles(args);

            // Sierpinski's Triangle, using Recursion
            new Recursion();

            // Conway's Game Of Life Simulator. Click to toggle square's status
            //new Conway();

            // Converts videos to epilepsy-friendly videos (removing red and bright lights)*
            //new Epilepsy();


            // * = Requires ffmpeg to be on a PATH variable.
        }
    }
}