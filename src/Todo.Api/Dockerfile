# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia apenas os .csproj primeiro para cachear o restore entre builds
COPY src/Todo.Domain/Todo.Domain.csproj src/Todo.Domain/
COPY src/Todo.Application/Todo.Application.csproj src/Todo.Application/
COPY src/Todo.Infrastructure/Todo.Infrastructure.csproj src/Todo.Infrastructure/
COPY src/Todo.Api/Todo.Api.csproj src/Todo.Api/
RUN dotnet restore src/Todo.Api/Todo.Api.csproj

COPY src/ src/
RUN dotnet publish src/Todo.Api/Todo.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080
ENTRYPOINT ["dotnet", "Todo.Api.dll"]
