using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HiveCity.Server.Photon.Server;

namespace HiveCity.Server.Sub.Common.Data.ClientData
{
    public class ServerData : Framework.IClientData
    {
        public PhotonServerPeer ServerPeer { get; set; }
    }
}
