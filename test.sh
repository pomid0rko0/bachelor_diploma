while read -e line;
do 
    echo $line > temp
    curl --header "Content-Type: application/json" \
    --request POST \
    --data "@temp" \
    http://localhost:5005/model/parse;
    echo;
    echo;
done < $1;
rm -rf temp
