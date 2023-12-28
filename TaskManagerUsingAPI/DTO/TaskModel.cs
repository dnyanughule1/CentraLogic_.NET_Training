using Newtonsoft.Json;

namespace TaskManagerUsingAPI.DTO
{
    public class TaskModel
    {
        //TaskModel work with UI

        [JsonProperty(PropertyName = "taskName", NullValueHandling = NullValueHandling.Ignore)]
        public string TaskName { get; set; }

        [JsonProperty(PropertyName = "taskDescription", NullValueHandling = NullValueHandling.Ignore)]
        public string TaskDescription { get; set; }
    }
}
