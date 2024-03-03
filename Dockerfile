FROM mcr.microsoft.com/dotnet/framework/runtime:4.8
WORKDIR /app
COPY . .
ENTRYPOINT ["/app/Gold_Diggerzz.exe"]
