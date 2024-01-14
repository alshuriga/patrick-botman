namespace PatrickBotman.Common.DTO;

public record OnlineGifDTO(int Id, string Url);

public record LocalGifDTO(int Id, long SizeKb);

public record ChatDTO(long Id);

public record GifFileDTO(string FileName, byte[] Data);

public record RandomGifFileDTO(int Id, string FileName, byte[] Data);



