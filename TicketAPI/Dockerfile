# 1. Aşama: Build (Derleme) işlemi için .NET SDK imajını kullan
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sadece proje dosyasını kopyala ve bağımlılıkları yükle (Cache optimizasyonu için en iyi pratiktir)
COPY ["TicketAPI.csproj", "./"]
RUN dotnet restore "TicketAPI.csproj"

# Tüm kodları kopyala ve projeyi derle
COPY . .
RUN dotnet publish "TicketAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Aşama: Çalıştırma (Run) işlemi için sadece ASP.NET Core Runtime imajını kullan (Daha hafiftir)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Google Cloud Run varsayılan olarak 8080 portunu dinler, bu ayar zorunludur
ENV ASPNETCORE_URLS=http://+:8080

# Build aşamasında derlenen dosyaları bu son imaja kopyala
COPY --from=build /app/publish .

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "TicketAPI.dll"]