using JobHuntAPI.Model;
using JobHuntAPI.Model.Dto;
using JobHuntAPI.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobHuntAPI.Services.Interfaces
{
	public interface IApplicationService
	{
		Task AddNewAsync(JobApplicationDto dto);
		Task AddNewAsync(IEnumerable<JobApplicationDto> dtos);
		Task DeleteByIdAsync(Guid userId, Guid applicationId);
		Task DeleteByIdAsync(Guid userId, IEnumerable<Guid> ids);
		Task UpdateApplicationAsync<T>(Guid userId, T application) where T : JobApplicationDisplayDto;
		Task UpdateApplicationAsync<T>(Guid userId, IEnumerable<T> application) where T : JobApplicationDisplayDto;
		Task<IEnumerable<T>> GetAllAsync<T>(Guid userId) where T : JobApplicationDisplayDto;
		Task<T?> GetByIdAsync<T>(Guid userId, Guid id) where T : JobApplicationDisplayDto, new();
	}
}
