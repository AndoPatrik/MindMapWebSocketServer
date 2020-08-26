using MongoDB.Driver;
using MongoDB.Bson;
using MindMapWebSocketServer.Models;
using System;

namespace MindMapWebSocketServer.Utility
{
    public class MongoDb
    {
        private IMongoDatabase _db;
        private IMongoCollection<Project> _projectCollection;

        public MongoDb()
        {
            var client = new MongoClient("mongodb+srv://admin:Cookiemonsta88@mindmap-bsvon.azure.mongodb.net/MindMapDb?retryWrites=true&w=majority");
            _db = client.GetDatabase("MindMapDb");
            _projectCollection = _db.GetCollection<Project>("Project");
        }

        public Project LoadProjectByName(string projectName) 
        {
            /*
            Project p = _projectCollection.Find<Project>(p => p.ProjectName == projectName).First();

            if (p == null)
            {
                return new Project();
            }

            return p;*/

            try
            {
              return  _projectCollection.Find<Project>(p => p.ProjectName == projectName).First();
            }
            catch (Exception)
            {
                return null;
                throw;
            }


        }

        public Project LoadProjectById (ObjectId id ) 
        {
            var project = _projectCollection.Find<Project>(p => p.Id == id).First();
            return project;
        }

        public void UpdateRecord(string collection, object obj) 
        {
            //TODO: To be fileld
        }
    }
}
