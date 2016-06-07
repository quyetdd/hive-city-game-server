// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionCollection.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the IConnectionCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Framework
{
    public interface IConnectionCollection<TServer, in TClient>
    {
        void OnConnect(TServer serverPeer);

        void OnDisconnect(TServer serverPeer);

        void OnClientConnect(TClient clientPeer);

        void OnClientDisconnect(TClient clientPeer);

        TServer GetServerByType(int serverType, params object[] additional); // Pick out server
    }
}
