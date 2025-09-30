FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["src/Gen.Cmd/Gen.Cmd.csproj", "src/Gen.Cmd/"]
COPY ["src/Gen.Lib/Gen.Lib.csproj", "src/Gen.Lib/"]
RUN dotnet restore "src/Gen.Cmd/Gen.Cmd.csproj"

# Copy remaining files and build
COPY . .
WORKDIR "/src/src/Gen.Cmd"
RUN dotnet build "Gen.Cmd.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gen.Cmd.csproj" -c $BUILD_CONFIGURATION -o /app/publish --runtime linux-musl-x64 --self-contained false /p:PublishTrimmed=false /p:UseAppHost=false

# Final runtime image (Alpine)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# COPY gencmd.sh /gencmd.sh

RUN mkdir -p /app/logs
WORKDIR /app

# Ensure we run as a non-root user (Optional: Define a user)
# RUN addgroup -S appgroup && adduser -S appuser -G appgroup
# USER appuser

# Set entrypoint
# ENTRYPOINT ["dotnet", "Gen.Cmd.dll"]
# CMD ["/app/data/Strings.lhq", "--data", "namespace=ABC.777", "--out", "."]

RUN chmod +x /gencmd.sh
ENTRYPOINT ["/gencmd.sh"]
CMD ["/app/data/Strings.lhq", "--data", "namespace=ABC.777", "--out", "."]