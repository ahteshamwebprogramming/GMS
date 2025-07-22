using GMS.Infrastructure.Models.Masters;
using GMS.Infrastructure.Models.ReviewAndFeedback;

namespace GMS.Infrastructure.ViewModels.ReviewAndFeedback;

public class FeedbackViewModel
{
    public FeedbackResultsDTO? FeedbackResult { get; set; }
    public List<FeedbackResultsDTO>? FeedbackResultList { get; set; }
    public List<FeedbackDTO>? FeedbackAttributeList { get; set; }
    public int? GuestId { get; set; }
}
