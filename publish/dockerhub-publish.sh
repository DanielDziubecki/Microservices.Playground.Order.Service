#!/bin/bash
DOCKER_ENV=''
DOCKER_TAG=''
case "$TRAVIS_BRANCH" in
  "master")
    DOCKER_ENV=production
    DOCKER_TAG=latest
    ;;
  "develop")
    DOCKER_ENV=development
    DOCKER_TAG=dev
    ;;    
esac

docker login -u $DOCKER_USERNAME -p $DOCKER_PASSWORD
docker build -f ./src/Order.Service/Dockerfile.$DOCKER_ENV -t Order.Service:$DOCKER_TAG ./src/Order.Service
docker tag Order.Service:$DOCKER_TAG $DOCKER_USERNAME/Order.Service:$DOCKER_TAG
docker push $DOCKER_USERNAME/Order.Service:$DOCKER_TAG