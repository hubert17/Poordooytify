using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Poordooytify.Models
{
    public class Song
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Artist { get; set; }
        public string Genre { get; set; }
        public string Moods { get; set; }
        public string OrigFilename { get; set; }
        public string Link { get; set; }
        [ForeignKey("CloudToken")]
        public int CloudTokenId { get; set; }
        public virtual CloudToken CloudToken { get; set; }
        public DateTime DateAdded { get; set; }
        public int PlayCount { get; set; }
    }
}