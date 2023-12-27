1- Build and Pack BPC.CodeGenerator solution.
2- BPC.Generator.1.0.0.nupkg package can be found in artifacts folder
3- install package as global package using dotnet tools

dotnet tool install --global --add-source D:\Projects\NewProjects\bpc.codegenerator\Source\artifacts  BPC.Generator

dotnet tool uninstall -g  BPC.Generator

4- copy configurations content (Templates & generation file) to a folder
5- navigate to destination folder and run below command: 
	ecg generate -c <ConnectionString>
	ecg generate -c "Data Source=DBIpAddress;Initial Catalog=Lms_UserManagement;Integrated Security=False;Persist Security Info=False;User ID=DBUserName;Password=DBPassword"