using MongoDB.Driver;
using System.Collections.Generic;
using WebApplication.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WebApplication.IRepository
{
    public interface IImageRepository
    {
        Task<Image> Find(string _id);
        Task<ActionResult> PostPicture(ImageRequest req);
        Task<IEnumerable<Image>> GetAllProducts();
        Task<List<Image>> ListImages(DateTime starttime, DateTime endtime, int page, int rows, int stationid);
<<<<<<< HEAD
    }   
=======
        void ChangeFamily(Image image, ImageRequest request);
    }
>>>>>>> 3b1a969f842095cdc231c3ac0af56ecc19f3554d
        
}