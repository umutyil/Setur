using System;
using System.Collections.Generic;

#nullable disable

namespace Rapor.Api.Models
{
    public partial class Raporlar
    {
        public Guid RaporId { get; set; }
        public int? Durum { get; set; }
        public DateTime? Tarih { get; set; }
        public string Icerik { get; set; }
    }
}
