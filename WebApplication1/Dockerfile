FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем только csproj файл для кэширования слоев
COPY WebApplication1/WebApplication1.csproj ./
RUN dotnet restore

# Копируем весь проект
COPY . ./

# Переходим в папку проекта перед сборкой
WORKDIR /src/WebApplication1
RUN dotnet build -c Release -o /app/build

# Публикация
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApplication1.dll"]
