using ToggleHub.Domain.Entities;
using ToggleHub.Domain.Repositories;

namespace ToggleHub.Application.Services;

public class AuditLogService : BaseService<AuditLog>
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogService(IAuditLogRepository auditLogRepository) 
        : base(auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }
}
