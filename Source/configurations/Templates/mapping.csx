public string WriteCode()
{
	CodeBuilder.Clear();

	CodeBuilder.AppendLine("using System;");
	CodeBuilder.AppendLine("using System.Collections.Generic;");
	CodeBuilder.AppendLine("using Bpc.Data.MapData;");
	CodeBuilder.AppendLine("using Microsoft.EntityFrameworkCore;");
	CodeBuilder.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");

	var projectName = TemplateOptions.Namespace.Substring(0, TemplateOptions.Namespace.IndexOf("."));

	CodeBuilder.AppendLine($"using {projectName}.Data.Domains;");
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
	var mappingClass = Entity.MappingClass.ToSafeName();
	var entityClass = Entity.EntityClass.ToSafeName();
	var safeName = $"{entityClass}";

	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Configuration for an entity type <see cref=\"{safeName}\" />");
	CodeBuilder.AppendLine("/// </summary>");

	CodeBuilder.AppendLine($"public class {mappingClass} : BpcEntityTypeConfiguration<{safeName}>");

	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{
		GenerateConfigure();
		//GenerateConstants();
	}

	CodeBuilder.AppendLine("}");

}

private void GenerateConstants()
{
	var entityClass = Entity.EntityClass.ToSafeName();
	var safeName = $"{Entity.EntityNamespace}.{entityClass}";

	CodeBuilder.AppendLine("#region Constants");

	CodeBuilder.AppendLine("public struct Table");
	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{

		CodeBuilder.AppendLine($"/// <summary>Table Schema name constant for entity <see cref=\"{safeName}\" /></summary>");

		CodeBuilder.AppendLine($"public const string Schema = \"{Entity.TableSchema}\";");

		CodeBuilder.AppendLine($"/// <summary>Table Name constant for entity <see cref=\"{safeName}\" /></summary>");
			

		CodeBuilder.AppendLine($"public const string Name = \"{Entity.TableName}\";");
	}
	
	CodeBuilder.AppendLine("}");

	CodeBuilder.AppendLine();
	CodeBuilder.AppendLine("public struct Columns");
	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{
		foreach (var property in Entity.Properties)
		{
			CodeBuilder.AppendLine($"/// <summary>Column Name constant for property <see cref=\"{safeName}.{property.PropertyName}\" /></summary>");				

			CodeBuilder.AppendLine($"public const string {property.PropertyName.ToSafeName()} = {property.ColumnName.ToLiteral()};");
		}
	}

	CodeBuilder.AppendLine("}");
	CodeBuilder.AppendLine("#endregion");
}

private void GenerateConfigure()
{
	var entityClass = Entity.EntityClass.ToSafeName();
	var entityFullName = $"{Entity.EntityNamespace}.{entityClass}";

	CodeBuilder
	.AppendLine("#region Configuration")
	.AppendLine();
	

	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Configures the entity of type <see cref=\"{entityClass}\" />");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine("/// <param name=\"builder\">The builder to be used to configure the entity type.</param>");

	CodeBuilder.AppendLine($"public override void Configure(EntityTypeBuilder<{entityClass}> builder)");
	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{		
		GenerateTableMapping();
		GenerateKeyMapping();
		GeneratePropertyMapping();
		GenerateRelationshipMapping();
	}

	CodeBuilder.AppendLine("}");
	
	CodeBuilder
	.AppendLine()
	.AppendLine("#endregion");
}


private void GenerateRelationshipMapping()
{
	CodeBuilder.AppendLine("// relationships");
	foreach (var relationship in Entity.Relationships.Where(e => e.IsMapped))
	{
		GenerateRelationshipMapping(relationship);
		CodeBuilder.AppendLine();
	}

}

private void GenerateRelationshipMapping(Relationship relationship)
{
	CodeBuilder.Append("builder.HasOne(t => t.");
	CodeBuilder.Append(relationship.PropertyName);
	CodeBuilder.Append(")");
	CodeBuilder.AppendLine();

	CodeBuilder.IncrementIndent();

	CodeBuilder.Append(relationship.PrimaryCardinality == Cardinality.Many
		? ".WithMany(t => t."
		: ".WithOne(t => t.");

	CodeBuilder.Append(relationship.PrimaryPropertyName);
	CodeBuilder.Append(")");

	CodeBuilder.AppendLine();
	CodeBuilder.Append(".HasForeignKey");
	if (relationship.IsOneToOne)
	{
		CodeBuilder.Append("<");
		CodeBuilder.Append(Entity.EntityClass.ToSafeName());
		CodeBuilder.Append(">");
	}
	CodeBuilder.Append("(d => ");

	var keys = relationship.Properties;
	bool wroteLine = false;

	if (keys.Count == 1)
	{
		var propertyName = keys.First().PropertyName.ToSafeName();
		CodeBuilder.Append($"d.{propertyName}");
	}
	else
	{
		CodeBuilder.Append("new { ");
		foreach (var p in keys)
		{
			if (wroteLine)
				CodeBuilder.Append(", ");

			CodeBuilder.Append($"d.{p.PropertyName}");
			wroteLine = true;
		}
		CodeBuilder.Append("}");
	}
	CodeBuilder.Append(")");

	if (!string.IsNullOrEmpty(relationship.RelationshipName))
	{
		CodeBuilder.AppendLine();
		CodeBuilder.Append(".HasConstraintName(\"");
		CodeBuilder.Append(relationship.RelationshipName);
		CodeBuilder.Append("\")");
	}

	CodeBuilder.DecrementIndent();

	CodeBuilder.AppendLine(";");
}


