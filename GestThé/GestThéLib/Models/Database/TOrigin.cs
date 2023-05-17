using System;
using System.Collections.Generic;

namespace GestThéLib.Models.Database;

public partial class TOrigin
{
    public long IdOrigin { get; set; }

    public string? OriginCountry { get; set; }

    public virtual ICollection<TRegion> TRegions { get; set; } = new List<TRegion>();
}
