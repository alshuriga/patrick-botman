
namespace PatrickBotman.Models.DTOs;

public record GifDTO (int GifId, string GifUrl);

public record GifVotesDTO(int ups, int downs);