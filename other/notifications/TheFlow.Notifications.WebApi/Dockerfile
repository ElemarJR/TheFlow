FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY other/notifications/TheFlow.Notifications.WebApi/TheFlow.Notifications.WebApi.csproj other/notifications/TheFlow.Notifications.WebApi/
COPY other/notifications/TheFlow.Notifications.Application/TheFlow.Notifications.Application.csproj other/notifications/TheFlow.Notifications.Application/
COPY src/TheFlow.Infrastructure.Stores.InstancesStore.RavenDB/TheFlow.Infrastructure.Stores.InstancesStore.RavenDB.csproj src/TheFlow.Infrastructure.Stores.InstancesStore.RavenDB/
COPY src/TheFlow/TheFlow.csproj src/TheFlow/
COPY external/HealthChecks/src/Microsoft.Extensions.HealthChecks/Microsoft.Extensions.HealthChecks.csproj external/HealthChecks/src/Microsoft.Extensions.HealthChecks/
COPY external/HealthChecks/src/Microsoft.AspNetCore.HealthChecks/Microsoft.AspNetCore.HealthChecks.csproj external/HealthChecks/src/Microsoft.AspNetCore.HealthChecks/
RUN dotnet restore other/notifications/TheFlow.Notifications.WebApi/TheFlow.Notifications.WebApi.csproj
COPY . .
WORKDIR /src/other/notifications/TheFlow.Notifications.WebApi
RUN dotnet build TheFlow.Notifications.WebApi.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TheFlow.Notifications.WebApi.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TheFlow.Notifications.WebApi.dll"]
