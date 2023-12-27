public string WriteCode()
{
	CodeBuilder.Clear();

	CodeBuilder.AppendLine("using System;");
	CodeBuilder.AppendLine("using FluentValidation;");
	
	var projectName = TemplateOptions.Namespace.Substring(0, TemplateOptions.Namespace.IndexOf("."));	
	CodeBuilder.AppendLine($"using {projectName}.APIClient.V1.Models.Parameters;");
	
	CodeBuilder.AppendLine();

	CodeBuilder.AppendLine($"namespace {TemplateOptions.Namespace};");
    CodeBuilder.AppendLine();

    using (CodeBuilder.Indent())
	{
        GenerateAddClass();

        CodeBuilder.AppendLine();
        CodeBuilder.AppendLine();

        GenerateUpdateClass();

    }


	return CodeBuilder.ToString();
}

#region Add Model Validator

private void GenerateAddClass()
{
    var validatorClass = Model.ValidatorClass.ToSafeName();
    validatorClass = validatorClass.Substring(0, validatorClass.LastIndexOf("Model")) + "AddModelValidator";
    var modelClass = Model.ModelClass.ToSafeName();
    modelClass = modelClass.Substring(0, modelClass.LastIndexOf("Model")) + "AddModel";

    CodeBuilder.AppendLine("/// <summary>");
    CodeBuilder.AppendLine($"/// Validator class for <see cref=\"{modelClass}\"/> .");
    CodeBuilder.AppendLine("/// </summary>");

    var validatorBase = Model.ValidatorBaseClass.ToSafeName();
    CodeBuilder.AppendLine($"public class {validatorClass} : AbstractValidator<{modelClass}>");

    CodeBuilder.AppendLine("{");

    using (CodeBuilder.Indent())
    {
        GenerateAddConstructor();
    }

    CodeBuilder.AppendLine("}");
}

private void GenerateAddConstructor()
{
    var validatorClass = Model.ValidatorClass.ToSafeName();
    validatorClass = validatorClass.Substring(0, validatorClass.LastIndexOf("Model")) + "AddModelValidator";

    CodeBuilder
    .AppendLine("#region Constructor")
    .AppendLine();

    CodeBuilder.AppendLine("/// <summary>");
    CodeBuilder.AppendLine($"/// Initializes a new instance of the <see cref=\"{validatorClass}\"/> class.");
    CodeBuilder.AppendLine("/// </summary>");

    CodeBuilder.AppendLine($"public {validatorClass}()");
    CodeBuilder.AppendLine("{");

    using (CodeBuilder.Indent())
    {
        foreach (var property in Model.Properties)
        {
            if (property.ValueGenerated.HasValue)
                continue;

            var propertyName = property.PropertyName.ToSafeName();

            if (property.IsRequired && property.SystemType == typeof(string))
                CodeBuilder.AppendLine($"RuleFor(p => p.{propertyName}).NotNull().NotEmpty();");

            if (property.Size.HasValue && property.SystemType == typeof(string))
                CodeBuilder.AppendLine($"RuleFor(p => p.{propertyName}).MaximumLength({property.Size});");

        }
    }

    CodeBuilder.AppendLine("}");
    CodeBuilder.AppendLine();
    CodeBuilder.AppendLine("#endregion");
}


#endregion

#region Update Model Validator

private void GenerateUpdateClass()
{
    var validatorClass = Model.ValidatorClass.ToSafeName();
    validatorClass = validatorClass.Substring(0, validatorClass.LastIndexOf("Model")) + "UpdateModelValidator";
    var modelClass = Model.ModelClass.ToSafeName();
    modelClass = modelClass.Substring(0, modelClass.LastIndexOf("Model")) + "UpdateModel";

    CodeBuilder.AppendLine("/// <summary>");
    CodeBuilder.AppendLine($"/// Validator class for <see cref=\"{modelClass}\"/> .");
    CodeBuilder.AppendLine("/// </summary>");

    var validatorBase = Model.ValidatorBaseClass.ToSafeName();
    CodeBuilder.AppendLine($"public class {validatorClass} : AbstractValidator<{modelClass}>");

    CodeBuilder.AppendLine("{");

    using (CodeBuilder.Indent())
    {
        GenerateUpdateConstructor();
    }

    CodeBuilder.AppendLine("}");
}

private void GenerateUpdateConstructor()
{
    var validatorClass = Model.ValidatorClass.ToSafeName();
    validatorClass = validatorClass.Substring(0, validatorClass.LastIndexOf("Model")) + "UpdateModelValidator";

    CodeBuilder
    .AppendLine("#region Constructor")
    .AppendLine();

    CodeBuilder.AppendLine("/// <summary>");
    CodeBuilder.AppendLine($"/// Initializes a new instance of the <see cref=\"{validatorClass}\"/> class.");
    CodeBuilder.AppendLine("/// </summary>");

    CodeBuilder.AppendLine($"public {validatorClass}()");
    CodeBuilder.AppendLine("{");

    using (CodeBuilder.Indent())
    {
        foreach (var property in Model.Properties)
        {
            if (property.ValueGenerated.HasValue)
                continue;

            var propertyName = property.PropertyName.ToSafeName();

            if (property.IsRequired && property.SystemType == typeof(string))
                CodeBuilder.AppendLine($"RuleFor(p => p.{propertyName}).NotNull().NotEmpty();");

            if (property.Size.HasValue && property.SystemType == typeof(string))
                CodeBuilder.AppendLine($"RuleFor(p => p.{propertyName}).MaximumLength({property.Size});");

        }
    }

    CodeBuilder.AppendLine("}");
    CodeBuilder.AppendLine();
    CodeBuilder.AppendLine("#endregion");
}


#endregion

// run script
WriteCode()