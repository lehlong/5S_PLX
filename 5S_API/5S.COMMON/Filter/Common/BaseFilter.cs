﻿namespace Common
{
    public class BaseFilter
    {
        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 50;

        public string? KeyWord { get; set; }

        public bool? IsActive { get; set; }

        public string? SortColumn { get; set; }

        public bool? IsDescending { get; set; }

        public List<string>? Fields { get; set; }

    }


    public class EvaluateFilter
    {
        public string? SurveyId { set; get; }
        public string? KiKhaoSatId { set; get; }
        public string? DoiTuongId { set; get; }
        public List<string>? LstData; 

    }
}
