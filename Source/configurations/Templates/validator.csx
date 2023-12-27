public string WriteCode()
{
	CodeBuilder.Clear();

	CodeBuilder.AppendLine("using System;");
	CodeBuilder.AppendLine("using FluentValidation;");
	CodeBuilder.AppendLine("using Selected.Core.Extension;");
	CodeBuilder.AppendLine("using Selected.Data.Validator;");	
	
	var projectName = TemplateOptions.Namespace.Substring(0, TemplateOptions.Namespace.IndexOf("."));	
	CodeBuilder.AppendLine($"using {projectName}.APIClient.V1.Models.Parameters;");
	
	CodeBuilder.AppendLine("using PublicModule.Data.Constants;");
	
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
	var validatorClass = Model.ValidatorClass.ToSafeName();
	var modelClass = Model.ModelClass.ToSafeName();

	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Validator class for <see cref=\"{modelClass}\"/> .");
	CodeBuilder.AppendLine("/// </summary>");

	var validatorBase = Model.ValidatorBaseClass.ToSafeName();
	CodeBuilder.AppendLine($"public class {validatorClass} : BaseSelectedValidator<{modelClass}>");

	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{
		GenerateConstructor();
	}

	CodeBuilder.AppendLine("}");
}

private void GenerateConstructor()
{
	var validatorClass = Model.ValidatorClass.ToSafeName();

	CodeBuilder
	.AppendLine("#region Constructor")
	.AppendLine();	

	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Initializes a new instance of the <see cref=\"{validatorClass}\"/> class.");
	CodeBuilder.AppendLine("/// </summary>");

	CodeBuilder.AppendLine($"public {validatorClass}(IBaseLocalizationService localizationService)");
	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{
		foreach (var property in Model.Properties)
		{
			if (property.ValueGenerated.HasValue)
				continue;

			var propertyName = property.PropertyName.ToSafeName();

			if (property.IsRequired && property.SystemType == typeof(string))
				CodeBuilder.AppendLine($"RuleFor(p => p.{propertyName}).NotEmpty();");
			if (property.Size.HasValue && property.SystemType == typeof(string))
				CodeBuilder.AppendLine($"RuleFor(p => p.{propertyName}).MaximumLength({property.Size});");

		}		
	}

	CodeBuilder.AppendLine("}");
	CodeBuilder.AppendLine();
	CodeBuilder.AppendLine("#endregion");
}

// run script
WriteCode()