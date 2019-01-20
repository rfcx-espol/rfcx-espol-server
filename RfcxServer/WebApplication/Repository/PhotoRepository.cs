using WebApplication.DbModels;
using WebApplication.IRepository;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using MongoDB.Driver;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;

namespace WebApplication.Repository
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly ObjectContext _context =null; 
        private readonly ISpecieRepository _SpecieRepository;

        public PhotoRepository(IOptions<Settings> settings, ISpecieRepository SpecieRepository)
        {
            _context = new ObjectContext(settings);
            _SpecieRepository = SpecieRepository;
        } 

        public List<Photo> Get()
        {
            try
            {
                return _context.Photos.Find(_ => true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Photo> Get(string id)
        {
            var filter = Builders<Photo>.Filter.Eq("Id", id);

            try
            {
                return await _context.Photos.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Photo> Get(int id)
        {
            var filter = Builders<Photo>.Filter.Eq("Id", id);

            try
            {
                return await _context.Photos.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Add(Photo item)
        {
            try
            {
                var list=_context.Photos.Find(_ => true).ToList();
                if(item.Id==0){
                    if(list.Count>0){
                        list.Sort();
                        item.Id=list[list.Count-1].Id+1;
                    }else{
                        item.Id=1;
                    }
                }else{
                    for (int i=0;i<list.Count;i++){
                        if(item.Id==list[i].Id){
                            return false;
                        }
                    }
                }
    
                 _context.Photos.InsertOne(item);
                 return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Remove(int id)
        {
            try
            {
                var filtro_foto = Builders<Photo>.Filter.Eq("Id", id);
                DeleteResult actionResult = _context.Photos.DeleteOne(filtro_foto);
                List<Specie> especies = _context.Species.Find(_ => true).ToList();
                foreach(Specie especie in especies) {
                    int indice = especie.Gallery.FindIndex(f => f.Id == id);
                    if(indice != -1) {
                        especie.Gallery.RemoveAt(indice);
                        _SpecieRepository.UpdateGallery(especie.Id, especie.Gallery);                        
                        DirectoryInfo DI = new DirectoryInfo(Constants.RUTA_ARCHIVOS_IMAGENES_ESPECIES + especie.Id.ToString() + "/");
                        foreach (var file in DI.GetFiles()) {
                            string[] extension = (file.Name).Split('.');
                            if(extension[0] == id.ToString()) {
                                string fileAddress = DI.FullName + file.Name;
                                File.Delete(fileAddress);
                                break;
                            }
                        }
                        break;
                    }
                }           
                return actionResult.IsAcknowledged 
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.Write("error: " + ex.StackTrace + "\n");
                Console.Write("error: " + ex.Message + "\n");
                throw ex;
            }
        }

        public bool Update(int PhotoId, Photo item)
        {
            try
            {
                ReplaceOneResult actionResult 
                    = _context.Photos
                                    .ReplaceOne(n => n.Id.Equals(PhotoId)
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

        public bool UpdateDescription(int id, string description)
        {
            var filter = Builders<Photo>.Filter.Eq("Id", id);
            Photo photo  = _context.Photos.Find(filter).FirstOrDefault();
            photo.Description = description;
            return Update(id, photo);
        }

    }

}