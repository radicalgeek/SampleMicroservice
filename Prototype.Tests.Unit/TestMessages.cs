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
        public static dynamic GetTestNeedUserMessage()
        {

            dynamic need = new
            {
                UserUuid = Guid.NewGuid()
            };

            dynamic solutions = new
            {

            };

            dynamic message = new
            {
                Uuid = Guid.NewGuid(),
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

        public static dynamic GetTestCreateSampleEntityMessageWithMultiple()
        {
            var solutions = new List<dynamic>();
            var needs = new List<dynamic>();

            dynamic solution1 = new ExpandoObject();

            solution1.NewGuidValue = new Guid();
            solution1.NewStringValue = "Test";
            solution1.NewIntValue = 123;
            solution1.NewDecimalValue = 134.45M;
            solutions.Add(solution1);

            dynamic solution2 = new ExpandoObject();

            solution2.NewGuidValue = new Guid();
            solution2.NewStringValue = "Test";
            solution2.NewIntValue = 123;
            solution2.NewDecimalValue = 134.45M;
            solutions.Add(solution2);

            dynamic message = new ExpandoObject();

            message.Uuid = Guid.NewGuid();
            message.Source = "UnitTest-Service";
            message.PublishTime = DateTime.Now.ToUniversalTime();
            message.ModifiedBy = "UnitTest-Service";
            message.ModifiedTime = DateTime.Now.ToUniversalTime();
            message.Method = "POST";
            message.Needs = needs;
            message.Solutions = solutions;

            return message;
        }

        public static dynamic GetTestCreateSampleEntityMessage()
        {
            var solutions = new List<dynamic>();
            dynamic need = new ExpandoObject();

            dynamic solution1 = new ExpandoObject();

            solution1.NewGuidValue = Guid.NewGuid();
            solution1.NewStringValue = "Test";
            solution1.NewIntValue = 123;
            solution1.NewDecimalValue = 134.45M;
            solutions.Add(solution1);

            dynamic message = new ExpandoObject();

            message.Uuid = Guid.NewGuid();
            message.Source = "UnitTest-Service";
            message.PublishTime = DateTime.Now.ToUniversalTime();
            message.ModifiedBy = "UnitTest-Service";
            message.ModifiedTime = DateTime.Now.ToUniversalTime();
            message.Method = "POST";
            message.Need = need;
            message.Solutions = solutions;
            

            return message;
        }

        public static dynamic GetTestReadSampleEntityMessage()
        {
            var solutions = new List<dynamic>();
            dynamic needs = new List<dynamic>();

            dynamic need1 = new ExpandoObject();

            need1.SampleEntity = Guid.NewGuid();
            needs.Add(need1);

            dynamic message = new ExpandoObject();

            message.Uuid = Guid.NewGuid();
            message.Source = "UnitTest-Service";
            message.PublishTime = DateTime.Now.ToUniversalTime();
            message.ModifiedBy = "UnitTest-Service";
            message.ModifiedTime = DateTime.Now.ToUniversalTime();
            message.Method = "GET";
            message.Needs = needs;
            message.Solutions = solutions;


            return message;
        }

        public static dynamic GetTestUpdateSampleEntityMesssage()
        {
            var solutions = new List<dynamic>();
            var needs = new List<dynamic>();
            var id = Guid.NewGuid();

            dynamic need1 = new ExpandoObject();
            need1.SampleEntity = id;
            needs.Add(need1);

            dynamic solution1 = new ExpandoObject();

            solution1.Id = id;
            solution1.NewGuidValue = Guid.NewGuid();
            solution1.NewStringValue = "Test";
            solution1.NewIntValue = 123;
            solution1.NewDecimalValue = 134.45M;
            solutions.Add(solution1);

            dynamic message = new ExpandoObject();

            message.Uuid = Guid.NewGuid();
            message.Source = "UnitTest-Service";
            message.PublishTime = DateTime.Now.ToUniversalTime();
            message.ModifiedBy = "UnitTest-Service";
            message.ModifiedTime = DateTime.Now.ToUniversalTime();
            message.Method = "PUT";
            message.Needs = needs;
            message.Solutions = solutions;

            return message;
        }
    }
}
