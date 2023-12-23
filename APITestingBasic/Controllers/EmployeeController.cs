using System.Threading.Tasks;
using APITestingBasic.DTO;
using APITestingBasic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace APITestingBasic.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EmployeeController : ControllerBase
    {
        private const string URI = "https://localhost:8081";
        private const string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private const string DatabaseName = "PersonalTaskManagerDB";
        private const string ContainerName = "Sample";

        [HttpPost]
        public async Task<EmployeeDetails> AddEmployee(EmployeeDetailsModel employeeDetailsModel)
        {
            //Database Connection
            // step 1 Connect With Account 
            CosmosClient cosmosclient = new CosmosClient(URI, PrimaryKey);
            // step 2 Connect with Our Databse
            Database databse = cosmosclient.GetDatabase(DatabaseName);
            // step 3 Connect with Our Container 
            Container container = databse.GetContainer(ContainerName);

            //Entity
            EmployeeDetails employeeDetails = new EmployeeDetails();

            //Mapping DTO to Entity
            employeeDetails.id = employeeDetailsModel.id;
            employeeDetails.EmployeeName = employeeDetailsModel.EmployeeName;
            employeeDetails.EmployeePhone = employeeDetailsModel.EmployeePhone;
            employeeDetails.EmployeeState = employeeDetailsModel.EmployeeState;
            employeeDetails.EmployeeId = employeeDetailsModel.EmployeeId;


            //Save record to database
            var response = await container.CreateItemAsync(employeeDetails);
            //return
            return response;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmp(string employeeID)
        {

            //Database Connection
            // step 1 Connect With Account 
            CosmosClient cosmosclient = new CosmosClient(URI, PrimaryKey);
            // step 2 Connect with Our Databse
            Database databse = cosmosclient.GetDatabase(DatabaseName);
            // step 3 Connect with Our Container 
            Container container = databse.GetContainer(ContainerName);

            var response = container.GetItemLinqQueryable<EmployeeDetails>(true).Where(b => b.EmployeeId == employeeID).AsEnumerable().FirstOrDefault();


            return Ok(response);

        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmp()
        {

            //Database Connection
            // step 1 Connect With Account 
            CosmosClient cosmosclient = new CosmosClient(URI, PrimaryKey);
            // step 2 Connect with Our Databse
            Database databse = cosmosclient.GetDatabase(DatabaseName);
            // step 3 Connect with Our Container 
            Container container = databse.GetContainer(ContainerName);

            var liostresponse = container.GetItemLinqQueryable<EmployeeDetails>(true).AsEnumerable().ToList();


            return Ok(liostresponse);

        }
    }
}
