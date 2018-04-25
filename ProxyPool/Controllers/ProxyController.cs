using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProxyPool.ViewModel;

namespace ProxyPool.Controllers
{
    [Route("api/[controller]")]
    public class ProxyController : Controller
    {
        private ProxyService proxyService;
        public ProxyController(ProxyService service)
        {
            proxyService = service;
        }
        // GET api/proxy/one
        [HttpGet("one")]
        public ApiResult<Proxy> One()
        {
            try
            {
                var proxy = proxyService.GetProxy();
                return new ApiResult<Proxy>() { Status = true, Message = "ok", Result = proxy };
            }
            catch(Exception ex)
            {
                return new ApiResult<Proxy>() { Status = false, Message = ex.Message };
            }
        }

        // GET api/proxy/all
        [HttpGet("all/{page:int=1}/{pageSize:int=10}")]
        public ApiResult<List<Proxy>> GetAll(int page,int pageSize)
        {
            try
            {
                var list = proxyService.GetList(page, pageSize);
                return new ApiResult<List<Proxy>>() { Status = true, Message = "ok", Result = list };
            }
            catch(Exception ex)
            {
                return new ApiResult<List<Proxy>>() { Status = false, Message = ex.Message };
            }
        }
    }
}
