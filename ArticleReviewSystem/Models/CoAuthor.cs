using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArticleReviewSystem.Models
{
    public class CoAuthor
    {
        [Key]
        public int CoAuthorId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Affiliation { get; set; }
        public virtual Article CoAuthoredArticle { get; set; }
    }
}