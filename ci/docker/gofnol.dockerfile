FROM feeds.axadmin.net/docker/dotnet/core/aspnet:2.2

ENV ASPNETCORE_URLS=http://+:8080
WORKDIR /app
COPY GoFNOL-binaries/app/ ./

ENTRYPOINT ["dotnet", "GoFNOL.dll"]