FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Set timezone to JST
ENV TZ=Asia/Tokyo
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["KdxReport.csproj", "./"]
RUN dotnet restore "KdxReport.csproj"
COPY . .
RUN dotnet build "KdxReport.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KdxReport.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KdxReport.dll"]
