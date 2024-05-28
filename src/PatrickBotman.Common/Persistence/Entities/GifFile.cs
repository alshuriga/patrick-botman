using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrickBotman.Common.Persistence.Entities
{
    public class GifFile : IDeletableEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public byte[] Data { get; set; } = null!;

        public bool Deleted { get; set; } = false;
    }
}
