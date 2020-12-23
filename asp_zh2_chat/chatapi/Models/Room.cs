using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace chatapi.Models
{
    public class Room
    {
        [Key]
        public string Uid { get; set; }

        [Required]
        [StringLength(50)]
        public string RoomName { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public IdentityUser Creator { get; set; }
    }
}
