// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultEventHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the DefaultEventHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Server
{
    using HiveCity.Server.Photon.Application;

    public abstract class DefaultEventHandler : PhotonServerHandler
    {
        protected DefaultEventHandler(PhotonApplication application)
            : base(application)
        {
        }
    }
}
