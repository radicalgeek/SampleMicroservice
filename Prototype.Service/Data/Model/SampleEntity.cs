using System;
using MongoRepository;

namespace Prototype.Service.Data.Model
{
    /// <summary>
    /// Sample POCO object for persisting data the data store (MongoDB)
    /// This class will be different for each instanciated microservice, and reflects the "schema" of the 
    /// data store. 
    /// </summary>
    public class SampleEntity : Entity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid NewGuidValue { get; set; }
        public string NewStringValue { get; set; }
        public int NewIntValue { get; set; }
        public decimal NewDecimalValue { get; set; }
    }
}


