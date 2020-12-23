using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace chatapi.Models
{
    public class Message
    {
        [Key]
        public string Uid { get; set; }

        [Required]
        public string MessageText { get; set; }

        [Required]
        public string RoomUid { get; set; }

        public IdentityUser Sender { get; set; }
    }
}
