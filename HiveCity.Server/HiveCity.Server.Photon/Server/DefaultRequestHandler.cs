// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRequestHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the DefaultRequestHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Server
{
    using HiveCity.Server.Photon.Application;

    public abstract class DefaultRequestHandler : PhotonServerHandler
    {
        protected DefaultRequestHandler(PhotonApplication application)
            : base(application)
        {
        }
    }
}
