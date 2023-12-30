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
        /*
        public string URI = "https://localhost:8081";
        public string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public string DatabaseName = "PersonalTaskManagerDB";
        public string ContainerName = "Task";
        */
        public Container container;

        public TaskController()
        {
            container = GetContainer();
        }
        [HttpPost]
        public async Task<IActionResult> AddTask(TaskModel taskModel)
        {
            try
            {

                Tasks task = new Tasks();
                task.TaskName = taskModel.TaskName;
                task.TaskDescription = taskModel.TaskDescription;

                //Assign mandetory field
                task.Id = Guid.NewGuid().ToString();
                task.UId = task.Id;
                task.DocumentType = "task";
                task.CreatedBy = "Dnyaneshwar's UId"; //UId of who created this data
                task.CreatedByName = "Dnyaneshwar";
                task.CreatedOn = DateTime.Now;
                task.UpdatedBy = "Dnyaneshwar's UId";
                task.UpdatedByName = "Dnyaneshwar";
                task.UpdatedOn = DateTime.Now;
                task.Version = 1;
                task.Active = true;
                task.Archieved = false;

                //Add data to Database
                task = await container.CreateItemAsync(task);

                //Return model to UI
                TaskModel model = new TaskModel();
                model.TaskName = task.TaskName;
                model.TaskDescription = task.TaskDescription;
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest($"Task Added Failed: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult GetAllTask()
        {
            try
            {
                // Get all tasks
                var tasks = container.GetItemLinqQueryable<Tasks>(true).Where(q => q.DocumentType == "task" && q.Archieved == false && q.Active == true).AsEnumerable().ToList();

                // Map all task data
                List<TaskModel> taskModelList = tasks.Select(task => new TaskModel
                {
                    TaskName = task.TaskName,
                    TaskDescription = task.TaskDescription
                }).ToList();

                return Ok(taskModelList);
            }
            catch (Exception ex)
            {
                return BadRequest($"Task Get Failed: {ex.Message}");
            }
        }


        [HttpGet]
        public IActionResult GetStudentByUId(string uId)
        {
            try
            {
                Tasks task = container.GetItemLinqQueryable<Tasks>(true).Where(q => q.DocumentType == "task" && q.UId == uId).AsEnumerable().FirstOrDefault();

                // Reverse Mapping 
                var taskModel = new TaskModel();
                taskModel.TaskName = task.TaskName;
                taskModel.TaskDescription = task.TaskDescription;

                return Ok(taskModel);

            }
            catch (Exception ex)
            {

                return BadRequest("Task Get Failed");
            }
        }
        [HttpPut("{uId}")]
        public async Task<IActionResult> UpdateTaskByUId(string uId, UpdatedTaskModel updatedTaskModel)
        {
            try
            {
                // Retrieve the task by UId
                Tasks existingTask = container.GetItemLinqQueryable<Tasks>(true).Where(q => q.DocumentType == "task" && q.UId == uId).AsEnumerable().FirstOrDefault();

                if (existingTask == null)
                {
                    return NotFound($"Task with UId '{uId}' not found.");
                }
                existingTask.Version++;
                existingTask.TaskName = updatedTaskModel.TaskName;
                existingTask.TaskDescription = updatedTaskModel.TaskDescription;

                // Use ReplaceItemAsync to update the task in CosmosDB
                Tasks replaceResponse = await container.ReplaceItemAsync<Tasks>(existingTask, uId);

                return Ok(replaceResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Data Update Failed: {ex.Message}");
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteTaskByUId(string taskUId)
        {
            try
            {
                // Get the task by UId
                var task = container.GetItemLinqQueryable<Tasks>(true).Where(q => q.UId == taskUId && q.DocumentType == "task" && q.Archieved == false && q.Active == true).AsEnumerable().FirstOrDefault();

                if (task == null)
                {
                    return NotFound($"Task with UId '{taskUId}' not found.");
                }

                // Mark the task as inactive
                task.Active = false;

                // Use DeleteItemAsync to delete the task in CosmosDB
                await container.DeleteItemAsync<Tasks>(task.Id, new PartitionKey(task.DocumentType));

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest($"Task Deletion Failed: {ex.Message}");
            }
        }

        private Container GetContainer()
        {
            string URI = Environment.GetEnvironmentVariable("cosmos-Uri");
            string PrimaryKey = Environment.GetEnvironmentVariable("Primary-Key");
            string DatabaseName = Environment.GetEnvironmentVariable("Database");
            string ContainerName = Environment.GetEnvironmentVariable("Container");

            CosmosClient cosmosClient = new CosmosClient(URI, PrimaryKey);
            Database db = cosmosClient.GetDatabase(DatabaseName);
            Container container = db.GetContainer(ContainerName);
            return container;
        }

    }

}
