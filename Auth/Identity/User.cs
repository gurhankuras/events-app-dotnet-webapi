using System.ComponentModel.DataAnnotations;

namespace mongoidentity
{
    public class User
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }

        //public string Image { get; set; }
    }
}
