using HiveCity.Server.Proxy.Common.MessageObjects;

namespace HiveCity.Server.Sub.Common.Data.NHibernate
{
    public class PlayerCharacter
    {
        public virtual int Id { get; set; }
        public virtual User UserId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Class { get; set; }
        public virtual string Chapter { get; set; }
        public virtual int Level { get; set; }
        public virtual string Stats { get; set; } // varchar 2048 - CLOB Character Large Object
        public virtual string Position { get; set; } // varchar 1024
        
        public virtual CharacterListItem BuildCharacterListItem()
        {
            return new CharacterListItem()
            {
                Id = Id,
                Class = Class,
                Name = Name,
                Level = Level,
                Chapter = Chapter
            };
        }
    }
}
