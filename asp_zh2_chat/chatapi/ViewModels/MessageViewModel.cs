using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace chatapi.ViewModels
{
    public class MessageViewModel
    {
        [Key]
        public string Uid { get; set; }

        [Required]
        public string MessageText { get; set; }

        [Required]
        public string RoomUid { get; set; }
    }
}
