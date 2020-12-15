using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models.Account
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please Enter a Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please Enter a Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberPassword { get; set; }

        public bool ValidUser { get; set; }
    }
}
