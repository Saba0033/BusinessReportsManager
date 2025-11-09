# ===============================
# 1️⃣ Build Stage
# ===============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project files
COPY ["BusinessReportsManager.Api/BusinessReportsManager.Api.csproj", "BusinessReportsManager.Api/"]
COPY ["BusinessReportsManager.Application/BusinessReportsManager.Application.csproj", "BusinessReportsManager.Application/"]
COPY ["BusinessReportsManager.Domain/BusinessReportsManager.Domain.csproj", "BusinessReportsManager.Domain/"]
COPY ["BusinessReportsManager.Infrastructure/BusinessReportsManager.Infrastructure.csproj", "BusinessReportsManager.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "BusinessReportsManager.Api/BusinessReportsManager.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build and publish
RUN dotnet publish "BusinessReportsManager.Api/BusinessReportsManager.Api.csproj" -c Release -o /app/publish

# ===============================
# 2️⃣ Runtime Stage
# ===============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "BusinessReportsManager.Api.dll"]
