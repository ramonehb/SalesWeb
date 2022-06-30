using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exception;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SalesWebMvc.Controllers
{
    public class SellersController : Controller
    {
        //Injecao de dependencia
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;

        public string Actvity { get; private set; }

        public SellersController(SellerService sellerService, DepartmentService departmentService)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _sellerService.FindAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var department = await _departmentService.FindAllAsync();
            var viewModel = new SellerFormViewModel()
            {
                Departments = department
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Seller seller)
        {
            if (!ModelState.IsValid)
            {
                var departments = await _departmentService.FindAllAsync();
                var viewModel = new SellerFormViewModel
                {
                    Seller = seller,
                    Departments = departments
                };

                return View(viewModel);
            }

            await _sellerService.Insert(seller);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return RedirectToAction(nameof(Error), new { message = "Id não foi fornecido" });

            var seller = await _sellerService.FindByIdAsync(id.Value);
            if (seller is null) return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });

            return View(seller);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _sellerService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return RedirectToAction(nameof(Error), new { message = "Id não foi fornecido" });

            var seller = await _sellerService.FindByIdAsync(id.Value);
            if (seller is null) return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });

            return View(seller);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return RedirectToAction(nameof(Error), new { message = "Id não foi fornecido" });

            var seller = await _sellerService.FindByIdAsync(id.Value);
            if (seller is null) return RedirectToAction(nameof(Error), new { message = "Id não encontrado" });

            List<Department> departments = await _departmentService.FindAllAsync();

            var viewModel = new SellerFormViewModel
            {
                Seller = seller,
                Departments = departments
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Seller seller)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var departments = await _departmentService.FindAllAsync();
                    var viewModel = new SellerFormViewModel
                    {
                        Seller = seller,
                        Departments = departments
                    };

                    return View(viewModel);
                }

                if (id != seller.Id) return RedirectToAction(nameof(Error), new { message = "Id diferente." });

                await _sellerService.UpdateAsync(seller);

                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
        }

        public IActionResult Error(string msg)
        {
            var viewModel = new ErrorViewModel()
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = msg
            };

            return View(viewModel);
        }
    }
}
