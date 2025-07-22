namespace GMS.Infrastructure.ViewModels.Guests;

public class MedicalSoultion_GuestCheckList
{
    public int GuestId { get; set; }
    public int opt { get; set; }
    public int ID { get; set; }
    public string? checklist { get; set; }
    public int IsChecked { get; set; }

    public bool? EarlyCheckOut { get; set; }
    public string? Reason { get; set; }
}
