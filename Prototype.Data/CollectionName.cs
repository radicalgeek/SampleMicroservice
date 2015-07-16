using System;

namespace Prototype.Data
{
     /// <summary> 
     /// Attribute used to annotate Enities with to override mongo _collection name. By default, when this attribute 
     /// is not specified, the classname will be used. 
     /// </summary> 
     [AttributeUsage(AttributeTargets.Class, Inherited = true)] 
     public class CollectionName : Attribute 
     { 
         /// <summary> 
         /// Initializes a new instance of the CollectionName class attribute with the desired name. 
         /// </summary> 
         /// <param name="value">Name of the _collection.</param> 
         public CollectionName(string value) 
         {  
             if (string.IsNullOrWhiteSpace(value)) 
                 throw new ArgumentException("Empty collectionname not allowed", "value"); 
 
             Name = value; 
         } 
 

         /// <summary> 
         /// Gets the name of the _collection. 
         /// </summary> 
         /// <value>The name of the _collection.</value> 
         public virtual string Name { get; private set; } 
     } 

}
