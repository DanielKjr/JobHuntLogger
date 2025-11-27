using DK.GenericLibrary.Interfaces;
using JobHuntAPI.Model;
using JobHuntAPI.Model.Dto;
using JobHuntAPI.Repository;
using JobHuntAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace JobHuntAPI.Services
{
	public class ApplicationService(IAsyncRepository<ApplicationContext> _applicationContext, IConfiguration _configuration) : IApplicationService
	{
		private readonly string _secret = _configuration["ApiConfig:Secret"] ?? throw new ArgumentNullException("Secret not found in configuration");
		private Dictionary<string, object> _cache = new();


		public async Task AddNewAsync(NewJobApplicationDto dto)
		{
			string userId = dto.UserId.ToString();

			var application = dto.ApplicationPdf is not null
	? PDFEncryptionHelper.EncryptPdf(dto.ApplicationPdf, userId, _secret)
	: null!;
			var resume = dto.ResumePdf is not null ? PDFEncryptionHelper.EncryptPdf(dto.ResumePdf, userId, _secret) : null!;
			JobApplication newApplication = new JobApplication()
			{
				UserId = dto.UserId,
				JobTitle = dto.JobTitle,
				Company = dto.Company,
				AppliedDate = dto.Date,
				Deadline = dto.Deadline
				//EncryptedResumePdf = PDFEncryptionHelper.EncryptPdf(dto.ResumePdf, userId, _secret)
			};
			if (application is not null)
				newApplication.PdfFiles.Add(application);
			if (resume is not null)
				newApplication.PdfFiles.Add(resume);

			await _applicationContext.AddItem(newApplication);
		}

		public async Task AddNewAsync(IEnumerable<NewJobApplicationDto> dtos)
		{
			string userId = dtos.First().UserId.ToString();
			List<JobApplication> newApplications = new List<JobApplication>();
			foreach (var item in dtos)
			{
				var application = item.ApplicationPdf is not null
	? PDFEncryptionHelper.EncryptPdf(item.ApplicationPdf, userId, _secret)
	: null!;
				var resume = item.ResumePdf is not null ? PDFEncryptionHelper.EncryptPdf(item.ResumePdf, userId, _secret) : null!;
				JobApplication newApplication = new JobApplication()
				{
					UserId = item.UserId,
					JobTitle = item.JobTitle,
					Company = item.Company,
					AppliedDate = item.Date,
					Deadline = item.Deadline
				};

				if (application is not null)
					newApplication.PdfFiles.Add(application);
				if (resume is not null)
					newApplication.PdfFiles.Add(resume);

				newApplications.Add(newApplication);
			}
			await _applicationContext.AddItems(newApplications);
		}

		public async Task DeleteByIdAsync(Guid userId, Guid applicationId)
		{
			await _applicationContext.RemoveItem<JobApplication>(q => q.UserId == userId && q.JobApplicationId == applicationId);
		}

		public async Task DeleteByIdAsync(Guid userId, IEnumerable<Guid> ids)
		{
			await _applicationContext.RemoveItems<JobApplication>(q => q.Where(i => i.UserId == userId && ids.Contains(i.JobApplicationId)));
		}

		//public async Task<IEnumerable<T>> GetAllAsync<T>(Guid userId) where T : JobApplicationDisplayDto
		//{
		//	List<T> results = new List<T>();
		//	var applications = await _applicationContext.GetAllItems<JobApplication>(q => q.Where(i => i.UserId == userId).Include(p => p.PdfFiles));
		//	foreach (var app in applications)
		//	{
		//		T dto = Activator.CreateInstance<T>();

		//		dto.JobApplicationId = app.JobApplicationId;
		//		dto.JobTitle = app.JobTitle;
		//		dto.Company = app.Company;
		//		dto.AppliedDate = app.AppliedDate;
		//		dto.ReplyDate = app.ReplyDate;
		//		var application = app.PdfFiles.Where(i => i.PdfType == PdfType.Application).FirstOrDefault();

		//		if (application is not null)
		//			dto.ApplicationPdf = PDFEncryptionHelper.DecryptPdf(application, userId.ToString(), _secret);
		//		else
		//			dto.ApplicationPdf = null;

		//		var resume = app.PdfFiles.Where(i => i.PdfType == PdfType.Resume).FirstOrDefault();
		//		if (resume is not null)
		//			dto.ResumePdf = PDFEncryptionHelper.DecryptPdf(resume, userId.ToString(), _secret);
		//		else
		//			dto.ResumePdf = null;
		//		results.Add(dto);
		//	}
		//	return results;
		//}

		//With temporary cache
		public async Task<IEnumerable<T>> GetAllAsync<T>(Guid userId) where T : JobApplicationDisplayDto
		{
			string cacheKey = $"apps:{userId}";

			if (_cache.TryGetValue(cacheKey, out var cached))
			{

				return (IEnumerable<T>)cached;
			}

			var results = new List<T>();

			var apps = await _applicationContext
				.GetAllItems<JobApplication>(
					q => q.Where(i => i.UserId == userId)
						  .Include(p => p.PdfFiles));

			foreach (var app in apps)
			{
				T dto = Activator.CreateInstance<T>();

				dto.JobApplicationId = app.JobApplicationId;
				dto.JobTitle = app.JobTitle;
				dto.Company = app.Company;
				dto.AppliedDate = app.AppliedDate;
				dto.ReplyDate = app.ReplyDate;

				var application = app.PdfFiles.FirstOrDefault(i => i.PdfType == PdfType.Application);
				if (application is not null)
					dto.ApplicationPdf = PDFEncryptionHelper.DecryptPdf(application, userId.ToString(), _secret);
				else
					dto.ApplicationPdf = null;

				var resume = app.PdfFiles.FirstOrDefault(i => i.PdfType == PdfType.Resume);
				if (resume is not null)
					dto.ResumePdf = PDFEncryptionHelper.DecryptPdf(resume, userId.ToString(), _secret);
				else
					dto.ResumePdf = null;

				results.Add(dto);
			}

			_cache[cacheKey] = results;


			return results;
		}
		//Experimental using the new() functionality
		public async Task<T?> GetByIdAsync<T>(Guid userId, Guid id) where T : JobApplicationDisplayDto, new()
		{
			var entry = await _applicationContext.GetItem<JobApplication>(q => q.Where(i => i.UserId == userId && i.JobApplicationId == id).Include(p => p.PdfFiles));
			var application = entry.PdfFiles.Where(i => i.PdfType == PdfType.Application).FirstOrDefault();

			var resume = entry.PdfFiles.Where(i => i.PdfType == PdfType.Resume).FirstOrDefault();

			return entry == null ? null : new T()
			{
				JobApplicationId = entry.JobApplicationId,
				JobTitle = entry.JobTitle,
				Company = entry.Company,
				AppliedDate = entry.AppliedDate,
				ReplyDate = entry.ReplyDate,
				ApplicationPdf = application is not null ? PDFEncryptionHelper.DecryptPdf(application, userId.ToString(), _secret) : null!,
				ResumePdf = resume is not null ? PDFEncryptionHelper.DecryptPdf(resume, userId.ToString(), _secret) : null!
			};
		}

		public async Task UpdateApplicationAsync<T>(Guid userId, T application) where T : JobApplicationDisplayDto
		{
			var entry = await _applicationContext.GetItem<JobApplication>(q => q.Where(i => i.UserId == userId && i.JobApplicationId == application.JobApplicationId));
			if (entry == null)
				throw new KeyNotFoundException("Application not found");
			entry.JobTitle = application.JobTitle;
			entry.Company = application.Company;
			entry.AppliedDate = application.AppliedDate;
			entry.ReplyDate = application.ReplyDate;

			string userIdString = userId.ToString();
			// Decrypt existing files for comparison
			var applicationPdfEntry = entry.PdfFiles.Where(i => i.PdfType == PdfType.Application).FirstOrDefault();
			if (applicationPdfEntry is not null)
			{
				var existingAppPdf = PDFEncryptionHelper.DecryptPdf(applicationPdfEntry, userIdString, _secret);
				// Only update if the file has changed
				if (PDFEncryptionHelper.IsFileChanged(application.ApplicationPdf!.Content, existingAppPdf.Content))
					applicationPdfEntry = PDFEncryptionHelper.EncryptPdf(application.ApplicationPdf, userIdString, _secret);
			}
			var entryResumePdf = entry.PdfFiles.Where(i => i.PdfType == PdfType.Resume).FirstOrDefault();
			if (entryResumePdf is not null)
			{
				var existingResumePdf = PDFEncryptionHelper.DecryptPdf(entryResumePdf, userIdString, _secret);



				if (PDFEncryptionHelper.IsFileChanged(application.ResumePdf!.Content, existingResumePdf.Content))
					entryResumePdf = PDFEncryptionHelper.EncryptPdf(application.ResumePdf, userIdString, _secret);

			}
			//TODO big todo to check whether the pdf is updated here

			await _applicationContext.UpdateItem(entry);
		}

		public async Task UpdateApplicationAsync<T>(Guid userId, IEnumerable<T> application) where T : JobApplicationDisplayDto
		{
			//for now just use other implementation, could be optimized later
			foreach (var app in application)
			{
				await UpdateApplicationAsync(userId, app);
			}
		}
	}
}
