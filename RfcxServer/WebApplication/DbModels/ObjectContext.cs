using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using WebApplication.Models;
using System.Collections.Generic;
using WebApplication.IRepository;



namespace WebApplication.DbModels
{
    public class ObjectContext
    {
        public IConfigurationRoot Configuration { get; }
        private IMongoDatabase _database = null;

        public ObjectContext(IOptions<Settings> settings){
            Configuration=settings.Value.iConfigurationRoot;
            settings.Value.ConnectionString=Configuration.GetSection("MongoConnection:ConnectionString").Value;
            settings.Value.Database=Configuration.GetSection("MongoConection:Database").Value;
        
            var client = new MongoClient(settings.Value.ConnectionString);
            if(client!=null){
                _database=client.GetDatabase(settings.Value.Database);
            }
        }

        public IMongoCollection<Alerta> Alertas
        {
            get
            {
                return _database.GetCollection<Alerta>("Alerta");
            }
        }

        public IMongoCollection<AlertaConfiguracion> AlertaConfiguracions
        {
            get
            {
                return _database.GetCollection<AlertaConfiguracion>("AlertaConfiguracion");
            }
        }

        public IMongoCollection<Audio> Audios
        {
            get
            {
                return _database.GetCollection<Audio>("Audio");
            }
        }

        public IMongoCollection<Dispositivo> Dispositivos
        {
            get
            {
                return _database.GetCollection<Dispositivo>("Dispositivo");
            }
        }

        public IMongoCollection<Etiqueta> Etiquetas
        {
            get
            {
                return _database.GetCollection<Etiqueta>("Etiqueta");
            }
        }

        public IMongoCollection<Sensor> Sensors
        {
            get
            {
                return _database.GetCollection<Sensor>("Sensor");
            }
        }


    } 
}