FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["OrderService/OrderService.csproj", "OrderService/"]
COPY ["Shared.Messages/Shared.Messages.csproj", "Shared.Messages/"]
RUN dotnet restore "OrderService/OrderService.csproj"

COPY OrderService/ OrderService/
COPY Shared.Messages/ Shared.Messages/

RUN dotnet publish "OrderService/OrderService.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "OrderService.dll"]