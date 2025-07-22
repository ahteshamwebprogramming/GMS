using GMS.Infrastructure.Models.Masters;

namespace GMS.Infrastructure.ViewModels.Masters
{
    public class FeedbackViewModel
    {
        public FeedbackDTO? Feedback { get; set; }
        public List<FeedbackDTO>? Feedbacks { get; set; }
    }
}
