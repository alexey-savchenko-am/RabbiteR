#!/usr/bin/env bash

set -e
set -x

docker-compose down

dotnet build -c Release

docker-compose up -d rabbitmq
                            
sleep 20s

docker ps
docker stats --no-stream --all

docker-compose run --rm rabbitertests

docker-compose down -v --remove-orphans
