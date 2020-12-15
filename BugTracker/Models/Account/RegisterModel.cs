using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Account
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "First Name is Required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Address is Required.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a Valid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is Required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is Required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password Confirmation is Required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
