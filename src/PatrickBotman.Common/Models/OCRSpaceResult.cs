using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatrickBotman.Common.Models
{

    public class OCRSpaceResult
    {
        public List<ParsedResult> ParsedResults { get; set; } = null!;
        public int OCRExitCode { get; set; }
        public bool IsErroredOnProcessing { get; set; }
        public string ProcessingTimeInMilliseconds { get; set; } = null!;
        public string SearchablePDFURL { get; set; } = null!;
    }

    public class ParsedResult
    {
        public TextOverlay TextOverlay { get; set; } = null!;
        public string TextOrientation { get; set; } = null!;
        public int FileParseExitCode { get; set; }
        public string ParsedText { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
        public string ErrorDetails { get; set; } = null!;
    }

    public class TextOverlay
    {
        public List<object> Lines { get; set; } = null!;
        public bool HasOverlay { get; set; }
        public string Message { get; set; } = null!;
    }


}
