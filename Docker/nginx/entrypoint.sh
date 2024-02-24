#!/bin/sh
# KEEP THIS FILE LF-LINEBREAKED !!!
export REDIS_ADDR=$(dig +short redis) 
export REDIS_DBNUM=$REDIS_DBNUM 

echo Redis address: $REDIS_ADDR 
echo Redis DB: $REDIS_DBNUM

env REDIS_ADDR=$REDIS_ADDR /usr/local/nginx/sbin/nginx -g 'daemon off;'