namespace PatrickBotman.Common.DTO;

public record GifDTO(int Id, string Url);

public record GifFileInfo(int Id, long SizeKb);

public record ChatDTO(long Id);

public record GifFileDTO(int Id, string FileName, byte[] Data);

public record CreateGifFileDTO(string FileName, byte[] Data);

