FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY api.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
EXPOSE 82
ENV ASPNETCORE_URLS=http://*:82
COPY --from=build /app .
ENTRYPOINT ["dotnet", "notification.dll"]