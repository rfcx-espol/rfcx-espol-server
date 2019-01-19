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
    public class SpecieRepository : ISpecieRepository
    {
        private readonly ObjectContext _context =null; 

        public SpecieRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 

        public List<Specie> Get()
        {
            try
            {
                return _context.Species.Find(_ => true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Specie> Get(string id)
        {
            var filter = Builders<Specie>.Filter.Eq("SpecieId", id);

            try
            {
                return await _context.Species.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Specie> GetSpecie(string name)
        {
            var filter = Builders<Specie>.Filter.Eq("Name", name);

            try
            {
                return await _context.Species.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Specie Get(int id)
        {
            var filter = Builders<Specie>.Filter.Eq("Id", id);

            try
            {
                return _context.Species.Find(filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Add(Specie item)
        {
            try
            {
                var list=_context.Species.Find(_ => true).ToList();
                if(item.Id==0){
                    if(list.Count>0){
                        list.Sort();
                        item.Id=list[list.Count-1].Id+1;
                    }
                    else{
                        item.Id=1;
                    } 
                }else{
                    for (int i=0;i<list.Count;i++){
                        if(item.Id==list[i].Id){
                            return false;
                        }
                    }
                }
    
                await _context.Species.InsertOneAsync(item);
                Core.MakeSpecieFolder(item.Id.ToString());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Update(int SpecieId, Specie item)
        {
            try
            {
                ReplaceOneResult actionResult 
                    = _context.Species
                                    .ReplaceOne(n => n.Id.Equals(SpecieId)
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

        public bool UpdateName(int id, string name)
        {
            var filter = Builders<Specie>.Filter.Eq("Id", id);
            Specie specie  = _context.Species.Find(filter).FirstOrDefault();
            specie.Name = name;
            return Update(id, specie);
        }
        public bool UpdateFamily(int id, string family)
        {
            var filter = Builders<Specie>.Filter.Eq("Id", id);
            Specie specie  = _context.Species.Find(filter).FirstOrDefault();
            specie.Family = family;
            return Update(id, specie);
        }

        /*public bool UpdateGallery(int id, int index, string description)
        {
            var filter = Builders<Specie>.Filter.Eq("Id", id);
            Specie specie  = _context.Species.Find(filter).FirstOrDefault();
            specie.Family = family;
            return Update(id, specie);
        }*/

        public bool AddPhoto(int specieId, Photo photo)
        {
            Specie specie = getSpecie(specieId);
            specie.Gallery.Add(photo);
            return Update(specie.Id, specie);
        }

        public Specie getSpecie(int id){
            var filter = Builders<Specie>.Filter.Eq("Id", id);
            Specie specie=_context.Species.Find(filter).FirstOrDefaultAsync().Result;
            return specie;
        }

        public bool Remove(int id)
        {
            try
            {
                var filtro_especie = Builders<Specie>.Filter.Eq("Id", id);
                Specie specie = _context.Species.Find(filtro_especie).FirstOrDefault();
                foreach(Photo photo in specie.Gallery) {
                    var filtro_foto = Builders<Photo>.Filter.Eq("Id", photo.Id);
                    _context.Photos.DeleteOne(filtro_foto);
                }
                DeleteResult actionResult = _context.Species.DeleteOne(filtro_especie);
                var filtro_pregunta = Builders<Question>.Filter.Eq("SpecieId", id);
                _context.Questions.DeleteMany(filtro_pregunta);
                string specieDeletedPath = Core.SpecieFolderPath(id.ToString());
                Directory.Delete(specieDeletedPath, true);              
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

    }

}