namespace PatrickBotman.Helpers;

public static class TextFormatter
{
    public static List<string> Format(string inputText, int charsPerLine)
    {
        if (inputText.Length <= charsPerLine)
            return new List<string>() { inputText };

        string text = inputText;
        int? separatorIndex = null;
        List<string> lines = new();

        for (int i = 0; i < text.Length; i++)
        {
            if (text.Length <= charsPerLine)
            {
                lines.Add(text);
                break;
            }
            if (Char.IsWhiteSpace(text.ElementAt(i)))
            {
                separatorIndex = i;
            }
            else if (i == charsPerLine - 1)
            {
                var line = string.Empty;
                line = text.Substring(0, separatorIndex ?? i);
                text = text.Substring((separatorIndex ?? i) + 1);
                lines.Add(line);
                i = 0;
                separatorIndex = null;
            }
        }
        if (lines.Count > 1)
        {
            var maxLength = lines.Max(l => l.Length);
            lines = lines.Select(l =>
            {
                if (l.Length < maxLength)
                    return $"{new String(' ', (maxLength-l.Length))}{l}";
                return l;
            }).ToList();
        }
        return lines;
    }
}