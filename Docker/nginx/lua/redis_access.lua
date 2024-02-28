local tools = require 'tools'
local md5 = require 'md5'
local red = tools.connect_redis()

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
local key_payload = video_token .. query .. client_addr .. token_salt
local key = 'videotoken_' .. md5.sumhexa(key_payload)

local res, err = red:get(key)
if not res or err or res == ngx.null then
    red:close()
    ngx.header.content_type = 'text/plain'
    ngx.status = 403
    ngx.say('Not allowed. ID: ' .. key)
    ngx.exit(403)
end

red:close()
