:: This script builds the docker image for arm64 architecture

docker buildx build --platform linux/arm64/v8 -t fafik77/przetrwaj:front.arm64 .