# EasyTagProject
EasyTag is a room management web application that helps to identify rooms using QR codes. 

Instructions to run the application locally

1. Clone the repository

2. Create an appsettings.json file in /EasyTagProject/
  Example /EasyTagProject/appsettings.json
  
3. Write the following json object in the file appsettings.json and save changes.
  
```  
{
  "Data": {
    "EasyTagDB": {
      "ConnectionString": "Server=(localdb)\\MSSQLLocalDB;Database=EasyTagDB;Trusted_Connection=True;MultipleActiveResultSets=true"
    },
      "EasyTagIdentity": {
        "ConnectionString": "Server=(localdb)\\MSSQLLocalDB;Database=IdentityUsersEasyTag;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
  }
}
```

4. Default Admin user, Professor user, and respective passwords can be changed in SeedDataIdentity.cs under models folder, however changing the default Admin user information is not recommended.

5. Db's will be created the first time the application is run using migration scripts.

6. Log in to have access to the special features of the application.
