if ngx.req.get_method() == 'OPTIONS' then
    return
end

local tools = require 'tools'
local md5 = require 'md5'

local token_salt = os.getenv('VIDEO_TOKEN_SALT')

if not token_salt then
    ngx.header.content_type = 'text/plain'
    ngx.status = 500
    ngx.say('Unable to gather token salt')
    ngx.exit(500)
end

local access_token = ngx.var.arg_access_token or ngx.var.http_access_token

if not access_token then
    ngx.header.content_type = 'text/plain'
    ngx.status = 403
    ngx.say('Not allowed. No token provided.')
    ngx.exit(403)
end

local query = tools.get_path()
local key_payload = access_token .. query .. token_salt

local key_hash = md5.sumhexa(key_payload)
local key = 'videotoken_' .. key_hash

local red = tools.connect_redis()
local res, err = red:exists(key)
if err or res == 0 or res == ngx.null then
    red:close()
    ngx.header.content_type = 'text/plain'
    ngx.status = 403

    ngx.say('Not allowed. ID: ' .. key_hash)
    ngx.log(ngx.INFO, 'tk: ' .. access_token)
    ngx.log(ngx.INFO, 'q: ' .. query)
    ngx.log(ngx.INFO, 's: ' .. token_salt)
    ngx.log(ngx.INFO, 'k: ' .. key_payload)

    ngx.exit(403)
end

red:close()
