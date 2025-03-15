FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App

COPY *.sln .
COPY ./PatrickBotman.Common/*.csproj ./PatrickBotman.Common/
COPY ./PatrickBotman.Bot/*.csproj ./PatrickBotman.Bot/
RUN dotnet nuget add source https://pkgs.dev.azure.com/tgbots/Telegram.Bot/_packaging/release/nuget/v3/index.json -n Telegram.Bot
RUN dotnet restore PatrickBotman.Bot

COPY . .
RUN dotnet publish PatrickBotman.Bot -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0

#FFMPEG Install
RUN apt update && apt install -y ffmpeg

# Hack to Allow Tesseract to work
RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
        libleptonica-dev \
        libtesseract-dev \
    && rm -rf /var/lib/apt/lists/*

RUN ln -s /usr/lib/x86_64-linux-gnu/libdl.so.2 /usr/lib/x86_64-linux-gnu/libdl.so

WORKDIR /app/x64

RUN ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5 /app/x64/libleptonica-1.82.0.so
#RUN ln -s /usr/lib/x86_64-linux-gnu/libtesseract.so.5 /app/x64/libtesseract50.so
RUN ln -s /usr/lib/x86_64-linux-gnu/libtesseract.so.4 /app/x64/libtesseract50.so

# Clean up to reduce image size
RUN apt-get clean && rm -rf /var/lib/apt/lists/*
# Hack to Allow Tesseract to work

WORKDIR /App

#Font install
COPY ./PatrickBotman.Bot/assets ./assets
#RUN mkdir -p /usr/share/fonts/truetype/
#RUN install -m644 impact.ttf /usr/share/fonts/truetype/
#RUN rm ./impact.ttf

# Build runtime image
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "PatrickBotman.Bot.dll"]
