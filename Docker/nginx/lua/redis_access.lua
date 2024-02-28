local tools = require 'tools'
local red = tools.connect_redis()

local res, err = red:get('avtobus')
if not res or err or res == ngx.null then
    red:close()
    ngx.header.content_type = 'text/plain'
    ngx.status = 403
    ngx.exit(403)
end

ngx.log(ngx.INFO, tools.get_path())

red:close()
