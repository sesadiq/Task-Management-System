namespace OfficeFinal.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    [Table("UserTask")]
    public partial class UserTask
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserTask()
        {
            UserTaskRelations = new HashSet<UserTaskRelation>();
            Feedbacks = new HashSet<Feedback>();
        }

        [Key]
        public int TaskId { get; set; }

        [StringLength(128)]
        public string CreatorId { get; set; }

        [StringLength(250)]
        [AllowHtml]
        public string Message { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString ="{0:dd MMM yyyy}")]
        public DateTime? CreateDate { get; set; }

        [Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DeadLine { get; set; }

        [Display(Name = "Browse File")]
        public string files { get; set; }

        public int StatusId { get; set; }

        public int? TaskTypeId { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Status Status { get; set; }

        public virtual TaskType TaskType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserTaskRelation> UserTaskRelations { get; set; }

        public virtual ICollection<FileDetail> FileDetails { get; set; }

        public virtual ICollection<UserComment> UserComments { get; set; }

        public virtual ICollection<Feedback> Feedbacks { get; set; }

    }
}
