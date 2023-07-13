## Patrick-Botman

Patrick-Botman is a Telegram bot that adds text to a random GIF from popular public databases such as Giphy and Tenor. This project was developed as a part of my study in .NET web development.

## Features

-   Send a private message to the bot to add text to a random GIF.
-   Use inline queries in any chat to add text to a random GIF.

## Technologies Used

-   ASP.NET Core
-   [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) library for the backend
-   FFmpeg for rendering MP4 with added text

## Services

Two services were implemented for handling the GIF functionality:

1.  GiphyService: Implements the `IGifService` interface and handles requests to the Giphy API.
2.  TenorService: Implements the `IGifService` interface and handles requests to the Tenor API.

## Getting Started

To use the bot, please follow these steps:

1.  Clone this repository to your local machine.
    
2.  Run a Docker container with the following environmental variables configured:
    
    -   `BotConfiguration.BotToken`: Your Telegram bot token.
    -   `BotConfiguration.HostAddress`: The webhook address (your app address).
    -   `GiphyConfiguration.ApiToken`: Your Giphy API token.
3.  You can check and play with the bot on Telegram by visiting [https://t.me/patrickbotman_bot](https://t.me/patrickbotman_bot).
