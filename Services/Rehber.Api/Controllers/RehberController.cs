using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rehber.Api.Models;

namespace Rehber.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RehberController : ControllerBase
    {
        
        private readonly ILogger<RehberController> _logger;

        public RehberController(ILogger<RehberController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Rehberdeki kisileri listeler
        /// </summary>
        /// <returns>Rehberdeki kisiler listesini doner</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get()
        {
            var res = new List<Kisiler>();
            using(var db  = new setur_databaseContext()){
                //rehberde kayitli kisiler alinir
                res = db.Kisilers.OrderBy(a => a.Soyad).ThenBy(a => a.Ad).ToList();
            }
            res.Add(new Kisiler(){
                Ad="Umut",
                Soyad="Yildirim"
            });
            // eger liste bos ise bulunamadi donulur
            if(res.Count == 0){
                return NotFound();
            }
            return Ok(res);
        }

        /// <summary>
        /// Rehberdeki bir kisinin iletisim bilgileri ile beraber getirir
        /// ID ile eslesen kisi bulunamaz ise 404 hatasi dondurur
        /// </summary>
        /// <param name="id">Kisi ID bilgisi. GUID string</param>
        /// <returns>Iletisim bilgileri olan kisi nesnesi</returns>
        [HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetKisiById(string id){
            using(var db  = new setur_databaseContext()){
                // Kisi bilgisi vt den primary key ile sorgulanir
                var res = db.Kisilers.Find(id);
                // eslesme bulunamadi
                if(res == null){
                    return NotFound();
                } else { // eslesme bulundu
                    //detay bilgileri var mi?
                    if(res.Iletisimbilgileris.Count == 0){
                        // yoksa ekle
                        res.Iletisimbilgileris = db.Iletisimbilgileris.Where(a => a.KisiId == res.KisiId).ToList();
                    }
                }

                return Ok(res);
            }
            
        }

    }
}
