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
            this._logger = logger;
        }

        [HttpGet]
        [Route("text")]
        public async Task<IActionResult> ValidateText(string textToValidate)
        {
            var validationControllers = Helpers.GetValidationRules(textToValidate, ValidationType.Text);
            var result = new List<ValidationResult>();
            foreach (var validationController in validationControllers)
            {
                result.AddRange(await validationController.Validate());
            }

            return new OkObjectResult(result);
        }

        [HttpGet]
        [Route("image")]
        public async Task<IActionResult> ValidateImage(string imageToValidate)
        {
            var validationControllers = Helpers.GetValidationRules(imageToValidate, ValidationType.Image);
            var result = new List<ValidationResult>();
            foreach (var validationController in validationControllers)
            {
                result.AddRange(await validationController.Validate());
            }

            return new OkObjectResult(result);
        }
    }
}
