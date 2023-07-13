## Patrick-Botman

Patrick-Botman is a Telegram bot that adds text to a random GIF from popular public databases such as Giphy and Tenor. This project was developed as a part of my study in .NET web development.

https://github.com/alshuriga/patrick-botman/assets/8162224/cd224f57-56ca-498f-a583-827af4f02920



## Features ✨

-   Send a private message to the bot to add text to a random GIF.
-   Use inline queries in any chat to add text to a random GIF.

## Technologies Used 🛠️

-   ASP.NET Core
-   [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) library for the backend
-   FFmpeg for rendering MP4 with added text

## Services 🌐

Two services were implemented for handling the GIF functionality:

1.  GiphyService (Default): Implements the `IGifService` interface and handles requests to the Giphy API.
2.  TenorService: Implements the `IGifService` interface and handles requests to the Tenor API.

By default, the GiphyService is used for fetching GIFs. If you want to use the TenorService instead, you can make the necessary changes in the code.

## To Do 🚀

-   Allow the bot to be added to group chats, where it can periodically respond to messages with GIFs (with the ability for users to set the time interval using commands).
