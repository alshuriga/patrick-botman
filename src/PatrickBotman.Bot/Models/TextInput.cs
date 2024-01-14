using System.Text;
using System.Text.RegularExpressions;

namespace PatrickBotman.Bot.Models;

public class TextInput
{
    public string FirstLine { get; set; } = string.Empty;
    public string SecondLine { get; set; } = string.Empty;

    private readonly int _maximumTextLength;

    public TextInput(string text, IConfiguration configuration)
    {
        _maximumTextLength = configuration.GetValue<int>("MaximumTextLength");

        PrepareText(text);
    }

    private void PrepareText(string text)
    {
        text = text.Substring(0, Math.Min(_maximumTextLength, text.Length)).Trim().ToUpper();

        var newLineChar = text.IndexOf('\n');

        if (newLineChar != -1 && text.IndexOf('\n', newLineChar + 1) != -1)
        {
            text = text.Substring(0, text.IndexOf('\n', newLineChar + 1));
        }

        text = text.Replace("\n", " ");

        int separationIndex = 0;

        for (int i = 0; i < text.Length; i++)
        {
            if (char.IsSeparator(text[i]))
            {
                if (Math.Abs(text.Length / 2 - i) < Math.Abs(text.Length / 2 - separationIndex))
                    separationIndex = i;
            }
        }

        if (separationIndex > 0)
        {
            FirstLine = text.Substring(0, separationIndex);
            SecondLine = text.Substring(separationIndex, text.Length - FirstLine.Length);
        }
        else
        {
            FirstLine = text;
            SecondLine = string.Empty;
        }

        FirstLine = EscapeSpecial(FirstLine);
        SecondLine = EscapeSpecial(SecondLine);

    }

    private string EscapeSpecial(string inputText)
    {
        var str = new StringBuilder(inputText);

        str = str
            .Replace("\\", "\\\\\\\\")
            .Replace("'", "'\\\\\\\''")
            .Replace("%", "\\\\\\%")
            .Replace(":", "\\\\\\:")
            .Replace("\"", "\\\\\\\"");

        return str.ToString();
    }
}