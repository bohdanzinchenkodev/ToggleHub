using FluentValidation;
using Mapster;
using ToggleHub.Application.DTOs.Project;
using ToggleHub.Application.Interfaces;
using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Exceptions;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class ProjectService : IProjectService
{
    private readonly ISluggedRepository _sluggedRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IValidator<CreateProjectDto> _createValidator;
    private readonly IValidator<UpdateProjectDto> _updateValidator;
    private readonly ISlugGenerator _slugGenerator;

    public ProjectService(ISluggedRepository sluggedRepository, IProjectRepository projectRepository, IValidator<CreateProjectDto> createValidator, ISlugGenerator slugGenerator, IValidator<UpdateProjectDto> updateValidator)
    {
        _sluggedRepository = sluggedRepository;
        _projectRepository = projectRepository;
        _createValidator = createValidator;
        _slugGenerator = slugGenerator;
        _updateValidator = updateValidator;
    }

    public async Task<ProjectDto?> GetByIdAsync(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        var dto = project?.Adapt<ProjectDto>();
        return dto;
    }

    public async Task<ProjectDto?> GetBySlugAsync(string slug)
    {
        var project = await _sluggedRepository.GetBySlugAsync<Project>(slug);
        var dto = project?.Adapt<ProjectDto>();
        return dto;
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto createProjectDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createProjectDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var project = createProjectDto.Adapt<Project>();
        project.Slug = await _slugGenerator.GenerateAsync<Project>(project.Name);
        project.CreatedAt = DateTime.UtcNow;
        
        project = await _projectRepository.CreateAsync(project);
        
        var dto = project.Adapt<ProjectDto>();
        return dto;
    }

    public async Task<ProjectDto> UpdateAsync(UpdateProjectDto updateProjectDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateProjectDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var project = await _projectRepository.GetByIdAsync(updateProjectDto.Id);
        if (project == null)
            throw new ApplicationException($"Project with ID {updateProjectDto.Id} not found");
        
        // Check if the name has changed to generate a new slug
        var slug = project.Slug;
        if (updateProjectDto.Name != project.Name)
            slug = await _slugGenerator.GenerateAsync<Project>(updateProjectDto.Name);
        
        project = updateProjectDto.Adapt(project);
        project.Slug = slug;
        
        await _projectRepository.UpdateAsync(project);
        
        var dto = project.Adapt<ProjectDto>();
        return dto;
    }
    public async Task DeleteAsync(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null)
            throw new ApplicationException($"Project with ID {id} not found");
        
        await _projectRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ProjectDto>> GetAllAsync(int? organizationId = null)
    {
        var projects = await _projectRepository.GetAllAsync(organizationId);
        var dtos = projects.Adapt<IEnumerable<ProjectDto>>();
        return dtos;
    }
}