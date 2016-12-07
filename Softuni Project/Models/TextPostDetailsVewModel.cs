using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Softuni_Project.Models
{
    public class TextPostDetailsVewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
      
        public string Author { get; set; }

        public int Score { get; set; }

        public IEnumerable<CommentViewModel> Comments { get; set; }

        public static Expression<Func<TextPost, TextPostDetailsVewModel>> ToViewModel
        {
            get
            {
                return model => new TextPostDetailsVewModel
                {
                    Id = model.Id,
                    Title = model.Title,
                    Content = model.Content,                
                    Author = model.Author.UserName,
                    Score=model.Score,
                 
                    Comments = model.Comments.Select(x => new CommentViewModel()
                    {
                        Id = x.Id,
                        AuthorName = x.Author.UserName,
                        Content = x.Content
                    })
                };
            }
        }
    }
}