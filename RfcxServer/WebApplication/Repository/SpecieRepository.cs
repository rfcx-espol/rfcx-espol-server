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

        public async Task<string> GetPhoto(int specieId, int photoId)
        {
            var filter = Builders<Photo>.Filter.Eq("Id", photoId);
            var photo = await _context.Photos.Find(filter).FirstOrDefaultAsync();
            var filePath = Path.Combine(Core.SpecieFolderPath(specieId.ToString()), photo.Filename);
            return filePath;
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

        public async Task<bool> Remove(int SpecieId)
        {
            try
            {
                DeleteResult actionResult = await _context.Species.DeleteOneAsync(
                Builders<Specie>.Filter.Eq("Id", SpecieId));
                var filter1 = Builders<Photo>.Filter.Eq("SpecieId", SpecieId);
                _context.Photos.DeleteMany(filter1);
                Core.MakeRecyclerFolder();
                string specieDeletedPath = Core.SpecieFolderPath(SpecieId.ToString());
                string reclyclerPath = Core.RecyclerFolderPath();
                string photoName="";
                string recyclerName="";
                string[] filesInRecycler = Directory.GetFiles(reclyclerPath);

                if (Directory.Exists(specieDeletedPath))
                {
                    /*string[] photos = Directory.GetFiles(specieDeletedPath);

                    foreach (string photo in photos)
                    {
                        photoName = Path.GetFileName(photo);
                        recyclerName = Path.Combine(reclyclerPath, photoName);
                        File.Move(photo, AutoRenameFilename(recyclerName));
                    }*/
                    DeleteDirectory(specieDeletedPath);

                }
                return actionResult.IsAcknowledged && actionResult.DeletedCount > 0;
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

        public Task<bool> UpdateName(int id, string name)
        {
            Specie specie = getSpecie(id);
            specie.Name = name;
            return Update(specie.Id, specie);
        }

        public Task<bool> UpdateFamily(int id, string family)
        {
            Specie specie = getSpecie(id);
            specie.Family = family;
            return Update(specie.Id, specie);
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

        public async Task<bool> RemoveAll()
        {
            try
            {
                DeleteResult actionResult 
                    = await _context.Species.DeleteManyAsync(new BsonDocument());

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string AutoRenameFilename(String fileCompleteName)
        {
            if (File.Exists(fileCompleteName))
            {
                string folder = Path.GetDirectoryName(fileCompleteName);
                string filename = Path.GetFileNameWithoutExtension(fileCompleteName);
                string extension = Path.GetExtension(fileCompleteName);
                int number = 1;

                Match regex = Regex.Match(fileCompleteName, @"(.+) \((\d+)\)\.\w+");

                if (regex.Success)
                {
                    filename = regex.Groups[1].Value;
                    number = int.Parse(regex.Groups[2].Value);
                }

                do
                {
                    number++;
                    fileCompleteName = Path.Combine(folder, string.Format("{0} ({1}){2}", filename, number, extension));
                }
                while (File.Exists(fileCompleteName));
            }
            return fileCompleteName;
        }

        public static void DeleteDirectory(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException) 
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }

    }

}