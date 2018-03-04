FROM microsoft/aspnetcore-build:2.0
ADD . /app
WORKDIR /app
<<<<<<< HEAD
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
EXPOSE 80:8081
=======
RUN ["rm", "appsettings.Db.json"]
RUN ["mv", "appsettings.Db.docker.json", "appsettings.Db.json"]
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]

>>>>>>> master
ENTRYPOINT ["dotnet", "run", "--server.urls", "http://*:80"]
