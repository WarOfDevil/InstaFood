﻿using InstaFood.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InstaFood.WebUI
{
    [Authorize(Roles = StaticDetails.ManagerRole)]
    public class CategoryModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}