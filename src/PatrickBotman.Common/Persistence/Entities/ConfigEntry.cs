using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrickBotman.Common.Persistence.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class ConfigEntry
    {
        [Key]
        public int Id { get; set; } 
        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
