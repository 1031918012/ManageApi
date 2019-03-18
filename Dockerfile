#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["TodoApi/ManageApi.csproj", "TodoApi/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure1/Infrastructure.csproj", "Infrastructure1/"]
COPY ["Service1/Service.csproj", "Service1/"]
COPY ["Repositories/Repositories.csproj", "Repositories/"]
RUN dotnet restore "TodoApi/ManageApi.csproj"
COPY . .
WORKDIR "/src/TodoApi"
RUN dotnet build "ManageApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ManageApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ManageApi.dll"]