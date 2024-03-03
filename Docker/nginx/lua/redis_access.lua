local tools = require 'tools'
local md5 = require 'md5'

local token_salt = os.getenv('VIDEO_TOKEN_SALT')

if not token_salt then
    ngx.header.content_type = 'text/plain'
    ngx.status = 500
    ngx.say('Unable to gather token salt')
    ngx.exit(500)
end

local video_token = ngx.var.cookie_video_token

if not video_token then
    ngx.header.content_type = 'text/plain'
    ngx.status = 403
    ngx.say('Not allowed. No token provided.')
    ngx.exit(403)
end

local query = tools.get_path()
local client_addr = tools.get_client_ip()
local is_dev = os.getenv('IS_DEV') == 'TRUE'
local key_payload

if is_dev then
    key_payload = video_token .. query .. token_salt
else
    key_payload = video_token .. query .. client_addr .. token_salt
end

local key_hash = md5.sumhexa(key_payload)
local key = 'videotoken_' .. key_hash

local red = tools.connect_redis()
local res, err = red:get(key)
if not res or err or res == ngx.null then
    red:close()
    ngx.header.content_type = 'text/plain'
    ngx.status = 403

    if is_dev then
        ngx.say('tk: ' .. video_token)
        ngx.say('q: ' .. query)
        ngx.say('s: ' .. token_salt)
        ngx.say('k: ' .. key_payload)
    else
        ngx.say('Not allowed. ID: ' .. key_hash)
        ngx.log('tk: ' .. video_token)
        ngx.log('q: ' .. query)
        ngx.log('ip: ' .. client_addr)
        ngx.log('s: ' .. token_salt)
        ngx.log('k: ' .. key_payload)
    end

    ngx.exit(403)
end

red:close()
