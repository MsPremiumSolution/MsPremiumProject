# Usa a imagem base do SDK do .NET 7 para a fase de build.
# 'AS build' nomeia esta fase para refer�ncia posterior.
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia o ficheiro .csproj do projeto principal e restaura as depend�ncias.
# Ajuste "MSPremiumProject/MSPremiumProject.csproj" se o nome da sua pasta/projeto for diferente.
COPY ["MSPremiumProject/MSPremiumProject.csproj", "MSPremiumProject/"]

# Se voc� tiver OUTROS projetos na sua solu��o que s�o referenciados pelo projeto principal,
# copie os .csproj deles e restaure tamb�m. Exemplo:
# COPY ["OutraBiblioteca/OutraBiblioteca.csproj", "OutraBiblioteca/"]
# Se n�o tiver, pode remover as linhas de exemplo acima.

RUN dotnet restore "MSPremiumProject/MSPremiumProject.csproj"

# Copia todos os outros ficheiros do projeto para o contentor.
# O primeiro "." refere-se ao contexto de build (a sua pasta local).
# O segundo "." refere-se ao diret�rio de trabalho atual dentro do contentor (/src).
COPY . .

# Define o diret�rio de trabalho para a pasta do projeto principal antes de compilar.
WORKDIR "/src/MSPremiumProject"
RUN dotnet build "MSPremiumProject.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Inicia uma nova fase 'publish' a partir da fase 'build'.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MSPremiumProject.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Usa a imagem base do runtime do ASP.NET Core 7 para a fase final (imagem de produ��o).
# Esta imagem � menor porque n�o cont�m o SDK completo.
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Copia apenas os artefactos publicados da fase 'publish' para a imagem final.
COPY --from=publish /app/publish .

# Informa ao Docker que a aplica��o dentro do contentor ouvir� na porta 8080.
# O Render.com ir� mapear o tr�fego externo para esta porta.
EXPOSE 8080

# Define a vari�vel de ambiente para que o Kestrel ou�a em todas as interfaces na porta 8080.
ENV ASPNETCORE_URLS=http://+:8080

# Define o comando de entrada para executar a aplica��o quando o contentor iniciar.
# Ajuste "MSPremiumProject.dll" se o nome da sua DLL principal for diferente.
ENTRYPOINT ["dotnet", "MSPremiumProject.dll"]