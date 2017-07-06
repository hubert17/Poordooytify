using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Poordooytify.Models
{
    public class SongMood
    {
        public int Id { get; set; }
        [MaxLength(30)]
        [Index("IX_FirstAndSecond", 1, IsUnique = true)]
        public string SongKey { get; set; }
        [MaxLength(30)]
        [Index("IX_FirstAndSecond", 2, IsUnique = true)]
        public string MoodKey { get; set; }
    }
}