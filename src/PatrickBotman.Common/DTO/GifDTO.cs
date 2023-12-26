namespace PatrickBotman.Common.DTO;

public record GifDTO(int Id, string Url);

public record ChatDTO(long Id);

public record GifFileDTO(string fileName, byte[] data);


