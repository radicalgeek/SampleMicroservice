using System;
using System.Collections.Generic;
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
                    _logger.Info("Removing entity {0}", need.SampleUuid);
                    string id = need.SampleUuid.ToString();
                    _sampleEntityRepository.Delete(id);
                    _logger.Info("Entity {0} Deleted", need.SampleUuid);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Unable to delete entity {0}", need.SampleUuid);
                }
            }

        }

        /// <summary>
        /// Retrive a message from the datastore
        /// </summary>
        /// <param name="message">The dynamic message from the bus containig the details of the item to retrive</param>
        public void GetSampleEntities(dynamic message)
        {
            _logger.Info("Locating SampleEntities for message: {0}", message.SampleUuid);
            var entities = new List<SampleEntity>();
            foreach (var need in message.Needs)
            {
                try
                {
                    string query = need.SampleUuid.ToString();
                    var entity = _sampleEntityRepository.GetById(query);
                    entities.Add(entity);
                    _logger.Info("SampleEntity {0} located", entity.Id);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Unable to locate SampleEntity {0}", need.SampleUuid.ToString());
                }
                if (entities.Count > 0)
                {
                    PublishSuccessMessage(message, entities, "A.B");
                }
                else
                {
                    //TODO:Publish fail message
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
            _logger.Info("Updating SampleEntities from message: {0}", message.SampleUuid);
            try
            {
                _sampleEntityRepository.Update(entity);
                _logger.Info("SampleEntities updated for message {0}", message.SampleUuid);
                PublishSuccessMessage(message, entity, "A.B");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to update SampleEntities for message {0}", message.SampleUuid);
            }
        }

        /// <summary>
        /// Create an item in the datastore
        /// </summary>
        /// <param name="message">The dynamic message from the bus containing the details of the item to be updated</param>
        public void CreateSampleEntities(dynamic message)
        {
            var entities = EntityMapper.MapMessageToEntities(message);
            _logger.Info("Storing new SampleEntities from message: {0}", message.SampleUuid);
            try
            {
                _sampleEntityRepository.Add(entities);
                foreach (SampleEntity entity in entities)
                {
                    _logger.Info("New SampleEntity {0} created", entity.Id);

                }
                PublishSuccessMessage(message, entities, "A.B");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to store new SampleEntities from message {0}", message.SampleUuid);
                //TODO: publish error message to bus
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
    }
}
