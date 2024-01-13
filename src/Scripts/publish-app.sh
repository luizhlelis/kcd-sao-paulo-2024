#!/usr/bin/env bash

arch=$(uname -m)

echo "### The host architecture is $arch"

if [ "$arch" == "arm64" ] || [ "$arch" == "aarch64" ]; then
  echo "### Publishing for arm64"
  dotnet publish "$1" -r linux-arm64 -o bin -c Release
else
  echo "### Publishing for amd64"
  dotnet publish "$1" -r linux-x64 -o bin -c Release
fi