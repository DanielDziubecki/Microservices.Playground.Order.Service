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
docker build -f ./src/order.service/Dockerfile.$DOCKER_ENV -t order.service:$DOCKER_TAG ./src/order.service
docker tag order.service:$DOCKER_TAG $DOCKER_USERNAME/order.service:$DOCKER_TAG
docker push $DOCKER_USERNAME/order.service:$DOCKER_TAG