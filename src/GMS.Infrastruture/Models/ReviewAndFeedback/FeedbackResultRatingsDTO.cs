namespace GMS.Infrastructure.Models.ReviewAndFeedback;

public class FeedbackResultRatingsDTO
{
    public int Id { get; set; }
    public int? FeedbackResultId { get; set; }
    public int? QuestionId { get; set; }
    public int? Rating { get; set; }
    public int? IsActive { get; set; }
}
