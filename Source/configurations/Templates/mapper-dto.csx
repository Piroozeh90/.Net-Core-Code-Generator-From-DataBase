public string WriteCode()
{
	CodeBuilder.Clear();

	CodeBuilder.AppendLine("using System;");
	CodeBuilder.AppendLine("using AutoMapper;");

	var imports = new SortedSet<string>();
	
	var projectName = TemplateOptions.Namespace.Substring(0, TemplateOptions.Namespace.IndexOf("."));	
	imports.Add($"{projectName}.Data.Domains");

	var entityClass = Entity.EntityClass.ToSafeName();
	imports.Add($"{projectName}.Services.Models.Results");		

	foreach (var import in imports)
		if (Entity.MapperNamespace != import)
			CodeBuilder.AppendLine($"using {import};");

	CodeBuilder.AppendLine();

	CodeBuilder.AppendLine($"namespace {TemplateOptions.Namespace};");

	using (CodeBuilder.Indent())
	{
		GenerateClass();
	}

	return CodeBuilder.ToString();
}

private void GenerateClass()
{
	var entityClass = Entity.EntityClass.ToSafeName();
	var mapperClass = Entity.MapperClass.ToSafeName();

	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Mapper class for entity <see cref=\"{entityClass}\"/> .");
	CodeBuilder.AppendLine("/// </summary>");

	var mapperBaseClass = Entity.MapperBaseClass.ToSafeName();
	CodeBuilder.AppendLine($"public class {mapperClass} : {mapperBaseClass}");	

	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{
		GenerateConstructor();
	}

	CodeBuilder.AppendLine("}");
}

private void GenerateConstructor()
{
	var mapperClass = Entity.MapperClass.ToSafeName();

	var entityClass = Entity.EntityClass.ToSafeName();
	var entityFullName = $"{entityClass}";

	CodeBuilder.AppendLine();
	CodeBuilder
	.AppendLine("#region Constructor")
	.AppendLine();

	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Initializes a new instance of the <see cref=\"{mapperClass}\"/> class.");
	CodeBuilder.AppendLine("/// </summary>");

	CodeBuilder.AppendLine($"public {mapperClass}()");
	CodeBuilder.AppendLine("{");

	string readFullName = null;
	string updateFullName = null;

	using (CodeBuilder.Indent())
	{
		foreach (var model in Entity.Models)
		{
			var modelClass = model.ModelClass.ToSafeName();
			var modelFullName = $"{modelClass}";

			switch (model.ModelType)
			{
				case ModelType.Read:
					readFullName = modelFullName;
					CodeBuilder.AppendLine($"CreateMap<{entityFullName}, {entityFullName}Dto>();").AppendLine();
					//CodeBuilder.AppendLine($"CreateMap<{modelFullName}, {entityFullName}>();").AppendLine();
					break;
				//case ModelType.Create:
				//	CodeBuilder.AppendLine($"CreateMap<{entityFullName}AddModel, {entityFullName}>();").AppendLine();
				//	break;
				//case ModelType.Update:
				//	updateFullName = modelFullName;
				//	CodeBuilder.AppendLine($"CreateMap<{entityFullName}UpdateModel, {entityFullName}>();").AppendLine();
				//	break;
			}
			
			
		}

		// include support for coping read model to update model
		if (readFullName.HasValue() && updateFullName.HasValue())
			CodeBuilder.AppendLine($"CreateMap<{readFullName}, {updateFullName}>();").AppendLine();

	}

	CodeBuilder.AppendLine("}");
	
	CodeBuilder
	.AppendLine("#endregion")
	.AppendLine();
}


// run script
WriteCode()