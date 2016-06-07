// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILambda.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ILambda type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Interfaces
{
    using HiveCity.Server.Region.Calculators;

    // Provides data to function
    // Dynamically
    public interface ILambda
    {
        float Calculate(Environment env);
    }
}
