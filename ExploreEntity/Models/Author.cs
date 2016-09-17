using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExploreEntity.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [MaxLength(length:20, ErrorMessage = "Pen Name can be no longer than 20 characters, friendo.")]
        public string PenName { get; set; }
        
        public DateTime DateOfBirth { get; set; }
    }
}