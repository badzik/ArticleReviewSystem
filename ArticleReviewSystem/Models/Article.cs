using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArticleReviewSystem.Models
{
    public class Article
    {
        [Key]
        public int ArticleId { get; set; }
        public byte[] Document { get; set; }
        public string ArticleName { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }

        public virtual ApplicationUser MainAuthor { get; set; }
        [InverseProperty("CoAuthoredArticle")]
        public virtual ICollection<CoAuthor> CoAuthors { get; set; }
    }
}