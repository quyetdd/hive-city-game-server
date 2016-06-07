// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBackgroundThread.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Functionality for threads for physics, updates, etc.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Framework
{
    public interface IBackgroundThread
    {
        void Setup();

        void Run(object threadContext);

        void Stop();
    }
}
