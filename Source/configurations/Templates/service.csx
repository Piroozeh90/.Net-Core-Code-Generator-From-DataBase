public string WriteCode()
{
	CodeBuilder.Clear();	

	CodeBuilder.AppendLine("using System;");	
	CodeBuilder.AppendLine("using System.Linq;");
	CodeBuilder.AppendLine("using System.Linq.Dynamic.Core;");
	CodeBuilder.AppendLine("using System.Threading.Tasks;");
	CodeBuilder.AppendLine("using System.Collections.Generic;");	
	CodeBuilder.AppendLine("using Bpc.Core.Exceptions;");	
	CodeBuilder.AppendLine("using Bpc.Core.Extension;");	
	CodeBuilder.AppendLine("using Bpc.Data.Repository;");		
	CodeBuilder.AppendLine("using PublicModule.Data.Constants;");
	CodeBuilder.AppendLine("using PublicModule.Services.General.ServiceProvider;");
	CodeBuilder.AppendLine("using PublicModule.Web.WebApi.Exceptions;");
	CodeBuilder.AppendLine("using PublicModule.Data.Enums;");
	CodeBuilder.AppendLine("using Microsoft.EntityFrameworkCore;");
	CodeBuilder.AppendLine("using Bpc.Core.Entities;");
	CodeBuilder.AppendLine("using Bpc.Data.Pagination;");
	CodeBuilder.AppendLine("using AutoMapper.QueryableExtensions;");
	CodeBuilder.AppendLine("using Bpc.Core.Infrastructure.Mapper;");

	var imports = new SortedSet<string>();		
	var projectName = TemplateOptions.Namespace.Substring(0, TemplateOptions.Namespace.IndexOf("."));
	
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
	CodeBuilder.AppendLine($"/// {entityClass} Service class for entity <see cref=\"{entityClass}\"/> .");	
	CodeBuilder.AppendLine("/// </summary>");

	CodeBuilder.AppendLine($"public class {entityClass}Service : BaseServiceProvider, I{entityClass}Service");	

	CodeBuilder.AppendLine("{");

	using (CodeBuilder.Indent())
	{
		GenerateConstructor();
		GenerateMethods();
	}

	CodeBuilder.AppendLine("}");
}

private void GenerateConstructor()
{	
	var entityClass = Entity.EntityClass.ToSafeName();
	var projectName = TemplateOptions.Namespace.Substring(0, TemplateOptions.Namespace.IndexOf("."));
	
	CodeBuilder.AppendLine("#region Fields").AppendLine();
	
	CodeBuilder.AppendLine($"private readonly IRepository<{entityClass}> _repository;");
	CodeBuilder.AppendLine();
	
	CodeBuilder.AppendLine("#endregion").AppendLine();
	
	
	CodeBuilder.AppendLine("#region Ctor").AppendLine();
	
	CodeBuilder.AppendLine($"/// <summary>");
	CodeBuilder.AppendLine("/// ");
	CodeBuilder.AppendLine("/// </summary>");
	
	CodeBuilder.AppendLine($"public {entityClass}Service()");
	CodeBuilder.AppendLine("{");
	using (CodeBuilder.Indent())
	{
		CodeBuilder.AppendLine("#region Repositories").AppendLine();		
		CodeBuilder.AppendLine($"_repository = ResolveByName<IRepository<{entityClass}>>(ContextNameConstants.{projectName});");		
		CodeBuilder.AppendLine();
		CodeBuilder.AppendLine("#endregion").AppendLine();
		
		
		CodeBuilder.AppendLine("#region Dependencies").AppendLine();
		CodeBuilder.AppendLine();
		CodeBuilder.AppendLine("#endregion");
	}
	CodeBuilder.AppendLine("}").AppendLine();
	
	CodeBuilder.AppendLine("#endregion").AppendLine();
}

