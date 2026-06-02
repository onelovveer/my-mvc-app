using Microsoft.EntityFrameworkCore.Infrastructure;

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем csproj файл и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем все файлы и публикуем проект
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Этап выполнения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 10000

# Устанавливаем переменные окружения
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

# Копируем опубликованные файлы
COPY --from=build /app/publish .

# Запускаем приложение
ENTRYPOINT ["dotnet", "WebApplication1.dll"]FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем csproj файл и восстанавливаем зависимости
COPY *.csproj ./
RUN dotnet restore

# Копируем все файлы и публикуем проект
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Этап выполнения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 10000

# Устанавливаем переменные окружения
ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

# Копируем опубликованные файлы
COPY --from=build /app/publish .

# Запускаем приложение
ENTRYPOINT ["dotnet", "WebApplication1.dll"]