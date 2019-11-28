using System;
using System.Collections.Generic;

namespace DataAnalyzer
{
    [Serializable]
    public class ResultModel
    {
        public ResultModel()
        {
            Data = new List<NamedResult>();
            TableData = new List<NamedResult>();
        }
        public string DocClass { get; set; }
        public int NumberOfPages { get; set; }
        public List<NamedResult> Data { get; set; }
        public List<NamedResult> TableData { get; set; }
    }
}
