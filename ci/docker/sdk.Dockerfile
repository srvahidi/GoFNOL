FROM microsoft/dotnet:2.1-runtime-deps-alpine3.7

# Disable the invariant mode (set in base image)
RUN apk add --no-cache icu-libs

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
	LC_ALL=en_US.UTF-8 \
	LANG=en_US.UTF-8

# Install .NET Core SDK
ENV DOTNET_SDK_VERSION 2.1.403

RUN apk add --no-cache --virtual .build-deps \
		openssl \
	&& wget -O dotnet.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$DOTNET_SDK_VERSION/dotnet-sdk-$DOTNET_SDK_VERSION-linux-musl-x64.tar.gz \
	&& dotnet_sha512='620f091eba8d111b13d440c20926f60919e64dd421c6cbf2696b6f3f643a3d654b7dc394e6e84b1c4bef6ff872c754a7317e9b94977cbcb93b5d0fdfe08d8b55' \
	&& echo "$dotnet_sha512  dotnet.tar.gz" | sha512sum -c - \
	&& mkdir -p /usr/share/dotnet \
	&& tar -C /usr/share/dotnet -xzf dotnet.tar.gz \
	&& ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet \
	&& rm dotnet.tar.gz \
	&& apk del .build-deps

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