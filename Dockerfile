# Use the official .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["AINewsAPI/AINewsAPI.csproj", "AINewsAPI/"]
COPY ["AINewsAPI.Application/AINewsAPI.Application.csproj", "AINewsAPI.Application/"]
COPY ["AINewsAPI.Infrastructure/AINewsAPI.Infrastructure.csproj", "AINewsAPI.Infrastructure/"]
COPY ["AINewsAPI.Domain/AINewsAPI.Domain.csproj", "AINewsAPI.Domain/"]
RUN dotnet restore "AINewsAPI/AINewsAPI.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/AINewsAPI"
RUN dotnet build "AINewsAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "AINewsAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AINewsAPI.dll"]
