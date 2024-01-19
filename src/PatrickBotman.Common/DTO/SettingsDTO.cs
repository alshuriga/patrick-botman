using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrickBotman.Common.DTO
{
    public record SettingsDTO
    {
        public int LocalGifProbability { get; set; }
        public int MaximumTextLength { get; set; }
        public IEnumerable<string> AdminID { get; set; } = null!;
    }

    public record UpdateSettingsDTO
    {
        public int LocalGifProbability { get; set; }
        public int MaximumTextLength { get; set; }
        public IEnumerable<string> AdminID { get; set; } = null!;
    }


}
