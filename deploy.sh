#!/usr/bin/env bash

set -e

dotnet publish web-fnol-tool/web-fnol-tool.csproj -c Release -o ../app
cf target -o gotimedriver_a2e -s digital-garage-claims
cf push -f app/manifest.yml  -p app
rm -r app
