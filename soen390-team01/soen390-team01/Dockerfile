FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
FROM ubuntu:latest
ENV TZ="America/Toronto"
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["soen390-team01/soen390-team01.csproj", "soen390-team01/"]
RUN dotnet restore "soen390-team01/soen390-team01.csproj"
COPY . .
WORKDIR "/src/soen390-team01"
RUN dotnet build "soen390-team01.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "soen390-team01.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "soen390-team01.dll"]
