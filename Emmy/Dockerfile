FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

# Copy everything else and build
COPY Emmy ./Emmy
COPY Emmy.Data ./Emmy.Data
COPY Emmy.Services ./Emmy.Services

RUN dotnet restore Emmy/Emmy.csproj
RUN dotnet publish Emmy/*.csproj -c Release -o out

# Build runtime image
WORKDIR /
FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY --from=build-env ./out .
ENTRYPOINT ["dotnet", "Emmy.dll"]