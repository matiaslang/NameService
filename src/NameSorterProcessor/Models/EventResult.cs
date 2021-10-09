using System.Collections.Generic;

namespace NameSorterProcessor.Models {
    public class EventResult {
        public bool OperationSucceeded { get; set; }
        public string Exception { get; set; }
        public int ExceptionCode { get; set; }
        public List<NameModel> NameList { get; set; }
    }
}
