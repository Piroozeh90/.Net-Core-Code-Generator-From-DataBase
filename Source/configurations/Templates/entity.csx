public string WriteCode()
{
	CodeBuilder.Clear();

	CodeBuilder.AppendLine("using System;");
	CodeBuilder.AppendLine("using System.Text;");
	CodeBuilder.AppendLine("using System.Collections.Generic;");
	CodeBuilder.AppendLine("using Bpc.Core.Entities;");
	CodeBuilder.AppendLine();

	CodeBuilder.AppendLine($"namespace {TemplateOptions.Namespace}");
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
	var entityClass = Entity.EntityClass.ToSafeName();

	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Entity class representing data for table <see cref=\"{Entity.TableName}\"/>");
	CodeBuilder.AppendLine("/// </summary>");

	CodeBuilder.AppendLine($"public class {entityClass} : Entity<int>");

	if (Entity.EntityBaseClass.HasValue())
	{
		var entityBaseClass = Entity.EntityBaseClass.ToSafeName();
		using (CodeBuilder.Indent())
			CodeBuilder.AppendLine($": {entityBaseClass}");
	}

	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{
		GenerateConstructor();

		GenerateProperties();
		GenerateRelationshipProperties();
	}

	CodeBuilder.AppendLine("}");

}

private void GenerateConstructor()
{
	var relationships = Entity.Relationships
		.Where(r => r.Cardinality == Cardinality.Many)
		.OrderBy(r => r.PropertyName)
		.ToList();

	var entityClass = Entity.EntityClass.ToSafeName();
	
	CodeBuilder.AppendLine("#region Constructor");
	CodeBuilder.AppendLine();
	
	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Initializes a new instance of the <see cref=\"{entityClass}\"/> class.");
	CodeBuilder.AppendLine("/// </summary>");

	CodeBuilder.AppendLine($"public {entityClass}()");
	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{
		foreach (var relationship in relationships)
		{
			var propertyName = relationship.PropertyName.ToSafeName();

			var primaryNamespace = relationship.PrimaryEntity.EntityNamespace;
			var primaryName = relationship.PrimaryEntity.EntityClass.ToSafeName();
			var primaryFullName = Entity.EntityNamespace != primaryNamespace
				? $"{primaryNamespace}.{primaryName}"
				: primaryName;

			CodeBuilder.AppendLine($"{propertyName} = new HashSet<{primaryFullName}>();");
		}
	}

	CodeBuilder.AppendLine("}");
	
	CodeBuilder.AppendLine();
	CodeBuilder.AppendLine("#endregion");
	CodeBuilder.AppendLine();
}

private void GenerateProperties()
{
	CodeBuilder
	.AppendLine("#region Properties")
	.AppendLine();
	
	foreach (var property in Entity.Properties)
	{
		
		var propertyType = property.SystemType.ToNullableType(property.IsNullable == true);
		var propertyName = property.PropertyName.ToSafeName();
		
		if(propertyName.ToLower() == "id")
			continue;

		CodeBuilder.AppendLine("/// <summary>");
		CodeBuilder.AppendLine($"/// Gets or sets the property value representing column '{property.ColumnName}'.");
		CodeBuilder.AppendLine("/// </summary>");
		CodeBuilder.AppendLine("/// <value>");
		CodeBuilder.AppendLine($"/// The property value representing column '{property.ColumnName}'.");
		CodeBuilder.AppendLine("/// </value>");

		CodeBuilder.AppendLine($"public {propertyType} {propertyName} {{ get; set; }}");
		CodeBuilder.AppendLine();
	}
	
	CodeBuilder.AppendLine("#endregion");
	CodeBuilder.AppendLine();
}

private void GenerateRelationshipProperties()
{
	CodeBuilder
	.AppendLine("#region Relationships")
	.AppendLine();
	
	foreach (var relationship in Entity.Relationships.OrderBy(r => r.PropertyName))
	{
		var propertyName = relationship.PropertyName.ToSafeName();
		var primaryNamespace = relationship.PrimaryEntity.EntityNamespace;
		var primaryName = relationship.PrimaryEntity.EntityClass.ToSafeName();
		var primaryFullName = Entity.EntityNamespace != primaryNamespace
			? $"{primaryNamespace}.{primaryName}"
			: primaryName;

		if (relationship.Cardinality == Cardinality.Many)
		{
			CodeBuilder.AppendLine("/// <summary>");
			CodeBuilder.AppendLine($"/// Gets or sets the navigation collection for entity <see cref=\"{primaryFullName}\" />.");
			CodeBuilder.AppendLine("/// </summary>");
			CodeBuilder.AppendLine("/// <value>");
			CodeBuilder.AppendLine($"/// The the navigation collection for entity <see cref=\"{primaryFullName}\" />.");
			CodeBuilder.AppendLine("/// </value>");


			CodeBuilder.AppendLine($"public virtual ICollection<{primaryFullName}> {propertyName} {{ get; set; }}");
			CodeBuilder.AppendLine();
		}
		else
		{
			CodeBuilder.AppendLine("/// <summary>");
			CodeBuilder.AppendLine($"/// Gets or sets the navigation property for entity <see cref=\"{primaryFullName}\" />.");
			CodeBuilder.AppendLine("/// </summary>");
			CodeBuilder.AppendLine("/// <value>");
			CodeBuilder.AppendLine($"/// The the navigation property for entity <see cref=\"{primaryFullName}\" />.");
			CodeBuilder.AppendLine("/// </value>");

			foreach (var property in relationship.Properties)
				CodeBuilder.AppendLine($"/// <seealso cref=\"{property.PropertyName}\" />");

			CodeBuilder.AppendLine($"public virtual {primaryFullName} {propertyName} {{ get; set; }}");
			CodeBuilder.AppendLine();
		}
	}
	CodeBuilder.AppendLine("#endregion");
	CodeBuilder.AppendLine();
}
// run script
WriteCode()