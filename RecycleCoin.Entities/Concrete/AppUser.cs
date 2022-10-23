using Microsoft.AspNet.Identity.EntityFramework;

namespace RecycleCoin.Entities.Concrete
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Lastname { get; set; }

    }
}
