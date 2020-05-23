using ChaosGameN.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChaosGameN.ChaosRules
{
    class NotFromSameHalfVertices : IChaosRuleGenerator
    {
        public void GenerateChaos(int iterations, List<PointF> polygon, Bitmap whereToDraw)
        {
            Random generator = Program.randomGenerator;

            PointF randomPoint = new PointF(generator.Next(whereToDraw.Width), generator.Next(whereToDraw.Height));
            //Number of iterations depends of how smooth you want it to be

            int vertices = polygon.Count;
            int index = 0;
            for (int i = 0; i <= iterations; i++)
            {
                if (polygon.IsPointInPolygon(randomPoint))
                {
                    int randomIndex = generator.Next(vertices);

                    if (i % 2 == 1)
                    {
                        while ((index + vertices / 2) % vertices > randomIndex && (vertices / 2 - index) % vertices < randomIndex)
                        {
                            randomIndex = generator.Next(vertices);
                        }
                    }
                    else
                    {
                        index = randomIndex;
                    }

                    randomPoint.X = (randomPoint.X + polygon[randomIndex].X) / 2;
                    randomPoint.Y = (randomPoint.Y + polygon[randomIndex].Y) / 2;

                    whereToDraw.SetPixel((int)randomPoint.X, (int)randomPoint.Y, Color.LightGreen);
                }
                else
                {
                    randomPoint = new PointF(generator.Next(whereToDraw.Width), generator.Next(whereToDraw.Height));
                }
            }
        }
    }
}
