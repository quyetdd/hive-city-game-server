namespace HiveCity.Server.Proxy.Common.MessageObjects
{
    public enum ChatType
    {
        // Region - List of players
        Local, // Region Server - 200 m radius to hear
        Region, // Region Server - Shout/Yell

        // Global
        Company, // Chat Server (Guild)
        Squad, // Chat Server (Group)
        General, // Chat Server
        Trade, // Chat Server
        PrivateMessage // Chat Server
    }
}
