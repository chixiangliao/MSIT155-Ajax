﻿using prjAjaxHomework.Models;

namespace AjaxDemoV2.Models.DTO
{
    public class SpotsPagingDTO
    {
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public List<SpotImagesSpot>? SpotsResult { get; set; }
    }
}