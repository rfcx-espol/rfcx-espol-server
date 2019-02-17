using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
namespace WebApplication.Models
{
    [BsonIgnoreExtraElements]
    public class Image
    {
        [BsonId]
        public ObjectId id {get; set;}
        public int StationId{ get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CaptureDate{ get; set; }
        public string Path{ get; set; }
        public string State{ get; set; }
        public string[] Family{ get; set; }
        

        public Image(){}
        public Image(int IdEstacion, string FechaCaptura, string Extension)
        {
            id = ObjectId.GenerateNewId();
            this.StationId = IdEstacion;
            this.CaptureDate = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(FechaCaptura)).DateTime;
            Path = id + Extension;
            State = "PENDIENTE";
        }
        
    }
    
}