using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ValidationService.Classes;

namespace ValidationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValidationController : ControllerBase
    {
        private readonly ILogger<ValidationController> _logger;

        public ValidationController(ILogger<ValidationController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "ValidateText")]
        public IActionResult Validate(string textToValidate)
        {
            var validationControllers = Helpers.GetValidationRules(textToValidate, ValidationType.Text);
            var result = new List<ValidationResult>();
            foreach (var validationController in validationControllers)
            {
                result.AddRange(validationController.Validate());
            }

            return new OkObjectResult(result);
        }
    }
}
