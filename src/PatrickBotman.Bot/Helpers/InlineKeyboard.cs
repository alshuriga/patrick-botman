using Telegram.Bot.Types.ReplyMarkups;

namespace PatrickBotman.Bot.Helpers
{
    public static class InlineKeyboard
    {
        public static InlineKeyboardMarkup CreateVotingInlineKeyboard(int gifId)
        {
            IEnumerable<InlineKeyboardButton> buttons = new[] {
                InlineKeyboardButton.WithCallbackData($"🗑️", $"blacklist {gifId}"),
            };

            var replyMarkup = new InlineKeyboardMarkup(buttons);

            return replyMarkup;
        }
    }
}
