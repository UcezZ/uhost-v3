local tools = {}

function tools.connect_redis()
    local redis_addr = os.getenv('REDIS_ADDR')

    if not redis_addr then
        ngx.header.content_type = 'text/plain'
        ngx.status = 500
        ngx.say('Unable to gather Redis address')
        ngx.exit(500)
    end

    local redis_db_num = os.getenv('REDIS_DBNUM')
    if not redis_db_num then
        ngx.header.content_type = 'text/plain'
        ngx.status = 500
        ngx.say('Unable to gather Redis DB num')
        ngx.exit(500)
    end

    local redis = require 'resty.redis'
    local red = redis:new()

    red:set_timeouts(1000, 1000, 1000)
    local ok, err = red:connect(redis_addr, 6379)
    if not ok then
        ngx.header.content_type = 'text/plain'
        ngx.status = 500
        ngx.say('Redis failure: ', err)
        ngx.exit(500)
    end

    local res, err = red:select(redis_db_num)
    if not res or err then
        red:close()
        ngx.header.content_type = 'text/plain'
        ngx.status = 500
        ngx.say('Redis failure: ', err)
        ngx.exit(500)
    end

    return red
end

function tools.get_path()
    local path = ngx.var.request_uri:sub(2)

    if string.match(path, '/[0-9a-fA-F]+/[0-9a-fA-F]+/[0-9a-fA-F]+/[%w%d%-_]+%.[%w%d%-_]+$') then
        local ix = path:find('[^/]*$')

        if ix and ix > 3 then
            path = path:sub(0, ix - 2)
        end

        if string.match(path, '^hls/videos') then
            path = path:sub(5)
        end
    end

    return path
end

function tools.get_client_ip()
    local x_forwarded_for = ngx.req.get_headers()['X-Forwarded-For']
    local x_forwarded = ngx.req.get_headers()['X-Forwarded']
    local addr = ngx.var.remote_addr

    if x_forwarded_for then
        return string.match(x_forwarded_for, '^([^:]+)')
    end

    if x_forwarded then
        return string.match(x_forwarded, '^([^:]+)')
    end

    return addr
end

return tools
