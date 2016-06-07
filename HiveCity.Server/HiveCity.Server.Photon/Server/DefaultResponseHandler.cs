// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultResponseHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the DefaultResponseHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Server
{
    using HiveCity.Server.Photon.Application;

    public abstract class DefaultResponseHandler : PhotonServerHandler
    {
        protected DefaultResponseHandler(PhotonApplication application)
            : base(application)
        {
        }
    }
}
