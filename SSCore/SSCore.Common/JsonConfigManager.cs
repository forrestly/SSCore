using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace SSCore.Common
{
    public class JsonConfigManager
    {
        public static JsonConfigManager Instance
        {
            get;
            private set;
        }

        private Dictionary<string, string> _configures;
        public Dictionary<string, string> Configures
        {
            get
            {
                if (_configures == null)
                    _configures = ReadConfig();

                
                return _configures;
            }
        }

        private Dictionary<string,string> ReadConfig()
        {
            try
            {
                using (FileStream fs = new FileStream("config.json", FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        return JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

                
        }



        private JsonConfigManager()
        {
            Instance = new JsonConfigManager();
        }

    }
}
