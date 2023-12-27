public string WriteCode()
{
    CodeBuilder.Clear();

    CodeBuilder.AppendLine("using System;");
    CodeBuilder.AppendLine("using System.Collections.Generic;");
    CodeBuilder.AppendLine("using Bpc.Core.Entities;");

    var projectName = TemplateOptions.Namespace.Substring(0, TemplateOptions.Namespace.IndexOf("."));

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

private void GenerateAddClass()
{
    var modelClass = Model.ModelClass.ToSafeName();
    var modelName = modelClass.Substring(0, modelClass.LastIndexOf("Model"));
    modelClass = modelClass.Substring(0, modelClass.LastIndexOf("Model")) + "AddModel";

    CodeBuilder.AppendLine("/// <summary>");
    CodeBuilder.AppendLine($"/// {modelName} Add Model");
    CodeBuilder.AppendLine("/// </summary>");

    CodeBuilder.AppendLine($"public class {modelClass} : BaseBpcModel");

    if (Model.ModelBaseClass.HasValue())
    {
        var modelBase = Model.ModelBaseClass.ToSafeName();
        using (CodeBuilder.Indent())
            CodeBuilder.AppendLine($": {modelBase}");
    }

    CodeBuilder.AppendLine("{");

    using (CodeBuilder.Indent())
    {
        GenerateProperties(false);
    }

    CodeBuilder.AppendLine("}");

}
private void GenerateUpdateClass()
{
    var modelClass = Model.ModelClass.ToSafeName();
    var modelName = modelClass.Substring(0, modelClass.LastIndexOf("Model"));
    modelClass = modelClass.Substring(0, modelClass.LastIndexOf("Model")) + "UpdateModel";

    CodeBuilder.AppendLine("/// <summary>");
    CodeBuilder.AppendLine($"/// {modelName} Update Model");
    CodeBuilder.AppendLine("/// </summary>");

    CodeBuilder.AppendLine($"public class {modelClass} : BaseBpcModel");

    if (Model.ModelBaseClass.HasValue())
    {
        var modelBase = Model.ModelBaseClass.ToSafeName();
        using (CodeBuilder.Indent())
            CodeBuilder.AppendLine($": {modelBase}");
    }

    CodeBuilder.AppendLine("{");

    using (CodeBuilder.Indent())
    {
        GenerateProperties(true);
    }

    CodeBuilder.AppendLine("}");

}

private void GenerateProperties(bool isUpdateMode)
{
    CodeBuilder
    .AppendLine("#region Properties")
    .AppendLine();

    foreach (var property in Model.Properties)
    {
        var propertyType = property.SystemType.ToNullableType(property.IsNullable == true);
        var propertyName = property.PropertyName.ToSafeName();

        if (!isUpdateMode)
            if (propertyName.ToLower() == "id")
                continue;

        if (propertyName.ToLower() == "isdeleted")
            continue;

        if (propertyName.ToLower() == "isactive")
            continue;

        if (propertyName.ToLower() == "isarchive")
            continue;

        if (propertyName.ToLower() == "lastmodifieruserid")
            continue;

        if (propertyName.ToLower() == "registeruserid")
            continue;

        if (propertyName.ToLower() == "lastmodifiedtime")
            continue;

        if (propertyName.ToLower() == "rowversion")
            continue;

        if (propertyName.ToLower() == "registertime")
            continue;

        if (propertyName.ToLower() == "registerdate")
            continue;

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