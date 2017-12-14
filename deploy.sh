#!/usr/bin/env bash

set -e


 cp ${PWD}/manifest.yml ${PWD}/web-fnol-tool/bin/Debug/netcoreapp2.0/publish
 cd ${PWD}/web-fnol-tool/bin/Debug/netcoreapp2.0/publish
 cf push