:: This script runs the docker image locally 
::  for testing purposes only
::  This is not production

:: force remove the old container
docker container remove -f przetrwaj_api
:: start a new container in Detached mode
docker container run --name przetrwaj_api -p 80:80 -d fafik77/przetrwaj:api