private void GenerateMethods()
{	
	var entityClass = Entity.EntityClass.ToSafeName();
	var entityClassCammelCase = Char.ToLowerInvariant(entityClass[0]) + entityClass.Substring(1);


    CodeBuilder.AppendLine("#region Get").AppendLine();

    CodeBuilder.AppendLine();
	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Get {entityClass} by id");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine($"public async Task<{entityClass}> GetById(int id, int userId)");
	CodeBuilder.AppendLine("{");
	using (CodeBuilder.Indent())
	{
		CodeBuilder.AppendLine("DataProtect.NotNegativeAndZero(id, nameof(id));");
		CodeBuilder.AppendLine();
		
		CodeBuilder.AppendLine($"var {entityClassCammelCase} = await _repository.TableNoTracking.SingleOrDefaultAsync(e => e.Id == id);");
		
		CodeBuilder.AppendLine($"if ({entityClassCammelCase}.IsNull())");
		using (CodeBuilder.Indent())
		{
			CodeBuilder.AppendLine("throw new ManualNotFoundException();");
		}
		
		CodeBuilder.AppendLine();
		
		CodeBuilder.AppendLine($"return {entityClassCammelCase};");		
	}
	CodeBuilder.AppendLine("}").AppendLine();


    CodeBuilder.AppendLine("/// <summary>");
    CodeBuilder.AppendLine($"/// Get {entityClass}s by filter");
    CodeBuilder.AppendLine("/// </summary>");
    CodeBuilder.AppendLine($"public async Task<IPagedList<T>> GetPagedListByFilter<T>(FilterPagedListParameter<T> model, int userId) where T : BaseBpcModel");
    CodeBuilder.AppendLine("{");
    using (CodeBuilder.Indent())
    {
        CodeBuilder.AppendLine($"//Check user permission");
        CodeBuilder.AppendLine($"var globalPermission = await _workContext.CheckUserAcccessToOperation(\"{entityClass}\", \"o\");");
        CodeBuilder.AppendLine();
        CodeBuilder.AppendLine($"//if (globalPermission.ProvinceId.HasValue)queryParameter.ProvinceId = globalPermission.ProvinceId.Value;");
        CodeBuilder.AppendLine($"//if (globalPermission.CityId.HasValue)queryParameter.CityId = globalPermission.CityId.Value;");
        CodeBuilder.AppendLine();
        CodeBuilder.AppendLine("var query = _repository.TableNoTracking;");
        CodeBuilder.AppendLine();
        CodeBuilder.AppendLine("if (model.IsHasFilter) query = query.Where(model.Filter);");
        CodeBuilder.AppendLine();
        CodeBuilder.AppendLine("if (model.IsHasOrderBy) query = query.OrderBy(model.OrderBy);");
        CodeBuilder.AppendLine("else");
        CodeBuilder.AppendLine("query = query.OrderByDescending(x => x.Id);");
        CodeBuilder.AppendLine();
        CodeBuilder.AppendLine(" var result = await PagedList<T>.InitializeAsync(query.ProjectTo<T>(AutoMapperConfiguration.MapperConfiguration),model.PageIndex, model.PageSize);");
        CodeBuilder.AppendLine();
        CodeBuilder.AppendLine("return result;");

    }
    CodeBuilder.AppendLine("}").AppendLine();

    CodeBuilder.AppendLine("#endregion").AppendLine();
    
	
	CodeBuilder.AppendLine("#region Add").AppendLine();
	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Add new {entityClass}");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine($"public async Task<{entityClass}> Add({entityClass} {entityClassCammelCase}, int userId)");
	CodeBuilder.AppendLine("{");
	using (CodeBuilder.Indent())
	{
		CodeBuilder.AppendLine($"DataProtect.NotNull({entityClassCammelCase}, nameof({entityClass}));");
		CodeBuilder.AppendLine();

        CodeBuilder.AppendLine($"//Check user permission");
        CodeBuilder.AppendLine($"var globalPermission = await _workContext.CheckUserAcccessToOperation(\"{entityClass}\", \"c\");");
        CodeBuilder.AppendLine();
        CodeBuilder.AppendLine($"//if (globalPermission.ProvinceId.HasValue){entityClassCammelCase}.ProvinceId = globalPermission.ProvinceId.Value;");
        CodeBuilder.AppendLine($"//if (globalPermission.CityId.HasValue){entityClassCammelCase}.CityId = globalPermission.CityId.Value;");
        CodeBuilder.AppendLine();


        CodeBuilder.AppendLine($"//Check exists {entityClass}");
		CodeBuilder.AppendLine($"//if (await CheckExists{entityClass}({entityClassCammelCase}))");
		using (CodeBuilder.Indent())
		{
			CodeBuilder.AppendLine($"//throw new BpcValidationException(nameof({entityClassCammelCase}.Name), PublicResourceConstants.GeneralDuplicateField);");
		}
		
		CodeBuilder.AppendLine();
		
		CodeBuilder.AppendLine($"//Add and Save new {entityClass}");
		CodeBuilder.AppendLine($"await _repository.InsertAndSaveChangesAsync({entityClassCammelCase});");

        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine($"//Log add {entityClass}");
        CodeBuilder.AppendLine($"await _logService.Add(\"{entityClass}\",(int)UserLogTypes.Global, (int)UserLogActions.Add, {entityClassCammelCase}.Id,\"افزودن  \" +  {entityClassCammelCase}.Name ,userId);");

        CodeBuilder.AppendLine();
		
		CodeBuilder.AppendLine($"return {entityClassCammelCase};");
		
	}	
	CodeBuilder.AppendLine("}").AppendLine();	
	CodeBuilder.AppendLine("#endregion").AppendLine();
	
	
	CodeBuilder.AppendLine("#region Update").AppendLine();	
	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Update {entityClass}");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine($"public async Task<{entityClass}> Update({entityClass} {entityClassCammelCase}, int userId)");		
	CodeBuilder.AppendLine("{");
	using (CodeBuilder.Indent())
	{
		CodeBuilder.AppendLine($"DataProtect.NotNull({entityClassCammelCase}, nameof({entityClass}));");
		CodeBuilder.AppendLine();
		
		CodeBuilder.AppendLine($"//Check exists {entityClass}");
		CodeBuilder.AppendLine($"//if (await CheckExists{entityClass}({entityClassCammelCase}))");
		using (CodeBuilder.Indent())
		{
			CodeBuilder.AppendLine($"//throw new BpcValidationException(nameof({entityClassCammelCase}.Name), PublicResourceConstants.GeneralDuplicateField);");
		}
		
		CodeBuilder.AppendLine();
		
		CodeBuilder.AppendLine($"//Update {entityClass}");
		CodeBuilder.AppendLine($"await _repository.UpdateAndSaveChangesAsync({entityClassCammelCase});");

        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine($"//Log update {entityClass}");
        CodeBuilder.AppendLine($"await _logService.Add(\"{entityClass}\",(int)UserLogTypes.Global, (int)UserLogActions.Edit, {entityClassCammelCase}.Id,\"بروزرسانی \" +  {entityClassCammelCase}.Name ,userId);");


        CodeBuilder.AppendLine();
		
		CodeBuilder.AppendLine($"return {entityClassCammelCase};");
		
	}	
	CodeBuilder.AppendLine("}").AppendLine();
	
	CodeBuilder.AppendLine("#endregion").AppendLine();


    CodeBuilder.AppendLine("#region Update Status").AppendLine();
    CodeBuilder.AppendLine("/// <summary>");
    CodeBuilder.AppendLine($"/// Update {entityClass}");
    CodeBuilder.AppendLine("/// </summary>");
    CodeBuilder.AppendLine($"public async Task UpdateStatus({entityClass} {entityClassCammelCase}, int userId)");
    CodeBuilder.AppendLine("{");
    using (CodeBuilder.Indent())
    {
        CodeBuilder.AppendLine($"DataProtect.NotNull({entityClassCammelCase}, nameof({entityClass}));");
        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine($"//Update {entityClass}");
        CodeBuilder.AppendLine($"await _repository.UpdateAndSaveChangesAsync({entityClassCammelCase}");
        CodeBuilder.AppendLine($",t => t.IsActive);");

        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine($"//Log update {entityClass}");
        CodeBuilder.AppendLine($"await _logService.Add(\"{entityClass}\",(int)UserLogTypes.Global, (int)UserLogActions.Edit, {entityClassCammelCase}.Id,\"بروزرسانی وضعیت \" +  {entityClassCammelCase}.Name ,userId);");


        CodeBuilder.AppendLine();

    }
    CodeBuilder.AppendLine("}").AppendLine();

    CodeBuilder.AppendLine("#endregion").AppendLine();

    CodeBuilder.AppendLine("#region Delete").AppendLine();
	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Delete a {entityClass}");
	CodeBuilder.AppendLine("/// </summary>");
	CodeBuilder.AppendLine($"public async Task Delete(int id, int userId)");
	CodeBuilder.AppendLine("{");
	using (CodeBuilder.Indent())
	{
		CodeBuilder.AppendLine($"var {entityClassCammelCase} = await GetById(id,userId);");
		CodeBuilder.AppendLine();
		
		CodeBuilder.AppendLine($"//Delete {entityClassCammelCase}");
		CodeBuilder.AppendLine($"{entityClassCammelCase}.IsDeleted = true;");
        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine($"await _repository.UpdateAndSaveChangesAsync({entityClassCammelCase}");
        CodeBuilder.AppendLine($",t => t.IsDeleted);");

        CodeBuilder.AppendLine();

        CodeBuilder.AppendLine($"//Log delete {entityClass}");
        CodeBuilder.AppendLine($"await _logService.Add(\"{entityClass}\",(int)UserLogTypes.Global, (int)UserLogActions.Delete, {entityClassCammelCase}.Id,\"حذف \" +  {entityClassCammelCase}.Name ,userId);");

    }
    CodeBuilder.AppendLine("}").AppendLine();	
	CodeBuilder.AppendLine("#endregion").AppendLine();


	CodeBuilder.AppendLine("#region Private").AppendLine();
	CodeBuilder.AppendLine("/// <summary>");
	CodeBuilder.AppendLine($"/// Check exists {entityClassCammelCase}");
    CodeBuilder.AppendLine("/// </summary>");   
	CodeBuilder.AppendLine($"private async Task<bool> CheckExists{entityClass}({entityClass} {entityClassCammelCase})");
	CodeBuilder.AppendLine("{");
	using (CodeBuilder.Indent())
	{
		CodeBuilder.AppendLine($"return await _repository");
		using (CodeBuilder.Indent())
		{
			CodeBuilder.AppendLine(".TableNoTracking");
			CodeBuilder.AppendLine($".AnyAsync(e => ({entityClassCammelCase}.Id == 0|| e.Id != {entityClassCammelCase}.Id) && ");
			using (CodeBuilder.Indent())
			{
				using (CodeBuilder.Indent())
				{
					CodeBuilder.AppendLine($"e.Name.Equals({entityClassCammelCase}.Name));");
				}				
			}
		}
	}	
	CodeBuilder.AppendLine("}");
	CodeBuilder.AppendLine();
	CodeBuilder.AppendLine("#endregion");
	
}


// run script
WriteCode()