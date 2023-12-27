public string WriteCode()
{
	CodeBuilder.Clear();	
	
	CodeBuilder.AppendLine("using Bpc.Core.Entities;");
	CodeBuilder.AppendLine("using Bpc.Data.Pagination;");
	CodeBuilder.AppendLine("using System.Threading.Tasks;");
	CodeBuilder.AppendLine("using System.Collections.Generic;");
	

	var imports = new SortedSet<string>();		
	var projectName = TemplateOptions.Namespace.Substring(0, TemplateOptions.Namespace.IndexOf("."));
	
	imports.Add($"{projectName}.Data.Domains");

	var entityClass = Entity.EntityClass.ToSafeName();

	foreach (var import in imports)
		if (Entity.MapperNamespace != import)
			CodeBuilder.AppendLine($"using {import};");

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
	CodeBuilder.AppendLine($"/// {entityClass} Service interface for entity <see cref=\"{entityClass}\"/> .");	
	CodeBuilder.AppendLine("/// </summary>");

	CodeBuilder.AppendLine($"public interface I{entityClass}Service");	

	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{
		GenerateMethods();
	}

	CodeBuilder.AppendLine("}");
}

private void GenerateMethods()
{	
	var entityClass = Entity.EntityClass.ToSafeName();
	var entityClassCammelCase = Char.ToLowerInvariant(entityClass[0]) + entityClass.Substring(1);
	
	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Get {entityClass} by id");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine($"Task<{entityClass}> GetById(int id, int userId);");
	
	CodeBuilder.AppendLine();

    CodeBuilder.AppendLine("/// <summary>");
    CodeBuilder.AppendLine($"/// Get {entityClass}s by filter");
    CodeBuilder.AppendLine("/// </summary>");
    CodeBuilder.AppendLine($"Task<IPagedList<T>> GetPagedListByFilter<T>(FilterPagedListParameter<T> model, int userId) where T : BaseBpcModel;");

    CodeBuilder.AppendLine();

    CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Add new {entityClass}");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine($"Task<{entityClass}> Add({entityClass} {entityClassCammelCase}, int userId);");	
	
	CodeBuilder.AppendLine();
	
	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Update {entityClass}");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine($"Task<{entityClass}> Update({entityClass} {entityClassCammelCase}, int userId);");

    CodeBuilder.AppendLine();

    CodeBuilder.AppendLine("/// <summary>");
    CodeBuilder.AppendLine($"/// Update {entityClass} Status");
    CodeBuilder.AppendLine("/// </summary>");
    CodeBuilder.AppendLine($"Task UpdateStatus({entityClass} {entityClassCammelCase}, int userId);");

    CodeBuilder.AppendLine();
	
	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Delete a {entityClass}");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine($"Task Delete(int id, int userId);");
	
}


// run script
WriteCode()