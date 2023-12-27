public string WriteCode()
{
	CodeBuilder.Clear();

	CodeBuilder.AppendLine("using System;");
	CodeBuilder.AppendLine("using Microsoft.EntityFrameworkCore;");
	CodeBuilder.AppendLine("using Microsoft.EntityFrameworkCore.Metadata;");
	CodeBuilder.AppendLine();

	CodeBuilder.AppendLine($"namespace {EntityContext.ContextNamespace}");
	CodeBuilder.AppendLine("{");
	
	using (CodeBuilder.Indent())
	{
		GenerateClass();
	}
	CodeBuilder.AppendLine("}");

	return CodeBuilder.ToString();
}
private void GenerateClass()
{
	var contextClass = EntityContext.ContextClass.ToSafeName();
	var baseClass = EntityContext.ContextBaseClass.ToSafeName();

	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// {contextClass} class");
	CodeBuilder.AppendLine("/// </summary>");

	CodeBuilder.AppendLine($"public class {contextClass} : {baseClass}");
	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{
		GenerateConstructors();
		GenerateDbSets();
		GenerateOnConfiguring();
	}

	CodeBuilder.AppendLine("}");
}

private void GenerateConstructors()
{
	var contextName = EntityContext.ContextClass.ToSafeName();
	
	CodeBuilder.AppendLine("#region Constructor");
	CodeBuilder.AppendLine();

	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Initializes a new instance of the {contextName} class");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine("/// <param name=\"options\">The options to be used by this DbContext</param>");

	CodeBuilder.AppendLine($"public {contextName}(DbContextOptions<{contextName}> options): base(options)")
		.AppendLine("{")
		.AppendLine("}")
		.AppendLine();

	CodeBuilder.AppendLine("#endregion").AppendLine();
}

private void GenerateDbSets()
{
	CodeBuilder.AppendLine("#region Properties");
	CodeBuilder.AppendLine();
	
	foreach (var entityType in EntityContext.Entities.OrderBy(e => e.ContextProperty))
	{
		var entityClass = entityType.EntityClass.ToSafeName();
		var propertyName = entityType.ContextProperty.ToSafeName();
		var fullName = $"{entityClass}";

		CodeBuilder.AppendLine($"public virtual DbSet<{fullName}> {propertyName} {{ get; set; }}");
		CodeBuilder.AppendLine();
	}

	CodeBuilder.AppendLine("#endregion");

	if (EntityContext.Entities.Any())
		CodeBuilder.AppendLine();
}

private void GenerateOnConfiguring()
{
	CodeBuilder.AppendLine("#region Configurations");
	CodeBuilder.AppendLine();

	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine("/// Configurations");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine("/// <param name=\"modelBuilder\">The builder being used to construct the model for this context.</param>");

	CodeBuilder.AppendLine("protected override void OnModelCreating(ModelBuilder modelBuilder)");
	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{
		foreach (var entityType in EntityContext.Entities.OrderBy(e => e.MappingClass))
		{
			var mappingClass = entityType.MappingClass.ToSafeName();

			CodeBuilder.AppendLine($"modelBuilder.ApplyConfiguration(new {entityType.MappingNamespace}.{mappingClass}());");
		}

	}

	CodeBuilder.AppendLine("}");
	
	CodeBuilder.AppendLine();
	CodeBuilder.AppendLine("#endregion");	
}

// run script
WriteCode()