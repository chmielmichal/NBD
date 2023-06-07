using Microsoft.AspNetCore.Mvc;
using NBD.Models;
using NBD.Services;
using System.Diagnostics;

namespace NBD.Controllers
{
	public class HomeController : Controller
	{

		private readonly IComputerService _computerService;
		public HomeController(IComputerService computerService)
		{
			_computerService = computerService;
		}

		public IActionResult Index(ComputerFilterModel filter)
		{
			var computers = _computerService.GetComputers(filter.Year, filter.ComputerName);
			var model = new ComputerListModel
			{
				Computers = computers.Result,
				Filter = filter
			};
			return View(model);
		}
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(ComputerModel computer)
		{
			if (ModelState.IsValid)
			{
				await _computerService.Create(computer);
				return RedirectToAction("Index");
			}
			return View(computer);
		}

		[HttpGet]
		public async Task<IActionResult> Edit(string id)
		{
			ComputerModel computer = await _computerService.GetComputer(id);
			if (computer == null)
			{
				return NotFound();
			}
			return View(computer);
		}
		[HttpPost]
		public async Task<IActionResult> Edit(ComputerModel computer)
		{
			if (ModelState.IsValid)
			{
				await _computerService.Update(computer);
				return RedirectToAction("Index");
			}
			return View(computer);
		}


		public async Task<IActionResult> Delete(string id)
		{
			await _computerService.Remove(id);
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> AttachImage(string id)
		{
			ComputerModel computer = await _computerService.GetComputer(id);
			if (computer == null)
				return NotFound();
			else
				return View(computer);
		}

		[HttpPost]
		public async Task<IActionResult> AttachImage(string id, IFormFile uploadedFile)
		{
			if (uploadedFile != null)
				await _computerService.StoreImage(id, uploadedFile.OpenReadStream(), uploadedFile.FileName);
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> GetImage(string id)
		{
			var image = await _computerService.GetImage(id);
			if (image == null) return NotFound();
			return File(image, "image/png");
		}
	}
}