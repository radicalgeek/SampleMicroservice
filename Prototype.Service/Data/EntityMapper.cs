using System;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;
using Prototype.Service.Data.Model;

namespace Prototype.Service.Data
{
    public class EntityMapper
    {
        /// <summary>
        /// Map dynamic bus message to SampleEntity ready for storage
        /// </summary>
        /// <param name="message">The dynamic message from the bus containing the details of the item to be mapped</param>
        /// <returns>a SampleEntity object populated withdata from the message</returns>
        public static List<SampleEntity> MapMessageToEntities(dynamic message)
        {
            var newEntities = new List<SampleEntity>();
            foreach (var need in message.Needs)
            {
                Guid id;

                try
                {
                    id = need.SampleUuid;
                }
                catch (RuntimeBinderException)
                {
                    id = Guid.NewGuid();
                }

                DateTime createdDate;
                try
                {
                    createdDate = need.CreatedDate;
                }
                catch (RuntimeBinderException)
                {
                    createdDate = DateTime.Now.ToUniversalTime();
                } 

                var newEntity = new SampleEntity
                {
                    Id = id.ToString(),
                    CreatedDate = createdDate,
                    UpdatedDate = DateTime.Now.ToUniversalTime(),
                    NewGuidValue = need.NewGuidValue,
                    NewStringValue = need.NewStringValue,
                    NewIntValue = need.NewIntValue,
                    NewDecimalValue = need.NewDecimalValue

                };
                newEntities.Add(newEntity);
            }
            return newEntities;
        }
    }
}
