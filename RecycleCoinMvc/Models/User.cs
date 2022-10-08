using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecycleCoinMvc.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }


        public User Login(string username, string password)
        {
            if (username == "mstf" && password == "mm123")
            {
                Id = 1;
                Name = "Mustafa";
                Lastname = "Uçar";
                Username = username;
                Email = "mm@gmail.com";

            }
            else
            {
                Id = 0;
            }

            return this;
        }
    }
}