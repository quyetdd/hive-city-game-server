// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoveSpeed.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the MoveSpeed type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Stats
{
    using HiveCity.Server.Region.Models.Interfaces;

    public class MoveSpeed : IStat
    {
        public string Name 
        {
            get { return "Move Speed"; }
        }

        public bool IsBaseStat
        {
            get { return true; }
        }

        public bool IsNonZero
        {
            get
            {
                return false;
            }
        }

        public float BaseValue
        {
            get
            {
                return 6.0f;
            }
            set
            {
            }
        } // hard coded
    }
}
