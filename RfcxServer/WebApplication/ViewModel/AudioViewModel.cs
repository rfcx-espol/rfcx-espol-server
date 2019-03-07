using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication.Models;

namespace WebApplication.ViewModel
{
    public class AudioViewModel
    {
        public IEnumerable<Audio> Audios { get; set; }
        public int StationId { get; set; }
        public IEnumerable<Station> Stations { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = @"{0:dd\/MM\/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }
    }
}