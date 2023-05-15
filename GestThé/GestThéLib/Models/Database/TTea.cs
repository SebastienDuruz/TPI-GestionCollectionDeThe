using System;
using System.Collections.Generic;

namespace GestThéLib.Models.Database;

public partial class TTea
{
    public long IdTea { get; set; }

    public string? TeaName { get; set; }

    public string? TeaDescription { get; set; }

    public byte[]? TeaPrice { get; set; }

    public long? TeaQuantity { get; set; }

    public long? TeaYear { get; set; }

    public bool? TeaIsArchived { get; set; }

    public DateTime? TeaAddDate { get; set; }

    public DateTime? TeaModificationDate { get; set; }

    public long IdRegion { get; set; }

    public long IdProvider { get; set; }

    public long IdVariety { get; set; }

    public long IdType { get; set; }

    public virtual TProvider IdProviderNavigation { get; set; } = null!;

    public virtual TRegion IdRegionNavigation { get; set; } = null!;

    public virtual TType IdTypeNavigation { get; set; } = null!;

    public virtual TVariety IdVarietyNavigation { get; set; } = null!;

    public virtual ICollection<TList> IdLists { get; set; } = new List<TList>();
}
