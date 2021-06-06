docker-compose down -v
git fetch
git reset --hard origin/main
docker-compose up --build --detach
