FROM feeds.axadmin.net/docker/dotnet/core/aspnet:2.2

ENV ASPNETCORE_URLS=http://+:8080
WORKDIR /app
COPY GoFNOL-binaries/app/ ./

# using a non-root user is a best practice for security related execution.
RUN useradd --uid $(shuf -i 2000-65000 -n 1) gofnolApp
USER gofnolApp

ENTRYPOINT ["dotnet", "GoFNOL.dll"]