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

namespace WebApplication.Repository
{
    public class KindRepository : IKindRepository
    {
        private readonly ObjectContext _context =null; 

        public KindRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 

        public async Task<IEnumerable<Kind>> Get()
        {
            try
            {
                return await _context.Kinds.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Kind> Get(string id)
        {
            var filter = Builders<Kind>.Filter.Eq("KindId", id);

            try
            {
                return await _context.Kinds.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Kind> Get(int id)
        {
            var filter = Builders<Kind>.Filter.Eq("Id", id);

            try
            {
                return await _context.Kinds.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Add(Kind item)
        {
            try
            {
                var list=_context.Kinds.Find(_ => true).ToList();
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
    
                await _context.Kinds.InsertOneAsync(item);
                Core.MakeKindFolder(item.Id.ToString());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Remove(int KindId)
        {
            try
            {
                DeleteResult actionResult = await _context.Kinds.DeleteOneAsync(
                Builders<Kind>.Filter.Eq("Id", KindId));
                var filter1 = Builders<Photo>.Filter.Eq("KindId", KindId);
                _context.Photos.DeleteMany(filter1);
                Core.MakeRecyclerFolder();
                string kindDeletedPath = Core.KindFolderPath(KindId.ToString());
                string reclyclerPath = Core.RecyclerFolderPath();
                string photoName="";
                string recyclerName="";
                string[] filesInRecycler = Directory.GetFiles(reclyclerPath);

                if (Directory.Exists(kindDeletedPath))
                {
                    /*string[] photos = Directory.GetFiles(kindDeletedPath);

                    foreach (string photo in photos)
                    {
                        photoName = Path.GetFileName(photo);
                        recyclerName = Path.Combine(reclyclerPath, photoName);
                        File.Move(photo, AutoRenameFilename(recyclerName));
                    }*/
                    DeleteDirectory(kindDeletedPath);

                }
                return actionResult.IsAcknowledged && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Update(int KindId, Kind item)
        {
            try
            {
                ReplaceOneResult actionResult 
                    = await _context.Kinds
                                    .ReplaceOneAsync(n => n.Id.Equals(KindId)
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
            Kind kind = getKind(id);
            kind.Name = name;
            return Update(kind.Id, kind);
        }

        public Task<bool> UpdateFamily(int id, string family)
        {
            Kind kind = getKind(id);
            kind.Family = family;
            return Update(kind.Id, kind);
        }

        public Kind getKind(int id){
            var filter = Builders<Kind>.Filter.Eq("Id", id);
            Kind kind=_context.Kinds.Find(filter).FirstOrDefaultAsync().Result;
            return kind;
        }

        public async Task<bool> RemoveAll()
        {
            try
            {
                DeleteResult actionResult 
                    = await _context.Kinds.DeleteManyAsync(new BsonDocument());

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