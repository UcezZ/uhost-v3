local tools = require 'tools'
local red = tools.connect_redis()
local md5 = require 'md5'

local sum = md5.sumhexa('amogus')

ngx.say(sum)

local token_salt = os.getenv('VIDEO_TOKEN_SALT')

if not token_salt then
    ngx.header.content_type = 'text/plain'
    ngx.status = 500
    ngx.say('Unable to gather token salt')
    ngx.exit(500)
end

ngx.say(token_salt)

local video_token = ngx.var.cookie_video_token

ngx.say(video_token)

local query = tools.get_path()

ngx.say(query)

local key_payload = video_token .. query .. token_salt

ngx.say(key_payload)

local key = 'videotoken_' .. md5.sumhexa(key_payload)

ngx.say(key)

local res, err = red:get(key)
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
