using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlagsNet;
using FlagsNet.Api.Models;
using FlagsNet.Providers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
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
            var parameters = ControllerContext.HttpContext.Request.Query;
            if (!parameters.ContainsKey("key"))
                return NotFound();

            var key = parameters["key"];
            bool activated = false;
            if (parameters.Count > 1)
            {
                var entries = parameters.Where(k => k.Key != "key").ToDictionary(k => k.Key, v => v.Value);
                activated = manager.Active<IDictionary<string, string>>(key,
                                    p => p.Keys.All(k => entries.ContainsKey(k) && entries[k] == p[k]));
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
