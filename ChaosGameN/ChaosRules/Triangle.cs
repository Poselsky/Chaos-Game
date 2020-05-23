﻿using ChaosGameN.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosGameN.ChaosRules
{
    class Triangle : IChaosRuleGenerator
    {
        public void GenerateChaos(int iterations, List<PointF> polygon, Bitmap whereToDraw)
        {
            Random generator = Program.randomGenerator;

            PointF randomPoint = new PointF(generator.Next(whereToDraw.Width), generator.Next(whereToDraw.Height));
            //Number of iterations depends of how smooth you want it to be

            int vertices = polygon.Count;
            for (int i = 0; i <= iterations; i++)
            {
                //Checking if the point is actually in polygon
                if (polygon.IsPointInPolygon(randomPoint))
                {

                    int randomIndex = generator.Next(vertices);
                    //Choosing the random point and random vertex, move it to the vertex and shorten the distance by half
                    randomPoint.X = (randomPoint.X + polygon[randomIndex].X) / 2;
                    randomPoint.Y = (randomPoint.Y + polygon[randomIndex].Y) / 2;

                    whereToDraw.SetPixel((int)randomPoint.X, (int)randomPoint.Y, Color.LightGreen);
                } else
                {
                    randomPoint = new PointF(generator.Next(whereToDraw.Width), generator.Next(whereToDraw.Height));
                }
            }
        }
    }
}
