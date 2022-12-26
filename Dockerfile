

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

WORKDIR /App



# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out


# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /App
COPY --from=build-env /App/out .
ENTRYPOINT ["dotnet", "patrick-botman.dll"]
ENV ASPNETCORE_URLS=http://+:5265

EXPOSE 5265

#FFMPEG Install
RUN apt update && apt install -y ffmpeg

#Font install
COPY ./impact.ttf ./
RUN mkdir -p /usr/share/fonts/truetype/
RUN install -m644 impact.ttf /usr/share/fonts/truetype/
RUN rm ./impact.ttf