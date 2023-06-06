## Build Image
* build image\
    `docker build -t dotnet-docker:v1.0.0 .`
* docker run\
`docker run -d --rm -p 5000:80 dotnet-app dotnet-docker:v1.0.0`
* docker stop\
`docker stop dotnet-app`

