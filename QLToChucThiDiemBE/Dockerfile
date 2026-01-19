# Dùng hình ảnh .NET 8 SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["QuanLyThi.API.csproj", "./"]
RUN dotnet restore "QuanLyThi.API.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet publish "QuanLyThi.API.csproj" -c Release -o /app/publish

# Dùng hình ảnh ASP.NET Core Runtime để chạy
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "QuanLyThi.API.dll"]