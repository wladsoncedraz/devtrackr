using DevTrackR.API.Entities;
using DevTrackR.API.Models;
using DevTrackR.API.Persistence.Repository;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace DevTrackR.API.Controllers
{
    [ApiController]
    [Route("api/packages")]
    public class PackagesController : ControllerBase
    {
        private readonly IPackageRepository _repository;
        private readonly ISendGridClient _client;
        public PackagesController(IPackageRepository repository, ISendGridClient client)
        {
            _repository = repository;
            _client = client;
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
        /// <summary>
        /// Cadastro de um pacote
        /// </summary>
        /// <remark>
        /// {
        ///   "title": "Mouse Logitech MX 500",
        ///   "weight": 25,
        ///   "senderName": "Wladson",
        ///   "senderEmail": "tareb92636@hbehs.com"
        /// }
        /// </remark>
        /// <param name="packageModel">Dados de um pacote</param>
        /// <returns>Objeto recem criado</returns>
        /// <response code="201">Cadastro realizado com Sucesso</response>
        /// <response code="400">Dados estao invalidos</response>
        [HttpPost]
        public async Task<IActionResult> Post(AddPackageInputModel packageModel){
            if(packageModel.Title.Length < 10)
                return BadRequest("Title length must be at least 10 characters long.");

            var package = new Package(packageModel.Title, packageModel.Weight);

            _repository.Add(package);

            var message = new SendGridMessage{
                From = new EmailAddress("tareb92636@hbehs.com", "DevTrackR"),
                Subject = "Your package was dispatched.",
                PlainTextContent = $"Your package with code {package.Code} was dispatched."
            };

            message.AddTo(packageModel.SenderEmail, packageModel.SenderName);

            await _client.SendEmailAsync(message);

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