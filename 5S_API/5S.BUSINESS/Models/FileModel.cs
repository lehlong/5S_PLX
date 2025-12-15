using Microsoft.AspNetCore.Http;
using PLX5S.CORE.Entities.BU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLX5S.BUSINESS.Models
{
    public class FileModel
    {
        public string? KinhDo { get; set; }
        public string? ViDo { get; set; }
        public IFormFile file { get; set; }
        public string Type { get; set; }
        public string FileName { get; set; }
        public string? TieuChiCode { get; set; }
        public string? EvaluateHeaderCode { get; set; }
    }

}