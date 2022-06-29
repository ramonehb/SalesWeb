using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SalesWebMvc.Models;

namespace SalesWebMvc.Controllers
{
    public class DepartmentsController : Controller
    {
        public IActionResult Index()
        {
            List<Department> departments = new List<Department>();

            departments.Add(new Department { Id = 1, Name = "Tecnologia da Informação" });
            departments.Add(new Department { Id = 2, Name = "RH" });
            
            return View(departments);
        }
    }
}
