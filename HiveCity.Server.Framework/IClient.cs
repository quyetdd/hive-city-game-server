// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClient.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the IClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Framework
{
    using System;

    public interface IClient : IPeer
    {
        Guid PeerId { get; }

        T ClientData<T>() where T : class, IClientData;
    }
}
