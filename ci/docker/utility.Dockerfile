FROM alpine

RUN apk update && \
	apk add zip unzip curl wget && \
	curl -L "https://packages.cloudfoundry.org/stable?release=linux64-binary&version=6.48.0" | tar -zx -C /usr/local/bin && \
	curl -L "https://github.com/stedolan/jq/releases/download/jq-1.6/jq-linux64" -o /usr/local/bin/jq && \
	chmod +x /usr/local/bin/jq