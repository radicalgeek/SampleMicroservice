﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using Prototype.Infrastructure;
using Prototype.Logger;
using Prototype.Logic.DataEntities;
using Prototype.MessageTypes.Messages;
using MongoRepository;

namespace Prototype.Logic
{
    /// <summary>
    /// This class proccesses incoming messages, and persists them to the data store
    /// </summary>
    public class SampleBusinessLogicClass : ISampleLogic
    {
        private IRepository<SampleEntity,string> _sampleEntityRepository = new MongoRepository<SampleEntity,string>();
        private ILogger _logger;
        private IMessagePublisher _publisher;

        public SampleBusinessLogicClass(ILogger logger, IMessagePublisher publisher, IRepository<SampleEntity, string> sampleRepository)
        {
            _logger = logger;
            _publisher = publisher;
            _sampleEntityRepository = sampleRepository;

        }

        /// <summary>
        /// Route the message based on the "method" field in the incoming message.
        /// </summary>
        /// <param name="message">dynamic message object from the bus keeps contracts loosly coupled</param>
        public void RouteSampleMessage(dynamic message)
        {
            switch ((string) message.Method.ToString())
            {
                case "GET":
                    GetSampleEntities(message);
                    break;
                case "POST":
                    CreateSampleEntities(message);
                    break;
                case "PUT":
                    UpdateSampleEntities(message);
                    break;
                case "DELETE":
                    DeleteSampleEntities(message);
                    break;
            }
        }

        /// <summary>
        /// Remove a message from the data store based on Id
        /// </summary>
        /// <param name="message">The dynamic message object from the bus containing details of the item to be deleted</param>
        private void DeleteSampleEntities(dynamic message)
        {
            foreach (var need in message.Needs)
            {
                try
                {
                    _logger.Info("Removing entity {0}", need.Uuid);
                    string id = need.Uuid.ToString();
                    _sampleEntityRepository.Delete(id);
                    _logger.Info("Entity {0} Deleted", need.Uuid);
                }
                catch (Exception ex )
                {
                    _logger.Error(ex, "Unable to delete entity {0}", need.Uuid);
                }
            }
            
        }

        /// <summary>
        /// Retrive a message from the datastore
        /// </summary>
        /// <param name="message">The dynamic message from the bus containig the details of the item to retrive</param>
        private void GetSampleEntities(dynamic message)
        {
            _logger.Info("Locating SampleEntities for message: {0}", message.Uuid);
            var entities = new List<SampleEntity>();
            foreach (var need in message.Needs)
            {
                try
                {
                    string query = need.Uuid.ToString();
                    var entity = _sampleEntityRepository.GetById(query);
                    entities.Add(entity);
                    _logger.Info("SampleEntity {0} located", entity.Id);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Unable to locate SampleEntity {0}", need.Uuid.ToString());
                }
                if (entities.Count > 0)
                {
                    PublishSuccessMessage(message, entities);
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
        private void UpdateSampleEntities(dynamic message)
        {
            var entity = MapMessageToEntities(message);
            _logger.Info("Updating SampleEntities from message: {0}", message.Uuid);
            try
            {
                _sampleEntityRepository.Update(entity);
                _logger.Info("SampleEntities updated for message {0}", message.Uuid);
                PublishSuccessMessage(message, entity);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to update SampleEntities for message {0}", message.Uuid);
            }
        }

        /// <summary>
        /// Create an item in the datastore
        /// </summary>
        /// <param name="message">The dynamic message from the bus containing the details of the item to be updated</param>
        private void CreateSampleEntities(dynamic message)
        {
            var entities = MapMessageToEntities(message);
            _logger.Info("Storing new SampleEntities from message: {0}", message.Uuid);
            try
            {
                _sampleEntityRepository.Add(entities);
                foreach (SampleEntity entity in entities)
                {
                    _logger.Info("New SampleEntity {0} created", entity.Id);
                    
                }
                PublishSuccessMessage(message, entities);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to store new SampleEntities from message {0}", message.Uuid);
                //TODO: publish error message to bus
            }
        }

        private void PublishSuccessMessage(dynamic orignalMessage, List<SampleEntity> entities )
        {
            orignalMessage.ModifiedTime = DateTime.Now.ToUniversalTime();
            orignalMessage.ModifiedBy = "Sample Service";
            var solutions = new List<dynamic>();

            foreach (var sampleEntity in entities)
            {
                dynamic solution = new ExpandoObject();
                solution.Uuid = sampleEntity.Id;
                solution.NewGuidValue = sampleEntity.NewGuidValue;
                solution.NewStringValue = sampleEntity.NewStringValue;
                solution.NewIntValue = sampleEntity.NewIntValue;
                solution.NewDecimalValue = sampleEntity.NewDecimalValue;
                solutions.Add(solution);

            }
            orignalMessage.Solutions = solutions;

            _publisher.Publish(orignalMessage);

        }

        /// <summary>
        /// Map dynamic bus message to SampleEntity ready for storage
        /// </summary>
        /// <param name="message">The dynamic message from the bus containing the details of the item to be mapped</param>
        /// <returns>a SampleEntity object populated withdata from the message</returns>
        private static List<SampleEntity> MapMessageToEntities(dynamic message)
        {
            var newEntities = new List<SampleEntity>();
            foreach (var need in message.Needs)
            {
                Guid id;

                try
                {
                    id = need.Uuid;
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

        private static bool IsPropertyExist(dynamic message, string name)
        {
            return message.GetType().GetProperty(name) != null;
        }

    }
}
