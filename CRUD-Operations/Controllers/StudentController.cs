using Azure;
using CRUD_Operations.DTO;
using CRUD_Operations.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using Container = Microsoft.Azure.Cosmos.Container;

namespace CRUD_Operations.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        public string URI = "https://localhost:8081";
        public string PrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public string DatabaseName = "SampleDB";
        public string ContainerName = "TestContainer";

        public readonly Container _container; // null 
        public StudentController()
        {
            _container = GetContainer();
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(StudentModel studentModel)
        {
            try
            {
                Student studentEntity = new Student();
                // Mapping 
                studentEntity.Name = studentModel.Name;
                studentEntity.Age = studentModel.Age;
                studentEntity.RollNo = studentModel.RollNo;


                // mandatory feilds 
                studentEntity.Id = Guid.NewGuid().ToString(); // 16 didit hex code
                studentEntity.UId = studentEntity.Id;
                studentEntity.DocumentType = "student";

                studentEntity.CreatedOn = DateTime.Now;
                studentEntity.CreatedByName = "Dnyaneshwar";
                studentEntity.CreatedBy = "Dnyaneshwar's UId";

                studentEntity.UpdatedOn = DateTime.Now;
                studentEntity.UpdatedByName = "Dnyaneshwar";
                studentEntity.UpdatedBy = "Dnyaneshwar's UId";

                studentEntity.Version = 1;
                studentEntity.Active = true;
                studentEntity.Archieved = false;  // Not Accesible to System

                Student resposne = await _container.CreateItemAsync(studentEntity);

                // Reverse MApping 
                studentModel.Name = resposne.Name;
                studentModel.Age = resposne.Age;
                studentModel.RollNo = resposne.RollNo;
                return Ok(studentModel);

            }
            catch (Exception ex)
            {

                return BadRequest("Data Adding Failed" + ex);
            }
        }

        [HttpPost]
        public IActionResult GetStudentByUId(string uId)
        {
            try
            {
                Student student = _container.GetItemLinqQueryable<Student>(true).Where(q => q.DocumentType == "student" && q.UId == uId).AsEnumerable().FirstOrDefault();

                // Reverse MApping 
                var studentModel = new StudentModel();
                studentModel.Name = student.Name;
                studentModel.Age = student.Age;
                studentModel.RollNo = student.RollNo;
                return Ok(studentModel);

            }
            catch (Exception ex)
            {

                return BadRequest("Data Get Failed");
            }
        }

        [HttpPost]
        public IActionResult GetAllStudent()
        {
            try
            {

                var studentList = _container.GetItemLinqQueryable<Student>(true).Where(q => q.DocumentType == "student").AsEnumerable().ToList();
                return Ok(studentList);    // 200 - succes 

            }
            catch (Exception ex)
            {

                return BadRequest("Data Get Failed");
            }
        }



        private Container GetContainer() // DRY
        {
            CosmosClient cosmosclient = new CosmosClient(URI, PrimaryKey);
            // step 2 Connect with Our Databse
            Database databse = cosmosclient.GetDatabase(DatabaseName);
            // step 3 Connect with Our Container 
            Container container = databse.GetContainer(ContainerName);

            return container;
        }
    }
}
