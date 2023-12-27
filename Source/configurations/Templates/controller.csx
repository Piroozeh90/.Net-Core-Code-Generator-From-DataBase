using System;

public string WriteCode()
{
	CodeBuilder.Clear();	

	CodeBuilder.AppendLine("using System;");
	CodeBuilder.AppendLine("using System.Net;");
	CodeBuilder.AppendLine("using System.Linq;");
	CodeBuilder.AppendLine("using System.Collections.Generic;");
	CodeBuilder.AppendLine("using Microsoft.AspNetCore.Http;");
	CodeBuilder.AppendLine("using Microsoft.AspNetCore.Mvc;");
	CodeBuilder.AppendLine("using System.Threading.Tasks;");
	CodeBuilder.AppendLine("using Bpc.Data.MapData;");
	CodeBuilder.AppendLine("using Bpc.Core.Extension;");
	CodeBuilder.AppendLine("using Bpc.Core.Exceptions;");
	CodeBuilder.AppendLine("using Bpc.Data.Pagination;");
	CodeBuilder.AppendLine("using PublicModule.Data.Constants;");
	CodeBuilder.AppendLine("using PublicModule.Services.IServices;");
	CodeBuilder.AppendLine("using PublicModule.Web.WebApi.API.V1;");
	CodeBuilder.AppendLine("using PublicModule.Web.WebApi.Exceptions;");
	CodeBuilder.AppendLine("using PublicModule.Web.WebApi.Security.Filters;");
	CodeBuilder.AppendLine("using PublicModule.Services.WorkContext;");

	var imports = new SortedSet<string>();		
	var projectName = TemplateOptions.Namespace.Substring(0, TemplateOptions.Namespace.IndexOf("."));
	
	imports.Add($"{projectName}.APIClient.V1.Models.DTOs");
    imports.Add($"{projectName}.Services.Models.Results");
    imports.Add($"{projectName}.APIClient.V1.Models.Parameters");
    imports.Add($"{projectName}.Services.Models.Parameters");
	imports.Add($"{projectName}.Data.Domains");
	imports.Add($"{projectName}.Services.IServices");


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
	var projectName = TemplateOptions.Namespace.Substring(0, TemplateOptions.Namespace.IndexOf("."));

	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// {entityClass} Controller");	
	CodeBuilder.AppendLine("/// </summary>");

	CodeBuilder.AppendLine($"public class {entityClass}Controller : BaseApiV1Controller");	

	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{
		GenerateFields();
		GenerateConstructor();
		GenerateGetActions();
		GeneratePostActions();
        GeneratePutModelActions();
        GeneratePutStatusActions();
		GenerateDeleteActions();
	}

	CodeBuilder.AppendLine("}");
}

private void GenerateFields()
{
	var entityClass = Entity.EntityClass.ToSafeName();
	var entityClassCammelCase = Char.ToLowerInvariant(entityClass[0]) + entityClass.Substring(1);
	
	CodeBuilder.AppendLine("#region Fields").AppendLine();
	
	CodeBuilder.AppendLine($"private readonly IWorkContext _workContext;");
	CodeBuilder.AppendLine($"private readonly I{entityClass}Service _{entityClassCammelCase}Service;");
	CodeBuilder.AppendLine();
	
	CodeBuilder.AppendLine("#endregion").AppendLine();
}

private void GenerateConstructor()
{	
	var entityClass = Entity.EntityClass.ToSafeName();
	var projectName = TemplateOptions.Namespace.Substring(0, TemplateOptions.Namespace.IndexOf("."));
	var entityClassCammelCase = Char.ToLowerInvariant(entityClass[0]) + entityClass.Substring(1);
	
	CodeBuilder.AppendLine("#region Ctor").AppendLine();
	
	CodeBuilder.AppendLine($"/// <summary>");
	CodeBuilder.AppendLine($"/// {entityClass} Controller Constructor");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine($"public {entityClass}Controller(IWorkContext workContext, I{entityClass}Service {entityClassCammelCase}Service)");
	CodeBuilder.AppendLine("{");
	using (CodeBuilder.Indent())
	{
		CodeBuilder.AppendLine($"_workContext = workContext;");
		CodeBuilder.AppendLine($"_{entityClassCammelCase}Service = {entityClassCammelCase}Service;");
	}
	CodeBuilder.AppendLine("}").AppendLine();
	
	CodeBuilder.AppendLine("#endregion").AppendLine();
}

