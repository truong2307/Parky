using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Models.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
