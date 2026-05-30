FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["NotificationService/NotificationService.csproj", "NotificationService/"]
COPY ["Shared.Messages/Shared.Messages.csproj", "Shared.Messages/"]
RUN dotnet restore "NotificationService/NotificationService.csproj"

COPY NotificationService/ NotificationService/
COPY Shared.Messages/ Shared.Messages/

RUN dotnet publish "NotificationService/NotificationService.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "NotificationService.dll"]