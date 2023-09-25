using System.Text.RegularExpressions;

namespace PatrickBotman.Models;

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
            text = text.Substring(0, text.IndexOf('\n', newLineChar));
        }

        int separationIndex = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (char.IsSeparator(text[i]))
            {
                if (Math.Abs(text.Length / 2 - i) < Math.Abs(text.Length / 2 - separationIndex))
                    separationIndex = i;
            }
        }
        if (text[separationIndex] != '\n')
            text = text.Replace("\n", string.Empty);
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

        var regex = new Regex("[}{:;\"'`]");

        FirstLine = regex.Replace(FirstLine, string.Empty);
        SecondLine = regex.Replace(SecondLine, string.Empty);
    }
}