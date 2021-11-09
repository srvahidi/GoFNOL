FROM alpine

RUN apk --update add nodejs npm
RUN npm install -g npm@6.3.0