﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MakingBot.config
{
    internal class JSONReader
    {
        public string Token { get; set; }
        public string Prefix { get; set; }

        public async Task ReadJSON()
        {
            using (StreamReader sr = new("config.json"))
            {
                string json = await sr.ReadToEndAsync();
                JSONStructure data = JsonConvert.DeserializeObject<JSONStructure>(json);

                this.Token = data.Token;
                this.Prefix = data.Prefix;
            }
        }
    }

    internal sealed class JSONStructure
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
    }
}
