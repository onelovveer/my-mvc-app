# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем только csproj для кэширования зависимостей
COPY ["WebApplication1/WebApplication1.csproj", "./"]
RUN dotnet restore

# Копируем весь проект
COPY . ./

# Переходим в папку проекта и публикуем
WORKDIR "/src/WebApplication1"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Этап запуска
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 10000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebApplication1.dll"]
