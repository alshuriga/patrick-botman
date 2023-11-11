FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

VOLUME /db

WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

#FFMPEG Install
RUN apt update && apt install -y ffmpeg

#Font install
COPY ./impact.ttf ./
RUN mkdir -p /usr/share/fonts/truetype/
RUN install -m644 impact.ttf /usr/share/fonts/truetype/
RUN rm ./impact.ttf

WORKDIR /App
COPY --from=build-env /App/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "patrick-botman.dll"]

