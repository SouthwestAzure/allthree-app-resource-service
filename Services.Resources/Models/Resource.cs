using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Resources.Models {

    public class Resource {

        public Resource() {

            this.Technologies = new List<string>();
            this.Categories = new List<string>();
        }

        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("technologies")]
        public IEnumerable<string> Technologies { get; set; }

        [JsonProperty("categories")]
        public IEnumerable<string> Categories { get; set; }
    }
}
