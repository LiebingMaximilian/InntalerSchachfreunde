using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InntalerSchachfreunde.Entities
{
    public class KeyValue
    {
        [Key]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
