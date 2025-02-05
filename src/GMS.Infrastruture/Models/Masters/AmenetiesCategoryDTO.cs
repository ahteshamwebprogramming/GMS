﻿namespace GMS.Infrastructure.Models.Masters;

public class AmenetiesCategoryDTO
{
    public int Id { get; set; }

    public string? AmenetiesCategoryName { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int? CreatedBy { get; set; }
    public int? ModifiedBy { get; set; }
}
