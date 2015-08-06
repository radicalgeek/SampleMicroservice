using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using MongoRepository;
using Prototype.Logger;
using Prototype.Service.Data.Model;
using Prototype.Service.Publish;
using IEnvironment = Prototype.Service.Settings.IEnvironment;

namespace Prototype.Service.Data
{
    public class DataOperations : IDataOperations
    {
        private readonly IRepository<SampleEntity, string> _sampleEntityRepository = new MongoRepository<SampleEntity, string>();
        private readonly ILogger _logger;
        private readonly IMessagePublisher _publisher;
        private readonly IEnvironment _environment;

        public DataOperations(ILogger logger, IMessagePublisher publisher, IRepository<SampleEntity, string> sampleRepository, IEnvironment environment)
        {
            _logger = logger;
            _publisher = publisher;
            _sampleEntityRepository = sampleRepository;
            _environment = environment;
        }

        /// <summary>
        /// Remove a message from the data store based on Id
        /// </summary>
        /// <param name="message">The dynamic message object from the bus containing details of the item to be deleted</param>
        public void DeleteSampleEntities(dynamic message)
        {
            foreach (var need in message.Needs)
            {
                try
                {
                    _logger.Info("Event=\"Removing entity\" Entity=\"{0}\"", need.SampleUuid);
                    string id = need.SampleUuid.ToString();
                    var stopwatch = GetStopwatch();
                    _sampleEntityRepository.Delete(id);
                    stopwatch.Stop();
                    _logger.Info("Event=\"Deleted entity\" Entity=\"{0}\" ResponseTime=\"{1}\"", need.SampleUuid, stopwatch.Elapsed);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Event=\"Faild to remove entity\" Entity=\"{0}\"", need.SampleUuid);
                }
            }
        }

        /// <summary>
        /// Retrive a message from the datastore
        /// </summary>
        /// <param name="message">The dynamic message from the bus containig the details of the item to retrive</param>
        public void GetSampleEntities(dynamic message)
        {
            var entities = new List<SampleEntity>();
            foreach (var need in message.Needs)
            {
                try
                {
                    string query = need.SampleUuid.ToString();
                    var stopwatch = GetStopwatch();
                    _logger.Info("Event=\"Retrived Entities\" Entity=\"{0}\" MessageUuid=\"{1}\"", query, message.SampleUuid);
                    var entity = _sampleEntityRepository.GetById(query);
                    stopwatch.Stop();
                    entities.Add(entity);
                    _logger.Info("Event=\"Retrived entity\" Entity=\"{0}\" ResponseTime=\"{1}\"", entity.Id,stopwatch.Elapsed);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Event=\"Unable to retrived entity\" Entity=\"{0}\"", need.SampleUuid.ToString());
                }
                if (entities.Count > 0)
                {
                    //TODO: retrive routing key from config
                    PublishSuccessMessage(message, entities, "A.B");
                }
                else
                {
                    var errorMessage = String.Format("Event=\"Unable to retrived entity\" Entity=\"{0}\"",
                        need.SampleUuid.ToString());
                                  
                    //TODO: retrive routing key from config
                    PublishErrorMessage(message, errorMessage, "A.B");
                }

            }

        }

        /// <summary>
        /// Update an item in the datastore
        /// </summary>
        /// <param name="message">The dynamic message from the bus containing the details of the item to be updated</param>
        public void UpdateSampleEntities(dynamic message)
        {
            var entity = EntityMapper.MapMessageToEntities(message);
            try
            {
                var stopwatch = GetStopwatch();
                _logger.Info("Event=\"Updating Entities\" MessageUuid=\"{0}\"", message.SampleUuid);
                _sampleEntityRepository.Update(entity);
                _logger.Info("Event=\"Updated entity\" Entity=\"{0}\" ResponseTime=\"{1}\"", message.SampleUuid, stopwatch.Elapsed);
                //TODO: retrive routing key from config
                PublishSuccessMessage(message, entity, "A.B");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Event=\"Unable to update entity\" Entity=\"{0}\"", message.SampleUuid);
                var errorMessage = String.Format("Event=\"Unable to update entity\" Entity=\"{0}\"",
                        message.SampleUuid.ToString());

                //TODO: retrive routing key from config
                PublishErrorMessage(message, errorMessage, "A.B");
            }
        }

        /// <summary>
        /// Create an item in the datastore
        /// </summary>
        /// <param name="message">The dynamic message from the bus containing the details of the item to be updated</param>
        public void CreateSampleEntities(dynamic message)
        {
            var entities = EntityMapper.MapMessageToEntities(message);
            try
            {
                var stopwatch = GetStopwatch();
                _logger.Info("Event=\"Creating Entities\" MessageUuid=\"{0}\"", message.SampleUuid);
                _sampleEntityRepository.Add(entities);
                _logger.Info("Event=\"Finished Creating Entities\" MessageUuid=\"{0}\" ResponseTime=\"{1}\"", message.SampleUuid, stopwatch.Elapsed);
                stopwatch.Stop();
                foreach (SampleEntity entity in entities)
                {
                    _logger.Info("Event=\"Created entity\" Entity=\"{0}\"", entity.Id);

                }
                //TODO: retrive routing key from config
                PublishSuccessMessage(message, entities, "A.B");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Event=\"Unable to create entity\" Entity=\"{0}\"", message.SampleUuid);

                var errorMessage = String.Format("Event=\"Unable to create entity\" Entity=\"{0}\"",
                        message.SampleUuid.ToString());

                //TODO: retrive routing key from config
                PublishErrorMessage(message, errorMessage, "A.B");
            }
        }

        public void PublishSuccessMessage(dynamic orignalMessage, List<SampleEntity> entities, string topic)
        {
            orignalMessage.ModifiedTime = DateTime.Now.ToUniversalTime();
            orignalMessage.ModifiedBy = _environment.GetServiceName();
            var solutions = new List<dynamic>();

            foreach (var sampleEntity in entities)
            {
                dynamic solution = new ExpandoObject();
                solution.SampleUuid = sampleEntity.Id;
                solution.NewGuidValue = sampleEntity.NewGuidValue;
                solution.NewStringValue = sampleEntity.NewStringValue;
                solution.NewIntValue = sampleEntity.NewIntValue;
                solution.NewDecimalValue = sampleEntity.NewDecimalValue;
                solutions.Add(solution);

            }
            orignalMessage.Solutions = solutions;

            var serializedMessage = Newtonsoft.Json.JsonConvert.SerializeObject(orignalMessage);

            _publisher.Publish(serializedMessage, topic);
        }

        public void PublishErrorMessage(dynamic orignalMessage, string errorMessage, string topic)
        {
            orignalMessage.ModifiedTime = DateTime.Now.ToUniversalTime();
            orignalMessage.ModifiedBy = _environment.GetServiceName();

            var errors = new List<dynamic>();
            dynamic error = new ExpandoObject();
            error.Source = _environment.GetServiceName();
            error.Message = errorMessage;
            errors.Add(error);

            orignalMessage.Errors = errors;

            var serializedMessage = Newtonsoft.Json.JsonConvert.SerializeObject(orignalMessage);

            _publisher.Publish(serializedMessage, topic);
        }

        private Stopwatch GetStopwatch()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            return stopwatch;
        }
    }
}
