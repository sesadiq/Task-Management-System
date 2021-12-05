using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeFinal.Models
{
    public class FileDetail
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public int? TaskId { get; set; }
        
        public virtual UserTask UserTask { get; set; }

        public virtual UserComment UserComment { get; set; }
    }
}