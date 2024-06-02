FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app


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
ENV ConnectionStrings__DefaultConnection='Host=localhost;Port=5432;Database=klustr;Username=postgres;Password=password'
ENV JWT__Issuer=http://localhost:5246
ENV JWT__Audience=http://localhost:5246
ENV JWT__SigningKey=secret
EXPOSE 80
EXPOSE 8080
ENTRYPOINT ["dotnet", "Klustr-api.dll"]