namespace PatrickBotman.Common.Interfaces
{
    public interface IImageToTextService
    {
        string? GetText(byte[] image);
    }
}