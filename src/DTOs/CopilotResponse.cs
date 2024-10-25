using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsCopilot.DTOs
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Choice
    {
        public int index { get; set; }
        public ContentFilterOffsets content_filter_offsets { get; set; }
        public ContentFilterResults content_filter_results { get; set; }
        public Delta delta { get; set; }
    }

    public class ContentFilterOffsets
    {
        public int check_offset { get; set; }
        public int start_offset { get; set; }
        public int end_offset { get; set; }
    }

    public class ContentFilterResults
    {
        public Error error { get; set; }
        public Hate hate { get; set; }
        public SelfHarm self_harm { get; set; }
        public Sexual sexual { get; set; }
        public Violence violence { get; set; }
    }

    public class Delta
    {
        public string content { get; set; }
    }

    public class Error
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class Hate
    {
        public bool filtered { get; set; }
        public string severity { get; set; }
    }

    public class CopilotResponse
    {
        public List<Choice> choices { get; set; }
        public int created { get; set; }
        public string id { get; set; }
        public string model { get; set; }
    }

    public class SelfHarm
    {
        public bool filtered { get; set; }
        public string severity { get; set; }
    }

    public class Sexual
    {
        public bool filtered { get; set; }
        public string severity { get; set; }
    }

    public class Violence
    {
        public bool filtered { get; set; }
        public string severity { get; set; }
    }


}
