using Telegram.Bot.Types.ReplyMarkups;

namespace patrick_botman.Helpers
{
    public static class InlineKeyboard
    {
        public static InlineKeyboardMarkup CreateVotingInlineKeyboard(int gifId, int rating)
        {
            IEnumerable<InlineKeyboardButton> buttons = new[] {
                InlineKeyboardButton.WithCallbackData($"👍", $"up {gifId}"),
                InlineKeyboardButton.WithCallbackData($"{rating}", $"get-votes {gifId}"),
                InlineKeyboardButton.WithCallbackData($"👎",  $"down {gifId}"),
            };

            var replyMarkup = new InlineKeyboardMarkup(buttons);

            return replyMarkup;
        }
    }
}
