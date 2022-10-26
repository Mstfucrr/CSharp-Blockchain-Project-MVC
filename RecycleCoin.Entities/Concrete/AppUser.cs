using Microsoft.AspNet.Identity.EntityFramework;

namespace RecycleCoin.Entities.Concrete
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public int Carbon { get; set; }
         
    }
}