private void GenerateGetActions()
{
	var entityClass = Entity.EntityClass.ToSafeName();
	var entityClassCammelCase = Char.ToLowerInvariant(entityClass[0]) + entityClass.Substring(1);
	
	CodeBuilder.AppendLine("#region Get").AppendLine();

    CodeBuilder.AppendLine("/// <summary>");
    CodeBuilder.AppendLine($"/// Get {entityClass} by Id");
    CodeBuilder.AppendLine("/// </summary>");
    CodeBuilder.AppendLine("[HttpGet(RoutesConstants.GetById)]");
    CodeBuilder.AppendLine($"[ProducesResponseType(typeof({entityClass}Dto) , (int)HttpStatusCode.OK)]");
    CodeBuilder.AppendLine("[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]");
    CodeBuilder.AppendLine("[ProducesResponseType(StatusCodes.Status500InternalServerError)]");
    CodeBuilder.AppendLine($"[BpcAuthorize(\"{entityClass}\",\"o\")]");
    CodeBuilder.AppendLine("public async Task<IActionResult> GetById(int id)");
    CodeBuilder.AppendLine("{");
    using (CodeBuilder.Indent())
    {
        CodeBuilder.AppendLine($"var {entityClassCammelCase} = await _{entityClassCammelCase}Service.GetById(id, _workContext.CurrentUserInfo.Id);");
        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine($"var response = {entityClassCammelCase}.ToObject<{entityClass}Dto>();");
        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine("return Ok(response);");
    }
    CodeBuilder.AppendLine("}");

    CodeBuilder.AppendLine();
    CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Get {entityClass}s by filter");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine("[HttpGet(\"GetPagedListByFilter\")]");
	CodeBuilder.AppendLine($"[ProducesResponseType(typeof(IPagedList<{entityClass}Dto>) , (int)HttpStatusCode.OK)]");
	CodeBuilder.AppendLine("[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]");
	CodeBuilder.AppendLine("[ProducesResponseType(StatusCodes.Status500InternalServerError)]");
    CodeBuilder.AppendLine($"[BpcAuthorize(\"{entityClass}\",\"o\")]");
	CodeBuilder.AppendLine($"public async Task<IActionResult> GetPagedListByFilter([FromQuery] FilterPagedListParameter<{entityClass}Dto> model)");
	CodeBuilder.AppendLine("{");
	using (CodeBuilder.Indent())
	{
        CodeBuilder.AppendLine("//Get by filter");

        CodeBuilder.AppendLine($"var result = await _{entityClassCammelCase}Service.GetPagedListByFilter(model, _workContext.CurrentUserInfo.Id );");
		CodeBuilder.AppendLine();

        CodeBuilder.AppendLine("//Set paged result");
        CodeBuilder.AppendLine($"var paginationResult = result.GetPagedListLink(model, \"GetPagedList\");");
		CodeBuilder.AppendLine();
		
		CodeBuilder.AppendLine("return Ok(paginationResult);");
	}	
	CodeBuilder.AppendLine("}").AppendLine();		

	
	
	CodeBuilder.AppendLine("#endregion").AppendLine();
}
                             
private void GeneratePostActions()
{
	var entityClass = Entity.EntityClass.ToSafeName();
	var entityClassCammelCase = Char.ToLowerInvariant(entityClass[0]) + entityClass.Substring(1);
	
	CodeBuilder.AppendLine("#region Post").AppendLine();	

	
	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Add new {entityClass}");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine("[HttpPost]");
	CodeBuilder.AppendLine($"[ProducesResponseType(typeof({entityClass}Dto) , (int)HttpStatusCode.OK)]");
	CodeBuilder.AppendLine("[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]");
	CodeBuilder.AppendLine("[ProducesResponseType(StatusCodes.Status500InternalServerError)]");
    CodeBuilder.AppendLine($"[BpcAuthorize(\"{entityClass}\",\"c\")]");
	CodeBuilder.AppendLine($"public async Task<IActionResult> Add({entityClass}AddModel model)");
	CodeBuilder.AppendLine("{");
	using (CodeBuilder.Indent())
	{
		CodeBuilder.AppendLine("//Map to Entity");
		CodeBuilder.AppendLine($"var entity = model.ToEntity<{entityClass}>();");
		CodeBuilder.AppendLine($"entity.RegisterUserId = _workContext.CurrentUserInfo.Id;");
		CodeBuilder.AppendLine($"entity.RegisterDate = DateTime.Now;");
		CodeBuilder.AppendLine();
		
		CodeBuilder.AppendLine($"//Add {entityClass}");
		CodeBuilder.AppendLine($"var {entityClassCammelCase} = await _{entityClassCammelCase}Service.Add(entity, _workContext.CurrentUserInfo.Id);");
		CodeBuilder.AppendLine();
	
		CodeBuilder.AppendLine("//Map to Dto");
		CodeBuilder.AppendLine($"var response = {entityClassCammelCase}.ToObject<{entityClass}Dto>();");
		CodeBuilder.AppendLine();
		
		CodeBuilder.AppendLine("return Ok(response);");
	}	
	CodeBuilder.AppendLine("}").AppendLine();		
			
	CodeBuilder.AppendLine("#endregion").AppendLine();
}

private void GeneratePutModelActions()
{
	var entityClass = Entity.EntityClass.ToSafeName();
	var entityClassCammelCase = Char.ToLowerInvariant(entityClass[0]) + entityClass.Substring(1);
	
	CodeBuilder.AppendLine("#region Put").AppendLine();	

	
	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Update {entityClass}");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine("[HttpPut]");
	CodeBuilder.AppendLine($"[ProducesResponseType(typeof({entityClass}Dto) , (int)HttpStatusCode.OK)]");
	CodeBuilder.AppendLine("[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]");
	CodeBuilder.AppendLine("[ProducesResponseType(StatusCodes.Status500InternalServerError)]");
    CodeBuilder.AppendLine($"[BpcAuthorize(\"{entityClass}\",\"u\")]");
	CodeBuilder.AppendLine($"public async Task<IActionResult> Update({entityClass}UpdateModel model)");
	CodeBuilder.AppendLine("{");
	using (CodeBuilder.Indent())
	{
        CodeBuilder.AppendLine($"DataProtect.NotNegativeAndZero(model.Id, nameof(model.Id));");
        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine($"//Get current {entityClass}");
		CodeBuilder.AppendLine($"var currentEntity = await _{entityClassCammelCase}Service.GetById(model.Id, _workContext.CurrentUserInfo.Id);");
		CodeBuilder.AppendLine("if (currentEntity.IsNull())");
		using (CodeBuilder.Indent())
		{
			CodeBuilder.AppendLine("throw new ManualNotFoundException();");
		}
		CodeBuilder.AppendLine();
		
		CodeBuilder.AppendLine("//Map to Entity");
		CodeBuilder.AppendLine($"var entity = model.ToEntity(currentEntity);");
		CodeBuilder.AppendLine();
		
		CodeBuilder.AppendLine($"//Update {entityClass}");
		CodeBuilder.AppendLine($"var {entityClassCammelCase} = await _{entityClassCammelCase}Service.Update(entity, _workContext.CurrentUserInfo.Id);");
		CodeBuilder.AppendLine();
		
		CodeBuilder.AppendLine("//Map to Dto");
		CodeBuilder.AppendLine($"var response = {entityClassCammelCase}.ToObject<{entityClass}Dto>();");
		CodeBuilder.AppendLine();
		
		CodeBuilder.AppendLine("return Ok(response);");	
	}	
	CodeBuilder.AppendLine("}").AppendLine();		
			
	CodeBuilder.AppendLine("#endregion").AppendLine();
}

private void GeneratePutStatusActions()
{
    var entityClass = Entity.EntityClass.ToSafeName();
    var entityClassCammelCase = Char.ToLowerInvariant(entityClass[0]) + entityClass.Substring(1);

    CodeBuilder.AppendLine("#region Update Status").AppendLine();

    CodeBuilder.AppendLine("/// <summary>");
    CodeBuilder.AppendLine($"/// Active {entityClass}");
    CodeBuilder.AppendLine("/// </summary>");
    CodeBuilder.AppendLine("[HttpPut(RoutesConstants.Activate)]");
    CodeBuilder.AppendLine($"[ProducesResponseType(typeof(int) , (int)HttpStatusCode.OK)]");
    CodeBuilder.AppendLine("[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]");
    CodeBuilder.AppendLine("[ProducesResponseType(StatusCodes.Status500InternalServerError)]");
    CodeBuilder.AppendLine($"[BpcAuthorize(\"{entityClass}\",\"u\")]");
    CodeBuilder.AppendLine($"public async Task<IActionResult> Activate(int id)");
    CodeBuilder.AppendLine("{");
    using (CodeBuilder.Indent())
    {
        CodeBuilder.AppendLine($"DataProtect.NotNegativeAndZero(id, nameof(id));");
        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine($"//Get current {entityClass}");
        CodeBuilder.AppendLine($"var currentEntity = await _{entityClassCammelCase}Service.GetById(id, _workContext.CurrentUserInfo.Id);");
        CodeBuilder.AppendLine("if (currentEntity.IsNull())");
        using (CodeBuilder.Indent())
        {
            CodeBuilder.AppendLine("throw new ManualNotFoundException();");
        }
        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine("//Update Status");
        CodeBuilder.AppendLine($"currentEntity.IsActive = true;");
        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine($"//Update {entityClass}");
        CodeBuilder.AppendLine($"await _{entityClassCammelCase}Service.UpdateStatus(currentEntity, _workContext.CurrentUserInfo.Id);");
        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine("return Ok();");
    }
    CodeBuilder.AppendLine("}").AppendLine();

    CodeBuilder.AppendLine();

    CodeBuilder.AppendLine("/// <summary>");
    CodeBuilder.AppendLine($"/// Deactivate {entityClass}");
    CodeBuilder.AppendLine("/// </summary>");
    CodeBuilder.AppendLine("[HttpPut(RoutesConstants.Deactivate)]");
    CodeBuilder.AppendLine($"[ProducesResponseType(typeof(int) , (int)HttpStatusCode.OK)]");
    CodeBuilder.AppendLine("[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]");
    CodeBuilder.AppendLine("[ProducesResponseType(StatusCodes.Status500InternalServerError)]");
    CodeBuilder.AppendLine($"[BpcAuthorize(\"{entityClass}\",\"u\")]");
    CodeBuilder.AppendLine($"public async Task<IActionResult> Deactivate(int id)");
    CodeBuilder.AppendLine("{");
    using (CodeBuilder.Indent())
    {
        CodeBuilder.AppendLine($"DataProtect.NotNegativeAndZero(id, nameof(id));");
        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine($"//Get current {entityClass}");
        CodeBuilder.AppendLine($"var currentEntity = await _{entityClassCammelCase}Service.GetById(id, _workContext.CurrentUserInfo.Id);");
        CodeBuilder.AppendLine("if (currentEntity.IsNull())");
        using (CodeBuilder.Indent())
        {
            CodeBuilder.AppendLine("throw new ManualNotFoundException();");
        }
        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine("//Update Status");
        CodeBuilder.AppendLine($"currentEntity.IsActive = false;");
        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine($"//Update {entityClass}");
        CodeBuilder.AppendLine($"await _{entityClassCammelCase}Service.UpdateStatus(currentEntity, _workContext.CurrentUserInfo.Id);");
        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine("return Ok();");
    }
    CodeBuilder.AppendLine("}").AppendLine();

    CodeBuilder.AppendLine("#endregion").AppendLine();
}


private void GenerateDeleteActions()
{
	var entityClass = Entity.EntityClass.ToSafeName();
	var entityClassCammelCase = Char.ToLowerInvariant(entityClass[0]) + entityClass.Substring(1);
	
	CodeBuilder.AppendLine("#region Delete").AppendLine();
	
	CodeBuilder.AppendLine();
	
	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Delete {entityClass} by Id");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine("[HttpDelete(RoutesConstants.DeleteById)]");
	CodeBuilder.AppendLine("[ProducesResponseType((int) HttpStatusCode.OK)]");
	CodeBuilder.AppendLine("[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]");
	CodeBuilder.AppendLine("[ProducesResponseType(StatusCodes.Status500InternalServerError)]");
    CodeBuilder.AppendLine($"[BpcAuthorize(\"{entityClass}\",\"d\")]");
	CodeBuilder.AppendLine("public async Task<IActionResult> Delete(int id)");
	CodeBuilder.AppendLine("{");
	using (CodeBuilder.Indent())
	{
		CodeBuilder.AppendLine($"DataProtect.NotNegativeAndZero(id, nameof(id));");	
		CodeBuilder.AppendLine();	
		CodeBuilder.AppendLine($"await _{entityClassCammelCase}Service.Delete(id, _workContext.CurrentUserInfo.Id);");	
		CodeBuilder.AppendLine();	
		CodeBuilder.AppendLine("return Ok();");	
	}
	CodeBuilder.AppendLine("}");
	
	CodeBuilder.AppendLine();
			
	CodeBuilder.AppendLine("#endregion").AppendLine();
}    

// run script
WriteCode()