namespace HiveCity.Server.Sub.Common.Data.NHibernate
{
    public class UserProfile
    {
        public virtual int Id { get; set; }
        public virtual User UserId { get; set; }
        public virtual int CharacterSlots { get; set; }
    }
}
