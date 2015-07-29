using System;
using System.Collections.Generic;
using Prototype.Logic.DataEntities;

namespace Prototype.Tests.Helpers
{
    public static class TestEntities
    {
        public static List<SampleEntity> SetUpSampleEntities(dynamic message)
        {
            var entitys = new List<SampleEntity>();

            var entity1 = new SampleEntity()
            {
                CreatedDate = DateTime.Now.AddMonths(-1).ToUniversalTime(),
                Id = message.Needs[0].SampleUuid.ToString(),
                NewDecimalValue = 123.45M,
                NewGuidValue = Guid.NewGuid(),
                NewIntValue = 1,
                NewStringValue = "Test",
                UpdatedDate = DateTime.Now.AddHours(-6).ToUniversalTime()
            };
            var entity2 = new SampleEntity()
            {
                CreatedDate = DateTime.Now.AddMonths(-1).ToUniversalTime(),
                Id = Guid.NewGuid().ToString(),
                NewDecimalValue = 123.45M,
                NewGuidValue = Guid.NewGuid(),
                NewIntValue = 1,
                NewStringValue = "Test",
                UpdatedDate = DateTime.Now.AddHours(-6).ToUniversalTime()
            };
            entitys.Add(entity1);
            entitys.Add(entity2);
            return entitys;
        }

        public static List<SampleEntity> SetUpSampleEntityFromMessage(dynamic message)
        {
            var entitys = new List<SampleEntity>();

            var entity1 = new SampleEntity()
            {
                CreatedDate = DateTime.Now.AddMonths(-1).ToUniversalTime(),
                Id = message.Needs[0].SampleUuid.ToString(),
                NewDecimalValue = message.Needs[0].NewDecimalValue,
                NewGuidValue = message.Needs[0].NewGuidValue,
                NewIntValue = message.Needs[0].NewIntValue,
                NewStringValue = message.Needs[0].NewStringValue,
                UpdatedDate = DateTime.Now.AddMonths(-1).ToUniversalTime()
            };
            entitys.Add(entity1);
            
            return entitys;
        }
    }
}
