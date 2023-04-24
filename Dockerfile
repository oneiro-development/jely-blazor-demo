FROM mcr.microsoft.com/dotnet/sdk:latest AS build
# Install Node.js (using the latest LTS version)
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash - && \
    apt-get install -y nodejs
RUN npm i -g corepack sass
RUN corepack prepare yarn@stable --activate
WORKDIR /src
COPY . .
RUN yarn install
RUN dotnet restore
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:latest AS runtime
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "JelyUI.dll"]