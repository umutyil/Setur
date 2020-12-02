using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Rehber.Api.Models
{
    public partial class Iletisimbilgileri
    {
        public Guid IletisimbilgiId { get; set; }
        [Required]
        public Guid? KisiId { get; set; }
        [Required]
        public int? BilgiTipi { get; set; }
        [Required]
        public string Icerik { get; set; }

        public virtual Kisiler Kisi { get; set; }
    }
}
