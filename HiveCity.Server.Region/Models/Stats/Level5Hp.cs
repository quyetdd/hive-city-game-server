// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Level5Hp.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the Level5Hp type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Stats
{
    using System.Collections.Generic;

    using HiveCity.Server.Region.Calculators.Functions;
    using HiveCity.Server.Region.Calculators.Lambdas;
    using HiveCity.Server.Region.Models.Interfaces;

    public abstract class Level5Hp : IDerivedStat
    {
        private readonly List<IFunction> functionList;

        protected Level5Hp()
        {
            this.functionList = new List<IFunction>() 
            {
                new FunctionAdd(this, 0, null, new LambdaConstant(5)),
                new FunctionMultiply(this, 1, null, new LambdaStat(new Level())),
            };
        }

        public string Name
        {
            get { return "HP"; }
        }

        public bool IsBaseStat
        {
            get { return false; }
        }

        public bool IsNonZero
        {
            get { return false; }
        }

        public float BaseValue
        {
            get { return 0; }
            set { }
        }

        public IEnumerable<IFunction> Functions
        {
            get
            {
                return this.functionList;
            }
        }
    }
}
