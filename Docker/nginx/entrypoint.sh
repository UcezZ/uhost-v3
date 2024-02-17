#!/bin/sh
# KEEP THIS FILE LF-LINEBREAKED !!!
export REDIS_ADDR=$(dig +short redis) 
export REDIS_DBNUM=$REDIS_DBNUM 

echo $REDIS_ADDR $REDIS_DBNUM

env REDIS_ADDR=$REDIS_ADDR /usr/local/nginx/sbin/nginx -g 'daemon off;'