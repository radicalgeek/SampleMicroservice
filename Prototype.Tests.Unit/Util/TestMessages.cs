using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prototype.Tests.Unit
{
    public static class TestMessages
    {
        //correct
        public static dynamic GetTestNeedUserMessage()
        {

            dynamic need = new
            {
                UserSampleUuid = Guid.NewGuid()
            };

            dynamic solutions = new
            {

            };

            dynamic message = new
            {
                SampleUuid = Guid.NewGuid(),
                Source = "UnitTest-Service",
                PublishTime = DateTime.Now.ToUniversalTime(),
                ModifiedBy = "UnitTest-Service",
                ModifiedTime = DateTime.Now.ToUniversalTime(),
                Method = "GET",
                Need = need,
                Solutions = solutions
            };

            return message;
        }

        //correct
        public static dynamic GetTestCreateSampleEntityMessageWithMultiple()
        {
            var solutions = new List<dynamic>();
            var needs = new List<dynamic>();

            dynamic need1 = new ExpandoObject();

            need1.SampleUuid = new Guid();
            need1.NewStringValue = "Test1";
            need1.NewIntValue = 123;
            need1.NewDecimalValue = 134.45M;
            need1.NewGuidValue = Guid.NewGuid();
            needs.Add(need1);

            dynamic need2 = new ExpandoObject();

            need2.SampleUuid = new Guid();
            need2.NewStringValue = "Test2";
            need2.NewIntValue = 123;
            need2.NewDecimalValue = 134.45M;
            need2.NewGuidValue = Guid.NewGuid();
            needs.Add(need2);

            dynamic message = new ExpandoObject();

            message.SampleUuid = Guid.NewGuid();
            message.Source = "UnitTest-Service";
            message.PublishTime = DateTime.Now.ToUniversalTime();
            message.ModifiedBy = "UnitTest-Service";
            message.ModifiedTime = DateTime.Now.ToUniversalTime();
            message.Method = "POST";
            message.Needs = needs;
            message.Solutions = solutions;

            return message;
        }

        //correct
        public static dynamic GetTestCreateSampleEntityMessage()
        {
            var solutions = new List<dynamic>();
            dynamic needs = new List<dynamic>();

            dynamic need = new ExpandoObject();

            need.NewGuidValue = Guid.NewGuid();
            need.NewStringValue = "Test";
            need.NewIntValue = 123;
            need.NewDecimalValue = 134.45M;
            needs.Add(need);

            dynamic message = new ExpandoObject();

            message.SampleUuid = Guid.NewGuid();
            message.Source = "UnitTest-Service";
            message.PublishTime = DateTime.Now.ToUniversalTime();
            message.ModifiedBy = "UnitTest-Service";
            message.ModifiedTime = DateTime.Now.ToUniversalTime();
            message.Method = "POST";
            message.Needs = needs;
            message.Solutions = solutions;
            

            return message;
        }

        //correct
        public static dynamic GetTestReadSampleEntityMessage()
        {
            var solutions = new List<dynamic>();
            dynamic needs = new List<dynamic>();

            dynamic need1 = new ExpandoObject();

            need1.SampleUuid = Guid.NewGuid();
            needs.Add(need1);

            dynamic message = new ExpandoObject();

            message.SampleUuid = Guid.NewGuid();
            message.Source = "UnitTest-Service";
            message.PublishTime = DateTime.Now.ToUniversalTime();
            message.ModifiedBy = "UnitTest-Service";
            message.ModifiedTime = DateTime.Now.ToUniversalTime();
            message.Method = "GET";
            message.Needs = needs;
            message.Solutions = solutions;


            return message;
        }

        //correct
        public static dynamic GetTestUpdateSampleEntityMesssage()
        {
            var solutions = new List<dynamic>();
            var needs = new List<dynamic>();
            var id = Guid.NewGuid();

            dynamic need1 = new ExpandoObject();

            need1.SampleUuid = id;
            need1.NewGuidValue = Guid.NewGuid();
            need1.NewStringValue = "Test";
            need1.NewIntValue = 123;
            need1.NewDecimalValue = 134.45M;
            needs.Add(need1);

            dynamic message = new ExpandoObject();

            message.SampleUuid = Guid.NewGuid();
            message.Source = "UnitTest-Service";
            message.PublishTime = DateTime.Now.ToUniversalTime();
            message.ModifiedBy = "UnitTest-Service";
            message.ModifiedTime = DateTime.Now.ToUniversalTime();
            message.Method = "PUT";
            message.Needs = needs;
            message.Solutions = solutions;

            return message;
        }

        //correct
        public static dynamic GetTestDeleteSampleEntityMesssage()
        {
            var solutions = new List<dynamic>();
            var needs = new List<dynamic>();
            var id = Guid.NewGuid();

            dynamic need1 = new ExpandoObject();
            need1.SampleUuid = id;
            needs.Add(need1);

            dynamic solution1 = new ExpandoObject();
            solutions.Add(solution1);

            dynamic message = new ExpandoObject();

            message.SampleUuid = Guid.NewGuid();
            message.Source = "UnitTest-Service";
            message.PublishTime = DateTime.Now.ToUniversalTime();
            message.ModifiedBy = "UnitTest-Service";
            message.ModifiedTime = DateTime.Now.ToUniversalTime();
            message.Method = "DELETE";
            message.Needs = needs;
            message.Solutions = solutions;

            return message;
        }

        public static dynamic GetTestVersion1Message()
        {
            var solutions = new List<dynamic>();
            var needs = new List<dynamic>();
            var versions = new List<dynamic>();
            var id = Guid.NewGuid();

            dynamic version1 = new ExpandoObject();
            version1.Service = "SampleService";
            version1.Version = 1;
            versions.Add(version1);

            dynamic need1 = new ExpandoObject();
            need1.SampleUuid = id;
            needs.Add(need1);

            dynamic solution1 = new ExpandoObject();
            solutions.Add(solution1);

            dynamic message = new ExpandoObject();

            message.SampleUuid = Guid.NewGuid();
            message.Source = "UnitTest-Service";
            message.PublishTime = DateTime.Now.ToUniversalTime();
            message.ModifiedBy = "UnitTest-Service";
            message.ModifiedTime = DateTime.Now.ToUniversalTime();
            message.Method = "DELETE";
            message.Needs = needs;
            message.Solutions = solutions;
            message.CompatibleServiceVersions = versions;

            return message;
        }
    }
}
