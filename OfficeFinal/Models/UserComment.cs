using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeFinal.Models
{
    public class UserComment
    {
        public int Id { get; set; }

        [AllowHtml]
        public String UserComments { get; set; }

        public int TaskId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public int StatusId { get; set; }

        [Display(Name = "Browse File")]
        public string file { get; set; }

        public virtual ICollection<FileDetail> FileDetails { get; set; }

        public virtual UserTask UserTasks { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Status Status { get; set; }

        public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}