#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/framework/runtime:4.8-windowsservercore-ltsc2019
ARG source
WORKDIR /app
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["C:\\app\\KProcess.KL2.API.exe"]

ENV DATASOURCE=.\SQLEXPRESS
ENV DATABASE=KProcess.KL2
ENV APPLICATIONSCHEME=http
ENV APPLICATIONPORT=8081
ENV FILESERVERSCHEME=http
ENV FILESERVER=localhost
ENV FILESERVERPORT=8082