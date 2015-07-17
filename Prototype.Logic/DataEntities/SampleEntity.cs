using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoRepository;

namespace Prototype.Logic.DataEntities
{
    /// <summary>
    /// Sample POCO object for persisting data the data store (MongoDB)
    /// This class will be different for each instanciated microservice, and reflects the "schema" of the 
    /// data store. 
    /// </summary>
    public class SampleEntity : Entity
    {
        public string Id { get; set; }
    }
}


