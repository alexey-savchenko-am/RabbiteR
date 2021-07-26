#!/usr/bin/env bash

set -e
set -x

docker-compose down

dotnet build -c Release
dotnet restore

docker-compose up -d rabbitmq

./wait-for-it.sh localhost:5672 --

docker-compose run --rm rabbitertests

docker ps
docker stats --no-stream --all


docker-compose down -v --remove-orphans
