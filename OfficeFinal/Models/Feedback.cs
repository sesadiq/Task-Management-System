using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OfficeFinal.Models
{
    [Table("Feedback")]
    public partial class Feedback
    {
        public int Id { get; set; }

        [Column("Feedback")]
        [StringLength(250)]
        public string UserFeedback { get; set; }

        public int UserCommentId { get; set; }

        public int UserTaskId { get; set; }

        [StringLength(128)]
        public string AspNetUserId { get; set; }

        public virtual UserComment UserComment { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual UserTask UserTask { get; set; }


    }

}