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

        public async Task<IEnumerable<Specie>> Get()
        {
            try
            {
                return await _context.Species.Find(_ => true).ToListAsync();
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

        public async Task<Specie> Get(int id)
        {
            var filter = Builders<Specie>.Filter.Eq("Id", id);

            try
            {
                return await _context.Species.Find(filter).FirstOrDefaultAsync();
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

        public async Task<bool> Update(int SpecieId, Specie item)
        {
            try
            {
                ReplaceOneResult actionResult 
                    = await _context.Species
                                    .ReplaceOneAsync(n => n.Id.Equals(SpecieId)
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

        public Task<bool> AddPhoto(int specieId, Photo photo)
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

    }

}