FROM microsoft/dotnet:2.1-aspnetcore-runtime

WORKDIR /app

COPY . .

EXPOSE 80/tcp

ENTRYPOINT ["dotnet","API.dll"]
