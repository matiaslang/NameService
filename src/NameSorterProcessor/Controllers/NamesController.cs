using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NameSorterProcessor.Commands;
using NameSorterProcessor.Models;

namespace NameSorterProcessor.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("[controller]")]
    public class NamesController : ControllerBase
    {
        
        public NamesController(){}
        
        [HttpGet]
        public async Task<IActionResult> Get() {
            var result = await HandleNameEventCommand.ProcessNameListGet();
            return result.OperationSucceeded
                ? (IActionResult) new OkObjectResult(result.NameList)
                : new BadRequestObjectResult("Getting the namelist was unsuccessful");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NameModel [] names) {
            if (names == null) return new BadRequestObjectResult("No nameslist found");
            var eventResult = await HandleNameEventCommand.ProcessNamePostingEvent(names);
            return eventResult.OperationSucceeded
                ? (IActionResult) new OkObjectResult("New list posted successfully")
                : new BadRequestObjectResult("Something went wrong when posting new list to db");
        }
        
        [HttpPost("newlist")]
        public async Task<IActionResult> PostNewList([FromBody]HttpRequestMessage names) {
            return new ObjectResult(Request.Body);
        }
        
        public string jsonMockResolver()
        {
            StreamReader r = new StreamReader("./Mocks/names.json");
            string jsonString = r.ReadToEnd();
            //NameModel[] m = JsonConvert.DeserializeObject<NameModel[]>(jsonString);
            return jsonString;
        }

    }
    
}