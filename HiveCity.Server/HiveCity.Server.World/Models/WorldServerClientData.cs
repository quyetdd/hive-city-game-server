using HiveCity.Server.Framework;
using HiveCity.Server.Photon.Server;

namespace HiveCity.Server.World.Models
{
    public class WorldServerClientData : IClientData
    {
        public PhotonServerPeer RegionServer { get; set; }
        public PhotonServerPeer ProxyServer { get; set; }
    }
}
