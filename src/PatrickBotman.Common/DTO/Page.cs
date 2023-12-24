using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrickBotman.Common.DTO
{
    public class Page<T> 
    {
        public ICollection<T> Items { get; set; } = null!;
        public int CollectionSize { get; set; }
    }
}
