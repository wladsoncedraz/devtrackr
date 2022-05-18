using DevTrackR.API.Entities;
using DevTrackR.API.Models;
using DevTrackR.API.Persistence.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DevTrackR.API.Controllers
{
    [ApiController]
    [Route("api/packages")]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageRepository _repository;
        public PackagesController(IPackageRepository repository)
        {
            _repository = repository;
        }

        // GET api/packages
        [HttpGet]
        public IActionResult GetAll(){
            var packages = _repository.GetAll();

            return Ok(packages);
        }

        // GET api/packages/{code}
        [HttpGet("{code}")]
        public IActionResult GetByCode(string code){
            var package = _repository.GetByCode(code);

            if(package == null)
                return NotFound();

            return Ok(package);
        }

        // POST api/packages
        [HttpPost]
        public IActionResult Post(AddPackageInputModel packageModel){
            if(packageModel.Title.Length < 10)
                return BadRequest("Title length must be at least 10 characters long.");

            var package = new Package(packageModel.Title, packageModel.Weight);

            _repository.Add(package);

            return CreatedAtAction(
                "GetByCode", 
                new { code = package.Code },
                package);
        }

        // POST api/packages/{code}/updates
        [HttpPost("{code}/updates")]
        public IActionResult PostUpdate(string code, AddPackageUpdateInputModel packageUpdateModel){
            var package = _repository.GetByCode(code);

            if(package == null)
                return NotFound();
            
            package.AddUpdate(packageUpdateModel.Status, packageUpdateModel.Delivered);
            _repository.Update(package);

            return NoContent();
        }
    }
}