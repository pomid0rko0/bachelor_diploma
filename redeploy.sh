docker-compose down
git fetch
git reset --hard origin/main
docker-compose up --build --detach
