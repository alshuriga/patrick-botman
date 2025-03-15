namespace PatrickBotman.Common.Services
{
    public interface IImageToTextService
    {
        string GetText(byte[] image);
    }
}