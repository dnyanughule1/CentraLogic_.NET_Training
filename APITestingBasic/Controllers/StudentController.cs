using APITestingBasic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APITestingBasic.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private List<PersonalDetails> myDataBase = new List<PersonalDetails>();
        public StudentController()
        {

        }


        [HttpPost]
        public IActionResult AddData(PersonalDetails myDetails)
        {
            try
            {
                myDataBase.Add(myDetails);
                return Ok("Data Added Successfully");    // 200 - succes 

            }
            catch (Exception ex)
            {

                return BadRequest("Data Adding Failed");
            }
        }

        [HttpPost]
        public IActionResult GetData()
        {
            try
            {
                return Ok(myDataBase);    // 200 - succes 

            }
            catch (Exception ex)
            {

                return BadRequest("Data Get Failed");
            }
        }
    }
}
