using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities
{
    [Dapper.Contrib.Extensions.Table("GuestDocumentAttachments")]
    public class GuestDocumentAttachments
    {
        [Dapper.Contrib.Extensions.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? ReferenceId { get; set; }
        public string? AttachmentName { get; set; }

        public string? AttachmentPath { get; set; }
        public string? AttachmentExtension { get; set; }
        public string? AttachmentType { get; set; }
        public string? AttachmentSource { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }        
    }
}
