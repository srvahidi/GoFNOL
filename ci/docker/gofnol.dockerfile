FROM feeds.axadmin.net/docker/dotnet/core/aspnet:2.2

# using a non-root user is a best practice for security related execution.
RUN useradd --uid $(shuf -i 2000-65000 -n 1) gofnolApp
USER gofnolApp

ENV ASPNETCORE_URLS=http://+:8080
WORKDIR /app
COPY --chown=gofnolApp GoFNOL-binaries/app/ ./

ENTRYPOINT ["dotnet", "GoFNOL.dll"]