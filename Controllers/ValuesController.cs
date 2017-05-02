using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SlimGis.MapKit.Layers;
using SlimGis.MapKit.Symbologies;
using SlimGis.MapKit.WebApi;
using SlimGis.MapKit.Geometries;

namespace HelloMap.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet]
        [Route("{z}/{x}/{y}")]
        public IActionResult GetImage(int z, int x, int y)
        {
            ShapefileLayer countriesLayer = new ShapefileLayer(@"AppData\countries-900913.shp");
            countriesLayer.Styles.Add(new FillStyle(GeoColor.FromHtml("#AAFFDF3E"), GeoColors.White));

            MapModel mapModel = new MapModel(GeoUnit.Meter);
            mapModel.Layers.Add(countriesLayer);


            return new XyzTileResult(mapModel, x, y, z);
        }
    }
}
