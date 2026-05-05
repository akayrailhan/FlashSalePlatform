# 1. ASAMA: BASE (Calisma Zamani - Sadece uygulamayi ayaga kaldirmak icin hafif motor)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Google Cloud Run varsayilan olarak 8080 portunu dinler
ENV ASPNETCORE_URLS=http://+:8080

# 2. ASAMA: BUILD (SDK - Kodu derlemek icin gereken agir motor)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# Once sadece .csproj dosyasini kopyalayip bagimliliklari indiriyoruz (Docker Cache avantaji icin)
COPY ["TicketAPI/TicketAPI.csproj", "TicketAPI/"]
RUN dotnet restore "TicketAPI/TicketAPI.csproj"

# Simdi tum kodu kopyala ve derle
COPY . .
WORKDIR "/src/TicketAPI"
RUN dotnet build "TicketAPI.csproj" -c Release -o /app/build

# 3. ASAMA: PUBLISH (Derlenmis kodu yayina hazir, sikistirilmis hale getir)
FROM build AS publish
RUN dotnet publish "TicketAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. ASAMA: FINAL (Temiz ve hafif base imajin icine, publish edilmis dosyalari koy)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicketAPI.dll"]
