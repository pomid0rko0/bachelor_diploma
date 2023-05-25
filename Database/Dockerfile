FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app/Database
EXPOSE 8000

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV ASPNETCORE_URLS=http://+:8000

# Creates a non-root user with an explicit UID and adds permission to access the /app/Database folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app/Database
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["./Database.csproj", "./"]
RUN dotnet restore "./Database.csproj"
COPY . .

RUN dotnet tool install --global dotnet-ef
ENV PATH /root/.dotnet/tools:$PATH

RUN dotnet ef migrations add InitialCreate

RUN dotnet build "Database.csproj" -c Release -o /app/Database/build

FROM build AS publish
RUN dotnet publish "Database.csproj" -c Release -o /app/Database/publish

FROM base AS final
WORKDIR /app/Database
COPY --from=publish /app/Database/publish .
COPY nlu_config.yml nlu_config.yml
ENTRYPOINT ["dotnet", "Database.dll"]
