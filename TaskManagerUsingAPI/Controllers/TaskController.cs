using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Security.Cryptography.X509Certificates;
using TaskManagerUsingAPI.DTO;
using TaskManagerUsingAPI.Entity;
using Container = Microsoft.Azure.Cosmos.Container;

namespace TaskManagerUsingAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        public string URI = "https://localhost:8081";
        public string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public string DatabaseName = "PersonalTaskManagerDB";
        public string ContainerName = "TaskContainer";

        public readonly Container _container; // null 
        public TaskController()
        {
            _container = GetContainer();
        }

        [HttpPost]
        public async Task<IActionResult> AddTask(TaskModel taskModel)
        {
            TaskDetails taskDetails = new TaskDetails();
            taskDetails.TaskName = taskModel.TaskName;
            taskDetails.TaskDescription = taskModel.TaskDescription;

            taskDetails.Id = Guid.NewGuid().ToString();
            taskDetails.UId = taskDetails.Id;
            taskDetails.DocumentType = "TaskDB";

            taskDetails.CreatedOn = DateTime.Now;
            taskDetails.CreatedByName = "Dnyaneshwar";
            taskDetails.CreatedBy = "Dnyaneshwar's UId";

            taskDetails.UpdatedOn = DateTime.Now;
            taskDetails.UpdatedByName = "Dnyaneshwar";
            taskDetails.UpdatedBy = "Dnyaneshwar's UId";

            taskDetails.Version = 1;
            taskDetails.Active = true;
            taskDetails.Archieved = false;

            TaskDetails response = await _container.CreateItemAsync(taskDetails);

            //reverse mapping
            taskModel.TaskName = response.TaskName;
            taskModel.TaskDescription = response.TaskDescription;

            return Ok(taskModel);
        }

        [HttpPost]
        public async Task<IActionResult> GetAllTask()
        {
            var tasks = _container.GetItemLinqQueryable<TaskDetails>(true).AsEnumerable().ToList();
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> GetTaskByUId(string uId)
        {
            try
            {
                TaskDetails tasks = _container.GetItemLinqQueryable<TaskDetails>(true).Where(q => q.DocumentType == "TaskDB" && q.UId == uId).AsEnumerable().FirstOrDefault();
                var viewTasks = new TaskModel();
                viewTasks.TaskName = tasks.TaskName;
                viewTasks.TaskDescription = tasks.TaskDescription;
                return Ok(viewTasks);
            }

            catch (Exception ex)
            {
                return BadRequest("Invalid Id");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTask(TaskDetails taskDetails)
        {
            var existingtask = _container.GetItemLinqQueryable<TaskDetails>(true).Where(q => q.DocumentType == "TaskDB" && q.UId == taskDetails.UId && q.Active == true && q.Archieved == false).AsEnumerable().FirstOrDefault();
            existingtask.Archieved = true;
            await _container.ReplaceItemAsync(existingtask, existingtask.Id);

            existingtask.Id = Guid.NewGuid().ToString();
            existingtask.UpdatedOn = DateTime.Now;
            existingtask.UpdatedByName = "Dnyaneshwar";
            existingtask.UpdatedBy = "Dnyaneshwar's UId";
            existingtask.Version = existingtask.Version + 1;
            existingtask.Active = true;
            existingtask.Archieved = false;

            existingtask.TaskName = taskDetails.TaskName;
            existingtask.TaskDescription = taskDetails.TaskDescription;

            existingtask = await _container.CreateItemAsync(existingtask);

            TaskDetails details = new TaskDetails();
            details.UId = existingtask.UId;
            details.TaskName = existingtask.TaskName;
            details.TaskDescription = existingtask.TaskDescription;

            return Ok(details);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask(string taskId)
        {
            var deletetask = _container.GetItemLinqQueryable<TaskDetails>(true).Where(q => q.DocumentType == "TaskDB" && q.UId == taskId && q.Active == true && q.Archieved == false).AsEnumerable().FirstOrDefault();
            deletetask.Active = false;
            await _container.ReplaceItemAsync(deletetask, deletetask.Id);

            return Ok(true);

        }

        private Container GetContainer()
        {
            CosmosClient cosmosclient = new CosmosClient(URI, PrimaryKey);
            // step 2 Connect with Our Database
            Database databse = cosmosclient.GetDatabase(DatabaseName);
            // step 3 Connect with Our Container 
            Container container = databse.GetContainer(ContainerName);

            return container;
        }
    } 

}