private void GeneratePropertyMapping()
{
	CodeBuilder.AppendLine("// properties");
	foreach (var property in Entity.Properties)
	{
		GeneratePropertyMapping(property);
		CodeBuilder.AppendLine();
	}
}

private void GeneratePropertyMapping(Property property)
{
	bool isString = property.SystemType == typeof(string);
	bool isByteArray = property.SystemType == typeof(byte[]);

	CodeBuilder.Append($"builder.Property(t => t.{property.PropertyName})");

	CodeBuilder.IncrementIndent();
	if (property.IsRequired)
	{
		CodeBuilder.AppendLine();
		CodeBuilder.Append(".IsRequired()");
	}

	if (property.IsRowVersion == true)
	{
		CodeBuilder.AppendLine();
		CodeBuilder.Append(".IsRowVersion()");
	}

	//CodeBuilder.AppendLine();
	//CodeBuilder.Append($".HasColumnName({property.ColumnName.ToLiteral()})");

	if (!string.IsNullOrEmpty(property.StoreType))
	{
		CodeBuilder.AppendLine();
		CodeBuilder.Append($".HasColumnType({property.StoreType.ToLiteral()})");
	}

	if ((isString || isByteArray) && property.Size > 0)
	{
		CodeBuilder.AppendLine();
		CodeBuilder.Append($".HasMaxLength({property.Size.Value.ToString()})");
	}

	if (!string.IsNullOrEmpty(property.Default))
	{
		CodeBuilder.AppendLine();
		CodeBuilder.Append($".HasDefaultValueSql({property.Default.ToLiteral()})");
	}

	switch (property.ValueGenerated)
	{
		case ValueGenerated.OnAdd:
			CodeBuilder.AppendLine();
			CodeBuilder.Append(".ValueGeneratedOnAdd()");
			break;
		case ValueGenerated.OnAddOrUpdate:
			CodeBuilder.AppendLine();
			CodeBuilder.Append(".ValueGeneratedOnAddOrUpdate()");
			break;
		case ValueGenerated.OnUpdate:
			CodeBuilder.AppendLine();
			CodeBuilder.Append(".ValueGeneratedOnUpdate()");
			break;
	}
	CodeBuilder.DecrementIndent();

	CodeBuilder.AppendLine(";");
}


private void GenerateKeyMapping()
{
	CodeBuilder.AppendLine("// key");

	var keys = Entity.Properties.Where(p => p.IsPrimaryKey == true).ToList();
	if (keys.Count == 0)
	{
		CodeBuilder.AppendLine("builder.HasNoKey();");
		CodeBuilder.AppendLine();

		return;
	}

	CodeBuilder.Append("builder.HasKey(t => ");

	if (keys.Count == 1)
	{
		var propertyName = keys.First().PropertyName.ToSafeName();
		CodeBuilder.AppendLine($"t.{propertyName});");
		CodeBuilder.AppendLine();

		return;
	}

	bool wroteLine = false;

	CodeBuilder.Append("new { ");
	foreach (var p in keys)
	{
		if (wroteLine)
			CodeBuilder.Append(", ");

		CodeBuilder.Append("t.");
		CodeBuilder.Append(p.PropertyName);
		wroteLine = true;
	}

	CodeBuilder.AppendLine(" });");
	CodeBuilder.AppendLine();
}

private void GenerateTableMapping()
{
	CodeBuilder.AppendLine("// table");

	var method = Entity.IsView
		? nameof(RelationalEntityTypeBuilderExtensions.ToView)
		: nameof(RelationalEntityTypeBuilderExtensions.ToTable);

	CodeBuilder.AppendLine(Entity.TableSchema.HasValue()
		? $"builder.{method}(nameof({Entity.TableName}), \"{Entity.TableSchema}\");"
		: $"builder.{method}(nameof({Entity.TableName}));");

	CodeBuilder.AppendLine();
}
// run script
WriteCode()