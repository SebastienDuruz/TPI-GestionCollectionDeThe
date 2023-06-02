using System;
using System.Collections.Generic;

namespace GestTheLib.Models.Database;

public partial class TField
{
    public long IdField { get; set; }

    public string? FieldName { get; set; }

    public virtual ICollection<TList> IdLists { get; set; } = new List<TList>();
}
