using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace chatapi.ViewModels
{
    public class RoomViewModel
    {
        [Key]
        public string Uid { get; set; }

        [Required]
        public string RoomName { get; set; }

        public int MessageCount { get; set; }
    }
}
