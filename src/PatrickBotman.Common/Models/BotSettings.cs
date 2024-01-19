using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrickBotman.Common.Models
{
    public class BotSettings
    {
        public int LocalGifProbability { get; set; }
        public int MaximumTextLength { get; set; }
        public IEnumerable<string> AdminID { get; set; } = null!;
    }
}
