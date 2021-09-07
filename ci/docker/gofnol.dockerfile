FROM feeds.axadmin.net/docker/dotnet/core/aspnet:2.2

ENV ASPNETCORE_URLS=http://+:8080
WORKDIR a2e-binaries
COPY a2e-binaries/ ./

ENTRYPOINT ["dotnet", "gofnol.dll"]