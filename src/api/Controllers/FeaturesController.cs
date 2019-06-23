using System.Collections.Generic;
using System.Linq;
using FlagsNet.Api.Models;
using FlagsNet.Filters;
using FlagsNet.Providers;
using Microsoft.AspNetCore.Mvc;

namespace FlagsNet.Api.Controllers
{
    public class FeaturesController : ControllerBase
    {
        private readonly Manager manager;

        public FeaturesController() {
            this.manager = new Manager(new RedisFlagSource("localhost:6379"));
        }

        [Route("[controller]")]
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new JsonResult(manager.GetFlags());
        }

        [Route("[controller]/active")]
        [HttpGet]
        public ActionResult<FeatureModel> Active()
        {
            var parameters = ControllerContext.HttpContext.Request.Query.ToDictionary(s => s.Key, s => s.Value.ToString());
            if (!parameters.ContainsKey("key"))
                return NotFound();

            var key = parameters["key"];
            bool activated = false;
            if (parameters.Count > 1)
            {
                var jsonPath = PathBuilder.Parse(parameters.Where(i => i.Key != "key"));
                activated = manager.Active(key, jsonPath);
            } else {
                activated = manager.Active(key);
            }
            return new JsonResult(new FeatureModel {
                Name = key,
                Activated = activated
            });
        }
    }
}