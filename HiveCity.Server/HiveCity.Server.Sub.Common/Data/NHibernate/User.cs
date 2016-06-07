﻿using System;

namespace HiveCity.Server.Sub.Common.Data.NHibernate
{
    public class User
    {
        public virtual int Id { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }
        public virtual string Salt { get; set; }
        public virtual string Email { get; set; }
        public virtual string Algorithm { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Updated { get; set; }
    }
}
