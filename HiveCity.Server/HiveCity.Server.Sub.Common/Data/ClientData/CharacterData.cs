using System;

namespace HiveCity.Server.Sub.Common.Data.ClientData
{
    public class CharacterData : Framework.IClientData
    {
        public int? CharacterId { get; set; }
        public int? UserId { get; set; }
    }
}
