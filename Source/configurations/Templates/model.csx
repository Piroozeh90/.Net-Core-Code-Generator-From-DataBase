public string WriteCode()
{
	CodeBuilder.Clear();

	CodeBuilder.AppendLine("using System;");
	CodeBuilder.AppendLine("using System.Collections.Generic;");
	CodeBuilder.AppendLine("using System.ComponentModel;");
	CodeBuilder.AppendLine("using Selected.Core;");
	CodeBuilder.AppendLine("using FluentValidation.Attributes;");
	
	var projectName = TemplateOptions.Namespace.Substring(0, TemplateOptions.Namespace.IndexOf("."));
	
	CodeBuilder.AppendLine($"using {projectName}.APIClient.V1.Validators;");
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
	var modelClass = Model.ModelClass.ToSafeName();
	modelClass = modelClass.Substring(0, modelClass.LastIndexOf("Model")) + "AddModel";

	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// {modelClass} Model class");
	CodeBuilder.AppendLine("/// </summary>");

	CodeBuilder.AppendLine($"[Validator(typeof({modelClass}Validator))]");
	CodeBuilder.AppendLine($"public class {modelClass} : BaseSelectedModel");

	if (Model.ModelBaseClass.HasValue())
	{
		var modelBase = Model.ModelBaseClass.ToSafeName();
		using (CodeBuilder.Indent())
			CodeBuilder.AppendLine($": {modelBase}");
	}

	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{
		GenerateProperties();
	}

	CodeBuilder.AppendLine("}");

}


private void GenerateProperties()
{
	CodeBuilder
	.AppendLine("#region Properties")
	.AppendLine();
	var modelClass = Model.ModelClass.ToSafeName();
	modelClass = modelClass.Substring(0, modelClass.LastIndexOf("Model")) + "AddModel";
	
	foreach (var property in Model.Properties)
	{
		var propertyType = property.SystemType.ToNullableType(property.IsNullable == true);
		var propertyName = property.PropertyName.ToSafeName();

		CodeBuilder.AppendLine("/// <summary>");
		CodeBuilder.AppendLine($"/// Gets or sets the property value for '{property.PropertyName}'.");
		CodeBuilder.AppendLine("/// </summary>");
		CodeBuilder.AppendLine("/// <value>");
		CodeBuilder.AppendLine($"/// The property value for '{property.PropertyName}'.");
		CodeBuilder.AppendLine("/// </value>");
		//CodeBuilder.AppendLine($"[DisplayName(\"{modelClass}.Fields.{propertyName}\")]");
		CodeBuilder.AppendLine($"public {propertyType} {propertyName} {{ get; set; }}");
		CodeBuilder.AppendLine();
	}
	
	CodeBuilder.AppendLine("#endregion");
	CodeBuilder.AppendLine();
}

// run script
WriteCode()