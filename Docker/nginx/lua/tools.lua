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
    local path = ngx.var.request_uri

    if path:sub(-3) == '.ts' then
        local ix = path:find('[^/]*$')

        if ix and ix > 3 then
            return path:sub(2, ix - 2)
        end
    end

    return path:sub(2)
end

function tools.get_client_ip()
    local x_forwarded_for = ngx.req.get_headers()['X-Forwarded-For']
    local addr = ngx.var.remote_addr

    if not x_forwarded_for then
        return addr
    else
        return string.match(x_forwarded_for, '^([^:]+)')
    end
end

return tools
