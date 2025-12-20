using Microsoft.VisualBasic;
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

        public static InlineKeyboardMarkup CreateVotebanInlineKeyboard(int gifId, long chatId, int forCount = 0, int againstCount = 0)
        {
            var forString = forCount == 0 ? "" : $"[{forCount}]";
            var againstString = againstCount == 0 ? "" : $"[{againstCount}]";
            IEnumerable<InlineKeyboardButton> buttons = new[] {
                
                InlineKeyboardButton.WithCallbackData($"✅ {forString}", $"voteban:+:{gifId}:{chatId}"),
                InlineKeyboardButton.WithCallbackData($"❌ {againstString}", $"voteban:-:{gifId}:{chatId}"),
            };

            var replyMarkup = new InlineKeyboardMarkup(buttons);

            return replyMarkup;
        }
    }
}
