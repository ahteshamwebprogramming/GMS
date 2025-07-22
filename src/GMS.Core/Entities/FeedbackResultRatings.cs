using System.ComponentModel.DataAnnotations.Schema;

namespace GMS.Core.Entities;
[Dapper.Contrib.Extensions.Table("FeedbackResultRatings")]
public class FeedbackResultRatings
{
    [Dapper.Contrib.Extensions.Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int? FeedbackResultId { get; set; }
    public int? QuestionId { get; set; }
    public int? Rating { get; set; }
    public int? IsActive { get; set; }
    
}
