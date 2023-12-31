namespace PatrickBotman.Common.DTO;

public record GifDTO(int Id, string Url);

public record ChatDTO(long Id);

public record GifFileDTO(string FileName, byte[] Data);

public record RandomGifFileDTO(int Id, string FileName, byte[] Data);



