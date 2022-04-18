FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["NumeralSystems.Net.Benchmark/NumeralSystems.Net.Benchmark.csproj", "NumeralSystems.Net.Benchmark/"]
RUN dotnet restore "NumeralSystems.Net.Benchmark/NumeralSystems.Net.Benchmark.csproj"
COPY . .
WORKDIR "/src/NumeralSystems.Net.Benchmark"
RUN dotnet build "NumeralSystems.Net.Benchmark.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NumeralSystems.Net.Benchmark.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NumeralSystems.Net.Benchmark.dll"]
