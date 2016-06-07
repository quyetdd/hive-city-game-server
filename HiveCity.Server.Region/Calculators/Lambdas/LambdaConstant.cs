// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LambdaConstant.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the LambdaConstant type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Calculators.Lambdas
{
    using HiveCity.Server.Region.Models.Interfaces;

    public class LambdaConstant : ILambda
    {
        private readonly float value;

        public LambdaConstant(float value)
        {
            this.value = value;
        }

        public float Calculate(Environment env)
        {
            return this.value;
        }
    }
}
