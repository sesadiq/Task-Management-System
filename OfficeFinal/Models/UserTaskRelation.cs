namespace OfficeFinal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserTaskRelation")]
    public partial class UserTaskRelation
    {
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public int TaskId { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual UserTask UserTask { get; set; }
    }
}
