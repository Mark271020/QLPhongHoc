FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj và restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ source code và build
COPY . ./
RUN dotnet publish -c Release -o out -f net8.0

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Tạo file firebase-key.json từ secret (nếu có)
RUN mkdir -p /app
RUN if [ -n "$FIREBASE_CREDENTIALS" ]; then echo "$FIREBASE_CREDENTIALS" > /app/firebase-key.json; fi

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "QLPhongHoc.dll"]