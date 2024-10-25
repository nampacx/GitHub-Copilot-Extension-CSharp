// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class CopilotReference
    {
        public string type { get; set; }
        public Data data { get; set; }
        public string id { get; set; }
        public bool is_implicit { get; set; }
        public Metadata metadata { get; set; }
    }

    public class Data
    {
        public string type { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
        public List<CopilotReference> copilot_references { get; set; }
        public List<object> copilot_confirmations { get; set; }
        public string name { get; set; }
    }

    public class Metadata
    {
        public string display_name { get; set; }
        public string display_icon { get; set; }
        public string display_url { get; set; }
    }

    public class CopilotData
    {
        public string copilot_thread_id { get; set; }
        public List<Message> messages { get; set; }
        public object stop { get; set; }
        public int top_p { get; set; }
        public int temperature { get; set; }
        public int max_tokens { get; set; }
        public int presence_penalty { get; set; }
        public int frequency_penalty { get; set; }
        public object response_format { get; set; }
        public object copilot_skills { get; set; }
        public string agent { get; set; }
        public object tools { get; set; }
        public object functions { get; set; }
        public string model { get; set; }
    }

