﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBG.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please write your comment")]
        public string Text { get; set; }
        public IdentityUser User { get; set; }  
        [NotMapped]
        public PFPModel PFP { get; set; }
        public List<PageModel> Pages { get; set; } = new List<PageModel>();
        public List<PFPModel> LikedUser { get; set; } = new List<PFPModel>();
        public List<ForumQuestion> PostedOnForums { get; set; } = new List<ForumQuestion>();
        public int? PageId { get; set; }
        public int? PostId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public List<CommentModel> Replies {get; set;} = new List<CommentModel>(); 
        [NotMapped]
        public bool LikedByUser { get; set; } = false;
    }
}
