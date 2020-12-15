using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Account
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Please enter a new Password.")]
        [DataType(DataType.Password, ErrorMessage = "Password Format was Incorrect.")]
        public string Password { get; set; }

        [DataType(DataType.Password, ErrorMessage = "Password Format was Incorrect.")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }
    }
}
