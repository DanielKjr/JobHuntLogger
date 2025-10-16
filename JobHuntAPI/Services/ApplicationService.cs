using DK.GenericLibrary.Interfaces;
using JobHuntAPI.Model;
using JobHuntAPI.Model.Dto;
using JobHuntAPI.Repository;
using JobHuntAPI.Services.Interfaces;

namespace JobHuntAPI.Services
{
	public class ApplicationService(IAsyncRepository<ApplicationContext> _applicationContext, IConfiguration _configuration) : IApplicationService
	{
		private readonly string _secret = _configuration["ApiConfig:Secret"] ?? throw new ArgumentNullException("Secret not found in configuration");
		public async Task AddNewAsync(JobApplicationDto dto)
		{
			string userId = dto.UserId.ToString();
			JobApplication newApplication = new JobApplication()
			{
				UserId = dto.UserId,
				JobTitle = dto.JobTitle,
				Company = dto.Company,
				EncryptedApplicationPdf = PDFEncryptionHelper.EncryptPdf(dto.ApplicationPdf, userId, _secret),
				EncryptedResumePdf = PDFEncryptionHelper.EncryptPdf(dto.ResumePdf, userId, _secret)
			};
			await _applicationContext.AddItem(dto);
		}

		public async Task AddNewAsync(IEnumerable<JobApplicationDto> dtos)
		{
			string userId = dtos.First().UserId.ToString();
			List<JobApplication> newApplications = new List<JobApplication>();
			foreach (var item in dtos)
			{
				JobApplication newApplication = new JobApplication()
				{
					UserId = item.UserId,
					JobTitle = item.JobTitle,
					Company = item.Company,
					EncryptedApplicationPdf = PDFEncryptionHelper.EncryptPdf(item.ApplicationPdf, userId, _secret),
					EncryptedResumePdf = PDFEncryptionHelper.EncryptPdf(item.ResumePdf, userId, _secret)
				};
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

		public async Task<IEnumerable<T>> GetAllAsync<T>(Guid userId) where T : JobApplicationDisplayDto
		{
			List<T> results = new List<T>();
			var applications = await _applicationContext.GetAllItems<JobApplication>(q => q.Where(i => i.UserId == userId));
			foreach (var app in applications)
			{
				T dto = Activator.CreateInstance<T>();
				dto.JobApplicationId = app.JobApplicationId;
				dto.JobTitle = app.JobTitle;
				dto.Company = app.Company;
				dto.AppliedDate = app.AppliedDate;
				dto.ReplyDate = app.ReplyDate;
				dto.ApplicationPdf = PDFEncryptionHelper.DecryptPdf(app.EncryptedApplicationPdf, userId.ToString(), _secret);
				dto.ResumePdf = PDFEncryptionHelper.DecryptPdf(app.EncryptedResumePdf, userId.ToString(), _secret);
				results.Add(dto);
			}
			return results;
		}
		//Experimental using the new() functionality
		public async Task<T?> GetByIdAsync<T>(Guid userId, Guid id) where T : JobApplicationDisplayDto, new()
		{
			var entry = await _applicationContext.GetItem<JobApplication>(q => q.Where(i => i.UserId == userId && i.JobApplicationId == id));
			return entry == null ? null : new T()
			{
				JobApplicationId = entry.JobApplicationId,
				JobTitle = entry.JobTitle,
				Company = entry.Company,
				AppliedDate = entry.AppliedDate,
				ReplyDate = entry.ReplyDate,
				ApplicationPdf = PDFEncryptionHelper.DecryptPdf(entry.EncryptedApplicationPdf, userId.ToString(), _secret),
				ResumePdf = PDFEncryptionHelper.DecryptPdf(entry.EncryptedResumePdf, userId.ToString(), _secret)
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
			var existingAppPdf = PDFEncryptionHelper.DecryptPdf(entry.EncryptedApplicationPdf, userIdString, _secret);
			var existingResumePdf = PDFEncryptionHelper.DecryptPdf(entry.EncryptedResumePdf, userIdString, _secret);

			// Only update if the file has changed
			if (PDFEncryptionHelper.IsFileChanged(application.ApplicationPdf.Content, existingAppPdf.Content))
				entry.EncryptedApplicationPdf = PDFEncryptionHelper.EncryptPdf(application.ApplicationPdf, userIdString, _secret);

			if (PDFEncryptionHelper.IsFileChanged(application.ResumePdf.Content, existingResumePdf.Content))
				entry.EncryptedResumePdf = PDFEncryptionHelper.EncryptPdf(application.ResumePdf, userIdString, _secret);

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
