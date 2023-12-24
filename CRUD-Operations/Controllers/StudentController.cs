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

        [HttpPost]
        public async Task<IActionResult> DeleteStudentByUId(string uId)
        {
            try
            {
                var partitionKey = new PartitionKey("student");

                // Use DeleteItemAsync to delete by Id and partition key
                var deleteResponse = await _container.DeleteItemAsync<Student>(
                    partitionKey: partitionKey,
                    id: uId
                );

                if (deleteResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return Ok($"Student with UId '{uId}' deleted successfully.");
                }
                else if (deleteResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound($"Student with UId '{uId}' not found.");
                }
                else
                {
                    return StatusCode((int)deleteResponse.StatusCode, $"Failed to delete student with UId '{uId}'.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Data Delete Failed: {ex.Message}");
            }
        }

        [HttpPut("{uId}")]
        public async Task<IActionResult> UpdateStudentByUId(string uId, UpdatedStudentModel updatedStudentModel)
        {
            try
            {
                // Retrieve the student by UId
                Student existingStudent = _container.GetItemLinqQueryable<Student>(true)
                    .Where(q => q.DocumentType == "student" && q.UId == uId)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (existingStudent == null)
                {
                    return NotFound($"Student with UId '{uId}' not found.");
                }

                // Update the properties of the existing student
                existingStudent.Name = updatedStudentModel.Name;
                existingStudent.Age = updatedStudentModel.Age;
                existingStudent.RollNo = updatedStudentModel.RollNo;

                existingStudent.UpdatedOn = DateTime.Now;
                existingStudent.UpdatedByName = "Updated User";  // Replace with the actual user updating the record
                existingStudent.UpdatedBy = "Updated User's UId";  // Replace with the actual user's UId updating the record

                existingStudent.Version++; // Increment the version

                // Use ReplaceItemAsync to update the student in CosmosDB
                var replaceResponse = await _container.ReplaceItemAsync<Student>(
                    partitionKey: new PartitionKey(existingStudent.DocumentType),
                    id: existingStudent.Id,
                    item: existingStudent
                );

                if (replaceResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Reverse Mapping 
                    var updatedStudentModel = new StudentModel
                    {
                        Name = existingStudent.Name,
                        Age = existingStudent.Age,
                        RollNo = existingStudent.RollNo
                    };

                    return Ok(updatedStudentModel);
                }
                else
                {
                    return StatusCode((int)replaceResponse.StatusCode, $"Failed to update student with UId '{uId}'.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Data Update Failed: {ex.Message}");
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
