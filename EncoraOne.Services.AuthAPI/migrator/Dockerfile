FROM mcr.microsoft.com/dotnet/sdk:8.0

# Install dotnet-ef as a global tool at build time so runtime doesn't need to install it
RUN dotnet tool install --global dotnet-ef --version 8.0.1 || dotnet tool update --global dotnet-ef --version 8.0.1
ENV PATH="$PATH:/root/.dotnet/tools"

WORKDIR /app

CMD ["/bin/bash"]
