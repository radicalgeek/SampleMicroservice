using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private MongoRepository<SampleEntity> _sampleEntityRepository = new MongoRepository<SampleEntity>();
        private ILogger _logger;
        private IMessagePublisher _publisher;

        public SampleBusinessLogicClass(ILogger logger, IMessagePublisher publisher)
        {
            _logger = logger;
            _publisher = publisher;

        }

        /// <summary>
        /// Route the message based on the "method" field in the incoming message.
        /// </summary>
        /// <param name="message">dynamic meeage object from the bus keeps contracts loosly coupled</param>
        public void RouteSampleMessage(dynamic message)
        {
            switch ((string) message.Source)
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
            //TODO: ensure this method can handle both single items and collections
            SampleEntity entity = MapMessageToEntity(message);
            _sampleEntityRepository.Delete(e => e.Id == entity.Id);
        }

        /// <summary>
        /// Retrive a message from the datastore
        /// </summary>
        /// <param name="message">The dynamic message from the bus containig the details of the item to retrive</param>
        private void GetSampleEntities(dynamic message)
        {
            //TODO: ensure this method can handle both single items and collections
            SampleEntity entity = MapMessageToEntity(message);
            var result = _sampleEntityRepository.Where(e => e.Id == entity.Id);
        }

        /// <summary>
        /// Update an item in the datastore
        /// </summary>
        /// <param name="message">The dynamic message from the bus containing the details of the item to be updated</param>
        private void UpdateSampleEntities(dynamic message)
        {
            //TODO: ensure this method can handle both single items and collections
            var entity = MapMessageToEntity(message);
            entity.LastName = "Castle"; //TODO: remove this and make a beter example of an update

            try
            {
                _sampleEntityRepository.Update(entity);
                _logger.Info("SampleEntity {0} updateed", entity.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to update SampleEntity {0}", entity.Id);
            }
        }

        /// <summary>
        /// Create an item in the datastore
        /// </summary>
        /// <param name="message">The dynamic message from the bus containing the details of the item to be updated</param>
        private void CreateSampleEntities(dynamic message)
        {
            //TODO: ensure this method can handle both single items and collections
            var newEntity = MapMessageToEntity(message);

            try
            {
                _sampleEntityRepository.Add(newEntity);
                _logger.Info("New SampleEntity {0} created", newEntity.Id);           
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unable to store new SampleEntity {0}", newEntity.Id);       
            }
        }


        private void PublishFaliureMessage()
        {
            dynamic message = new
            {
                value1 = 1,
                value2 = 2,
                value3 = 3,
            };        
            _publisher.Publish(message);            
        }

        private void PublishSuccessMessage()
        {
            dynamic message = new
            {
                value1 = 1,
                value2 = 2,
                value3 = 3,
            };
            _publisher.Publish(message); 
        }

        /// <summary>
        /// Map dynamic bus message to SampleEntity ready for storage
        /// </summary>
        /// <param name="message">The dynamic message from the bus containing the details of the item to be mapped</param>
        /// <returns>a SampleEntity object populated withdata from the message</returns>
        private static SampleEntity MapMessageToEntity(dynamic message)
        {
            var newEntity = new SampleEntity
            {

            };
            return newEntity;
        }
    }
}
