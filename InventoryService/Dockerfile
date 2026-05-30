FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["InventoryService/InventoryService.csproj", "InventoryService/"]
COPY ["Shared.Messages/Shared.Messages.csproj", "Shared.Messages/"]
RUN dotnet restore "InventoryService/InventoryService.csproj"

COPY InventoryService/ InventoryService/
COPY Shared.Messages/ Shared.Messages/

RUN dotnet publish "InventoryService/InventoryService.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "InventoryService.dll"]