using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Services.Resources.Models;

namespace Services.Resources.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase {

        private readonly IRepository<Resource> Repository;

        public ResourcesController(IRepository<Resource> repository) {

            this.Repository = repository;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Resource>>> GetAllResourcesAsync() {

            var resources = await this.Repository.GetAllItemsAsync();

            return resources.ToList();
        }

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public ActionResult<string> Get(Guid id) {
        //    return "value";
        //}

        // POST api/values
        [HttpPost]
        public async Task<ActionResult<Resource>> Post([FromBody]Resource resource) {

            if (String.IsNullOrEmpty(resource.ID)) {
                resource.ID = Guid.NewGuid().ToString().Substring(0, 8).ToLower();
            }

            var updatedResource = await this.Repository.UpsertItemAsync(resource.ID, resource);

            return this.Ok(updatedResource);
        }

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value) {
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id) {
        //}
    }
}
