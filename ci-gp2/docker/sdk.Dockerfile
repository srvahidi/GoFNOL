FROM mcr.microsoft.com/dotnet/core/runtime-deps:2.2-alpine3.9

# Disable the invariant mode (set in base image)
RUN apk add --no-cache icu-libs

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
	LC_ALL=en_US.UTF-8 \
	LANG=en_US.UTF-8

# Install .NET Core SDK
ENV DOTNET_SDK_VERSION 2.2.203

RUN wget -O dotnet.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$DOTNET_SDK_VERSION/dotnet-sdk-$DOTNET_SDK_VERSION-linux-musl-x64.tar.gz \
	&& dotnet_sha512='18c821c8f9c110d3e1bc4e8d6a88e01c56903a58665a23a898457a85afa27abfa23ef24709602d7ad15845f1cd5b3c3dd8c24648ab8ab9e281b5705968e60e41' \
	&& echo "$dotnet_sha512  dotnet.tar.gz" | sha512sum -c - \
	&& mkdir -p /usr/share/dotnet \
	&& tar -C /usr/share/dotnet -xzf dotnet.tar.gz \
	&& ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet \
	&& rm dotnet.tar.gz

# Enable correct mode for dotnet watch (only mode supported in a container)
ENV DOTNET_USE_POLLING_FILE_WATCHER=true \ 
	# Skip extraction of XML docs - generally not useful within an image/container - helps performance
	NUGET_XMLDOC_MODE=skip \
	# Disable populating local packages cache
	DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true

RUN apk --update add git nodejs nodejs-npm && \
	npm install -g npm@6.3.0

# Install all NuGet packages used in solution
COPY GoFNOL.sln GoFNOL.sln
COPY GoFNOL/GoFNOL.csproj GoFNOL/GoFNOL.csproj
COPY GoFNOL.tests/GoFNOL.tests.csproj GoFNOL.tests/GoFNOL.tests.csproj
RUN dotnet restore && \
	rm -r GoFNOL*