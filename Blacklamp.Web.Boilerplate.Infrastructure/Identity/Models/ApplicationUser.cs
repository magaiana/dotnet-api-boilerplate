using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Blacklamp.Web.Boilerplate.Infrastructure.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string EmailAddress { get; set; }
        public bool IsEnabled { get; set; }

        [IgnoreDataMember]
        public string FullName => $"{FirstName} {LastName}";
    }
}