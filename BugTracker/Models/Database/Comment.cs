using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public ApplicationUser Commenter { get; set; }

        [Required]
        [StringLength(150)]
        public string Message { get; set; }

        public DateTime CreateDate { get; set; }

        public Ticket Ticket { get; set; }
    }
}
