using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArticleReviewSystem.Models
{
    [Table("AspNetArticles")]
    public class Articles
    {
        [Key]
        public int ArticleId { get; set; }
        public byte[] Article { get; set; }
        public string ArticleName { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
    }
}