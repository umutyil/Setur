using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Rapor.Api.Models
{
    public partial class Raporlar
    {
        public Guid RaporId { get; set; }
        public int? Durum { get; set; }
        public DateTime? Tarih { get; set; }
        [Required]
        public string Icerik { get; set; }
    }
}
