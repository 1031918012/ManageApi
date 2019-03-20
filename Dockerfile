FROM microsoft/dotnet:2.1-aspnetcore-runtime
RUN ln -s /lib/x86_64-linux-gnu/libdl-2.24.so /lib/x86_64-linux-gnu/libdl.so

RUN echo "deb http://mirrors.aliyun.com/debian wheezy main contrib non-free \
deb-src http://mirrors.aliyun.com/debian wheezy main contrib non-free \
deb http://mirrors.aliyun.com/debian wheezy-updates main contrib non-free \
deb-src http://mirrors.aliyun.com/debian wheezy-updates main contrib non-free \
deb http://mirrors.aliyun.com/debian-security wheezy/updates main contrib non-free \
deb-src http://mirrors.aliyun.com/debian-security wheezy/updates main contrib non-free" > /etc/apt/sources.list
 
RUN apt-get update
RUN apt-get install libgdiplus -y && ln -s libgdiplus.so gdiplus.dll

WORKDIR /app

COPY . .

EXPOSE 80/tcp

ENTRYPOINT ["dotnet","API.dll"]
