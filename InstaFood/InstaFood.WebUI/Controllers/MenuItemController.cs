﻿using InstaFood.DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace InstaFood.WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public MenuItemController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { data = _unitOfWork.MenuItem.GetAll(null, null, "Category, FoodType") });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                var objFromDb = _unitOfWork.MenuItem.GetFirstOrDefault(u => u.Id == id);

                if (objFromDb == null)
                {
                    return Json(new { success = false, message = "Error while deleting, object not found in database." });
                }

                var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, objFromDb.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                _unitOfWork.MenuItem.Remove(objFromDb);
                _unitOfWork.Save();
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = "Error while deleting!" + ex.ToString() });
            }

            return Json(new { success = true, message = "Delete success." });
        }
    }
}