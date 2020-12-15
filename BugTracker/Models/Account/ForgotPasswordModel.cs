using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Account
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Please enter the Email Address associated with your Account.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Please enter a Valid Email Address.")]
        public string Email { get; set; }

        public bool ValidEmail { get; set; }
    }
}
