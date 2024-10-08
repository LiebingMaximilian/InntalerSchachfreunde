﻿using Microsoft.Data.SqlClient.DataClassification;

namespace InntalerSchachfreunde.Entities
{
    public class Image
    {
        public int Id { get; set; } 
        public string? Name { get; set; }
        public string? Description { get; set; }
        public byte[] ImageBytes {  get; set; }
        public int? ArticleId { get; set; }
        public virtual Article? Article { get; set; }
    }
}
