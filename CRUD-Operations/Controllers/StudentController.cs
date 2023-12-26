using Azure;
using CRUD_Operations.DTO;
using CRUD_Operations.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.ComponentModel;
using System.Drawing.Text;
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
        public string ContainerName = "TestContainer1";

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
                //if roll number is aldready exist in database data not added
                bool isRollNumberExists = await IsRollNumberExistsAsync(studentModel.RollNo);

                if (isRollNumberExists)
                {
                    return BadRequest("Roll number already exists in the database.");
                }
                
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

            async Task<bool> IsRollNumberExistsAsync(int RollNo)
            {
                List<int> RollNumber = new List<int>();
                {
                    RollNumber.Add(RollNo);
                }
                return RollNumber.Contains(RollNo);
            }

        }

        [HttpPost]
        public IActionResult GetStudentByUId(string uId)
        {
            try
            {
                Student student = _container.GetItemLinqQueryable<Student>(true).Where(q => q.DocumentType == "student" && q.UId == uId).AsEnumerable().FirstOrDefault();

                // Reverse Mapping 
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
        [HttpPut("{uId}")]
        public async Task<IActionResult> UpdateStudentByUId(string uId, UpdatedStudentModel updatedStudentModel)
        {
            try
            {
                // Retrieve the student by UId
                Student existingStudent = _container.GetItemLinqQueryable<Student>(true).Where(q => q.DocumentType == "student" && q.UId == uId).AsEnumerable().FirstOrDefault();

                if (existingStudent == null)
                {
                    return NotFound($"Student with UId '{uId}' not found.");
                }

                // Increment the version by one
                existingStudent.Version++;

                // Update the properties of the existing student
                existingStudent.Name = updatedStudentModel.Name;
                existingStudent.Age = updatedStudentModel.Age;
                existingStudent.RollNo = updatedStudentModel.RollNo;

                // Use ReplaceItemAsync to update the student in CosmosDB
                Student replaceResponse = await _container.ReplaceItemAsync<Student>(existingStudent, uId);

                return Ok(replaceResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Data Update Failed: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStudentByUId(string uId, string partitionKey)
        {
            try
            { 
                // Use DeleteItemAsync to delete by Id and partition key
                var deleteResponse = await _container.DeleteItemAsync<Student>(
                   uId, new PartitionKey(partitionKey)
                );
                return Ok(deleteResponse);

            }
            catch (Exception ex)
            {
                return BadRequest($"Data Delete Failed: {ex.Message}");
            }
        }


        private Container GetContainer() // DRY
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
