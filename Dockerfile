FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TodoApp.Api/TodoApp.Api.csproj", "TodoApp.Api/"]
RUN dotnet restore "TodoApp.Api/TodoApp.Api.csproj"
COPY . .
WORKDIR "/src/TodoApp.Api"
RUN dotnet build "TodoApp.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoApp.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TodoApp.Api.dll"]
