FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY portfolio.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://*:80
COPY --from=build /app .
ENTRYPOINT ["dotnet", "portfolio.dll"]