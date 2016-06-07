﻿using System;

namespace HiveCity.Server.Sub.Common.Math
{
    public static class RandomExtensions
    {
        public static double NextGaussian(this Random r, double mu = 0, double sigma = 1)
        {
            var u1 = r.NextDouble();
            var u2 = r.NextDouble();

            var randStdNormal = System.Math.Sqrt(-2.0*System.Math.Log(u1))*System.Math.Sin(2.0*System.Math.PI*u2);

            var randNormal = mu + sigma*randStdNormal;

            return randNormal;
        }
    }
}
