﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobHuntAPI.Model
{
	[Table("PdfFiles")]
	public class PdfFile
	{
		[Key]
		public Guid PdfFileId { get; set; }
		[ForeignKey("JobApplicationId")]
		public Guid JobApplicationId { get; set; }

		public string FileName { get; set; } = string.Empty;
		public byte[] Content { get; set; } = Array.Empty<byte>();
		public string ContentType { get; set; } = string.Empty;
	}
}
