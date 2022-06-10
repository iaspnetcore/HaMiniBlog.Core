

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Miniblog.Core.ApiControllers
{
    using Microsoft.AspNetCore.Mvc;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    [Route("api/[controller]")]
    [ApiController]
    public class IpAddressController : ControllerBase
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        public IpAddressController(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;

            //}
        }

        // GET: api/<IpAddressController>
        [HttpGet]
        public string Get()
        {

            var ipv6address = this._httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            ipv6address = this._httpContextAccessor.HttpContext.Request.Headers["CF-Connecting-IP"].ToString();

            return ipv6address;
        }


    }
}
