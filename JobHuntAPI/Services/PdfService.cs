using DK.GenericLibrary.Interfaces;
using JobHuntAPI.Model;
using JobHuntAPI.Model.Dto;
using JobHuntAPI.Repository;
using JobHuntAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace JobHuntAPI.Services
{
	public class PdfService(IRepository<ApplicationContext> repository, IConfiguration _configuration) : IPdfService
	{
		private readonly string _secret = _configuration["ApiConfig:Secret"] ?? throw new ArgumentNullException("Secret not found in configuration");

		public PdfFile GetPdf(PdfRequestDto dto) 
		{
			var entry =  repository.GetItem<JobApplication>(q => q.Where(i => i.UserId == dto.UserId).Include(p => p.PdfFiles));
			var items =  repository.GetAllItems<JobApplication>(q => q.Where(i => i.UserId == dto.UserId).Include(p => p.PdfFiles));
			var pdf = entry.PdfFiles.Where(i => i.PdfFileId == dto.PdfId).First();
			return PDFEncryptionHelper.DecryptPdf(pdf, dto.UserId.ToString(), _secret);
		}
	}
}
