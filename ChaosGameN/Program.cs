using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

//Accord libraries are great, there are many different libraries for machine learning, image editing, math calculations and etc... 
//Right now ffmpeg provides us creating video and writing in its self bitmaps.
using Accord.Video.FFMPEG;

namespace ChaosGameN
{
    class Program
    {
        static void Main(string[] args)
        {

            //Creating instance
            Chaos chaos = new Chaos(new int[] { 1024, 862 }, Chaos.Rules.NotLocalVertices, 5);

            //Class from Accord - you might remember the old structure from AForge Library
            VideoFileWriter file = new VideoFileWriter();
            //Outputing into bin folder
            
            //Setting width and height based on instance of the class, should be roughly the same
            //Setting good framerate - 30fps should be sufficient, H264 is standard, and good bitrate - otherwise the video will be blurry
            file.Open("ChaosGame.mp4", chaos.dimensions[0], chaos.dimensions[1], 30, VideoCodec.H264, (int)10E6);

            //Depends how you want to see the video, (totalFrames / number of FPS in video) == number of seconds in the video
            int totalFrames = 100;
            
            for(int i =0; i < totalFrames; i++)
            {

                file.WriteVideoFrame(chaos.Frame(10000));
            }

            //Closing the stream. Never forget to close the stream. NEVER
            file.Close();


        }
    }

    class Chaos
    {
        // I recommend to watch the explanation of the Chaos game here: https://www.youtube.com/watch?v=droTYSmSGHg

        //Size of each picture
        public int[] dimensions { get; private set; }

        //In what shape wil be chaos game created (triangle, square, etc.)
        public int vertices { get; private set; }

        public Bitmap lastFrame { get; private set; }

        private Rules rule;

        private bool background = false;

        private List<PointF> points;

        //There are several rules in chaos game
        public enum Rules
        {
            Triangle,
            OppositeVertex,
            NotOppositeVertex,
            NotSameVertex,
            NotNextVertex,
            NotLocalVertices,
            OddEvenIndex,
            NotFromSameHalfVertices
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dimensions">Dimensions of picture</param>
        /// <param name="rule">Which rule to use for game</param>
        /// <param name="vertices">What kind of shape your game should be in</param>

        public Chaos(int[] dimensions, Rules rule = Rules.Triangle, int vertices = 3)
        {
            this.dimensions = dimensions;
            this.vertices = vertices;
            this.rule = rule;

            //Creating 
            lastFrame = new Bitmap(dimensions[0], dimensions[1]);
        }

        //Method to create the shape of chaos game
        //Idea is to have a circle and position points around the circle proportionally
        private List<PointF> GeneratePolygon()
        {
            List<PointF> points = new List<PointF>();

            //The middle of picture which is the center of the circle
            double[] mid = new double[] { dimensions[0] / 2, dimensions[1] / 2 };

            double angle = 0;

            for (int i = 0; i < vertices; i++)
            {
                //Calculations to move along the circle, double mid is 
                float x = (float)(mid[0] + mid[0] * Math.Cos(angle));
                float y = (float)(mid[1] + mid[1] * Math.Sin(angle));
                points.Add(new PointF(x, y));

                //Proportional angle - in rad the full angle of circle is 2 * PI - divide it by the number of vertices and you have the angle of each vertex
                angle += 2 * Math.PI /vertices;
            }

            return points;
        }

