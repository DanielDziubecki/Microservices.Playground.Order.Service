#!/bin/bash
echo Publishing application
./publish/publish.sh
echo Building and pushing Docker images
./publish/dockerhub-publish.sh