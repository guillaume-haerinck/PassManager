using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

namespace ConsoleApp.SQLite
{
    public class AccessDb : DbContext
    {
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Password> Passwords { get; set; }
        public DbSet<UserName> Users { get; set; }
        public DbSet<UserPassword> UsersPasswords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=database.db");
        }
    }

    public class UserName
    {
        public int UserNameId { get; set; }
        public string Name { get; set; }

        public int UserPasswordForeignKey { get; set; }
    }

    public class UserPassword
    {
        public int UserPasswordId { get; set; }
        //Base64 string
        public string SaltAndHash { get; set; }
        public string Salt { get; set; }

        public int UserNameForeignKey { get; set; }
    }

    public class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; }

        public int PasswordForeignKey { get; set; }
        public int UserNameForeignKey { get; set; }
    }

    public class Password
    {
        public int PasswordId { get; set; }
        public string EncryptedPassword { get; set; }
        public string Key { get; set; }

        public int TagForeignKey { get; set; }
        public int UserNameForeignKey { get; set; }
    }

}
