#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/framework/runtime:4.8-windowsservercore-ltsc2019
ARG source
WORKDIR /app
COPY ${source:-obj/Docker/publish} .
COPY System32/* c:/windows/system32/
COPY SysWOW64/* c:/windows/syswow64/
ENTRYPOINT ["C:\\app\\Kprocess.KL2.FileServer.exe"]

ENV DATASOURCE=.\SQLEXPRESS
ENV DATABASE=KProcess.KL2
ENV APPLICATIONSCHEME=http
ENV APPLICATIONPORT=8082
ENV FILEPROVIDER=SFtp
ENV SFTP_SERVER=127.0.0.1
ENV SFTP_PORT=22
ENV SFTP_USER=kl2
ENV SFTP_PASSWORD=kl2
ENV SFTP_PUBLISHEDFILESDIRECTORY=/PublishedFiles
ENV SFTP_UPLOADEDFILESDIRECTORY=/UploadedFiles
ENV LOCAL_PUBLISHEDFILESDIRECTORY=C:\PublishedFiles
ENV LOCAL_UPLOADEDFILESDIRECTORY=C:\UploadedFiles