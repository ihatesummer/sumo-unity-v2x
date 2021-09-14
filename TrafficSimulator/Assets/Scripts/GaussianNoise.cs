using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Simulation
{
    public class GaussianNoise
    {
        public double dist_err = 1.0;                  // vard (variance )
        public double angle_err = Math.Pow((Math.PI / 180) * 3, 2);
        public double NormalDistribution(double mean, double stdev)
        {
            // Box-Muller transform
            System.Random rng = new System.Random();
            double u1 = 1.0 - rng.NextDouble();         // unifrom (0,1]
            double u2 = 1.0 - rng.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            double randNormal = mean + stdev * randStdNormal;
            return randNormal;
        }
    }
}
