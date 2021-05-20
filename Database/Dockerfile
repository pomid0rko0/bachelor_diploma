FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app/Database
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

# Creates a non-root user with an explicit UID and adds permission to access the /app/Database folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app/Database
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["./Database.csproj", "./"]
RUN dotnet restore "./Database.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Database.csproj" -c Release -o /app/Database/build

FROM build AS publish
RUN dotnet publish "Database.csproj" -c Release -o /app/Database/publish

FROM base AS final
WORKDIR /app/Database
COPY --from=publish /app/Database/publish .
ENTRYPOINT ["dotnet", "Database.dll"]
