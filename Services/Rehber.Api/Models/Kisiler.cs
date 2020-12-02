using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Rehber.Api.Models
{
    public partial class Kisiler
    {
        public Kisiler()
        {
            Iletisimbilgileris = new HashSet<Iletisimbilgileri>();
        }

        public Guid KisiId { get; set; }
        [Required]
        public string Ad { get; set; }
        [Required]
        public string Soyad { get; set; }
        [Required]
        public string Firma { get; set; }

        public virtual ICollection<Iletisimbilgileri> Iletisimbilgileris { get; set; }
    }
}
