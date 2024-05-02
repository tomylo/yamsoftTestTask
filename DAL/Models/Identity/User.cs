using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DAL.Models.Identity
{
    [Table("aspnetusers")]
    public class User : IdentityUser<long>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime? DOB { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

    }
}