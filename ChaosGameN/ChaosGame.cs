using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosGameN
{
    class ChaosGame
    {
        // I recommend to watch the explanation of the Chaos game here: https://www.youtube.com/watch?v=droTYSmSGHg

        //Size of each picture
        public Tuple<int,int> dimensions { get; private set; }

        //In what shape wil be chaos game created (triangle, square, etc.)
        public int vertices { get; private set; }

        public Bitmap lastFrame { get; private set; }

        private bool background = false;

        private List<PointF> points;
        private IChaosRuleGenerator chaosRuleGenerator;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dimensions">Dimensions of picture</param>
        /// <param name="rule">Which rule to use for game</param>
        /// <param name="vertices">What kind of shape your game should be in</param>

        public ChaosGame(Tuple<int,int> dimensions, int vertices, IChaosRuleGenerator chaosRuleGenerator)
        {
            this.dimensions = dimensions;
            this.vertices = vertices <= 0 ? 3 : vertices;
            this.chaosRuleGenerator = chaosRuleGenerator;

            //Creating 
            lastFrame = new Bitmap(dimensions.Item1, dimensions.Item2);
        }

        //Method to create the shape of chaos game
        //Idea is to have a circle and position points around the circle proportionally
        private List<PointF> GeneratePolygon()
        {
            List<PointF> points = new List<PointF>();

            //The middle of picture which is the center of the circle
            double[] mid = new double[] { dimensions.Item1 / 2, dimensions.Item2 / 2 };

            double angle = 0;

            for (int i = 0; i < vertices; i++)
            {
                //Calculations to move along the circle, double mid is 
                float x = (float)(mid[0] + mid[0] * Math.Cos(angle));
                float y = (float)(mid[1] + mid[1] * Math.Sin(angle));
                points.Add(new PointF(x, y));

                //Proportional angle - in rad the full angle of circle is 2 * PI - divide it by the number of vertices and you have the angle of each vertex
                angle += 2 * Math.PI / vertices;
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

            chaosRuleGenerator.GenerateChaos(iterations, points, lastFrame);

            return lastFrame;
        }


        /// <summary>
        /// Determines if the given point is inside the polygon
        /// </summary>
        /// <param name="polygon">The vertices of polygon</param>
        /// <param name="testPoint">The given point</param>
        /// <returns>true if the point is inside the polygon; otherwise, false</returns>
        
    }
}
