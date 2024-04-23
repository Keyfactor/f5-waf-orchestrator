namespace Keyfactor.Extensions.Orchestrator.F5WafOrchestrator;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class JobAttribute : Attribute
{
    public JobAttribute(string jobClass)
    {
        JobClassName = jobClass;
    }

    private string JobClassName { get; }

    public virtual string JobClass => JobClassName;
}