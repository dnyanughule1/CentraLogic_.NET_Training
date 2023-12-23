using Newtonsoft.Json;

namespace CRUD_Operations.DTO
{
    public class StudentModel
    {
        [JsonProperty(PropertyName = "rollNo", NullValueHandling = NullValueHandling.Ignore)]
        public int RollNo { get; set; }

        [JsonProperty(PropertyName = "age", NullValueHandling = NullValueHandling.Ignore)]
        public int Age { get; set; }

        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }
}
