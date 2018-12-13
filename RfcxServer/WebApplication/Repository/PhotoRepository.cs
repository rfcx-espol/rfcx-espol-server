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

namespace WebApplication.Repository
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly ObjectContext _context =null; 

        public PhotoRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 

        public async Task<IEnumerable<Photo>> Get()
        {
            try
            {
                return await _context.Photos.Find(_ => true).ToListAsync();
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

        public void Add(Photo item)
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
                            return;
                        }
                    }
                }
    
                 _context.Photos.InsertOne(item);
                 return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}