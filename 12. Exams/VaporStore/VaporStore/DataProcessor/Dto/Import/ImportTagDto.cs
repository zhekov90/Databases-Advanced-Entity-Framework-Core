
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace VaporStore.DataProcessor.Dto.Import
{
    [JsonArray("Tags")]
    public class ImportTagDto
    {

        public string Name { get; set; }

    }
}
