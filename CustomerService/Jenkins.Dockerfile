FROM jenkins/jenkins:lts-jdk21

USER root

# Install prerequisites
RUN apt-get update && \
    apt-get install -y wget ca-certificates curl gnupg && \
    rm -rf /var/lib/apt/lists/*

# Add Microsoft package repository for Debian 13
RUN wget https://packages.microsoft.com/config/debian/13/packages-microsoft-prod.deb \
        -O /tmp/packages-microsoft-prod.deb && \
    dpkg -i /tmp/packages-microsoft-prod.deb && \
    rm /tmp/packages-microsoft-prod.deb

# Install .NET 8 SDK
RUN apt-get update && \
    apt-get install -y dotnet-sdk-8.0 && \
    rm -rf /var/lib/apt/lists/*

# Install Docker CLI
RUN apt-get update && \
    apt-get install -y docker.io && \
    rm -rf /var/lib/apt/lists/*

USER jenkins