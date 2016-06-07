// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LambdaCalculator.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the LambdaCalculator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Calculators.Lambdas
{
    using System.Collections.Generic;

    using HiveCity.Server.Region.Models.Interfaces;

    public class LambdaCalculator : ILambda
    {
        public LambdaCalculator()
        {
            this.Functions = new List<IFunction>();
        }

        private List<IFunction> Functions { get; set; }

        public float Calculate(Environment env)
        {
            float saveValue = env.Value;
            float returnValue = 0;

            env.Value = 0;

            foreach (var function in this.Functions)
            {
                function.Calc(env);
            }

            returnValue = env.Value;
            env.Value = saveValue;

            return returnValue;
        }
    }
}
