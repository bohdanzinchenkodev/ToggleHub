using FluentValidation;
using ToggleHub.Application.DTOs.Project;
using ToggleHub.Application.Interfaces;
using ToggleHub.Application.Mapping;
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
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ISlugGenerator _slugGenerator;

    public ProjectService(ISluggedRepository sluggedRepository, IProjectRepository projectRepository, IValidator<CreateProjectDto> createValidator, ISlugGenerator slugGenerator, IValidator<UpdateProjectDto> updateValidator, IOrganizationRepository organizationRepository)
    {
        _sluggedRepository = sluggedRepository;
        _projectRepository = projectRepository;
        _createValidator = createValidator;
        _slugGenerator = slugGenerator;
        _updateValidator = updateValidator;
        _organizationRepository = organizationRepository;
    }

    public async Task<ProjectDto?> GetByIdAsync(int id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        return project?.ToDto();
    }

    public async Task<ProjectDto?> GetBySlugAsync(string slug)
    {
        var project = await _sluggedRepository.GetBySlugAsync<Project>(slug);
        return project?.ToDto();
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto createProjectDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createProjectDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var organization = await _organizationRepository.GetByIdAsync(createProjectDto.OrganizationId);
        if (organization == null)
            throw new ApplicationException($"Organization with ID {createProjectDto.OrganizationId} not found");
        
        // Check if a project with the same name already exists in the organization
        if(await _projectRepository.NameExistsAsync(createProjectDto.Name, organization.Id))
            throw new ApplicationException($"Project with name '{createProjectDto.Name}' already exists in your organization.");
        
        var project = createProjectDto.ToEntity();
    
        project.Slug = await _slugGenerator.GenerateAsync<Project>(project.Name);
        project.CreatedAt = DateTime.UtcNow;
        
        project = await _projectRepository.CreateAsync(project);
        
        return project.ToDto();
    }

    public async Task<ProjectDto> UpdateAsync(UpdateProjectDto updateProjectDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateProjectDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
        
        var project = await _projectRepository.GetByIdAsync(updateProjectDto.Id);
        if (project == null)
            throw new ApplicationException($"Project with ID {updateProjectDto.Id} not found");
        
        var organization = await _organizationRepository.GetByIdAsync(project.OrganizationId);
        
        // Check if the name has changed to generate a new slug
        var slug = project.Slug;
        if (updateProjectDto.Name != project.Name)
        {
            if(await _projectRepository.NameExistsAsync(updateProjectDto.Name, organization!.Id))
                throw new ApplicationException($"Project with name '{updateProjectDto.Name}' already exists.");
            
            slug = await _slugGenerator.GenerateAsync<Project>(updateProjectDto.Name);
        }
        
        updateProjectDto.ToEntity(project);
        project.Slug = slug;
        
        await _projectRepository.UpdateAsync(project);
        
        return project.ToDto();
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
        return projects.Select(p => p.ToDto());
    }
}