        /// <summary>
        /// Creates one single frame for chaos game
        /// </summary>
        /// <param name="iterations">Smoothnes of creating picture</param>
        /// <returns>Bitmap</returns>
        public Bitmap Frame(int iterations = 1000)
        {
            //Creating background if doesn't exist
            if (!background)
            {
                //Creating background color of images
                using (Graphics s = Graphics.FromImage(lastFrame))
                {
                    s.Clear(Color.Black);
                }

                points = GeneratePolygon();

                background = true;
            }


            Random generator = new Random();
            PointF randomPoint = new PointF(generator.Next(dimensions[0]), generator.Next(dimensions[1]));
            //Number of iterations depends of how smooth you want it to be

            int index = 0;

            for (int i=0; i <= iterations; i++)
            {
                //Checking if the point is actually in polygon
                if (IsPointInPolygon(points, randomPoint))
                {
                    if(this.rule == Rules.Triangle)
                    {
                        int randomIndex = generator.Next(vertices);
                        //Choosing the random point and random vertex, move it to the vertex and shorten the distance by half
                        randomPoint.X = (randomPoint.X + points[randomIndex].X) / 2;
                        randomPoint.Y = (randomPoint.Y + points[randomIndex].Y) / 2;


                    } else if(this.rule == Rules.OppositeVertex)
                    {

                        int randomIndex = generator.Next(vertices);
                        if (i % 2 == 0)
                        {
                            //Choosing the random point and random vertex, move it to the vertex and shorten the distance by half
                            index = randomIndex;
                            randomPoint.X = (randomPoint.X + points[randomIndex].X) / 2;
                            randomPoint.Y = (randomPoint.Y + points[randomIndex].Y) / 2;

                        } else
                        {
                            int next = (vertices / 2 + index) % vertices;
                            //Choosing the oppposing vertex after each other iteration
                            randomPoint.X = (randomPoint.X + points[next].X) / 2;
                            randomPoint.Y = (randomPoint.Y + points[next].Y) / 2;
                        }
                    }else if(this.rule == Rules.NotOppositeVertex)
                    {

                        int randomIndex = generator.Next(vertices);
                        if (i % 2 == 0)
                        {
                            //remembering last index
                            index = randomIndex;
                        }
                        else
                        {
                            //cannot choose  opposite vertex
                            int next = (vertices / 2 + index) % vertices;
                            while (randomIndex == next)
                            {
                                randomIndex = generator.Next(vertices);
                            }
                        }

                        randomPoint.X = (randomPoint.X + points[randomIndex].X) / 2;
                        randomPoint.Y = (randomPoint.Y + points[randomIndex].Y) / 2;
                    } else if(this.rule == Rules.NotSameVertex)
                    {
                        int randomIndex = generator.Next(vertices);
                        if (i % 2 == 0)
                        {
                            //remembering last index
                            index = randomIndex;
                        }
                        else
                        {
                            //cannot choose same vertex int this iteration
                            while (randomIndex == index)
                            {
                                randomIndex = generator.Next(vertices);
                            }
                        }

                        randomPoint.X = (randomPoint.X + points[randomIndex].X) / 2;
                        randomPoint.Y = (randomPoint.Y + points[randomIndex].Y) / 2;
                    } else if(this.rule == Rules.NotNextVertex)
                    {
                        int randomIndex = generator.Next(vertices);

                        int next = (index -1 ) % vertices;
                        if (i % 2 == 0)
                        {
                            //remembering last index
                            index = randomIndex;
                        }
                        else
                        {
                            //cannot choose the next vertex from previously chosen vertex
                            while (randomIndex == next)
                            {
                                randomIndex = generator.Next(vertices);
                            }
                        }

                        randomPoint.X = (randomPoint.X + points[randomIndex].X) /2;
                        randomPoint.Y = (randomPoint.Y + points[randomIndex].Y) /2;
                    }else if(this.rule == Rules.NotLocalVertices)
                    {
                        int randomIndex = generator.Next(vertices);

                        int nextLeft = (index - 1) % vertices;
                        int nextRight = (index + 1) % vertices;
                        if (i % 2 == 0)
                        {
                            //remembering last index
                            index = randomIndex;
                        }
                        else
                        {
                            //cannot choose the next right and next left (local vertices) from the last selected vertex
                            while (randomIndex == nextLeft || randomIndex == nextRight)
                            {
                                randomIndex = generator.Next(vertices);
                            }
                        }

                        randomPoint.X = (randomPoint.X + points[randomIndex].X) / 2;
                        randomPoint.Y = (randomPoint.Y + points[randomIndex].Y) / 2;
                    }else if(this.rule == Rules.OddEvenIndex)
                    {
                        int randomIndex = generator.Next(vertices);
                        if (i % 2 == 0)
                        {
                            //Choosing odd number
                            while (randomIndex % 2 == 0)
                            {
                                randomIndex = generator.Next(vertices);
                            }
                        }
                        else
                        {
                            //Choosing even number
                            while (randomIndex % 2 == 1)
                            {
                                randomIndex = generator.Next(vertices);
                            }
                        }

                        randomPoint.X = (randomPoint.X + points[randomIndex].X) / 2;
                        randomPoint.Y = (randomPoint.Y + points[randomIndex].Y) / 2;
                    }else if(this.rule == Rules.NotFromSameHalfVertices)
                    {
                        int randomIndex = generator.Next(vertices);

                        if (i % 2 == 1)
                        {
                            while ((index + vertices / 2) % vertices > randomIndex && (vertices/2 - index) % vertices < randomIndex)
                            {
                                randomIndex = generator.Next(vertices);
                            }
                        } else
                        {
                            index = randomIndex;
                        }

                        randomPoint.X = (randomPoint.X + points[randomIndex].X) / 2;
                        randomPoint.Y = (randomPoint.Y + points[randomIndex].Y) / 2;
                    }
                    //Drawing it to the bitmap
                    lastFrame.SetPixel((int)randomPoint.X, (int)randomPoint.Y, Color.LightGreen);

                }
            }


            return lastFrame;
        }


        /// <summary>
        /// Determines if the given point is inside the polygon
        /// </summary>
        /// <param name="polygon">The vertices of polygon</param>
        /// <param name="testPoint">The given point</param>
        /// <returns>true if the point is inside the polygon; otherwise, false</returns>
        private bool IsPointInPolygon(List<PointF> polygon, PointF point)
        {
            bool result = false;
            PointF a = polygon.Last();
            foreach (PointF b in polygon)
            {
                if ((b.X == point.X) && (b.Y == point.Y))
                {
                    return true;
                }

                if ((b.Y == a.Y) && (point.Y == a.Y) && (a.X <= point.X) && (point.X <= b.X))
                {
                    return true;
                }

                if ((b.Y < point.Y) && (a.Y >= point.Y) || (a.Y < point.Y) && (b.Y >= point.Y))
                {
                    if (b.X + (point.Y - b.Y) / (a.Y - b.Y) * (a.X - b.X) <= point.X)
                    {
                        result = !result;
                    }
                }
                a = b;
            }

            return result;
        }
    }
}
