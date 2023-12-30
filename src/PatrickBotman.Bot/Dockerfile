FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /App

COPY *.sln .
COPY ./PatrickBotman.Common/*.csproj ./PatrickBotman.Common/
COPY ./PatrickBotman.Bot/*.csproj ./PatrickBotman.Bot/

RUN dotnet restore PatrickBotman.Bot

COPY . .
RUN dotnet publish PatrickBotman.Bot -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0

#FFMPEG Install
RUN apt update && apt install -y ffmpeg





WORKDIR /App

#Font install
COPY ./PatrickBotman.Bot/assets ./assets
#RUN mkdir -p /usr/share/fonts/truetype/
#RUN install -m644 impact.ttf /usr/share/fonts/truetype/
#RUN rm ./impact.ttf

# Build runtime image
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "PatrickBotman.Bot.dll"]
