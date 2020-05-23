using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

//Accord libraries are great, there are many different libraries for machine learning, image editing, math calculations and etc... 
//Right now ffmpeg provides us creating video and writing in its self bitmaps.
using Accord.Video.FFMPEG;
using ChaosGameN.ChaosRules;

namespace ChaosGameN
{
    class Program
    {

        public static readonly Random randomGenerator = new Random();

        static void Main(string[] args)
        {
            int width = 1024;
            int height = 862;

            //Creating instance
            ChaosGame chaos = new ChaosGame(new Tuple<int, int>(width, height), 5, new NotLocalVertices());

            //Class from Accord - you might remember the old structure from AForge Library
            using (VideoFileWriter file = new VideoFileWriter())
            {
                //Outputing into bin folder

                //Setting width and height based on instance of the class, should be roughly the same
                //Setting good framerate - 30fps should be sufficient, H264 is standard, and good bitrate - otherwise the video will be blurry
                try
                {
                    file.Open("ChaosGame.mp4", width, height, 30, VideoCodec.H264, (int)10E6);

                    //Depends how you want to see the video, (totalFrames / number of FPS in video) == number of seconds in the video
                    int totalFrames = 100;

                    for (int i = 0; i < totalFrames; i++)
                    {

                        file.WriteVideoFrame(chaos.Frame(1000));
                    }

                } catch (Exception e)
                {
                    Console.WriteLine("Something went horribly wrong: {0}", e);
                    Console.WriteLine("Do you wish to try again? [y,N]");

                    if(Console.ReadKey().Key == ConsoleKey.Y)
                    {
                        Main(args);
                    }
                }
            }
        }
    }
}
