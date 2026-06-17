FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY BetterCRM.Core/BetterCRM.Core.csproj BetterCRM.Core/
COPY BetterCRM.Business/BetterCRM.Business.csproj BetterCRM.Business/
COPY BetterCRM.DataAccess/BetterCRM.DataAccess.csproj BetterCRM.DataAccess/
COPY BetterCRM.Api/BetterCRM.Api.csproj BetterCRM.Api/
RUN dotnet restore BetterCRM.Api/BetterCRM.Api.csproj

COPY BetterCRM.Core/ BetterCRM.Core/
COPY BetterCRM.Business/ BetterCRM.Business/
COPY BetterCRM.DataAccess/ BetterCRM.DataAccess/
COPY BetterCRM.Api/ BetterCRM.Api/
RUN dotnet publish BetterCRM.Api/BetterCRM.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "BetterCRM.Api.dll"]
