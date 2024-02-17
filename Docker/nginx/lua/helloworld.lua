local redisAddr = os.getenv('REDIS_ADDR')

if not redisAddr then
    ngx.header.content_type = 'text/plain'
    ngx.status = 500
    ngx.say('Unable to gather Redis address')
    return
end

local redisDbNum = os.getenv('REDIS_DBNUM')
if not redisDbNum then
    ngx.header.content_type = 'text/plain'
    ngx.status = 500
    ngx.say('Unable to gather Redis DB num')
    return
end

local redis = require 'resty.redis'
local red = redis:new()

red:set_timeouts(1000, 1000, 1000) -- подключение к Redis
local ok, err = red:connect(redisAddr, 6379)
if not ok then
    ngx.header.content_type = 'text/plain'
    ngx.status = 500
    ngx.say('Unable to connect to Redis: ', err)
    return
end

local res, err = red:select(redisDbNum)
if not res then
    ngx.say('Failed to select database: ', err)
    return
end

local res, err = red:get('avtobus')
if not res then
    ngx.header.content_type = 'text/plain'
    ngx.status = 500
    ngx.say('Failed to get data from Redis: ', err)
    return
end

ngx.header.content_type = 'text/plain'
ngx.status = 200
ngx.say('Data from Redis: ', res)

red:set_keepalive(10000, 100)
return