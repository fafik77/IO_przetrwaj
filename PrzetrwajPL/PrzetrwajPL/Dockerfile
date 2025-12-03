# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["PrzetrwajPL/PrzetrwajPL.csproj", "./PrzetrwajPL/"]
COPY ["PrzetrwajPL.Client/PrzetrwajPL.Client.csproj", "./PrzetrwajPL.Client/"]
RUN dotnet restore "PrzetrwajPL/PrzetrwajPL.csproj"
RUN dotnet restore "PrzetrwajPL.Client/PrzetrwajPL.Client.csproj"
COPY . .
RUN dotnet build "./PrzetrwajPL/PrzetrwajPL.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./PrzetrwajPL/PrzetrwajPL.csproj" -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "PrzetrwajPL.dll"]