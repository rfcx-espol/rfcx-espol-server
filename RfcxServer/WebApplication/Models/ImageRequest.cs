using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using System.IO;
namespace WebApplication.Models
{
    public class ImageRequest
    {
        
        public string FechaCaptura {get;set;}
        public int IdEstacion{get;set;}
        public string Base64Image{get;set;}
       
        

    }
    
}