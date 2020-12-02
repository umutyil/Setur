using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rapor.Api.Models;

namespace Rapor.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RaporController : ControllerBase
    {
        
        private readonly ILogger<RaporController> _logger;

        public RaporController(ILogger<RaporController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Tanimli raporlari listeler kisileri listeler
        /// </summary>
        /// <returns>Tanimli Rapor listesini doner</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get()
        {
            var res = new List<Raporlar>();
            using(var db  = new setur_databaseContext()){
                //rehberde kayitli kisiler alinir
                res = db.Raporlars.OrderByDescending(a => a.Tarih).ToList();
            }
            res.Add(new Raporlar(){
                Tarih=DateTime.Now,
                Durum=1,
                Icerik="Ankara"
            });
            // eger liste bos ise bulunamadi donulur
            if(res.Count == 0){
                return NotFound();
            }
            return Ok(res);
        }

        /// <summary>
        /// Tanimli bir raporun icerigini doner
        /// ID ile eslesen rapor bulunamaz ise 404 hatasi dondurur
        /// Rapor durumu hazirlandi degil ise 404 hatasi dondurur
        /// </summary>
        /// <param name="id">Rapor ID bilgisi. GUID string</param>
        /// <returns>Rapor icerigi</returns>
        [HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetRaporById(string id){
            using(var db  = new setur_databaseContext()){
                // Kisi bilgisi vt den primary key ile sorgulanir
                var res = db.Raporlars.Find(id);
                // eslesme bulunamadi
                if(res == null){
                    return NotFound();
                } else { // eslesme bulundu
                    //Rapor durumu Tamamlandi mi?
                    if(res.Durum == 1){
                        // yoksa ekle
                        return BadRequest("Rapor su an hazirlanmaktadir")
                    }
                }

                return Ok(res);
            }
            
        }


        

        
    }
}
