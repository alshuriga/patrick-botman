namespace PatrickBotman.Models;

public class TextInput {
    public string FirstLine {get; set;}
    public string SecondLine { get; set; }

    public TextInput(string firstLine, string secondLine = "")
    {
        FirstLine = firstLine;
        SecondLine = secondLine;
    }
}