using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using NameSorterProcessor.Commands;
using NameSorterProcessor.Models;

namespace NameSorterProcessor.Controllers
{
    [Authorize]
    [Microsoft.AspNetCore.Mvc.Route("[controller]")]
    public class NamesController : ControllerBase
    {
        
        public NamesController(){}
        
        [HttpGet]
        [EnableCors("default")]
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
        
        [HttpPost("single")]
        public async Task<IActionResult> PostNewList([FromBody]NameModel name) {
            try {
                var eventResult = await HandleNameEventCommand.ProcessSinglePostingEvent(name);    
                return eventResult.OperationSucceeded 
                    ? (IActionResult) new OkObjectResult($"{name.name} was updated successfully") 
                    :  new BadRequestObjectResult($"There was an error processing update of {name.name}. Error message: {eventResult.Exception}");
            }
            catch (Exception e) {
                return new BadRequestObjectResult(
                    $"There was a problem with updating {name.name}. Error message: {e.Message}");
            }
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