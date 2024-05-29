# Use the official .NET runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Klustr-api.csproj", "./"]
RUN dotnet restore "./Klustr-api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Klustr-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Klustr-api.csproj" -c Release -o /app/publish

# Final stage: running the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT Production
ENTRYPOINT ["dotnet", "Klustr-api.dll"]
