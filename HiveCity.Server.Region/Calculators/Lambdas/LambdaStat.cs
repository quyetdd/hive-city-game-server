// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LambdaStat.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the LambdaStat type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Calculators.Lambdas
{
    using HiveCity.Server.Region.Models.Interfaces;

    public class LambdaStat : ILambda
    {
        private readonly IStat stat;

        private readonly bool useTarget;

        public LambdaStat(IStat stat, bool useTarget = false)
        {
            this.stat = stat;
            this.useTarget = useTarget;
        }

        public float Calculate(Environment env)
        {
            if (this.useTarget && env.Target == null)
            {
                return 1;
            }

            if (!this.useTarget && env.Target == null)
            {
                return 1;
            }

            if (this.useTarget)
            {
                return env.Target.Stats.GetStat(this.stat);
            }

            return env.Character.Stats.GetStat(this.stat);
        }
    }
}