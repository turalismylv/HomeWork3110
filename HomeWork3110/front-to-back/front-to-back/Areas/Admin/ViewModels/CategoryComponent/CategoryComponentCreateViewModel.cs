﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace front_to_back.Areas.Admin.ViewModels.CategoryComponent
{
    public class CategoryComponentCreateViewModel
    {
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public IFormFile Photo { get; set; }
        [Required]
        [Display(Name =("Category"))]
        public int CategoryId { get; set; }
        public List<SelectListItem>? Categories { get; set; }
    }
}
