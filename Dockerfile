# Usa a imagem base do SDK do .NET 7 para a fase de build.
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia o ficheiro .csproj (est� na raiz do contexto de build, que � a raiz do reposit�rio)
# O destino "./" copia-o para o WORKDIR atual (/src)
COPY ["MSPremiumProject.csproj", "./"]

# Restaura as depend�ncias usando o .csproj que est� agora em /src
RUN dotnet restore "./MSPremiumProject.csproj"

# Copia todos os outros ficheiros do reposit�rio para /src
COPY . .
# WORKDIR j� � /src, onde todos os ficheiros do projeto (incluindo Program.cs, Controllers, etc.) est�o agora.

# Compila o projeto. O .csproj est� no diret�rio de trabalho atual (/src).
RUN dotnet build "./MSPremiumProject.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Inicia uma nova fase 'publish' a partir da fase 'build'.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# Publica o projeto. O .csproj est� no diret�rio de trabalho atual (/src).
RUN dotnet publish "./MSPremiumProject.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Usa a imagem base do runtime do ASP.NET Core 7 para a fase final.
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Ajuste "MSPremiumProject.dll" se o nome da sua DLL principal for diferente.
ENTRYPOINT ["dotnet", "MSPremiumProject.dll"]