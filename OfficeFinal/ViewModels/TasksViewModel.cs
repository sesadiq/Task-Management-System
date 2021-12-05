using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OfficeFinal.ViewModels
{
    public class TasksViewModel
    {
        public int TaskId { get; set; }

        [StringLength(128)]
        public string CreatorId { get; set; }

        [StringLength(250)]
        public string Message { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CreateDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DeadLine { get; set; }

        public int StatusId { get; set; }


    }
}