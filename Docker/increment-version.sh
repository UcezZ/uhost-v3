#!/bin/bash

if [ "$#" -ne 2 ]; then
    echo "Usage: $0 <file_path> <key>"
    exit 1
fi

file_path=$1
key=$2

# Чтение JSON из файла и сохранение в переменную
data=$(<"$file_path") || exit 1;

# Получение значения параметра по ключу
value=$(jq ".$key" <<< "$data") || exit 1;
if [ "$value" == "null" ]; then
    echo "Key not found in JSON, creating"
    value="1.0.0"
fi

value=$(sed 's/"//g' <<< "$value")

# Увеличение значения параметра
if ! [[ "$value" =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
    echo "Bad version $value, setting default"
    value="1.0.0"
fi

IFS='.' read -r major minor build <<< "$value"
build=$((build + 1))

# Обновление JSON
new_value="$major.$minor.$build"
updated_data=$(jq ".$key=\"$new_value\"" <<< "$data")
echo "$updated_data" > "$file_path"

echo "$key set to $new_value"
