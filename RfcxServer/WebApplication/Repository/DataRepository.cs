using WebApplication.DbModels;
using WebApplication.IRepository;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;


namespace WebApplication.Repository
{
    public class DataRepository : IDataRepository
    {
        private readonly ObjectContext _context =null; 

        public DataRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 


        public async Task<IEnumerable<Data>> Get()
        {
            try
            {
                return await _context.Datas.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Data> Get(string id)
        {
            var filter = Builders<Data>.Filter.Eq("DataId", id);

            try
            {
                return await _context.Datas.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Data> Get(int id)
        {
            var filter = Builders<Data>.Filter.Eq("Id", id);

            try
            {
                return await _context.Datas.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Data>> GetByStation(int StationId)
        {
            try
            {
                var filter =Builders<Data>.Filter.Eq("StationId", StationId);
                return await _context.Datas.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Data>> GetLasts()
        {
            try
            {
                List<int> dataIdList=new List<int>();
                List<Data> data;
                List<Sensor> sensor;
                List<Station> stations= _context.Stations.Find(_=>true).ToList();
                var stationCount= stations.Count;
                var sensorCount=0;
                for (int i=0;i<stationCount;i++){
                    var filter=Builders<Sensor>.Filter.Eq("StationId",stations[i].Id);
                    sensor=_context.Sensors.Find(filter).ToList();
                    sensorCount=sensor.Count;
                    for(int j=0;j<sensorCount;j++){
                        var filter1 =Builders<Data>.Filter.Eq("StationId", stations[i].Id) & Builders<Data>.Filter.Eq("SensorId", sensor[j].Id);
                        data=_context.Datas.Find(filter1).ToList();
                        if(data.Count>0){
                            var dataId=data.Count-1;
                            dataIdList.Add(data[dataId].Id);
                        }
                        
                    }
                }
                var filter2=Builders<Data>.Filter.In("Id", dataIdList);
                return await _context.Datas.Find(filter2).ToListAsync();
            
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Data>> GetByStationSensor(int StationId, int SensorId)
        {
            try
            {
                var filter =Builders<Data>.Filter.Eq("StationId", StationId) & Builders<Data>.Filter.Eq("SensorId", SensorId);
                return await _context.Datas.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Data>> GetByStationSensorTimestamp(int StationId, int SensorId, long StartTimestamp,
            long EndTimestamp)
        {
            try{
                
                var filter =Builders<Data>.Filter.Eq("StationId", StationId) & 
                Builders<Data>.Filter.Eq("SensorId", SensorId) & 
                Builders<Data>.Filter.Gte("Timestamp", StartTimestamp) &
                Builders<Data>.Filter.Lte("Timestamp", EndTimestamp);
                

                return await _context.Datas.Find(filter).ToListAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<IEnumerable<Data>> GetByStationSensorTimestampFilter(int StationId, int SensorId, 
            long StartTimestamp, long EndTimestamp, string Filter, int FilterValue)
        {
            if(Filter==null  || FilterValue==0){
                return null;
            }

            var finalFilter=0;
            switch (Filter)
            {
                case "Hours":
                    var hourTimestamp=3600;
                    finalFilter = hourTimestamp*FilterValue;
                    break;
                case "Days":
                    var daysTimestamp=86400;
                    finalFilter = daysTimestamp*FilterValue;
                    break;
                
                case "Weeks":
                    var weeksTimestamp=604800;
                    finalFilter = weeksTimestamp*FilterValue;
                    break;
                case "Months":
                    var monthsTimestamp=2592000;
                    finalFilter = monthsTimestamp*FilterValue;
                    break;
                case "Year":
                    var yearsTimestamp=31536000;
                    finalFilter = yearsTimestamp*FilterValue;
                    break;
                default:
                    finalFilter=0;
                    break;
            }

            try{   
                var filter =Builders<Data>.Filter.Eq("StationId", StationId) & 
                Builders<Data>.Filter.Eq("SensorId", SensorId) & 
                Builders<Data>.Filter.Gte("Timestamp", StartTimestamp) &
                Builders<Data>.Filter.Lte("Timestamp", EndTimestamp);
                var DataFilteredList =_context.Datas.Find(filter).ToList();
                var Count= DataFilteredList.Count;
                if(Count==0){
                    return null;
                }
                var valueTemp=0;
                int valueCountTemp=0;
                var StartTimestampTemp=StartTimestamp;
                var DataAgreggateList= new List<Data>();
                for (int i=0;i<Count;i++){
                    if(i==0){
                        if(!((Convert.ToInt64(DataFilteredList[i].Timestamp)>=Convert.ToInt64(StartTimestampTemp)) && 
                        (Convert.ToInt64(DataFilteredList[i].Timestamp)<(Convert.ToInt64(StartTimestampTemp)+finalFilter)))){
                            StartTimestampTemp=DataFilteredList[i].Timestamp;                              
                    }
                       
                    }
                    if((Convert.ToInt64(DataFilteredList[i].Timestamp)>=Convert.ToInt64(StartTimestampTemp)) && 
                    (Convert.ToInt64(DataFilteredList[i].Timestamp)<(Convert.ToInt64(StartTimestampTemp)+finalFilter))){
                        Console.Write(DataFilteredList[i].Value);
                        valueTemp+=Convert.ToInt32(DataFilteredList[i].Value);
                        valueCountTemp++;                        
                    }else{
                        if(valueCountTemp>0){
                            Data DataTemp= new Data();
                            DataTemp.StationId=DataFilteredList[0].StationId;
                            DataTemp.SensorId=DataFilteredList[0].SensorId;
                            DataTemp.Type=DataFilteredList[0].Type;
                            DataTemp.Units=DataFilteredList[0].Units;
                            DataTemp.Location=DataFilteredList[0].Location;
                            DataTemp.Value=Convert.ToString(Convert.ToInt32(valueTemp/valueCountTemp));
                            DataTemp.Timestamp=StartTimestampTemp;
                            valueTemp=0;
                            valueCountTemp=0;
                            DataAgreggateList.Add(DataTemp);
                            StartTimestampTemp+=finalFilter;
                            DataTemp=null;
                        }

                    }
                }
                var DataResult=(IEnumerable<Data>) DataAgreggateList;

                return DataResult;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<Data> GetLastByStationSensor(int StationId, int SensorId)
        {
            
            try
            {
                int id=0;
                var filter =Builders<Data>.Filter.Eq("StationId", StationId) & Builders<Data>.Filter.Eq("SensorId", SensorId);
                List<Data> data=_context.Datas.Find(filter).ToList();
                if(data.Count>0){
                    id=data[data.Count-1].Id;
                }
                
                var filter2=Builders<Data>.Filter.Eq("Id", id);
                return await _context.Datas.Find(filter2).FirstOrDefaultAsync();
            
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Data> GetLastByStation(int StationId)
        {
            
            try
            {
                int id=0;
                var filter =Builders<Data>.Filter.Eq("StationId", StationId);
                List<Data> data=_context.Datas.Find(filter).ToList();
                if(data.Count>0){
                    id=data[data.Count-1].Id;
                }
                
                var filter2=Builders<Data>.Filter.Eq("Id", id);
                return await _context.Datas.Find(filter2).FirstOrDefaultAsync();
            
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Data> Get(int StationId, int SensorId, int DataId)
        {
            var filter = Builders<Data>.Filter.Eq("Id", DataId) & Builders<Data>.Filter.Eq("StationId", StationId) & Builders<Data>.Filter.Eq("SensorId", SensorId);

            try
            {
                return await _context.Datas.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task Add(Data item)
        {
            try
            {
                item.Id=_context.Datas.Find(_ => true).ToList().Count+1;
                await _context.Datas.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Remove(string id)
        {
            try
            {
                DeleteResult actionResult = await _context.Datas.DeleteOneAsync(
                        Builders<Data>.Filter.Eq("DataId", id));

                return actionResult.IsAcknowledged 
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public async Task<bool> Update(string id, Data item)
        {
            try
            {
                ReplaceOneResult actionResult 
                    = await _context.Datas
                                    .ReplaceOneAsync(n => n.DataId.Equals(id)
                                            , item
                                            , new UpdateOptions { IsUpsert = true });
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveAll()
        {
            try
            {
                DeleteResult actionResult 
                    = await _context.Datas.DeleteManyAsync(new BsonDocument());

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
    }
}
