#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/client/Jobsity.CodeChallenge.Chat.UI/Jobsity.CodeChallenge.Chat.UI.csproj", "src/client/Jobsity.CodeChallenge.Chat.UI/"]
RUN dotnet restore "src/client/Jobsity.CodeChallenge.Chat.UI/Jobsity.CodeChallenge.Chat.UI.csproj"
COPY . .
WORKDIR "/src/src/client/Jobsity.CodeChallenge.Chat.UI"
RUN dotnet build "Jobsity.CodeChallenge.Chat.UI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Jobsity.CodeChallenge.Chat.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Jobsity.CodeChallenge.Chat.UI.dll"]