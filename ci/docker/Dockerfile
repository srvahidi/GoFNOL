FROM microsoft/dotnet:2.1-sdk

# Install node
RUN curl -sL https://deb.nodesource.com/setup_8.x | bash - && \
	apt-get update && \
	apt-get install -y nodejs && \
	npm install -g npm@6.3.0

# Install a2e toolset
RUN apt-get update && \
	apt-get install vim zip unzip -y && \
	curl -L "https://cli.run.pivotal.io/stable?release=linux64-binary&version=6.29.1" | tar -zx -C /usr/local/bin && \
	curl https://raw.githubusercontent.com/pivotalservices/artifactory-resource/master/tools/jq -o /usr/local/bin/jq && \
	chmod a+x /usr/local/bin/jq

COPY GoFNOL.sln GoFNOL.sln
COPY GoFNOL/GoFNOL.csproj GoFNOL/GoFNOL.csproj
COPY GoFNOL.tests/GoFNOL.tests.csproj GoFNOL.tests/GoFNOL.tests.csproj
RUN dotnet restore && \
	rm -rd GoFNOL*
