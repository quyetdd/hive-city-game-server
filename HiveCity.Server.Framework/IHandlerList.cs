// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHandlerList.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the IHandlerList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Framework
{
    public interface IHandlerList<T>
    {
        bool RegisterHandler(IHandler<T> handler);

        bool HandleMessage(IMessage message, T peer);
    }
}
