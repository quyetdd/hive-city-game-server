// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FunctionAdd.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the FunctionAdd type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Calculators.Functions
{
    using HiveCity.Server.Region.Models;
    using HiveCity.Server.Region.Models.Interfaces;

    public class FunctionAdd : IFunction
    {
        private readonly ILambda lambda;

        public FunctionAdd(IStat stat, int order, CObject owner, ILambda lambda)
        {
            this.Stat = stat;
            this.Order = order;
            this.Owner = owner;
            this.lambda = lambda;
        }

        public IStat Stat { get; private set; }

        public int Order { get; private set; }

        public CObject Owner { get; set; }

        public ICondition Condition { get; set; }

        public void Calc(Environment environment)
        {
            if (this.Condition == null || this.Condition.Test(environment))
            {
                environment.Value += this.lambda.Calculate(environment);
            }
        }

    }
}
