using MicroServices.ActiveDirectory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ActiveDirectoryWebExample.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ADController : ControllerBase
    {
        
        /// <summary>
        /// get user name who login
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IActionResult CheckUser()
        {
            var username = User.Identity.Name;
            return Ok(username);
        }


        /// <summary>
        /// get all paramters for user(identity) inside active diretory
        /// </summary>
        /// <param name="username"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult UserParamters([FromQuery] string username,[FromServices] IActiveDirectoryService service)
        {
            return Ok(service.GetUserJson(username));
        }

        /// <summary>
        /// get all user in LDAP (AD)
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Users([FromServices] IActiveDirectoryService  service)
        {
            return Ok(service.GetUsers());
        }

    }
}
