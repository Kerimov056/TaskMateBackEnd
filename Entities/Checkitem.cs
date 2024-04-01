﻿using TaskMate.Entities.Common;

namespace TaskMate.Entities;

public class Checkitem:BaseEntity
{
    public string Text { get; set; }
    public DateTime? DueDate { get; set; }
    public bool Check { get; set; } = false;
    //Rellations
    public Checklist Checklist { get; set; }
    public Guid ChecklistId { get; set; }
}
