// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LambdaRandom.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the LambdaRandom type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Calculators.Lambdas
{
    using System;

    using HiveCity.Server.Region.Models.Interfaces;
    using HiveCity.Server.Sub.Common.Math;

    using Environment = HiveCity.Server.Region.Calculators.Environment;

    public class LambdaRandom : ILambda
    {
        private readonly Random rand;
        private readonly ILambda max;
        private readonly bool linear; // next value out of list

        public LambdaRandom(ILambda max, bool linear)
        {
            this.max = max;
            this.linear = linear;
            this.rand = new Random();
        }
        
        public float Calculate(Environment env)
        {
            if (this.linear)
            {
                return this.max.Calculate(env) * (float)this.rand.NextDouble();
            }

            return this.max.Calculate(env) * (float)this.rand.NextGaussian();
        }
    }
}
