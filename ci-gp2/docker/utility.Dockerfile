FROM alpine

RUN apk update && \
	apk add zip unzip curl && \
	curl -L "https://packages.cloudfoundry.org/stable?release=linux64-binary&version=6.43.0" | tar -zx -C /usr/local/bin && \	
	curl https://raw.githubusercontent.com/pivotalservices/artifactory-resource/master/tools/jq -o /usr/local/bin/jq && \
	chmod a+x /usr/local/bin/jq