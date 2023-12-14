using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace user.Data.Entities
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
