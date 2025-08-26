using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MarkovWebApp.Models;
using MarkovWebApp.Logic;
using MarkovWebApp.Services;

namespace MarkovWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MarkovDbService _dbService;
        private readonly MarkovGenerator _generator;

        public HomeController(ILogger<HomeController> logger, MarkovDbService dbService, MarkovGenerator generator)
        {
            _logger = logger;
            _dbService = dbService;
            _generator = generator;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                StatusMessage = TempData["StatusMessage"] as string
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Train()
        {
            return View(new TrainingViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Train(TrainingViewModel model)
        {
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(model.InputText))
            {
                await _dbService.TrainAsync(model.InputText, 2);
                ViewBag.Message = "Model has been trained successfully with the new text!";
            }
            else
            {
                ViewBag.Message = "Please provide some text to train the model.";
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Generate()
        {
            return View(new GenerateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(GenerateViewModel model)
        {
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(model.SeedPhrase))
            {
                var generatedText = new StringBuilder();
                await foreach (var word in _generator.GenerateTextAsync(model.SeedPhrase, 100, 2))
                {
                    generatedText.Append(word).Append(' ');
                }
                model.GeneratedText = generatedText.ToString().Trim();
            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}