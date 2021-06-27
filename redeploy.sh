#!/bin/bash

docker-compose down
git fetch
git reset --hard origin/main
chmod u+x redeploy.sh
docker-compose up --build --detach
