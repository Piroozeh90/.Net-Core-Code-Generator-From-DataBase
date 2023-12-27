public string WriteCode()
{
	CodeBuilder.Clear();

	CodeBuilder.AppendLine("using System;");
	CodeBuilder.AppendLine("using Bpc.Core.Entities;");
	CodeBuilder.AppendLine("using Bpc.Data.Pagination;");
	CodeBuilder.AppendLine("using System.Collections.Generic;");
	CodeBuilder.AppendLine();

	CodeBuilder.AppendLine($"namespace {TemplateOptions.Namespace};");
    CodeBuilder.AppendLine();

    using (CodeBuilder.Indent())
	{
		GenerateClass();
	}


	return CodeBuilder.ToString();
}

private void GenerateClass()
{
	var entityClass = Entity.EntityClass.ToSafeName();

	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// {Entity.TableName} DTO");
	CodeBuilder.AppendLine("/// </summary>");

	CodeBuilder.AppendLine($"public class {entityClass}Dto : BaseBpcModel");

	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{
		GenerateProperties();
	}

	CodeBuilder.AppendLine("}");

}

private void GenerateProperties()
{
	foreach (var property in Entity.Properties)
	{
		
		var propertyType = property.SystemType.ToNullableType(property.IsNullable == true);
		var propertyName = property.PropertyName.ToSafeName();
			
		if(propertyName.ToLower() == "isdeleted")
			continue;
        
		if(propertyName.ToLower() == "rowversion")
			continue;

		CodeBuilder.AppendLine($"[Filterable] public {propertyType} {propertyName} {{ get; set; }}");
	}
}

// run script
WriteCode()